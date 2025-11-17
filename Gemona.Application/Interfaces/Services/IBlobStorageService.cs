namespace Gemona.Application.Interfaces.Services;

public interface IBlobStorageService
{
    Task<string> UploadImageAsync(Stream imageStream, string fileName, string contentType);
    Task<bool> DeleteImageAsync(string blobName);
    Task<Stream> DownloadImageAsync(string blobName);
    Task<bool> BlobExistsAsync(string blobName);
    string GetBlobUrl(string blobName);
}
