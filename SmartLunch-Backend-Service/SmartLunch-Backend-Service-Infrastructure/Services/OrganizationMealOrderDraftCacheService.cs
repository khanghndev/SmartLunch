using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.OrganizationMealOrders;

namespace SmartLunch.Backend.Service.Infrastructure.Services;

public sealed class OrganizationMealOrderDraftCacheService : IOrganizationMealOrderDraftCache
{
    private readonly ICacheService _cache;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OrganizationMealOrderDraftCacheService> _logger;

    public OrganizationMealOrderDraftCacheService(
        ICacheService cache,
        IConfiguration configuration,
        ILogger<OrganizationMealOrderDraftCacheService> logger)
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
            if (int.TryParse(_configuration["OrgMealDraft:TtlMinutes"], out var parsed))
                minutes = parsed;
            return TimeSpan.FromMinutes(Math.Clamp(minutes, 15, 24 * 60));
        }
    }

    private string KeyPrefix => string.IsNullOrWhiteSpace(_configuration["OrgMealDraft:KeyPrefix"])
        ? "org-meal-draft"
        : _configuration["OrgMealDraft:KeyPrefix"]!.Trim();

    private static string BuildKey(string prefix, int userId, string draftId) =>
        $"{prefix}:user:{userId}:draft:{draftId}";

    public Task<OrganizationMealOrderDraftPayload?> GetAsync(int userId, string draftId, CancellationToken cancellationToken = default)
    {
        var key = BuildKey(KeyPrefix, userId, draftId);
        return _cache.GetAsync<OrganizationMealOrderDraftPayload>(key, cancellationToken);
    }

    public async Task SaveAsync(int userId, string draftId, OrganizationMealOrderDraftPayload payload, CancellationToken cancellationToken = default)
    {
        var key = BuildKey(KeyPrefix, userId, draftId);
        await _cache.SetAsync(key, payload, Ttl, cancellationToken);
        _logger.LogDebug("Org meal draft saved user={UserId} draft={DraftId}", userId, draftId);
    }

    public Task RemoveAsync(int userId, string draftId, CancellationToken cancellationToken = default)
    {
        var key = BuildKey(KeyPrefix, userId, draftId);
        return _cache.RemoveAsync(key, cancellationToken);
    }
}
