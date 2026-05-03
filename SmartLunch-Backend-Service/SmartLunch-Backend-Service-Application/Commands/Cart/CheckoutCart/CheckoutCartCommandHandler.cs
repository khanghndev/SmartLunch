using MediatR;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Orders;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Commands.Cart.CheckoutCart;

public class CheckoutCartCommandHandler : IRequestHandler<CheckoutCartCommand, GetOrderResponse>
{
    private const int MinimumOrderQuantity = 20;
    private readonly ICartCacheService _cartCache;
    private readonly IOrderRepository _orderRepository;
    private readonly IDishRepository _dishRepository;

    public CheckoutCartCommandHandler(
        ICartCacheService cartCache,
        IOrderRepository orderRepository,
        IDishRepository dishRepository)
    {
        _cartCache = cartCache;
        _orderRepository = orderRepository;
        _dishRepository = dishRepository;
    }

    public async Task<GetOrderResponse> Handle(CheckoutCartCommand command, CancellationToken cancellationToken)
    {
        var cart = await _cartCache.GetAsync(command.UserId, cancellationToken);
        if (cart == null || cart.UserId != command.UserId || cart.Items.Count == 0)
            throw new InvalidOperationException("Cart is empty.");

        if (cart.Items.Any(i => i.Quantity < MinimumOrderQuantity))
            throw new ArgumentException($"Each item quantity must be at least {MinimumOrderQuantity}.");

        var dishIds = cart.Items.Select(i => i.DishId).Distinct().ToList();
        var dishes = await _dishRepository.GetByIdsAsync(dishIds, cancellationToken);
        if (dishes.Count != dishIds.Count)
            throw new ArgumentException("One or more dishes are not available anymore.");

        var dishById = dishes.ToDictionary(d => d.Id);
        foreach (var d in dishes)
        {
            if (!d.IsActive)
                throw new ArgumentException($"Dish '{d.Name}' is not available.");
        }

        var scheduledDate = command.Request.ScheduledDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var scheduledUtc = DateTime.SpecifyKind(scheduledDate.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);

        var order = new Order
        {
            UserId = command.UserId,
            OrderDate = DateTime.UtcNow,
            ScheduledDate = scheduledUtc,
            Status = OrderLifecycleStatus.Pending,
            PaymentStatus = OrderPaymentStatus.Unpaid,
            CreatedAt = DateTime.UtcNow
        };

        decimal total = 0;
        foreach (var line in cart.Items)
        {
            var dish = dishById[line.DishId];
            var unitPrice = dish.Price;
            var lineTotal = decimal.Round(unitPrice * line.Quantity, 2, MidpointRounding.AwayFromZero);
            total += lineTotal;

            order.OrderItems.Add(new OrderItem
            {
                DishId = line.DishId,
                Quantity = line.Quantity,
                UnitPrice = unitPrice,
                TotalPrice = lineTotal
            });
        }

        order.TotalAmount = decimal.Round(total, 2, MidpointRounding.AwayFromZero);

        await _orderRepository.AddAsync(order, cancellationToken);
        await _orderRepository.CommitAsync();
        await _cartCache.RemoveAsync(command.UserId, cancellationToken);

        var reloaded = await _orderRepository.GetByIdWithDetailsAsync(order.Id)
            ?? throw new InvalidOperationException("Order created but failed to reload.");

        return new GetOrderResponse { Order = OrderDtoMapping.ToDto(reloaded) };
    }
}
