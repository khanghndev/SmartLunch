using MediatR;
using SmartLunch.Backend.Service.Application.Cart;
using SmartLunch.Backend.Service.Application.DTOs.Response.Cart;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Commands.Cart.RemoveCartLine;

public class RemoveCartLineCommandHandler : IRequestHandler<RemoveCartLineCommand, GetShoppingCartResponse>
{
    private readonly ICartCacheService _cartCache;

    public RemoveCartLineCommandHandler(ICartCacheService cartCache)
    {
        _cartCache = cartCache;
    }

    public async Task<GetShoppingCartResponse> Handle(RemoveCartLineCommand command, CancellationToken cancellationToken)
    {
        if (command.LineId <= 0)
            throw new ArgumentException("LineId is required.");

        var cart = await _cartCache.GetAsync(command.UserId, cancellationToken);
        if (cart == null || cart.UserId != command.UserId)
            throw new KeyNotFoundException("Cart not found.");

        var removed = cart.Items.RemoveAll(i => i.Id == command.LineId);
        if (removed == 0)
            throw new KeyNotFoundException("Cart line not found.");

        if (cart.Items.Count == 0)
            cart.OrganizationId = null;

        ShoppingCartMath.RecalculateTotals(cart);
        await _cartCache.SaveAsync(cart, cancellationToken);

        return new GetShoppingCartResponse { Cart = cart };
    }
}
