using ReportCanvas.Domain.Common;

namespace ReportCanvas.Domain.Entities;

public class Workspace : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;

    public Guid OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;

    public ICollection<Dataset> Datasets { get; set; } = [];
    public ICollection<Report> Reports { get; set; } = [];
    public ICollection<BrandKit> BrandKits { get; set; } = [];
    public ICollection<FileAsset> FileAssets { get; set; } = [];
}
