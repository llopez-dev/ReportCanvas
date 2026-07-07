using ReportCanvas.Domain.Common;

namespace ReportCanvas.Domain.Entities;

public class DatasetColumn : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string InferredType { get; set; } = string.Empty; // string | number | date | boolean
    public int ColumnIndex { get; set; }

    public Guid DatasetId { get; set; }
    public Dataset Dataset { get; set; } = null!;
}
