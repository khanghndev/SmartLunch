using MediatR;
using SmartLunch.Backend.Service.Application.Cart;
using SmartLunch.Backend.Service.Application.DTOs.Request.Cart;
using SmartLunch.Backend.Service.Application.DTOs.Response.Cart;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Commands.Cart.AddCartLine;

public class AddCartLineCommandHandler : IRequestHandler<AddCartLineCommand, GetShoppingCartResponse>
{
    private const int MinimumOrderQuantity = 20;
    private readonly ICartCacheService _cartCache;
    private readonly IDishRepository _dishRepository;

    public AddCartLineCommandHandler(ICartCacheService cartCache, IDishRepository dishRepository)
    {
        _cartCache = cartCache;
        _dishRepository = dishRepository;
    }

    public async Task<GetShoppingCartResponse> Handle(AddCartLineCommand command, CancellationToken cancellationToken)
    {
        var req = command.Request;
        if (req.DishId == Guid.Empty)
            throw new ArgumentException("DishId is required.");
        if (req.Quantity < MinimumOrderQuantity)
            throw new ArgumentException($"Quantity must be at least {MinimumOrderQuantity}.");

        var dish = await _dishRepository.GetByIdAsync(req.DishId);
        if (dish == null)
            throw new KeyNotFoundException("Dish was not found.");
        if (!dish.IsActive)
            throw new InvalidOperationException("Dish is not available.");

        var cart = await LoadOrCreateAsync(command.UserId, cancellationToken);

        if (req.UnitId.HasValue && req.UnitId.Value != Guid.Empty)
        {
            if (cart.UnitId.HasValue && cart.UnitId.Value != req.UnitId.Value)
                throw new InvalidOperationException("Cart is bound to a different unit. Clear the cart first.");
            cart.UnitId ??= req.UnitId;
        }

        var existing = cart.Items.FirstOrDefault(i => i.DishId == req.DishId);
        if (existing != null)
        {
            existing.Quantity += req.Quantity;
            existing.UnitPrice = dish.Price;
            existing.DishName = dish.Name;
        }
        else
        {
            cart.Items.Add(new ShoppingCartLineDto
            {
                Id = Guid.NewGuid(),
                DishId = dish.Id,
                DishName = dish.Name,
                UnitPrice = dish.Price,
                Quantity = req.Quantity,
                LineTotal = 0
            });
        }

        ShoppingCartMath.RecalculateTotals(cart);
        await _cartCache.SaveAsync(cart, cancellationToken);

        return new GetShoppingCartResponse { Cart = cart };
    }

    private async Task<ShoppingCartDto> LoadOrCreateAsync(Guid userId, CancellationToken cancellationToken)
    {
        var cart = await _cartCache.GetAsync(userId, cancellationToken);
        if (cart == null || cart.UserId != userId)
        {
            return new ShoppingCartDto
            {
                UserId = userId,
                UnitId = null,
                Items = new List<ShoppingCartLineDto>(),
                TotalAmount = 0,
                UpdatedAtUtc = DateTime.UtcNow
            };
        }

        return cart;
    }
}
