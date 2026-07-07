namespace ReportCanvas.Application.Features.Datasets.DTOs;

public record DatasetColumnDto(
    string Name,
    string InferredType,
    int ColumnIndex
);

public record DatasetPreviewResponse(
    Guid Id,
    string Name,
    string OriginalFileName,
    string FileType,
    long FileSizeBytes,
    int RowCount,
    int ColumnCount,
    IReadOnlyList<DatasetColumnDto> Columns,
    IReadOnlyList<IReadOnlyList<string>> PreviewRows
);

public record DatasetSummaryResponse(
    Guid Id,
    string Name,
    string OriginalFileName,
    string FileType,
    long FileSizeBytes,
    int RowCount,
    int ColumnCount,
    DateTime CreatedAt
);
