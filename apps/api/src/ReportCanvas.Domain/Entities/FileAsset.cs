using ReportCanvas.Domain.Common;

namespace ReportCanvas.Domain.Entities;

/// <summary>Binary assets uploaded by the user (logos, images) stored in Blob Storage</summary>
public class FileAsset : BaseEntity
{
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string StoragePath { get; set; } = string.Empty; // assets/{workspaceId}/...
    public string? PublicUrl { get; set; } // filled only for assets that need direct URL access

    public Guid WorkspaceId { get; set; }
    public Workspace Workspace { get; set; } = null!;
}
