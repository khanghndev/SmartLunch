using MediatR;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Orders;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Application.Helpers;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Orders.CreateSalesInvoice;

public class CreateSalesInvoiceCommandHandler : IRequestHandler<CreateSalesInvoiceCommand, GetOrderResponse>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IDishRepository _dishRepository;

    public CreateSalesInvoiceCommandHandler(
        IOrderRepository orderRepository,
        IOrganizationRepository organizationRepository,
        IUserRepository userRepository,
        IDishRepository dishRepository)
    {
        _orderRepository = orderRepository;
        _organizationRepository = organizationRepository;
        _userRepository = userRepository;
        _dishRepository = dishRepository;
    }

    public async Task<GetOrderResponse> Handle(CreateSalesInvoiceCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;

        if (req.OrganizationId == 0)
            throw new ArgumentException("OrganizationId is required.");

        if (req.Lines == null || req.Lines.Count == 0)
            throw new ArgumentException("At least one line item is required.");

        foreach (var line in req.Lines)
        {
            if (line.DishId == 0)
                throw new ArgumentException("Each line must include a valid DishId.");
            if (line.Quantity < 1)
                throw new ArgumentException("Quantity must be at least 1 for each line.");
        }

        var organization = await _organizationRepository.GetByIdAsync(req.OrganizationId);
        if (organization == null)
            throw new ArgumentException("Organization was not found.");

        if (req.UserId.HasValue && req.UserId.Value != 0)
        {
            var customer = await _userRepository.GetByIdAsync(req.UserId.Value);
            if (customer == null)
                throw new ArgumentException("Customer user was not found.");
            if (!customer.IsActive)
                throw new ArgumentException("Customer user is inactive.");
        }

        var scheduledDate = req.ScheduledDate == default
            ? DateOnly.FromDateTime(DateTime.UtcNow)
            : req.ScheduledDate;

        var scheduledUtc = scheduledDate.ToDateTime(TimeOnly.MinValue);
        if (scheduledUtc.Kind == DateTimeKind.Unspecified)
            scheduledUtc = DateTime.SpecifyKind(scheduledUtc, DateTimeKind.Utc);

        // --- CUT-OFF TIME VALIDATION ---
        CutOffTimeValidator.Validate(organization.Type, scheduledUtc, DateTime.UtcNow);

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

        var invoiceCode = await AllocateInvoiceCodeAsync(scheduledDate, cancellationToken);

        var order = new Order
        {
            UserId = req.UserId is { } uid && uid != 0 ? uid : null,
            OrderDate = DateTime.UtcNow,
            ScheduledDate = scheduledUtc,
            Status = OrderLifecycleStatus.Confirmed,
            TotalAmount = total,
            PaymentStatus = OrderPaymentStatus.Unpaid,
            CreatedAt = DateTime.UtcNow,
            InvoiceCode = invoiceCode,
            CreatedBySalesUserId = request.SalesUserId
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
                TotalPrice = lineTotal
            });
        }

        await _orderRepository.AddAsync(order, cancellationToken);
        await _orderRepository.CommitAsync();

        var refreshed = await _orderRepository.GetByIdWithDetailsAsync(order.Id);
        if (refreshed == null)
            return new GetOrderResponse { Order = new OrderDto() };

        return new GetOrderResponse { Order = OrderDtoMapping.ToDto(refreshed) };
    }

    private async Task<string> AllocateInvoiceCodeAsync(DateOnly scheduledDate, CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < 12; attempt++)
        {
            var suffix = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
            var candidate = $"HD-{scheduledDate:yyyyMMdd}-{suffix}";
            if (!await _orderRepository.InvoiceCodeExistsAsync(candidate, cancellationToken))
                return candidate;
        }

        throw new InvalidOperationException("Could not allocate a unique invoice code.");
    }
}
