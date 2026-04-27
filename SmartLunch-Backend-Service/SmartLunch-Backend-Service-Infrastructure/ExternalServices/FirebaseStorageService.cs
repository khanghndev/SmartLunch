using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using System.Net.Http;

namespace SmartLunch.Backend.Service.Infrastructure.ExternalServices;

public class FirebaseStorageService : IStorageService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<FirebaseStorageService> _logger;
    private readonly StorageClient _storageClient;
    private readonly UrlSigner _urlSigner;
    private readonly string _bucket;

    public FirebaseStorageService(IConfiguration configuration, ILogger<FirebaseStorageService> logger)
    {
        _configuration = configuration;
        _logger = logger;

        var serviceAccountPath = ResolveFirebaseServiceAccountPath()
            ?? throw new InvalidOperationException("Firebase service account JSON not found. Configure Firebase:ConfigPath.");

        _bucket = ResolveStorageBucket()
            ?? throw new InvalidOperationException("Firebase Storage bucket not configured. Configure Firebase:StorageBucket (or Firebase:ProjectId).");

        var credential = GoogleCredential.FromFile(serviceAccountPath);
        _storageClient = StorageClient.Create(credential);
        _urlSigner = UrlSigner.FromCredential(credential);

        _logger.LogInformation("Firebase Storage initialized. Bucket={Bucket}, ServiceAccountPath={ServiceAccountPath}", _bucket, serviceAccountPath);
    }

    public Task<StorageSignedUrlResult> CreateSignedUrlAsync(
        string objectName,
        HttpMethod method,
        string? contentType,
        TimeSpan expiresIn,
        IDictionary<string, string>? requiredHeaders = null)
    {
        if (string.IsNullOrWhiteSpace(objectName)) throw new ArgumentException("ObjectName is required", nameof(objectName));
        if (expiresIn <= TimeSpan.Zero) throw new ArgumentException("expiresIn must be positive", nameof(expiresIn));

        // If we sign headers, clients MUST send exactly those headers.
        // If you include Content-Type, the upload request must include Content-Type.
        var contentHeaders = new List<KeyValuePair<string, IEnumerable<string>>>();
        if (!string.IsNullOrWhiteSpace(contentType))
        {
            contentHeaders.Add(new KeyValuePair<string, IEnumerable<string>>("Content-Type", new[] { contentType! }));
        }

        var requestHeaders = new List<KeyValuePair<string, IEnumerable<string>>>();
        var requiredHeadersNormalized = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (requiredHeaders != null)
        {
            foreach (var (key, value) in requiredHeaders)
            {
                if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value)) continue;
                requestHeaders.Add(new KeyValuePair<string, IEnumerable<string>>(key, new[] { value }));
                requiredHeadersNormalized[key] = value;
            }
        }

        if (!string.IsNullOrWhiteSpace(contentType))
        {
            requiredHeadersNormalized["Content-Type"] = contentType!;
        }

        var template = UrlSigner.RequestTemplate
            .FromBucket(_bucket)
            .WithObjectName(objectName)
            .WithHttpMethod(method)
            .WithContentHeaders(contentHeaders)
            .WithRequestHeaders(requestHeaders);

        var options = UrlSigner.Options
            .FromDuration(expiresIn)
            .WithSigningVersion(SigningVersion.V4);

        var url = _urlSigner.Sign(template, options);
        var expiresAtUtc = DateTime.UtcNow.Add(expiresIn);

        return Task.FromResult(new StorageSignedUrlResult(
            _bucket,
            objectName,
            url,
            expiresAtUtc,
            requiredHeadersNormalized));
    }

    public async Task<StorageObjectMetadata?> GetObjectMetadataAsync(string objectName)
    {
        try
        {
            var obj = await _storageClient.GetObjectAsync(_bucket, objectName);
            var sizeBytes = obj.Size.HasValue ? checked((long)obj.Size.Value) : 0L;
            return new StorageObjectMetadata(
                _bucket,
                objectName,
                sizeBytes,
                obj.ContentType,
                obj.Md5Hash,
                UpdatedAt: null);
        }
        catch (Google.GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task UploadObjectAsync(
        string objectName,
        Stream content,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(objectName)) throw new ArgumentException("ObjectName is required", nameof(objectName));
        if (content == null) throw new ArgumentNullException(nameof(content));
        if (string.IsNullOrWhiteSpace(contentType)) throw new ArgumentException("ContentType is required", nameof(contentType));

        await _storageClient.UploadObjectAsync(
            bucket: _bucket,
            objectName: objectName,
            contentType: contentType,
            source: content,
            cancellationToken: cancellationToken);
    }

    public async Task DeleteObjectAsync(string objectName)
    {
        await _storageClient.DeleteObjectAsync(_bucket, objectName);
    }

    private string? ResolveStorageBucket()
    {
        var bucket = _configuration["Firebase:StorageBucket"];
        if (!string.IsNullOrWhiteSpace(bucket))
        {
            return bucket;
        }

        var projectId = _configuration["Firebase:ProjectId"];
        if (!string.IsNullOrWhiteSpace(projectId))
        {
            // Default Firebase Storage bucket naming convention
            return $"{projectId}.appspot.com";
        }

        return null;
    }

    private string? ResolveFirebaseServiceAccountPath()
    {
        var firebaseConfigPath = _configuration["Firebase:ConfigPath"];
        if (string.IsNullOrWhiteSpace(firebaseConfigPath))
        {
            return null;
        }

        // Absolute path
        if (Path.IsPathRooted(firebaseConfigPath) && File.Exists(firebaseConfigPath))
        {
            return firebaseConfigPath;
        }

        // Relative to current working directory
        var workingDir = Directory.GetCurrentDirectory();
        var cwdPath = Path.Combine(workingDir, firebaseConfigPath);
        if (File.Exists(cwdPath))
        {
            return cwdPath;
        }

        // Relative to application base directory
        var baseDir = AppContext.BaseDirectory;
        var basePath = Path.Combine(baseDir, firebaseConfigPath);
        if (File.Exists(basePath))
        {
            return basePath;
        }

        // Try walking up from baseDir (bin folder)
        var currentDir = baseDir;
        for (int i = 0; i < 6 && !string.IsNullOrEmpty(currentDir); i++)
        {
            var testPath = Path.Combine(currentDir, firebaseConfigPath);
            if (File.Exists(testPath))
            {
                return testPath;
            }
            currentDir = Directory.GetParent(currentDir)?.FullName;
        }

        return null;
    }
}

