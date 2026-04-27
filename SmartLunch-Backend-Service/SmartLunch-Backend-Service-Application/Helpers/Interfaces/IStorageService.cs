using System.Net.Http;

namespace SmartLunch.Backend.Service.Application.Helpers.Interfaces;

public interface IStorageService
{
    Task<StorageSignedUrlResult> CreateSignedUrlAsync(
        string objectName,
        HttpMethod method,
        string? contentType,
        TimeSpan expiresIn,
        IDictionary<string, string>? requiredHeaders = null);

    Task<StorageObjectMetadata?> GetObjectMetadataAsync(string objectName);

    Task UploadObjectAsync(
        string objectName,
        Stream content,
        string contentType,
        CancellationToken cancellationToken = default);

    Task DeleteObjectAsync(string objectName);
}

public sealed record StorageSignedUrlResult(
    string Bucket,
    string ObjectName,
    string Url,
    DateTime ExpiresAtUtc,
    IReadOnlyDictionary<string, string> RequiredHeaders);

public sealed record StorageObjectMetadata(
    string Bucket,
    string ObjectName,
    long SizeBytes,
    string? ContentType,
    string? Md5HashBase64,
    DateTimeOffset? UpdatedAt);

