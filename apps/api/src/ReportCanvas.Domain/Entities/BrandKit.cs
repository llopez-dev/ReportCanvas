using ReportCanvas.Domain.Common;

namespace ReportCanvas.Domain.Entities;

public class BrandKit : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public bool IsDefault { get; set; }

    public string PrimaryColor { get; set; } = "#1a1a2e";
    public string SecondaryColor { get; set; } = "#16213e";
    public string AccentColor { get; set; } = "#0f3460";
    public string FontFamily { get; set; } = "Inter";

    /// <summary>Storage path for logo: assets/{workspaceId}/{brandKitId}/logo.png</summary>
    public string? LogoStoragePath { get; set; }

    public Guid WorkspaceId { get; set; }
    public Workspace Workspace { get; set; } = null!;
}
