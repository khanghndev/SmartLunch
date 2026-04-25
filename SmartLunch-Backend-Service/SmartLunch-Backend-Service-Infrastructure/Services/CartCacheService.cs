using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.Cart;
using SmartLunch.Backend.Service.Application.DTOs.Response.Cart;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Infrastructure.Services;

public class CartCacheService : ICartCacheService
{
    private readonly ICacheService _cache;
    private readonly IConfiguration _configuration;
    private readonly ILogger<CartCacheService> _logger;

    public CartCacheService(
        ICacheService cache,
        IConfiguration configuration,
        ILogger<CartCacheService> logger)
    {
        _cache = cache;
        _configuration = configuration;
        _logger = logger;
    }

    public TimeSpan CacheTtl
    {
        get
        {
            var days = 30;
            if (int.TryParse(_configuration["CartCache:TtlDays"], out var parsed))
                days = parsed;
            return TimeSpan.FromDays(Math.Clamp(days, 1, 365));
        }
    }

    private string KeyPrefix => string.IsNullOrWhiteSpace(_configuration["CartCache:KeyPrefix"])
        ? "cart"
        : _configuration["CartCache:KeyPrefix"]!.Trim();

    private static string BuildKey(string prefix, Guid userId) => $"{prefix}:user:{userId:D}";

    public Task<ShoppingCartDto?> GetAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var key = BuildKey(KeyPrefix, userId);
        return _cache.GetAsync<ShoppingCartDto>(key, cancellationToken);
    }

    public async Task SaveAsync(ShoppingCartDto cart, CancellationToken cancellationToken = default)
    {
        var key = BuildKey(KeyPrefix, cart.UserId);
        cart.UpdatedAtUtc = DateTime.UtcNow;
        ShoppingCartMath.RecalculateTotals(cart);
        await _cache.SetAsync(key, cart, CacheTtl, cancellationToken);
        _logger.LogDebug("Cart saved to cache for user {UserId}, lines={Count}", cart.UserId, cart.Items.Count);
    }

    public Task RemoveAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var key = BuildKey(KeyPrefix, userId);
        return _cache.RemoveAsync(key, cancellationToken);
    }

}
