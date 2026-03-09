namespace IzTek.Carbon.Footprint.Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<Result<string>> UploadFileAsync(Stream fileStream, string fileName, string contentType, string? bucket = null, CancellationToken cancellationToken = default);
    Task<Result<Stream>> DownloadFileAsync(string fileName, string? bucket = null, CancellationToken cancellationToken = default);
    Task<Result> DeleteFileAsync(string fileName, string? bucket = null, CancellationToken cancellationToken = default);
    Task<Result<string>> GetPresignedUrlAsync(string fileName, int expirationMinutes = 60, string? bucket = null, CancellationToken cancellationToken = default);
    Task<Result<bool>> FileExistsAsync(string fileName, string? bucket = null, CancellationToken cancellationToken = default);
}
