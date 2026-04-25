using SmartLunch.Backend.Service.Application.DTOs.Response.Cart;

namespace SmartLunch.Backend.Service.Application.Interfaces;

/// <summary>Lưu giỏ hàng trên Redis (distributed cache) với TTL dài hạn.</summary>
public interface ICartCacheService
{
    TimeSpan CacheTtl { get; }

    Task<ShoppingCartDto?> GetAsync(int userId, CancellationToken cancellationToken = default);

    Task SaveAsync(ShoppingCartDto cart, CancellationToken cancellationToken = default);

    Task RemoveAsync(int userId, CancellationToken cancellationToken = default);
}
