using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.Models.AiMenuPlanner;

namespace SmartLunch.Backend.Service.Infrastructure.ExternalServices;

public class AiMenuPlannerClient : IAiMenuPlannerClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AiMenuPlannerClient> _logger;
    private readonly string _baseUrl;

    public AiMenuPlannerClient(IConfiguration configuration, ILogger<AiMenuPlannerClient> logger)
    {
        _logger = logger;
        _baseUrl = (configuration["AiService:BaseUrl"] ?? "http://localhost:8001").TrimEnd('/');
        _httpClient = new HttpClient();
    }

    public async Task<AiIndustrialMenuPlansResponse> RecommendIndustrialMenusAsync(
        AiIndustrialMenuPlansRequest request,
        CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrl}/api/v1/recommend/industrial/menus";

        using var res = await _httpClient.PostAsJsonAsync(url, request, cancellationToken);
        if (!res.IsSuccessStatusCode)
        {
            var body = await res.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning("AI service error {StatusCode}: {Body}", (int)res.StatusCode, body);
            throw new InvalidOperationException($"AI service call failed: {(int)res.StatusCode}");
        }

        var parsed = await res.Content.ReadFromJsonAsync<AiIndustrialMenuPlansResponse>(cancellationToken: cancellationToken);
        return parsed ?? new AiIndustrialMenuPlansResponse();
    }

    public async Task<AiIndustrialIngredientPrepResponse> RecommendIndustrialIngredientPreparationAsync(
        AiIndustrialIngredientPrepRequest request,
        CancellationToken cancellationToken = default)
    {
        var url = $"{_baseUrl}/api/v1/recommend/industrial/ingredients/prepare";

        using var res = await _httpClient.PostAsJsonAsync(url, request, cancellationToken);
        if (!res.IsSuccessStatusCode)
        {
            var body = await res.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning("AI service error {StatusCode}: {Body}", (int)res.StatusCode, body);
            throw new InvalidOperationException($"AI service call failed: {(int)res.StatusCode}");
        }

        var parsed = await res.Content.ReadFromJsonAsync<AiIndustrialIngredientPrepResponse>(cancellationToken: cancellationToken);
        return parsed ?? new AiIndustrialIngredientPrepResponse();
    }
}

