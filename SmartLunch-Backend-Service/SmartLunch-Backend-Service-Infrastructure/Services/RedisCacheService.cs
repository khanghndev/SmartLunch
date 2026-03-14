using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Infrastructure.Services;

public class RedisCacheService : ICacheService
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<RedisCacheService> _logger;

    public RedisCacheService(IDistributedCache distributedCache, ILogger<RedisCacheService> logger)
    {
        _distributedCache = distributedCache;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var cachedJson = await _distributedCache.GetStringAsync(key, cancellationToken);
            if (string.IsNullOrWhiteSpace(cachedJson))
            {
                _logger.LogDebug("Cache miss for key {CacheKey}", key);
                return null;
            }

            var cachedValue = JsonSerializer.Deserialize<T>(cachedJson, SerializerOptions);
            if (cachedValue is null)
            {
                _logger.LogWarning("Cache entry for key {CacheKey} could not be deserialized", key);
                await _distributedCache.RemoveAsync(key, cancellationToken);
                return null;
            }

            _logger.LogDebug("Cache hit for key {CacheKey}", key);
            return cachedValue;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to read cache for key {CacheKey}", key);
            return null;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var serializedValue = JsonSerializer.Serialize(value, SerializerOptions);
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            };

            await _distributedCache.SetStringAsync(key, serializedValue, cacheOptions, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to write cache for key {CacheKey}", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _distributedCache.RemoveAsync(key, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to remove cache for key {CacheKey}", key);
        }
    }

    public async Task<T> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan expiration,
        CancellationToken cancellationToken = default) where T : class
    {
        var cachedValue = await GetAsync<T>(key, cancellationToken);
        if (cachedValue is not null)
        {
            return cachedValue;
        }

        var createdValue = await factory(cancellationToken);
        await SetAsync(key, createdValue, expiration, cancellationToken);

        return createdValue;
    }
}
