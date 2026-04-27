using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.Cart;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Commands.Cart.ClearShoppingCart;

public class ClearShoppingCartCommandHandler : IRequestHandler<ClearShoppingCartCommand, GetShoppingCartResponse>
{
    private readonly ICartCacheService _cartCache;

    public ClearShoppingCartCommandHandler(ICartCacheService cartCache)
    {
        _cartCache = cartCache;
    }

    public async Task<GetShoppingCartResponse> Handle(ClearShoppingCartCommand command, CancellationToken cancellationToken)
    {
        await _cartCache.RemoveAsync(command.UserId, cancellationToken);

        var empty = new ShoppingCartDto
        {
            UserId = command.UserId,
            OrganizationId = null,
            Items = new List<ShoppingCartLineDto>(),
            TotalAmount = 0,
            UpdatedAtUtc = DateTime.UtcNow
        };

        return new GetShoppingCartResponse { Cart = empty };
    }
}
