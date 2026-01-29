using System.Net.Http;

namespace SmartLunch.Backend.Service.Application.Helpers.Interfaces;

public interface IFirebaseStorageService
{
    Task<FirebaseSignedUrlResult> CreateSignedUrlAsync(
        string objectName,
        HttpMethod method,
        string? contentType,
        TimeSpan expiresIn,
        IDictionary<string, string>? requiredHeaders = null);

    Task<FirebaseObjectMetadata?> GetObjectMetadataAsync(string objectName);

    Task DeleteObjectAsync(string objectName);
}

public sealed record FirebaseSignedUrlResult(
    string Bucket,
    string ObjectName,
    string Url,
    DateTime ExpiresAtUtc,
    IReadOnlyDictionary<string, string> RequiredHeaders);

public sealed record FirebaseObjectMetadata(
    string Bucket,
    string ObjectName,
    long SizeBytes,
    string? ContentType,
    string? Md5HashBase64,
    DateTimeOffset? UpdatedAt);

