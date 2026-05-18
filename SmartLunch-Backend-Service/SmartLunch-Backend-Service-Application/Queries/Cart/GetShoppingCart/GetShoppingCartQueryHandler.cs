using MediatR;
using SmartLunch.Backend.Service.Application.Cart;
using SmartLunch.Backend.Service.Application.DTOs.Response.Cart;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.Cart.GetShoppingCart;

public class GetShoppingCartQueryHandler : IRequestHandler<GetShoppingCartQuery, GetShoppingCartResponse>
{
    private readonly ICartCacheService _cartCache;

    public GetShoppingCartQueryHandler(ICartCacheService cartCache)
    {
        _cartCache = cartCache;
    }

    public async Task<GetShoppingCartResponse> Handle(GetShoppingCartQuery request, CancellationToken cancellationToken)
    {
        var cart = await _cartCache.GetAsync(request.UserId, cancellationToken);
        if (cart == null || cart.UserId != request.UserId)
            cart = CreateEmpty(request.UserId);
        else
            ShoppingCartMath.RecalculateTotals(cart);

        return new GetShoppingCartResponse { Cart = cart };
    }

    private static ShoppingCartDto CreateEmpty(int userId)
    {
        return new ShoppingCartDto
        {
            UserId = userId,
            OrganizationId = null,
            Items = new List<ShoppingCartLineDto>(),
            TotalAmount = 0,
            UpdatedAtUtc = VietnamTime.Now
        };
    }
}
