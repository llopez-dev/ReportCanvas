using ReportCanvas.Domain.Common;
using ReportCanvas.Domain.Enums;

namespace ReportCanvas.Domain.Entities;

public class ExportJob : BaseEntity
{
    public ExportStatus Status { get; set; } = ExportStatus.Pending;
    public string Format { get; set; } = "pdf"; // pdf | png (future)

    /// <summary>Storage path for the generated export file</summary>
    public string? OutputStoragePath { get; set; } // exports/{workspaceId}/{reportId}/{exportId}.pdf
    public string? ErrorMessage { get; set; }

    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public Guid ReportId { get; set; }
    public Report Report { get; set; } = null!;
}
