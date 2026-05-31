using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;

namespace Khoa_Luan_KS_Web.Services;

public class BackendOrganizationChatbotAdminClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly IApiTokenService? _apiTokenService;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    public BackendOrganizationChatbotAdminClient(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        IApiTokenService apiTokenService)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _apiTokenService = apiTokenService;
    }

    public Task<OrganizationChatbotAdminConfigClientDto> GetConfigAsync(string accessToken, CancellationToken ct = default)
        => GetAsync<OrganizationChatbotAdminConfigClientDto>("/api/v1/admin/organization-chatbot/config", accessToken, ct);

    public Task<OrganizationChatbotAdminConfigClientDto> SaveConfigAsync(
        UpdateOrganizationChatbotConfigClientRequest request,
        string accessToken,
        CancellationToken ct = default)
        => PutAsync<OrganizationChatbotAdminConfigClientDto>(
            "/api/v1/admin/organization-chatbot/config", request, accessToken, ct);

    public Task<OrganizationChatbotTestResultClientDto> TestConnectionAsync(string accessToken, CancellationToken ct = default)
        => PostAsync<OrganizationChatbotTestResultClientDto>(
            "/api/v1/admin/organization-chatbot/test-connection", new { }, accessToken, ct);

    private Task<T> GetAsync<T>(string path, string accessToken, CancellationToken ct) =>
        BackendApiAuthHelper.SendWithRefreshAsync(_apiTokenService, accessToken, async (token, cancellationToken) =>
        {
            using var res = await CreateClient(token).GetAsync(path, cancellationToken);
            return await BackendApiAuthHelper.HandleResponseAsync<T>(res, cancellationToken);
        }, ct);

    private Task<T> PutAsync<T>(string path, object payload, string accessToken, CancellationToken ct) =>
        BackendApiAuthHelper.SendWithRefreshAsync(_apiTokenService, accessToken, async (token, cancellationToken) =>
        {
            var json = JsonSerializer.Serialize(payload, JsonOptions);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            using var res = await CreateClient(token).PutAsync(path, content, cancellationToken);
            return await BackendApiAuthHelper.HandleResponseAsync<T>(res, cancellationToken);
        }, ct);

    private Task<T> PostAsync<T>(string path, object payload, string accessToken, CancellationToken ct) =>
        BackendApiAuthHelper.SendWithRefreshAsync(_apiTokenService, accessToken, async (token, cancellationToken) =>
        {
            var json = JsonSerializer.Serialize(payload, JsonOptions);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            using var res = await CreateClient(token).PostAsync(path, content, cancellationToken);
            return await BackendApiAuthHelper.HandleResponseAsync<T>(res, cancellationToken);
        }, ct);

    private HttpClient CreateClient(string accessToken)
    {
        var baseUrl = _configuration["BackendApi:BaseUrl"]?.TrimEnd('/');
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(baseUrl!);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return client;
    }
}

public class OrganizationChatbotAdminConfigClientDto
{
    public bool Enabled { get; set; }
    public string Provider { get; set; } = "Gemini";
    public string Model { get; set; } = "gemini-2.5-flash";
    public string? ApiKeyMasked { get; set; }
    public bool HasApiKey { get; set; }
    public double Temperature { get; set; }
    public int MaxOutputTokens { get; set; }
    public string? StoredSystemPrompt { get; set; }
    public string DefaultSystemPrompt { get; set; } = string.Empty;
    public bool UseDefaultSystemPrompt { get; set; } = true;
    public string EffectiveSystemPrompt { get; set; } = string.Empty;
    public string? CustomRules { get; set; }
    public string Status { get; set; } = "offline";
    public string LlmModeLabel { get; set; } = string.Empty;
    public string ConfigSource { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
    public int? LastTestLatencyMs { get; set; }
    public string? LastTestMessage { get; set; }
    public List<OrganizationChatbotRuleItemClientDto> ProcessingRules { get; set; } = new();
}

public class OrganizationChatbotRuleItemClientDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsFixed { get; set; }
}

public class UpdateOrganizationChatbotConfigClientRequest
{
    public bool Enabled { get; set; } = true;
    public string Provider { get; set; } = "Gemini";
    public string? ApiKey { get; set; }
    public string Model { get; set; } = "gemini-2.5-flash";
    public double Temperature { get; set; }
    public int MaxOutputTokens { get; set; }
    public string? SystemPrompt { get; set; }
    public string? CustomRules { get; set; }
    public bool UseDefaultSystemPrompt { get; set; } = true;
}

public class OrganizationChatbotTestResultClientDto
{
    public bool Success { get; set; }
    public int LatencyMs { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Model { get; set; }
}
