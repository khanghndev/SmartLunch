using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.OrganizationMealContractOrders;

namespace SmartLunch.Backend.Service.Infrastructure.Services;

public sealed class OrganizationMealPeriodContractDraftCacheService : IOrganizationMealPeriodContractDraftCache
{
    private readonly ICacheService _cache;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OrganizationMealPeriodContractDraftCacheService> _logger;

    public OrganizationMealPeriodContractDraftCacheService(
        ICacheService cache,
        IConfiguration configuration,
        ILogger<OrganizationMealPeriodContractDraftCacheService> logger)
    {
        _cache = cache;
        _configuration = configuration;
        _logger = logger;
    }

    private TimeSpan Ttl
    {
        get
        {
            var minutes = 120;
            if (int.TryParse(_configuration["OrgMealPeriodDraft:TtlMinutes"], out var parsed))
                minutes = parsed;
            return TimeSpan.FromMinutes(Math.Clamp(minutes, 15, 24 * 60));
        }
    }

    private string KeyPrefix => string.IsNullOrWhiteSpace(_configuration["OrgMealPeriodDraft:KeyPrefix"])
        ? "org-meal-period-draft"
        : _configuration["OrgMealPeriodDraft:KeyPrefix"]!.Trim();

    private static string BuildKey(string prefix, int userId, string draftId) =>
        $"{prefix}:user:{userId}:draft:{draftId}";

    public Task<OrganizationMealPeriodContractDraftPayload?> GetAsync(int userId, string draftId, CancellationToken cancellationToken = default)
    {
        var key = BuildKey(KeyPrefix, userId, draftId);
        return _cache.GetAsync<OrganizationMealPeriodContractDraftPayload>(key, cancellationToken);
    }

    public async Task SaveAsync(int userId, string draftId, OrganizationMealPeriodContractDraftPayload payload, CancellationToken cancellationToken = default)
    {
        var key = BuildKey(KeyPrefix, userId, draftId);
        await _cache.SetAsync(key, payload, Ttl, cancellationToken);
        _logger.LogDebug("Org meal period draft saved user={UserId} draft={DraftId}", userId, draftId);
    }

    public Task RemoveAsync(int userId, string draftId, CancellationToken cancellationToken = default)
    {
        var key = BuildKey(KeyPrefix, userId, draftId);
        return _cache.RemoveAsync(key, cancellationToken);
    }
}
