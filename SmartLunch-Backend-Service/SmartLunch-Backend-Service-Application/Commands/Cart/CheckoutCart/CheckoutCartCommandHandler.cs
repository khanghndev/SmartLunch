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
    private readonly IContractPdfService _contractPdfService;

    public CheckoutCartCommandHandler(
        ICartCacheService cartCache,
        IOrderRepository orderRepository,
        IDishRepository dishRepository,
        IWeeklyMenuRepository weeklyMenuRepository,
        IContractRepository contractRepository,
        IPartnerRepository partnerRepository,
        IOrganizationRepository organizationRepository,
        IUnitOfWork unitOfWork,
        IContractPdfService contractPdfService)
    {
        _cartCache = cartCache;
        _orderRepository = orderRepository;
        _dishRepository = dishRepository;
        _weeklyMenuRepository = weeklyMenuRepository;
        _contractRepository = contractRepository;
        _partnerRepository = partnerRepository;
        _organizationRepository = organizationRepository;
        _unitOfWork = unitOfWork;
        _contractPdfService = contractPdfService;
    }

    public async Task<GetOrderResponse> Handle(CheckoutCartCommand command, CancellationToken cancellationToken)
    {
        var cart = await _cartCache.GetAsync(command.UserId, cancellationToken);
        if (cart == null || cart.UserId != command.UserId || cart.Items.Count == 0)
            throw new InvalidOperationException("Cart is empty.");

        if (cart.Items.Any(i => i.Quantity < MinimumOrderQuantity))
            throw new ArgumentException($"Each item quantity must be at least {MinimumOrderQuantity}.");

        var excludedDays = command.Request.ExcludedDates.Count > 0
            ? command.Request.ExcludedDates.Distinct().ToHashSet()
            : new HashSet<DateOnly>();

        // Aggregate dishes from weekly menu(s), skipping excluded calendar days on the schedule
        var weeklyMenuIds = cart.Items.Select(i => i.WeeklyMenuId).Distinct().ToList();
        if (weeklyMenuIds.Any(id => id <= 0))
            throw new ArgumentException("WeeklyMenuId is required.");

        var dishIdToQuantity = new Dictionary<int, int>();
        DateTime? minIncludedScheduleUtc = null;

        foreach (var line in cart.Items)
        {
            var weeklyMenu = await _weeklyMenuRepository.GetByIdWithSchedulesAsync(line.WeeklyMenuId, cancellationToken);
            if (weeklyMenu == null)
                throw new ArgumentException($"WeeklyMenu '{line.WeeklyMenuId}' is not available anymore.");

            foreach (var schedule in weeklyMenu.MenuSchedules)
            {
                var calendarDay = DateOnly.FromDateTime(schedule.Date.Date);
                if (excludedDays.Contains(calendarDay))
                    continue;

                var dayUtc = VietnamTime.CalendarDateMidnight(calendarDay);
                if (!minIncludedScheduleUtc.HasValue || dayUtc < minIncludedScheduleUtc.Value)
                    minIncludedScheduleUtc = dayUtc;

                if (!dishIdToQuantity.TryGetValue(schedule.DishId, out var q))
                    q = 0;
                dishIdToQuantity[schedule.DishId] = q + line.Quantity;
            }
        }

        if (dishIdToQuantity.Count == 0)
            throw new InvalidOperationException(
                "Nothing left to order after applying excluded dates. Check that at least one menu day stays included.");

        var scheduledUtc = minIncludedScheduleUtc!.Value;
        var scheduledDate = DateOnly.FromDateTime(scheduledUtc);

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
        Organization? checkoutOrg = null;
        if (cart.OrganizationId.HasValue && cart.OrganizationId.Value > 0)
        {
            checkoutOrg = await _organizationRepository.GetByIdAsync(cart.OrganizationId.Value);
            if (checkoutOrg == null || !checkoutOrg.IsActive)
                throw new ArgumentException("Organization is not available.");

            contract = await _contractRepository.GetActiveForOrganizationAsync(checkoutOrg.Id, cancellationToken);
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
                    OrganizationId = checkoutOrg.Id,
                    ContractType = "Order-Based",
                    Description = $"Auto-created contract for order on {scheduledDate:yyyy-MM-dd}",
                    SupplySchedule = null,
                    StartDate = VietnamTime.Now.Date,
                    EndDate = null,
                    TotalValue = null,
                    DepositAmount = null,
                    Status = "active",
                    CreatedAt = VietnamTime.Now
                };
            }
        }

        var order = new Order
        {
            UserId = command.UserId,
            ContractId = contract?.Id,
            OrderDate = VietnamTime.Now,
            ScheduledDate = scheduledUtc,
            Status = OrderLifecycleStatus.Pending,
            PaymentStatus = OrderPaymentStatus.Unpaid,
            CreatedAt = VietnamTime.Now
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

        var shouldGenerateContractPdfAfterInsert =
            checkoutOrg != null && contract != null && contract.Id == 0;

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

        if (shouldGenerateContractPdfAfterInsert && contract != null && checkoutOrg != null)
        {
            var forPdf = await _contractRepository.GetByIdAsync(contract.Id)
                         ?? throw new InvalidOperationException("Contract created but reload failed before PDF.");

            if (forPdf.Partner != null)
            {
                var url = await _contractPdfService.GenerateUploadAndResolveUrlAsync(
                    forPdf,
                    forPdf.Partner,
                    checkoutOrg,
                    cancellationToken: cancellationToken);
                forPdf.ContractFileUrl = url;
                forPdf.UpdatedAt = VietnamTime.Now;
                await _contractRepository.UpdateAsync(forPdf);
            }
        }

        // Non-DB side effect: remove cart only after DB commit succeeds
        await _cartCache.RemoveAsync(command.UserId, cancellationToken);

        var reloaded = await _orderRepository.GetByIdWithDetailsAsync(order.Id)
            ?? throw new InvalidOperationException("Order created but failed to reload.");

        return new GetOrderResponse { Order = OrderDtoMapping.ToDto(reloaded) };
    }
}
