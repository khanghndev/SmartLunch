using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Khoa_Luan_KS_Web.Services;

public class BackendOrganizationChatbotClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly IApiTokenService? _apiTokenService;

    private static readonly JsonSerializerOptions JsonPostOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    public BackendOrganizationChatbotClient(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        IApiTokenService apiTokenService)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _apiTokenService = apiTokenService;
    }

    public Task<OrganizationChatbotMessageClientDto> SendMessageAsync(
        string message,
        string accessToken,
        CancellationToken ct = default)
        => PostAsync<OrganizationChatbotMessageClientDto>(
            "/api/v1/organization/chatbot/message",
            new { message },
            accessToken,
            ct);

    private HttpClient CreateClient(string accessToken)
    {
        var baseUrl = _configuration["BackendApi:BaseUrl"]?.TrimEnd('/');
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(baseUrl!);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return client;
    }

    private Task<T> PostAsync<T>(string path, object payload, string accessToken, CancellationToken ct) =>
        BackendApiAuthHelper.SendWithRefreshAsync(_apiTokenService, accessToken, async (token, cancellationToken) =>
        {
            var client = CreateClient(token);
            var json = JsonSerializer.Serialize(payload, JsonPostOptions);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            using var res = await client.PostAsync(path, content, cancellationToken);
            return await BackendApiAuthHelper.HandleResponseAsync<T>(res, cancellationToken);
        }, ct);
}

public class OrganizationChatbotMessageClientDto
{
    public string Reply { get; set; } = string.Empty;
    public string Intent { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public List<string> Suggestions { get; set; } = new();
    public int? LogId { get; set; }
}
