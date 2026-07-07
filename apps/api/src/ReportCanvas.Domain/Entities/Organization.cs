using ReportCanvas.Domain.Common;

namespace ReportCanvas.Domain.Entities;

public class Organization : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? PrimaryColor { get; set; }

    public ICollection<Workspace> Workspaces { get; set; } = [];
    public ICollection<Membership> Memberships { get; set; } = [];
}
