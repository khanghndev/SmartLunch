using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.OrganizationMealOrders;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Domain.Time;

namespace SmartLunch.Backend.Service.Application.OrganizationMealContractOrders;

/// <summary>Auto random 5–10 món chính / tuần, phân bổ theo từng ngày phục vụ.</summary>
public sealed class OrganizationMealContractWeeklyJobService
{
    private readonly IContractRepository _contractRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IDishRepository _dishRepository;

    public OrganizationMealContractWeeklyJobService(
        IContractRepository contractRepository,
        IOrderRepository orderRepository,
        IDishRepository dishRepository)
    {
        _contractRepository = contractRepository;
        _orderRepository = orderRepository;
        _dishRepository = dishRepository;
    }

    public async Task<int> AutoFillNextWeekAsync(CancellationToken cancellationToken = default)
    {
        var today = VietnamTime.Today;
        var nextMonday = OrganizationMealPeriodContractCalculator.GetWeekMonday(today).AddDays(7);
        var filled = 0;

        var contracts = await _contractRepository.GetActivePeriodBasedContractsAsync(cancellationToken);
        foreach (var contract in contracts)
        {
            if (contract.WeeklyAutoFillWeekStart == nextMonday)
                continue;

            if (await _orderRepository.ContractWeekHasMainItemsAsync(contract.Id, nextMonday, cancellationToken))
            {
                contract.WeeklyAutoFillWeekStart = nextMonday;
                contract.UpdatedAt = VietnamTime.Now;
                await _contractRepository.UpdateAsync(contract);
                continue;
            }

            var created = await TryAutoFillWeekAsync(contract, nextMonday, cancellationToken);
            if (created)
            {
                contract.WeeklyAutoFillWeekStart = nextMonday;
                contract.UpdatedAt = VietnamTime.Now;
                await _contractRepository.UpdateAsync(contract);
                filled++;
            }
        }

        return filled;
    }

    private async Task<bool> TryAutoFillWeekAsync(
        Contract contract,
        DateOnly weekMonday,
        CancellationToken cancellationToken)
    {
        if (!contract.MealsPerDay.HasValue || contract.MealsPerDay < 1)
            return false;

        if (!contract.SourceOrderId.HasValue)
            return false;

        var source = await _orderRepository.GetByIdAsync(contract.SourceOrderId.Value);
        if (source?.UserId is not int userId)
            return false;

        var contractStart = DateOnly.FromDateTime(contract.StartDate);
        if (!contract.EndDate.HasValue)
            return false;
        var contractEnd = DateOnly.FromDateTime(contract.EndDate.Value);
        var excluded = contract.ExcludedDates.Select(e => e.ExcludedDate).ToList();
        var serviceDates = OrganizationMealPeriodContractCalculator
            .GetWeekServiceDates(weekMonday, contractStart, contractEnd, excluded)
            .ToList();

        if (serviceDates.Count == 0)
            return false;

        var mainDishes = await _dishRepository.GetDishesAsync(1, 500, isActive: true, category: "main");
        var candidates = mainDishes.Dishes;
        if (candidates.Count == 0)
            return false;

        var mealDays = OrganizationMealWeeklyMealPlanBuilder.BuildRandomWeeklyMainMealDays(
            serviceDates,
            contract.MealsPerDay.Value,
            candidates);

        await PersistWeeklySelectionAsync(contract, userId, weekMonday, mealDays, cancellationToken);
        return true;
    }

    private async Task PersistWeeklySelectionAsync(
        Contract contract,
        int userId,
        DateOnly weekMonday,
        List<DTOs.Request.OrganizationMealOrders.OrganizationMealDayRequest> mealDays,
        CancellationToken cancellationToken)
    {
        var mergedDays = OrganizationMealWeeklyMealPlanBuilder.MergeMainOnlyMealDays(
            mealDays,
            contract.MealsPerDay);
        var mealUnitPrice = contract.MealUnitPrice ?? 0m;
        var scheduledUtc = VietnamTime.CalendarDateMidnight(weekMonday);

        var weekOrder = await _orderRepository.GetContractWeekOrderAsync(contract.Id, weekMonday, cancellationToken);
        if (weekOrder == null)
        {
            weekOrder = new Order
            {
                UserId = userId,
                ContractId = contract.Id,
                OrderDate = VietnamTime.Now,
                ScheduledDate = scheduledUtc,
                Status = OrderLifecycleStatus.Confirmed,
                PaymentStatus = OrderPaymentStatus.Paid,
                TotalAmount = 0,
                CreatedAt = VietnamTime.Now,
                InvoiceCode = $"Tuan-{weekMonday:yyyyMMdd}-{contract.Id}-AUTO",
            };

            if (contract.SourceOrderId is int sourceId)
            {
                var source = await _orderRepository.GetByIdAsync(sourceId);
                if (source != null)
                {
                    OrganizationMealDeliveryValidator.ApplyToOrder(weekOrder, new OrganizationMealOrderDraftDelivery
                    {
                        RecipientName = source.RecipientName ?? "",
                        RecipientPhone = source.RecipientPhone ?? "",
                        RecipientEmail = source.RecipientEmail ?? "",
                        DeliveryAddress = source.DeliveryAddress ?? "",
                        DeliveryWardDistrict = source.DeliveryWardDistrict,
                        DeliveryNotes = "[Auto-fill món chính]",
                        PreferredDeliveryTime = source.PreferredDeliveryTime,
                    });
                }
            }

            await _orderRepository.AddAsync(weekOrder, cancellationToken);
            await _orderRepository.CommitAsync();
            weekOrder = await _orderRepository.GetContractWeekOrderAsync(contract.Id, weekMonday, cancellationToken)
                ?? weekOrder;
        }

        weekOrder.OrderItems.Clear();
        foreach (var day in mergedDays.Values.OrderBy(d => d.ServiceDate))
        {
            foreach (var line in day.Main)
            {
                weekOrder.OrderItems.Add(new OrderItem
                {
                    DishId = line.DishId,
                    Quantity = line.Quantity,
                    UnitPrice = mealUnitPrice,
                    TotalPrice = decimal.Round(mealUnitPrice * line.Quantity, 2, MidpointRounding.AwayFromZero),
                    ServiceDate = day.ServiceDate,
                });
            }
        }

        weekOrder.UpdatedAt = VietnamTime.Now;
        await _orderRepository.CommitAsync();
    }
}
