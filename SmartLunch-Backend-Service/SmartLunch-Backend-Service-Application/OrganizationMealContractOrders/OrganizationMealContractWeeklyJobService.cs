using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.OrganizationMealOrders;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Domain.Time;

namespace SmartLunch.Backend.Service.Application.OrganizationMealContractOrders;

/// <summary>Auto random món chính / tuần khi khách không chọn đúng hạn.</summary>
public sealed class OrganizationMealContractWeeklyJobService
{
    private readonly IContractRepository _contractRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IDishRepository _dishRepository;
    private readonly OrganizationMealContractWeeklySelectionService _weeklySelectionService;

    public OrganizationMealContractWeeklyJobService(
        IContractRepository contractRepository,
        IOrderRepository orderRepository,
        IDishRepository dishRepository,
        OrganizationMealContractWeeklySelectionService weeklySelectionService)
    {
        _contractRepository = contractRepository;
        _orderRepository = orderRepository;
        _dishRepository = dishRepository;
        _weeklySelectionService = weeklySelectionService;
    }

    public async Task<int> AutoFillOpenWeekAsync(
        DateOnly openWeekMonday,
        CancellationToken cancellationToken = default)
    {
        var filled = 0;
        var contracts = await _contractRepository.GetActivePeriodBasedContractsAsync(cancellationToken);

        foreach (var contract in contracts)
        {
            if (contract.WeeklyAutoFillWeekStart == openWeekMonday)
                continue;

            if (await _weeklySelectionService.WeekIsFilledAsync(contract.Id, openWeekMonday, cancellationToken))
            {
                contract.WeeklyAutoFillWeekStart = openWeekMonday;
                contract.UpdatedAt = VietnamTime.Now;
                await _contractRepository.UpdateAsync(contract);
                continue;
            }

            if (await TryAutoFillWeekAsync(contract, openWeekMonday, cancellationToken))
            {
                contract.WeeklyAutoFillWeekStart = openWeekMonday;
                contract.UpdatedAt = VietnamTime.Now;
                await _contractRepository.UpdateAsync(contract);
                filled++;
            }
        }

        return filled;
    }

    public async Task<(Contract Contract, DateOnly WeekMonday)?> TryAutoFillOpenWeekForContractAsync(
        Contract contract,
        CancellationToken cancellationToken = default)
    {
        if (!contract.EndDate.HasValue)
            return null;

        var contractStart = DateOnly.FromDateTime(contract.StartDate);
        var contractEnd = DateOnly.FromDateTime(contract.EndDate.Value);
        var excluded = contract.ExcludedDates.Select(e => e.ExcludedDate).ToList();
        var today = VietnamTime.Today;
        var openWeekMonday = OrganizationMealWeeklySelectionRules.ResolveOpenWeekMonday(
            today, contractStart, contractEnd, excluded);
        if (!openWeekMonday.HasValue)
            return null;

        if (await _weeklySelectionService.WeekIsFilledAsync(contract.Id, openWeekMonday.Value, cancellationToken))
            return null;

        var ok = await TryAutoFillWeekAsync(contract, openWeekMonday.Value, cancellationToken);
        return ok ? (contract, openWeekMonday.Value) : null;
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
        var dailyOverrides = OrganizationMealPeriodContractSchedule.ToOverrideDictionary(
            contract.DailyMealPortions.Select(p => new ContractDailyMealPortionSource(p.ServiceDate, p.MealCount)));
        var serviceDates = OrganizationMealPeriodContractCalculator
            .GetWeekServiceDates(weekMonday, contractStart, contractEnd, excluded)
            .ToList();

        if (serviceDates.Count == 0)
            return false;

        var mainDishes = await _dishRepository.GetDishesAsync(1, 500, isActive: true, category: "main");
        if (mainDishes.Dishes.Count == 0)
            return false;

        await _weeklySelectionService.EnsureWeeksSeededAsync(contract, cancellationToken);

        var mealDays = OrganizationMealWeeklyMealPlanBuilder.BuildRandomWeeklyMainMealDays(
            serviceDates,
            contract.MealsPerDay.Value,
            mainDishes.Dishes,
            dailyOverrides: dailyOverrides);

        var defaultMeals = contract.MealsPerDay.Value;
        var mergedDays = OrganizationMealWeeklyMealPlanBuilder.MergeMainOnlyMealDays(
            mealDays,
            defaultMeals,
            date => OrganizationMealPeriodContractSchedule.ResolveMealsForDate(
                date, defaultMeals, dailyOverrides));

        await _weeklySelectionService.SaveWeeklySelectionAsync(
            contract,
            userId,
            weekMonday,
            mergedDays,
            ContractWeeklySelectionStatuses.AutoFilled,
            invoiceSuffix: "-AUTO",
            cancellationToken);

        return true;
    }
}
