using ReportCanvas.Domain.Common;

namespace ReportCanvas.Domain.Entities;

public class Report : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public Guid WorkspaceId { get; set; }
    public Workspace Workspace { get; set; } = null!;

    public Guid? BrandKitId { get; set; }
    public BrandKit? BrandKit { get; set; }

    public ICollection<ReportPage> Pages { get; set; } = [];
    public ICollection<ExportJob> ExportJobs { get; set; } = [];
}
