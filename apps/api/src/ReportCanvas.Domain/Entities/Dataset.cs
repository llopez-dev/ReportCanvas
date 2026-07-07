using ReportCanvas.Domain.Common;

namespace ReportCanvas.Domain.Entities;

public class Dataset : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty; // csv | xlsx
    public string StoragePath { get; set; } = string.Empty; // e.g. datasets/{workspaceId}/{datasetId}/original.csv
    public long FileSizeBytes { get; set; }

    public int RowCount { get; set; }
    public int ColumnCount { get; set; }

    public Guid WorkspaceId { get; set; }
    public Workspace Workspace { get; set; } = null!;

    public ICollection<DatasetColumn> Columns { get; set; } = [];
}
