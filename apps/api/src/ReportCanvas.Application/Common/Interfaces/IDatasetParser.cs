namespace ReportCanvas.Application.Common.Interfaces;

public record ParsedDatasetResult(
    IReadOnlyList<string> Headers,
    IReadOnlyList<string> InferredTypes,
    IReadOnlyList<IReadOnlyList<string>> PreviewRows,
    int TotalRowCount
);

public interface IDatasetParser
{
    bool CanHandle(string fileExtension);

    /// <summary>
    /// Parses the file, infers column types, and returns headers + a preview (first N rows).
    /// Does NOT load all rows into memory — only what's needed for metadata.
    /// </summary>
    Task<ParsedDatasetResult> ParseAsync(Stream fileStream, int previewRowCount = 20, CancellationToken ct = default);
}
