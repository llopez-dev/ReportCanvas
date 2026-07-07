using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReportCanvas.Application.Common.Interfaces;
using ReportCanvas.Application.Features.Datasets.DTOs;
using ReportCanvas.Domain.Entities;
using ReportCanvas.Infrastructure.Persistence;

namespace ReportCanvas.Api.Controllers;

[ApiController]
[Route("api/workspaces/{workspaceId:guid}/datasets")]
[Authorize]
public class DatasetsController : ControllerBase
{
    private static readonly string[] AllowedExtensions = [".csv", ".xlsx", ".xls"];
    private const long MaxFileSizeBytes = 50 * 1024 * 1024; // 50 MB

    private readonly ReportCanvasDbContext _db;
    private readonly IFileStorageService _storage;
    private readonly IEnumerable<IDatasetParser> _parsers;
    private readonly ICurrentUserService _currentUser;

    public DatasetsController(
        ReportCanvasDbContext db,
        IFileStorageService storage,
        IEnumerable<IDatasetParser> parsers,
        ICurrentUserService currentUser)
    {
        _db = db;
        _storage = storage;
        _parsers = parsers;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(Guid workspaceId, CancellationToken ct)
    {
        var datasets = await _db.Datasets
            .Where(d => d.WorkspaceId == workspaceId)
            .OrderByDescending(d => d.CreatedAt)
            .Select(d => new DatasetSummaryResponse(
                d.Id, d.Name, d.OriginalFileName, d.FileType,
                d.FileSizeBytes, d.RowCount, d.ColumnCount, d.CreatedAt))
            .ToListAsync(ct);

        return Ok(datasets);
    }

    [HttpPost]
    public async Task<IActionResult> Upload(
        Guid workspaceId,
        IFormFile file,
        [FromForm] string? name,
        CancellationToken ct)
    {
        // --- Validation ---
        if (file is null || file.Length == 0)
            return BadRequest(new { message = "No file provided." });

        if (file.Length > MaxFileSizeBytes)
            return BadRequest(new { message = $"File exceeds the {MaxFileSizeBytes / 1024 / 1024} MB limit." });

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext))
            return BadRequest(new { message = $"File type '{ext}' is not supported. Allowed: {string.Join(", ", AllowedExtensions)}" });

        var parser = _parsers.FirstOrDefault(p => p.CanHandle(ext));
        if (parser is null)
            return BadRequest(new { message = "No parser available for this file type." });

        // --- Store original file in Azurite/Blob ---
        var datasetId = Guid.NewGuid();
        var blobPath = $"{workspaceId}/{datasetId}/original{ext}";

        using (var stream = file.OpenReadStream())
        {
            await _storage.UploadAsync("datasets", blobPath, stream, file.ContentType, ct);
        }

        // --- Parse for metadata and preview ---
        ParsedDatasetResult parsed;
        using (var stream = file.OpenReadStream())
        {
            parsed = await parser.ParseAsync(stream, previewRowCount: 20, ct);
        }

        // --- Persist metadata to PostgreSQL ---
        var dataset = new Dataset
        {
            Id = datasetId,
            Name = name ?? Path.GetFileNameWithoutExtension(file.FileName),
            OriginalFileName = file.FileName,
            FileType = ext.TrimStart('.'),
            StoragePath = blobPath,
            FileSizeBytes = file.Length,
            RowCount = parsed.TotalRowCount,
            ColumnCount = parsed.Headers.Count,
            WorkspaceId = workspaceId
        };

        dataset.Columns = parsed.Headers
            .Select((header, index) => new DatasetColumn
            {
                Name = header,
                InferredType = parsed.InferredTypes.Count > index ? parsed.InferredTypes[index] : "string",
                ColumnIndex = index,
                DatasetId = datasetId
            }).ToList();

        _db.Datasets.Add(dataset);
        await _db.SaveChangesAsync(ct);

        // --- Return preview response ---
        var response = new DatasetPreviewResponse(
            dataset.Id,
            dataset.Name,
            dataset.OriginalFileName,
            dataset.FileType,
            dataset.FileSizeBytes,
            dataset.RowCount,
            dataset.ColumnCount,
            dataset.Columns.Select(c => new DatasetColumnDto(c.Name, c.InferredType, c.ColumnIndex)).ToList(),
            parsed.PreviewRows);

        return CreatedAtAction(nameof(GetById), new { workspaceId, id = dataset.Id }, response);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid workspaceId, Guid id, CancellationToken ct)
    {
        var dataset = await _db.Datasets
            .Include(d => d.Columns)
            .FirstOrDefaultAsync(d => d.WorkspaceId == workspaceId && d.Id == id, ct);

        if (dataset is null) return NotFound();

        // We don't re-read the file for preview — store preview rows in next iteration
        var response = new DatasetPreviewResponse(
            dataset.Id, dataset.Name, dataset.OriginalFileName, dataset.FileType,
            dataset.FileSizeBytes, dataset.RowCount, dataset.ColumnCount,
            dataset.Columns.OrderBy(c => c.ColumnIndex)
                .Select(c => new DatasetColumnDto(c.Name, c.InferredType, c.ColumnIndex)).ToList(),
            []);

        return Ok(response);
    }
}
