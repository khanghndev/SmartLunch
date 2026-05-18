using MediatR;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Orders;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Orders;
using SmartLunch.Backend.Service.Application.Helpers;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.Promotions;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Domain.Time;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Orders.CreateCustomerMealOrder;

public class CreateCustomerMealOrderCommandHandler : IRequestHandler<CreateCustomerMealOrderCommand, GetOrderResponse>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUserRepository _userRepository;
    private readonly IDishRepository _dishRepository;
    private readonly IPromotionEngine _promotionEngine;
    private readonly IPromotionRepository _promotionRepository;

    public CreateCustomerMealOrderCommandHandler(
        IOrderRepository orderRepository,
        IUserRepository userRepository,
        IDishRepository dishRepository,
        IPromotionEngine promotionEngine,
        IPromotionRepository promotionRepository)
    {
        _orderRepository = orderRepository;
        _userRepository = userRepository;
        _dishRepository = dishRepository;
        _promotionEngine = promotionEngine;
        _promotionRepository = promotionRepository;
    }

    public async Task<GetOrderResponse> Handle(CreateCustomerMealOrderCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;
        if (req.Lines == null || req.Lines.Count == 0)
            throw new ArgumentException("At least one line item is required.");

        if (request.CustomerUserId <= 0)
            throw new ArgumentException("Invalid customer user.");

        var customer = await _userRepository.GetByIdAsync(request.CustomerUserId);
        if (customer == null || !customer.IsActive)
            throw new ArgumentException("Customer user was not found or is inactive.");

        foreach (var line in req.Lines)
        {
            if (line.DishId == 0)
                throw new ArgumentException("Each line must include a valid DishId.");
            if (line.Quantity < 1)
                throw new ArgumentException("Quantity must be at least 1 for each line.");
        }

        var scheduledDate = req.ScheduledDate == default
            ? DateOnly.FromDateTime(VietnamTime.Now)
            : req.ScheduledDate;

        var scheduledUtc = VietnamTime.CalendarDateMidnight(scheduledDate);

        var merged = req.Lines
            .GroupBy(l => l.DishId)
            .Select(g => (DishId: g.Key, Quantity: g.Sum(x => x.Quantity)))
            .ToList();

        var dishIds = merged.Select(m => m.DishId).ToList();
        var dishes = await _dishRepository.GetByIdsAsync(dishIds, cancellationToken);
        if (dishes.Count != dishIds.Count)
            throw new ArgumentException("One or more dishes were not found.");

        foreach (var d in dishes)
        {
            if (!d.IsActive)
                throw new ArgumentException($"Dish '{d.Name}' is not active.");
        }

        var dishById = dishes.ToDictionary(d => d.Id);
        decimal total = 0;
        foreach (var line in merged)
        {
            var price = dishById[line.DishId].Price;
            total += price * line.Quantity;
        }

        var invoiceCode = await AllocateCustomerInvoiceCodeAsync(scheduledDate, cancellationToken);

        var order = new Order
        {
            UserId = request.CustomerUserId,
            OrderDate = VietnamTime.Now,
            ScheduledDate = scheduledUtc,
            Status = OrderLifecycleStatus.Pending,
            TotalAmount = total,
            PaymentStatus = OrderPaymentStatus.Unpaid,
            CreatedAt = VietnamTime.Now,
            InvoiceCode = invoiceCode,
            CreatedBySalesUserId = null,
        };

        foreach (var line in merged)
        {
            var dish = dishById[line.DishId];
            var unitPrice = dish.Price;
            var lineTotal = unitPrice * line.Quantity;
            order.OrderItems.Add(new OrderItem
            {
                DishId = line.DishId,
                Quantity = line.Quantity,
                UnitPrice = unitPrice,
                TotalPrice = lineTotal,
            });
        }

        var promoInput = new OrderPromotionEvaluateInput
        {
            Channel = PromotionConstants.ChannelB2C,
            UserId = request.CustomerUserId,
            PromotionCode = req.PromotionCode,
            Subtotal = total,
            TotalQuantity = merged.Sum(m => m.Quantity),
            Lines = merged.Select(m =>
            {
                var dish = dishById[m.DishId];
                var lineTotal = dish.Price * m.Quantity;
                return new OrderPromotionLineInput
                {
                    DishId = m.DishId,
                    Quantity = m.Quantity,
                    LineTotal = lineTotal,
                };
            }).ToList(),
        };

        await OrderPromotionApplyHelper.EvaluateAndApplyToOrderAsync(
            order,
            promoInput,
            _promotionEngine,
            _promotionRepository,
            cancellationToken);

        await _orderRepository.AddAsync(order, cancellationToken);
        await _orderRepository.CommitAsync();

        var refreshed = await _orderRepository.GetByIdWithDetailsAsync(order.Id);
        if (refreshed == null)
            return new GetOrderResponse { Order = new OrderDto() };

        return new GetOrderResponse { Order = OrderDtoMapping.ToDto(refreshed) };
    }

    private async Task<string> AllocateCustomerInvoiceCodeAsync(DateOnly scheduledDate, CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < 12; attempt++)
        {
            var suffix = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
            var candidate = $"KH-{scheduledDate:yyyyMMdd}-{suffix}";
            if (!await _orderRepository.InvoiceCodeExistsAsync(candidate, cancellationToken))
                return candidate;
        }

        throw new InvalidOperationException("Could not allocate a unique customer invoice code.");
    }
}
