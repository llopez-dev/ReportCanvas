using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.Configuration;
using ReportCanvas.Application.Common.Interfaces;

namespace ReportCanvas.Infrastructure.Storage;

public class AzureBlobStorageService : IFileStorageService
{
    private readonly BlobServiceClient _client;

    public AzureBlobStorageService(IConfiguration config)
    {
        var connectionString = config["Storage:ConnectionString"]
            ?? throw new InvalidOperationException("Storage:ConnectionString is not configured.");
        _client = new BlobServiceClient(connectionString);
    }

    public async Task<string> UploadAsync(
        string containerName,
        string blobPath,
        Stream content,
        string contentType,
        CancellationToken ct = default)
    {
        var container = _client.GetBlobContainerClient(containerName);
        await container.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: ct);

        var blob = container.GetBlobClient(blobPath);
        await blob.UploadAsync(content, new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
        }, ct);

        return blobPath;
    }

    public async Task<Stream> DownloadAsync(string containerName, string blobPath, CancellationToken ct = default)
    {
        var blob = _client.GetBlobContainerClient(containerName).GetBlobClient(blobPath);
        var response = await blob.DownloadStreamingAsync(cancellationToken: ct);
        return response.Value.Content;
    }

    public async Task DeleteAsync(string containerName, string blobPath, CancellationToken ct = default)
    {
        var blob = _client.GetBlobContainerClient(containerName).GetBlobClient(blobPath);
        await blob.DeleteIfExistsAsync(cancellationToken: ct);
    }

    public async Task<string> GetDownloadUrlAsync(
        string containerName,
        string blobPath,
        TimeSpan expiry,
        CancellationToken ct = default)
    {
        var blob = _client.GetBlobContainerClient(containerName).GetBlobClient(blobPath);

        if (blob.CanGenerateSasUri)
        {
            var sasUri = blob.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow.Add(expiry));
            return sasUri.ToString();
        }

        // Azurite fallback: return direct URL
        return blob.Uri.ToString();
    }
}
