using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Khoa_Luan_KS_Web.Services;

public class BackendAuthClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public BackendAuthClient(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<LoginResponse> LoginAdminAsync(string username, string password, CancellationToken ct)
    {
        var payload = new { username, password };
        return await PostAsync<LoginResponse>("/api/v1/Auth/login-admin", payload, bearerToken: null, ct);
    }

    public async Task<LoginResponse> LoginUserAsync(string email, string password, CancellationToken ct)
    {
        var payload = new { email, password };
        return await PostAsync<LoginResponse>("/api/v1/Auth/login", payload, bearerToken: null, ct);
    }

    public async Task LogoutAsync(string accessToken, CancellationToken ct)
    {
        var payload = new { token = accessToken };
        _ = await PostAsync<object>("/api/v1/Auth/logout", payload, bearerToken: accessToken, ct);
    }

    private async Task<T> PostAsync<T>(string path, object payload, string? bearerToken, CancellationToken ct)
    {
        var baseUrl = _configuration["BackendApi:BaseUrl"]?.TrimEnd('/');
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new InvalidOperationException("BackendApi:BaseUrl is not configured in appsettings.json");

        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(baseUrl);

        if (!string.IsNullOrWhiteSpace(bearerToken))
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

        var json = JsonSerializer.Serialize(payload);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var res = await client.PostAsync(path, content, ct);
        var body = await res.Content.ReadAsStringAsync(ct);

        if (!res.IsSuccessStatusCode)
        {
            // backend uses a consistent envelope, but still surface raw message if parsing fails
            var msg = TryExtractBackendMessage(body) ?? $"Backend request failed ({(int)res.StatusCode})";
            throw new InvalidOperationException(msg);
        }

        var envelope = JsonSerializer.Deserialize<BaseApiResponse<T>>(body, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (envelope == null)
            throw new InvalidOperationException("Empty response from backend");

        if (!envelope.Success)
            throw new InvalidOperationException(envelope.Message ?? "Login failed");

        if (envelope.Data == null)
            throw new InvalidOperationException("Backend returned no data");

        return envelope.Data;
    }

    private static string? TryExtractBackendMessage(string body)
    {
        try
        {
            var doc = JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("message", out var msg) && msg.ValueKind == JsonValueKind.String)
                return msg.GetString();
            return null;
        }
        catch
        {
            return null;
        }
    }
}

public sealed class BaseApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
}

public sealed class LoginResponse
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiresAt { get; set; }
}

