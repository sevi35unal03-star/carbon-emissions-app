namespace IzTek.Carbon.Footprint.Infrastructure.Services;

public class MinioFileStorageService(IMinioClient minioClient, IOptions<FileStorageOptions> options) : IFileStorageService
{
    private readonly IMinioClient _minioClient = minioClient;
    private readonly FileStorageOptions _options = options.Value;

    public async Task<Result<string>> UploadFileAsync(Stream fileStream, string fileName, string contentType, string? bucket = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var bucketName = bucket ?? _options.DefaultBucket;
            await EnsureBucketExistsAsync(bucketName, cancellationToken);

            var putObjectArgs = new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(fileName)
                .WithStreamData(fileStream)
                .WithObjectSize(fileStream.Length)
                .WithContentType(contentType);

            await _minioClient.PutObjectAsync(putObjectArgs, cancellationToken);

            return Result<string>.Success(fileName);
        }
        catch (MinioException ex)
        {
            return Result<string>.Failure(SystemErrorCodes.SystemError, ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<Result<Stream>> DownloadFileAsync(string fileName, string? bucket = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var bucketName = bucket ?? _options.DefaultBucket;
            var memoryStream = new MemoryStream();

            var getObjectArgs = new GetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(fileName)
                .WithCallbackStream(stream =>
                {
                    stream.CopyTo(memoryStream);
                    memoryStream.Position = 0;
                });

            await _minioClient.GetObjectAsync(getObjectArgs, cancellationToken);

            return Result<Stream>.Success(memoryStream);
        }
        catch (ObjectNotFoundException)
        {
            return Result<Stream>.Failure(SystemErrorCodes.SystemError, "File not found", HttpStatusCode.NotFound);
        }
        catch (MinioException ex)
        {
            return Result<Stream>.Failure(SystemErrorCodes.SystemError, ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<Result> DeleteFileAsync(string fileName, string? bucket = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var bucketName = bucket ?? _options.DefaultBucket;

            var removeObjectArgs = new RemoveObjectArgs()
                .WithBucket(bucketName)
                .WithObject(fileName);

            await _minioClient.RemoveObjectAsync(removeObjectArgs, cancellationToken);

            return Result.Success();
        }
        catch (MinioException ex)
        {
            return Result.Failure(SystemErrorCodes.SystemError, ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<Result<string>> GetPresignedUrlAsync(string fileName, int expirationMinutes = 60, string? bucket = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var bucketName = bucket ?? _options.DefaultBucket;

            var presignedGetObjectArgs = new PresignedGetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(fileName)
                .WithExpiry(expirationMinutes * 60);

            var url = await _minioClient.PresignedGetObjectAsync(presignedGetObjectArgs);

            return Result<string>.Success(url);
        }
        catch (MinioException ex)
        {
            return Result<string>.Failure(SystemErrorCodes.SystemError, ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<Result<bool>> FileExistsAsync(string fileName, string? bucket = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var bucketName = bucket ?? _options.DefaultBucket;

            var statObjectArgs = new StatObjectArgs()
                .WithBucket(bucketName)
                .WithObject(fileName);

            await _minioClient.StatObjectAsync(statObjectArgs, cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (ObjectNotFoundException)
        {
            return Result<bool>.Success(false);
        }
        catch (MinioException ex)
        {
            return Result<bool>.Failure(SystemErrorCodes.SystemError, ex.Message, HttpStatusCode.InternalServerError);
        }
    }

    private async Task EnsureBucketExistsAsync(string bucketName, CancellationToken cancellationToken)
    {
        var bucketExistsArgs = new BucketExistsArgs().WithBucket(bucketName);
        var exists = await _minioClient.BucketExistsAsync(bucketExistsArgs, cancellationToken);

        if (!exists)
        {
            var makeBucketArgs = new MakeBucketArgs().WithBucket(bucketName);
            await _minioClient.MakeBucketAsync(makeBucketArgs, cancellationToken);
        }
    }
}
