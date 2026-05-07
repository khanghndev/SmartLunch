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
    private readonly IWeeklyMenuRepository _weeklyMenuRepository;
    private readonly IContractRepository _contractRepository;
    private readonly IPartnerRepository _partnerRepository;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CheckoutCartCommandHandler(
        ICartCacheService cartCache,
        IOrderRepository orderRepository,
        IDishRepository dishRepository,
        IWeeklyMenuRepository weeklyMenuRepository,
        IContractRepository contractRepository,
        IPartnerRepository partnerRepository,
        IOrganizationRepository organizationRepository,
        IUnitOfWork unitOfWork)
    {
        _cartCache = cartCache;
        _orderRepository = orderRepository;
        _dishRepository = dishRepository;
        _weeklyMenuRepository = weeklyMenuRepository;
        _contractRepository = contractRepository;
        _partnerRepository = partnerRepository;
        _organizationRepository = organizationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<GetOrderResponse> Handle(CheckoutCartCommand command, CancellationToken cancellationToken)
    {
        var cart = await _cartCache.GetAsync(command.UserId, cancellationToken);
        if (cart == null || cart.UserId != command.UserId || cart.Items.Count == 0)
            throw new InvalidOperationException("Cart is empty.");

        if (cart.Items.Any(i => i.Quantity < MinimumOrderQuantity))
            throw new ArgumentException($"Each item quantity must be at least {MinimumOrderQuantity}.");

        var scheduledDate = command.Request.ScheduledDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var scheduledUtc = DateTime.SpecifyKind(scheduledDate.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);

        // Build order items from weekly menu(s) for the scheduled date
        var weeklyMenuIds = cart.Items.Select(i => i.WeeklyMenuId).Distinct().ToList();
        if (weeklyMenuIds.Any(id => id <= 0))
            throw new ArgumentException("WeeklyMenuId is required.");

        var dishIdToQuantity = new Dictionary<int, int>();
        foreach (var line in cart.Items)
        {
            var weeklyMenu = await _weeklyMenuRepository.GetByIdWithSchedulesAsync(line.WeeklyMenuId, cancellationToken);
            if (weeklyMenu == null)
                throw new ArgumentException($"WeeklyMenu '{line.WeeklyMenuId}' is not available anymore.");

            var schedules = weeklyMenu.MenuSchedules
                .Where(ms => ms.Date.Date == scheduledUtc.Date)
                .ToList();

            if (schedules.Count == 0)
                throw new InvalidOperationException(
                    $"WeeklyMenu '{weeklyMenu.Id}' has no schedules for date {scheduledDate:yyyy-MM-dd}.");

            foreach (var schedule in schedules)
            {
                if (!dishIdToQuantity.TryGetValue(schedule.DishId, out var q))
                    q = 0;
                dishIdToQuantity[schedule.DishId] = q + line.Quantity;
            }
        }

        var dishIds = dishIdToQuantity.Keys.Distinct().ToList();
        var dishes = await _dishRepository.GetByIdsAsync(dishIds, cancellationToken);
        if (dishes.Count != dishIds.Count)
            throw new ArgumentException("One or more dishes are not available anymore.");

        var dishById = dishes.ToDictionary(d => d.Id);
        foreach (var d in dishes)
        {
            if (!d.IsActive)
                throw new ArgumentException($"Dish '{d.Name}' is not available.");
        }

        // If this cart is for an organization, bind to (or create) an order-based contract
        Contract? contract = null;
        if (cart.OrganizationId.HasValue && cart.OrganizationId.Value > 0)
        {
            var org = await _organizationRepository.GetByIdAsync(cart.OrganizationId.Value);
            if (org == null || !org.IsActive)
                throw new ArgumentException("Organization is not available.");

            contract = await _contractRepository.GetActiveForOrganizationAsync(org.Id, cancellationToken);
            if (contract == null)
            {
                var (partners, _) = await _partnerRepository.GetPartnersAsync(
                    page: 1,
                    pageSize: 1,
                    searchTerm: null,
                    isActive: true);

                var partner = partners.FirstOrDefault()
                    ?? throw new InvalidOperationException("No active partner found to create contract.");

                contract = new Contract
                {
                    PartnerId = partner.Id,
                    OrganizationId = org.Id,
                    ContractType = "Order-Based",
                    Description = $"Auto-created contract for order on {scheduledDate:yyyy-MM-dd}",
                    SupplySchedule = null,
                    StartDate = DateTime.UtcNow.Date,
                    EndDate = null,
                    TotalValue = null,
                    DepositAmount = null,
                    Status = "active",
                    CreatedAt = DateTime.UtcNow
                };
            }
        }

        var order = new Order
        {
            UserId = command.UserId,
            ContractId = contract?.Id,
            OrderDate = DateTime.UtcNow,
            ScheduledDate = scheduledUtc,
            Status = OrderLifecycleStatus.Pending,
            PaymentStatus = OrderPaymentStatus.Unpaid,
            CreatedAt = DateTime.UtcNow
        };

        decimal total = 0;
        foreach (var (dishId, quantity) in dishIdToQuantity)
        {
            var dish = dishById[dishId];
            var unitPrice = dish.Price;
            var lineTotal = decimal.Round(unitPrice * quantity, 2, MidpointRounding.AwayFromZero);
            total += lineTotal;

            order.OrderItems.Add(new OrderItem
            {
                DishId = dishId,
                Quantity = quantity,
                UnitPrice = unitPrice,
                TotalPrice = lineTotal
            });
        }

        order.TotalAmount = decimal.Round(total, 2, MidpointRounding.AwayFromZero);

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            // Contract creation (if needed) may call SaveChanges internally,
            // but it will still participate in the same EF Core transaction.
            if (contract != null && contract.Id == 0)
                contract = await _contractRepository.CreateAsync(contract);

            order.ContractId = contract?.Id;
            await _orderRepository.AddAsync(order, cancellationToken);

            await _unitOfWork.CommitAsync();
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }

        // Non-DB side effect: remove cart only after DB commit succeeds
        await _cartCache.RemoveAsync(command.UserId, cancellationToken);

        var reloaded = await _orderRepository.GetByIdWithDetailsAsync(order.Id)
            ?? throw new InvalidOperationException("Order created but failed to reload.");

        return new GetOrderResponse { Order = OrderDtoMapping.ToDto(reloaded) };
    }
}
