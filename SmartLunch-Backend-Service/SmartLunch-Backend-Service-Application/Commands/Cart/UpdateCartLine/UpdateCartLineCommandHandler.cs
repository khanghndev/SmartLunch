using MediatR;
using SmartLunch.Backend.Service.Application.Cart;
using SmartLunch.Backend.Service.Application.DTOs.Response.Cart;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Commands.Cart.UpdateCartLine;

public class UpdateCartLineCommandHandler : IRequestHandler<UpdateCartLineCommand, GetShoppingCartResponse>
{
    private const int MinimumOrderQuantity = 20;
    private readonly ICartCacheService _cartCache;

    public UpdateCartLineCommandHandler(ICartCacheService cartCache)
    {
        _cartCache = cartCache;
    }

    public async Task<GetShoppingCartResponse> Handle(UpdateCartLineCommand command, CancellationToken cancellationToken)
    {
        if (command.LineId <= 0)
            throw new ArgumentException("LineId is required.");
        if (command.Request.Quantity < MinimumOrderQuantity)
            throw new ArgumentException($"Quantity must be at least {MinimumOrderQuantity}.");

        var cart = await _cartCache.GetAsync(command.UserId, cancellationToken);
        if (cart == null || cart.UserId != command.UserId)
            throw new KeyNotFoundException("Cart not found.");

        var line = cart.Items.FirstOrDefault(i => i.Id == command.LineId);
        if (line == null)
            throw new KeyNotFoundException("Cart line not found.");

        line.Quantity = command.Request.Quantity;
        ShoppingCartMath.RecalculateTotals(cart);
        await _cartCache.SaveAsync(cart, cancellationToken);

        return new GetShoppingCartResponse { Cart = cart };
    }
}
