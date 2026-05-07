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
    private readonly IWeeklyMenuRepository _weeklyMenuRepository;

    public AddCartLineCommandHandler(ICartCacheService cartCache, IWeeklyMenuRepository weeklyMenuRepository)
    {
        _cartCache = cartCache;
        _weeklyMenuRepository = weeklyMenuRepository;
    }

    public async Task<GetShoppingCartResponse> Handle(AddCartLineCommand command, CancellationToken cancellationToken)
    {
        var req = command.Request;
        if (req.WeeklyMenuId <= 0)
            throw new ArgumentException("WeeklyMenuId is required.");
        if (req.Quantity < MinimumOrderQuantity)
            throw new ArgumentException($"Quantity must be at least {MinimumOrderQuantity}.");

        var weeklyMenu = await _weeklyMenuRepository.GetByIdAsync(req.WeeklyMenuId);
        if (weeklyMenu == null)
            throw new KeyNotFoundException("WeeklyMenu was not found.");

        var cart = await LoadOrCreateAsync(command.UserId, cancellationToken);

        if (req.OrganizationId.HasValue && req.OrganizationId.Value > 0)
        {
            if (cart.OrganizationId.HasValue && cart.OrganizationId.Value != req.OrganizationId.Value)
                throw new InvalidOperationException("Cart is bound to a different organization. Clear the cart first.");
            cart.OrganizationId ??= req.OrganizationId;
        }

        var existing = cart.Items.FirstOrDefault(i => i.WeeklyMenuId == req.WeeklyMenuId);
        if (existing != null)
        {
            existing.Quantity += req.Quantity;
            existing.UnitPrice = 0;
            existing.WeeklyMenuName = weeklyMenu.Description ?? weeklyMenu.Code ?? $"WeeklyMenu#{weeklyMenu.Id}";
        }
        else
        {
            cart.Items.Add(new ShoppingCartLineDto
            {
                Id = cart.Items.Any() ? cart.Items.Max(i => i.Id) + 1 : 1,
                WeeklyMenuId = weeklyMenu.Id,
                WeeklyMenuName = weeklyMenu.Description ?? weeklyMenu.Code ?? $"WeeklyMenu#{weeklyMenu.Id}",
                UnitPrice = 0,
                Quantity = req.Quantity,
                LineTotal = 0
            });
        }

        ShoppingCartMath.RecalculateTotals(cart);
        await _cartCache.SaveAsync(cart, cancellationToken);

        return new GetShoppingCartResponse { Cart = cart };
    }

    private async Task<ShoppingCartDto> LoadOrCreateAsync(int userId, CancellationToken cancellationToken)
    {
        var cart = await _cartCache.GetAsync(userId, cancellationToken);
        if (cart == null || cart.UserId != userId)
        {
            return new ShoppingCartDto
            {
                UserId = userId,
                OrganizationId = null,
                Items = new List<ShoppingCartLineDto>(),
                TotalAmount = 0,
                UpdatedAtUtc = DateTime.UtcNow
            };
        }

        return cart;
    }
}
