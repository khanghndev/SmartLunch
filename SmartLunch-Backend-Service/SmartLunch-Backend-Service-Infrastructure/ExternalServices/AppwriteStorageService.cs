using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;

namespace SmartLunch.Backend.Service.Infrastructure.ExternalServices;

public class AppwriteStorageService : IFirebaseStorageService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AppwriteStorageService> _logger;
    private readonly string _endpoint;
    private readonly string _projectId;
    private readonly string _apiKey;
    private readonly string _bucketId;

    public AppwriteStorageService(IConfiguration configuration, ILogger<AppwriteStorageService> logger)
    {
        _logger = logger;
        _endpoint = (configuration["Appwrite:Endpoint"] ?? "https://cloud.appwrite.io/v1").TrimEnd('/');
        _projectId = configuration["Appwrite:ProjectId"] ?? throw new InvalidOperationException("Missing Appwrite:ProjectId");
        _apiKey = configuration["Appwrite:ApiKey"] ?? throw new InvalidOperationException("Missing Appwrite:ApiKey");
        _bucketId = configuration["Appwrite:BucketId"] ?? throw new InvalidOperationException("Missing Appwrite:BucketId");

        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("X-Appwrite-Project", _projectId);
        _httpClient.DefaultRequestHeaders.Add("X-Appwrite-Key", _apiKey);
    }

    public Task<FirebaseSignedUrlResult> CreateSignedUrlAsync(
        string objectName,
        HttpMethod method,
        string? contentType,
        TimeSpan expiresIn,
        IDictionary<string, string>? requiredHeaders = null)
    {
        if (string.IsNullOrWhiteSpace(objectName))
            throw new ArgumentException("ObjectName is required", nameof(objectName));

        var expiresAt = DateTime.UtcNow.Add(expiresIn);
        var fileId = ToFileId(objectName);

        if (method == HttpMethod.Get)
        {
            return CreateDownloadUrlAsync(objectName, fileId, expiresAt);
        }

        var uploadUrl = $"{_endpoint}/storage/buckets/{_bucketId}/files";
        var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (!string.IsNullOrWhiteSpace(contentType))
            headers["Content-Type"] = contentType;

        if (requiredHeaders != null)
        {
            foreach (var pair in requiredHeaders)
                headers[pair.Key] = pair.Value;
        }

        return Task.FromResult(new FirebaseSignedUrlResult(
            _bucketId,
            objectName,
            uploadUrl,
            expiresAt,
            headers));
    }

    public async Task<FirebaseObjectMetadata?> GetObjectMetadataAsync(string objectName)
    {
        var fileId = ToFileId(objectName);
        var url = $"{_endpoint}/storage/buckets/{_bucketId}/files/{Uri.EscapeDataString(fileId)}";
        using var res = await _httpClient.GetAsync(url);
        if (res.StatusCode == HttpStatusCode.NotFound)
            return null;

        res.EnsureSuccessStatusCode();
        var json = await res.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var mimeType = root.TryGetProperty("mimeType", out var mt) ? mt.GetString() : null;
        var sizeRaw = root.TryGetProperty("sizeOriginal", out var so) ? so.GetString() : "0";
        var md5 = root.TryGetProperty("signature", out var sig) ? sig.GetString() : null;
        var updated = root.TryGetProperty("$updatedAt", out var upd) ? upd.GetString() : null;
        _ = long.TryParse(sizeRaw, out var size);
        DateTimeOffset? updatedAt = null;
        if (!string.IsNullOrWhiteSpace(updated) && DateTimeOffset.TryParse(updated, out var parsed))
            updatedAt = parsed;

        return new FirebaseObjectMetadata(
            _bucketId,
            objectName,
            size,
            mimeType,
            md5,
            updatedAt);
    }

    public async Task UploadObjectAsync(
        string objectName,
        Stream content,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        var fileId = ToFileId(objectName);
        using var ms = new MemoryStream();
        await content.CopyToAsync(ms, cancellationToken);
        var payload = ms.ToArray();
        var uploadUrl = $"{_endpoint}/storage/buckets/{_bucketId}/files";
        using var firstForm = BuildUploadForm(fileId, objectName, contentType, payload);
        using var res = await _httpClient.PostAsync(uploadUrl, firstForm, cancellationToken);
        if (res.StatusCode == HttpStatusCode.Conflict)
        {
            // Replace existing object by deleting then re-uploading.
            await DeleteObjectAsync(objectName);
            using var retryForm = BuildUploadForm(fileId, objectName, contentType, payload);
            using var retryRes = await _httpClient.PostAsync(uploadUrl, retryForm, cancellationToken);
            retryRes.EnsureSuccessStatusCode();
            return;
        }

        res.EnsureSuccessStatusCode();
    }

    public async Task DeleteObjectAsync(string objectName)
    {
        var fileId = ToFileId(objectName);
        var url = $"{_endpoint}/storage/buckets/{_bucketId}/files/{Uri.EscapeDataString(fileId)}";
        using var res = await _httpClient.DeleteAsync(url);
        if (res.StatusCode != HttpStatusCode.NotFound)
            res.EnsureSuccessStatusCode();
    }

    private async Task<FirebaseSignedUrlResult> CreateDownloadUrlAsync(string objectName, string fileId, DateTime expiresAt)
    {
        try
        {
            var tokenUrl = $"{_endpoint}/storage/buckets/{_bucketId}/files/{Uri.EscapeDataString(fileId)}/tokens";
            using var tokenReq = new HttpRequestMessage(HttpMethod.Post, tokenUrl)
            {
                Content = new StringContent("{}")
            };
            tokenReq.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            using var tokenRes = await _httpClient.SendAsync(tokenReq);
            tokenRes.EnsureSuccessStatusCode();

            var tokenJson = await tokenRes.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(tokenJson);
            var token = doc.RootElement.GetProperty("token").GetString() ?? string.Empty;

            var url = $"{_endpoint}/storage/buckets/{_bucketId}/files/{Uri.EscapeDataString(fileId)}/view?project={Uri.EscapeDataString(_projectId)}&token={Uri.EscapeDataString(token)}";
            return new FirebaseSignedUrlResult(_bucketId, objectName, url, expiresAt, new Dictionary<string, string>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create Appwrite download token for {ObjectName}", objectName);
            throw;
        }
    }

    private static string ToFileId(string objectName)
    {
        var normalized = objectName.Trim();
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(normalized));
        var hex = Convert.ToHexString(hash).ToLowerInvariant();
        return $"f_{hex[..34]}";
    }

    private static MultipartFormDataContent BuildUploadForm(
        string fileId,
        string objectName,
        string contentType,
        byte[] payload)
    {
        var contentDisposition = new ContentDispositionHeaderValue("form-data")
        {
            Name = "\"file\"",
            FileName = $"\"{Path.GetFileName(objectName)}\""
        };

        var streamContent = new ByteArrayContent(payload);
        if (!string.IsNullOrWhiteSpace(contentType))
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        streamContent.Headers.ContentDisposition = contentDisposition;

        return new MultipartFormDataContent
        {
            { new StringContent(fileId), "fileId" },
            { streamContent, "file", Path.GetFileName(objectName) }
        };
    }
}
