namespace ReportCanvas.Application.Common.Interfaces;

public interface IFileStorageService
{
    /// <summary>Upload a stream and return the final storage path.</summary>
    Task<string> UploadAsync(string containerName, string blobPath, Stream content, string contentType, CancellationToken ct = default);

    /// <summary>Download a blob as a stream.</summary>
    Task<Stream> DownloadAsync(string containerName, string blobPath, CancellationToken ct = default);

    /// <summary>Delete a blob if it exists.</summary>
    Task DeleteAsync(string containerName, string blobPath, CancellationToken ct = default);

    /// <summary>Returns a short-lived SAS URL or direct URL depending on environment.</summary>
    Task<string> GetDownloadUrlAsync(string containerName, string blobPath, TimeSpan expiry, CancellationToken ct = default);
}
