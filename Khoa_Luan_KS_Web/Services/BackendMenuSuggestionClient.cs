using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Khoa_Luan_KS_Web.Services;

public class BackendMenuSuggestionClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public BackendMenuSuggestionClient(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<GetMenuSuggestionsResponse> GetMenuSuggestionsAsync(string accessToken, int page = 1, int pageSize = 10, string? searchTerm = null, CancellationToken ct = default)
    {
        var query = $"?Page={page}&PageSize={pageSize}";
        if (!string.IsNullOrEmpty(searchTerm)) query += $"&SearchTerm={Uri.EscapeDataString(searchTerm)}";

        return await GetAsync<GetMenuSuggestionsResponse>($"/api/v1/master-data/MenuSuggestion{query}", accessToken, ct);
    }

    public async Task<MenuSuggestionDto> GetMenuSuggestionAsync(int id, string accessToken, CancellationToken ct = default)
    {
        var wrapper = await GetAsync<GetMenuSuggestionResponseWrapper>($"/api/v1/master-data/MenuSuggestion/{id}", accessToken, ct);
        return wrapper.MenuSuggestion;
    }

    public async Task<MenuSuggestionDto> GenerateMenuSuggestionAsync(GenerateMenuSuggestionFromAiRequest request, string accessToken, CancellationToken ct = default)
    {
        var wrapper = await PostAsync<GetMenuSuggestionResponseWrapper>("/api/v1/master-data/MenuSuggestion/generate", request, accessToken, ct);
        return wrapper.MenuSuggestion;
    }

    private async Task<T> GetAsync<T>(string path, string accessToken, CancellationToken ct)
    {
        var client = CreateClient(accessToken);
        using var res = await client.GetAsync(path, ct);
        return await HandleResponse<T>(res, ct);
    }

    private async Task<T> PostAsync<T>(string path, object payload, string accessToken, CancellationToken ct)
    {
        var client = CreateClient(accessToken);
        var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var res = await client.PostAsync(path, content, ct);
        return await HandleResponse<T>(res, ct);
    }

    private HttpClient CreateClient(string accessToken)
    {
        var baseUrl = _configuration["BackendApi:BaseUrl"]?.TrimEnd('/');
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(baseUrl!);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return client;
    }

    private async Task<T> HandleResponse<T>(HttpResponseMessage res, CancellationToken ct)
    {
        var body = await res.Content.ReadAsStringAsync(ct);
        if (!res.IsSuccessStatusCode)
        {
            var msg = TryExtractBackendMessage(body) ?? $"Backend request failed ({(int)res.StatusCode})";
            throw new InvalidOperationException(msg);
        }

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var envelope = JsonSerializer.Deserialize<BaseApiResponse<T>>(body, options);
        if (envelope == null || envelope.Data == null)
        {
            if (typeof(T) == typeof(object)) return (T)(object)new { };
            throw new InvalidOperationException("Invalid response from backend");
        }
        return envelope.Data;
    }

    private static string? TryExtractBackendMessage(string body)
    {
        try
        {
            var doc = JsonDocument.Parse(body);
            string? finalMsg = null;
            if (doc.RootElement.TryGetProperty("message", out var msg) && msg.ValueKind == JsonValueKind.String)
                finalMsg = msg.GetString();

            if (doc.RootElement.TryGetProperty("errors", out var errors) && errors.ValueKind == JsonValueKind.Array && errors.GetArrayLength() > 0)
            {
                var errorList = errors.EnumerateArray().Select(e => e.GetString()).Where(e => !string.IsNullOrEmpty(e));
                var errorStr = string.Join(" | ", errorList);
                if (!string.IsNullOrEmpty(errorStr))
                    finalMsg = finalMsg == null ? errorStr : $"{finalMsg} ({errorStr})";
            }

            return finalMsg;
        }
        catch { return null; }
    }
}

// ─── DTOs ───────────────────────────────────────────────────────────────────

public class GenerateMenuSuggestionFromAiRequest
{
    public DateTime WeekStartUtc { get; set; }
    public string RulesKey { get; set; } = "industrial";
    public decimal BudgetPerServing { get; set; }
    public int TopK { get; set; } = 3;
    public decimal TimeLimitSeconds { get; set; } = 10;
    public List<string> Days { get; set; } = new();
    public List<string> MealStructure { get; set; } = new();
    public List<int> DishIds { get; set; } = new();
}

public class GetMenuSuggestionsResponse : PaginationResponse<MenuSuggestionSummaryDto> { }

public class GetMenuSuggestionResponseWrapper
{
    public MenuSuggestionDto MenuSuggestion { get; set; } = new();
}

public class MenuSuggestionSummaryDto
{
    public int Id { get; set; }
    public DateTime WeekStart { get; set; }
    public DateTime GeneratedAt { get; set; }
    public int Version { get; set; }
    public string? RulesKey { get; set; }
    public decimal? BudgetPerServing { get; set; }
    public int? TopK { get; set; }
    public int? PlanCount { get; set; }
    public string SuggestionText { get; set; } = string.Empty;
}

public class MenuSuggestionDto : MenuSuggestionSummaryDto
{
    public List<MenuSuggestionPlanDto> Plans { get; set; } = new();
}

public class MenuSuggestionPlanDto
{
    public int Id { get; set; }
    public int Rank { get; set; }
    public decimal PlanScore { get; set; }
    public decimal ObjectiveValue { get; set; }
    public List<MenuSuggestionPlanDayDto> Days { get; set; } = new();
}

public class MenuSuggestionPlanDayDto
{
    public int Id { get; set; }
    public int DayIndex { get; set; }
    public string DayName { get; set; } = string.Empty;
    public List<MenuSuggestionPlanItemDto> Items { get; set; } = new();
}

public class MenuSuggestionPlanItemDto
{
    public int Id { get; set; }
    public int? DishId { get; set; }
    public string SlotCategory { get; set; } = string.Empty;
    public string DishName { get; set; } = string.Empty;
    public string? DishSourceCategory { get; set; }
    public decimal Score { get; set; }
    public decimal CostPerServing { get; set; }
    public List<string> Reasons { get; set; } = new();
}
