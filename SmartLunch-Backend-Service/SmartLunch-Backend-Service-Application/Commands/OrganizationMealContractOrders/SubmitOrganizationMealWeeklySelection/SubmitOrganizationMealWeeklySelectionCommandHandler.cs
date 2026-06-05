using MediatR;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.DTOs.Request.OrganizationMealContractOrders;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationMealContractOrders;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.OrganizationMealContractOrders;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Domain.Time;

namespace SmartLunch.Backend.Service.Application.Commands.OrganizationMealContractOrders.SubmitOrganizationMealWeeklySelection;

public sealed class SubmitOrganizationMealWeeklySelectionCommandHandler
    : IRequestHandler<SubmitOrganizationMealWeeklySelectionCommand, SubmitOrganizationMealWeeklySelectionResponse>
{
    private readonly IUserOrganizationRepository _userOrganizationRepository;
    private readonly IContractRepository _contractRepository;
    private readonly IDishRepository _dishRepository;
    private readonly OrganizationMealContractWeeklySelectionService _weeklySelectionService;

    public SubmitOrganizationMealWeeklySelectionCommandHandler(
        IUserOrganizationRepository userOrganizationRepository,
        IContractRepository contractRepository,
        IDishRepository dishRepository,
        OrganizationMealContractWeeklySelectionService weeklySelectionService)
    {
        _userOrganizationRepository = userOrganizationRepository;
        _contractRepository = contractRepository;
        _dishRepository = dishRepository;
        _weeklySelectionService = weeklySelectionService;
    }

    public async Task<SubmitOrganizationMealWeeklySelectionResponse> Handle(
        SubmitOrganizationMealWeeklySelectionCommand command,
        CancellationToken cancellationToken)
    {
        var req = command.Request;
        var weekMonday = OrganizationMealPeriodContractCalculator.GetWeekMonday(req.WeekStart);
        if (weekMonday != req.WeekStart)
            throw new ArgumentException("WeekStart must be a Monday.");

        var contract = await _contractRepository.GetPeriodBasedWithExcludedDatesAsync(command.ContractId, cancellationToken);
        if (contract == null)
        {
            var any = await _contractRepository.GetByIdAsync(command.ContractId);
            if (any == null)
            {
                throw new ArgumentException(
                    $"Không tìm thấy hợp đồng #{command.ContractId}. Vui lòng mở lại từ mục Hợp đồng hoặc Lịch sử đơn.");
            }

            throw new ArgumentException(
                $"Hợp đồng #{command.ContractId} là loại «{any.ContractType}», không thể gửi thực đơn tuần. " +
                "Chỉ hợp đồng đặt suất theo kỳ (Period-Based) mới dùng trang này.");
        }

        if (!contract.OrganizationId.HasValue)
            throw new ArgumentException("Contract has no organization.");

        var membership = await _userOrganizationRepository.GetByUserAndOrganizationAsync(
            command.UserId,
            contract.OrganizationId.Value);
        if (membership == null || !membership.IsActive)
            throw new UnauthorizedAccessException("You do not have access to this contract.");

        if (!contract.SourceOrderId.HasValue)
            throw new InvalidOperationException("Contract checkout is not complete.");

        var contractStart = DateOnly.FromDateTime(contract.StartDate);
        var contractEnd = contract.EndDate.HasValue
            ? DateOnly.FromDateTime(contract.EndDate.Value)
            : throw new ArgumentException("Contract end date is required.");

        var excluded = contract.ExcludedDates.Select(e => e.ExcludedDate).ToList();
        var today = VietnamTime.Today;
        var openWeekMonday = OrganizationMealWeeklySelectionRules.ResolveOpenWeekMonday(
            today, contractStart, contractEnd, excluded);
        if (!openWeekMonday.HasValue)
            throw new ArgumentException("Hiện không có tuần nào cần chọn món trong hợp đồng.");
        if (weekMonday != openWeekMonday.Value)
        {
            var openEnd = openWeekMonday.Value.AddDays(6);
            throw new ArgumentException(
                $"Chỉ được đặt món cho tuần đang mở ({openWeekMonday.Value:dd/MM/yyyy} – {openEnd:dd/MM/yyyy}). " +
                "Mỗi tuần chỉ chọn một lần; tuần sau sẽ mở khi đến kỳ.");
        }

        if (await _weeklySelectionService.WeekIsFilledAsync(contract.Id, weekMonday, cancellationToken))
        {
            throw new ArgumentException(
                "Tuần này đã chọn món. Vui lòng xem thực đơn trong chi tiết hợp đồng.");
        }

        var manualDeadline = weekMonday.AddDays(-OrganizationMealWeeklySelectionRules.ManualSelectionLeadDays);
        if (today > manualDeadline)
        {
            throw new ArgumentException(
                $"Đã quá hạn chọn món thủ công (trước {OrganizationMealWeeklySelectionRules.ManualSelectionLeadDays} ngày so với đầu tuần). " +
                "Hệ thống sẽ tự chọn món đúng số suất hoặc bạn xem thực đơn đã có trong chi tiết hợp đồng.");
        }

        var dailyOverrides = OrganizationMealPeriodContractSchedule.ToOverrideDictionary(
            contract.DailyMealPortions.Select(p => new ContractDailyMealPortionSource(p.ServiceDate, p.MealCount)));
        var defaultMeals = contract.MealsPerDay is > 0 ? contract.MealsPerDay.Value : 1;
        var allowedDates = OrganizationMealPeriodContractCalculator
            .GetWeekServiceDates(weekMonday, contractStart, contractEnd, excluded)
            .ToHashSet();

        if (allowedDates.Count == 0)
            throw new ArgumentException("No service days in this week for the contract period.");

        var mergedDays = OrganizationMealWeeklyMealPlanBuilder.MergeMainOnlyMealDays(
            req.MealDays,
            defaultMeals,
            date => OrganizationMealPeriodContractSchedule.ResolveMealsForDate(
                date, defaultMeals, dailyOverrides));

        foreach (var day in mergedDays.Keys)
        {
            if (!allowedDates.Contains(day))
            {
                throw new ArgumentException(
                    $"Date {day:yyyy-MM-dd} is not a service day for this contract week.");
            }
        }

        var dishIds = mergedDays.Values
            .SelectMany(d => d.Main)
            .Select(l => l.DishId)
            .Distinct()
            .ToList();

        var dishes = await _dishRepository.GetByIdsWithIngredientsAsync(dishIds, cancellationToken);
        if (dishes.Count != dishIds.Count)
            throw new ArgumentException("One or more dishes were not found.");

        foreach (var d in dishes)
        {
            if (!d.IsActive)
                throw new ArgumentException($"Dish '{d.Name}' is not active.");
            if (!OrganizationMealWeeklyMealPlanBuilder.DishIsMain(d))
                throw new ArgumentException($"Dish '{d.Name}' is not a main dish.");
        }

        var selection = await _weeklySelectionService.SaveWeeklySelectionAsync(
            contract,
            command.UserId,
            weekMonday,
            mergedDays,
            ContractWeeklySelectionStatuses.Selected,
            cancellationToken: cancellationToken);

        return new SubmitOrganizationMealWeeklySelectionResponse
        {
            ContractId = contract.Id,
            WeeklySelectionId = selection.Id,
            OrderId = selection.FulfillmentOrderId ?? 0,
            WeekStart = weekMonday,
            Status = selection.Status,
            ItemCount = selection.Items.Count,
        };
    }
}
