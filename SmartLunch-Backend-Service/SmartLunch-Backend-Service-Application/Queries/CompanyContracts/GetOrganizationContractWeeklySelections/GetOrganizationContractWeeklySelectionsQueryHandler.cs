using MediatR;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Contracts;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.OrganizationMealContractOrders;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Domain.Time;

namespace SmartLunch.Backend.Service.Application.Queries.CompanyContracts.GetOrganizationContractWeeklySelections;

public sealed class GetOrganizationContractWeeklySelectionsQueryHandler
    : IRequestHandler<GetOrganizationContractWeeklySelectionsQuery, GetContractWeeklySelectionsResponse>
{
    private readonly IContractRepository _contractRepository;
    private readonly IUserOrganizationRepository _userOrganizationRepository;
    private readonly IContractWeeklySelectionRepository _weeklyRepository;
    private readonly OrganizationMealContractWeeklySelectionService _weeklySelectionService;

    public GetOrganizationContractWeeklySelectionsQueryHandler(
        IContractRepository contractRepository,
        IUserOrganizationRepository userOrganizationRepository,
        IContractWeeklySelectionRepository weeklyRepository,
        OrganizationMealContractWeeklySelectionService weeklySelectionService)
    {
        _contractRepository = contractRepository;
        _userOrganizationRepository = userOrganizationRepository;
        _weeklyRepository = weeklyRepository;
        _weeklySelectionService = weeklySelectionService;
    }

    public async Task<GetContractWeeklySelectionsResponse> Handle(
        GetOrganizationContractWeeklySelectionsQuery request,
        CancellationToken cancellationToken)
    {
        var contract = await _contractRepository.GetPeriodBasedWithExcludedDatesAsync(request.ContractId, cancellationToken)
            ?? throw new KeyNotFoundException($"Contract with ID {request.ContractId} was not found.");

        if (!contract.OrganizationId.HasValue)
            throw new UnauthorizedAccessException("This contract is not available for organization self-service.");

        var membership = await _userOrganizationRepository.GetByUserAndOrganizationAsync(
            request.UserId,
            contract.OrganizationId.Value);
        if (membership == null || !membership.IsActive)
            throw new UnauthorizedAccessException("You do not have access to this contract.");

        if (!OrganizationMealContractTypes.IsPeriodBased(contract.ContractType) || !contract.EndDate.HasValue)
        {
            return new GetContractWeeklySelectionsResponse { ContractId = contract.Id };
        }

        await _weeklySelectionService.EnsureWeeksSeededAsync(contract, cancellationToken);

        var contractStart = DateOnly.FromDateTime(contract.StartDate);
        var contractEnd = DateOnly.FromDateTime(contract.EndDate.Value);
        var excluded = contract.ExcludedDates.Select(e => e.ExcludedDate).ToList();
        var today = VietnamTime.Today;
        var openWeekMonday = OrganizationMealWeeklySelectionRules.ResolveOpenWeekMonday(
            today, contractStart, contractEnd, excluded);

        var dailyOverrides = OrganizationMealPeriodContractSchedule.ToOverrideDictionary(
            contract.DailyMealPortions.Select(p => new ContractDailyMealPortionSource(p.ServiceDate, p.MealCount)));
        var defaultMeals = contract.MealsPerDay is > 0 ? contract.MealsPerDay.Value : 1;

        var selections = await _weeklyRepository.GetAllByContractIdAsync(
            contract.Id, includeItems: true, cancellationToken);
        var (total, filled) = await _weeklyRepository.GetProgressAsync(contract.Id, cancellationToken);

        var weeks = selections.Select(sel =>
        {
            var weekEnd = sel.WeekMonday.AddDays(6);
            if (weekEnd > contractEnd)
                weekEnd = contractEnd;

            var isOpen = openWeekMonday.HasValue && sel.WeekMonday == openWeekMonday.Value;
            var isFilled = ContractWeeklySelectionStatuses.IsFilled(sel.Status);
            var serviceDates = OrganizationMealPeriodContractCalculator
                .GetWeekServiceDates(sel.WeekMonday, contractStart, contractEnd, excluded)
                .ToList();

            var days = serviceDates.Select(date =>
            {
                var required = OrganizationMealPeriodContractSchedule.ResolveMealsForDate(
                    date, defaultMeals, dailyOverrides);
                var lines = sel.Items
                    .Where(i => i.ServiceDate == date)
                    .Select(i => new ContractWeeklySelectionLineDto
                    {
                        DishId = i.DishId,
                        DishName = i.Dish?.Name ?? $"Món #{i.DishId}",
                        Quantity = i.Quantity,
                    })
                    .ToList();

                return new ContractWeeklySelectionDayDto
                {
                    ServiceDate = date,
                    RequiredMeals = required,
                    Main = lines,
                };
            }).ToList();

            return new ContractWeeklySelectionDto
            {
                Id = sel.Id,
                WeekMonday = sel.WeekMonday,
                WeekEnd = weekEnd,
                Status = sel.Status,
                FulfillmentOrderId = sel.FulfillmentOrderId,
                SelectedAt = sel.SelectedAt,
                IsOpenWeek = isOpen,
                IsFilled = isFilled,
                CanSelect = isOpen && !isFilled && serviceDates.Count > 0
                    && OrganizationMealWeeklySelectionRules.CanManuallySelectWeek(today, sel.WeekMonday),
                Days = days,
            };
        }).ToList();

        return new GetContractWeeklySelectionsResponse
        {
            ContractId = contract.Id,
            OpenWeekMonday = openWeekMonday,
            TotalServiceWeeks = total,
            FilledServiceWeeks = filled,
            Weeks = weeks,
        };
    }
}
