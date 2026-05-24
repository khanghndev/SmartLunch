using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Common.Caching;

public static class MasterDataCacheInvalidation
{
    private static readonly TimeSpan VersionTtl = TimeSpan.FromDays(30);

    public static async Task InvalidateDishCachesAsync(
        ICacheService cache,
        int dishId,
        CancellationToken cancellationToken = default)
    {
        await cache.RemoveAsync(MasterDataCacheKeys.Dish(dishId), cancellationToken);
        await BumpDishesListVersionAsync(cache, cancellationToken);
    }

    public static async Task<long> GetDishesListVersionAsync(
        ICacheService cache,
        CancellationToken cancellationToken = default)
    {
        var entry = await cache.GetAsync<CacheVersionEntry>(
            MasterDataCacheKeys.DishesListVersion,
            cancellationToken);
        return entry?.Value ?? 0;
    }

    public static async Task BumpDishesListVersionAsync(
        ICacheService cache,
        CancellationToken cancellationToken = default)
    {
        var next = await GetDishesListVersionAsync(cache, cancellationToken) + 1;
        await cache.SetAsync(
            MasterDataCacheKeys.DishesListVersion,
            new CacheVersionEntry { Value = next },
            VersionTtl,
            cancellationToken);
    }

    private sealed class CacheVersionEntry
    {
        public long Value { get; set; }
    }
}
