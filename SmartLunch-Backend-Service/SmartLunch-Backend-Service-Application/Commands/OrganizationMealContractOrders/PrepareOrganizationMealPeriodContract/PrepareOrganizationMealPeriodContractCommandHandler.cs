using MediatR;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.DTOs.Request.OrganizationMealContractOrders;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationMealContractOrders;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationMealOrders;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.OrganizationMealContractOrders;
using SmartLunch.Backend.Service.Application.OrganizationMealOrders;
using SmartLunch.Backend.Service.Application.Promotions;

namespace SmartLunch.Backend.Service.Application.Commands.OrganizationMealContractOrders.PrepareOrganizationMealPeriodContract;

public sealed class PrepareOrganizationMealPeriodContractCommandHandler
    : IRequestHandler<PrepareOrganizationMealPeriodContractCommand, PrepareOrganizationMealPeriodContractResponse>
{
    private readonly IUserOrganizationRepository _userOrganizationRepository;
    private readonly IOrganizationMealPeriodContractDraftCache _draftCache;
    private readonly OrganizationMealPeriodContractPersistence _persistence;
    private readonly IPromotionEngine _promotionEngine;
    private readonly IContractRepository _contractRepository;

    public PrepareOrganizationMealPeriodContractCommandHandler(
        IUserOrganizationRepository userOrganizationRepository,
        IOrganizationMealPeriodContractDraftCache draftCache,
        OrganizationMealPeriodContractPersistence persistence,
        IPromotionEngine promotionEngine,
        IContractRepository contractRepository)
    {
        _userOrganizationRepository = userOrganizationRepository;
        _draftCache = draftCache;
        _persistence = persistence;
        _promotionEngine = promotionEngine;
        _contractRepository = contractRepository;
    }

    public async Task<PrepareOrganizationMealPeriodContractResponse> Handle(
        PrepareOrganizationMealPeriodContractCommand command,
        CancellationToken cancellationToken)
    {
        var req = command.Request;
        ValidateRequest(req);

        var membership = await _userOrganizationRepository.GetByUserAndOrganizationAsync(
            command.UserId,
            req.OrganizationId);
        if (membership == null || !membership.IsActive)
            throw new UnauthorizedAccessException("You do not have access to this organization.");

        var delivery = OrganizationMealDeliveryValidator.Normalize(req.Delivery);
        var excluded = NormalizeExcludedDates(req.StartDate, req.EndDate, req.ExcludedDates);
        var dailyOverrides = OrganizationMealPeriodContractSchedule.NormalizeDailyOverrides(
            req.StartDate,
            req.EndDate,
            excluded,
            req.MealsPerDay,
            req.DailyMealPortions?.Select(p => new KeyValuePair<DateOnly, int>(p.ServiceDate, p.MealCount)));
        var mealUnitPrice = decimal.Round(req.MealUnitPrice, 2, MidpointRounding.AwayFromZero);
        var serviceDays = OrganizationMealPeriodContractCalculator.CountServiceDays(
            req.StartDate, req.EndDate, excluded);
        var totalMeals = OrganizationMealPeriodContractCalculator.CountTotalMeals(
            req.StartDate, req.EndDate, excluded, req.MealsPerDay, dailyOverrides);
        var subtotal = OrganizationMealPeriodContractCalculator.ComputeTotalValue(
            req.StartDate, req.EndDate, excluded, req.MealsPerDay, mealUnitPrice, dailyOverrides);
        var totalQuantity = totalMeals;

        var activeContract = await _contractRepository.GetActiveForOrganizationAsync(req.OrganizationId, cancellationToken);
        var evaluation = await _promotionEngine.EvaluateAsync(new OrderPromotionEvaluateInput
        {
            Channel = PromotionConstants.ChannelB2BOrg,
            UserId = command.UserId,
            OrganizationId = req.OrganizationId,
            ContractId = activeContract?.Id,
            ContractType = activeContract?.ContractType,
            PromotionCode = req.PromotionCode,
            PromotionId = req.PromotionId,
            Subtotal = subtotal,
            TotalQuantity = totalQuantity,
            Lines = new List<OrderPromotionLineInput>(),
        }, cancellationToken);

        var draftId = Guid.NewGuid().ToString("N");
        var payload = new OrganizationMealPeriodContractDraftPayload
        {
            UserId = command.UserId,
            OrganizationId = req.OrganizationId,
            StartDate = req.StartDate,
            EndDate = req.EndDate,
            ExcludedDates = excluded,
            DailyMealOverrides = dailyOverrides,
            MealsPerDay = req.MealsPerDay,
            TotalMeals = totalMeals,
            MealUnitPrice = mealUnitPrice,
            ServiceDays = serviceDays,
            SubtotalAmount = evaluation.Subtotal,
            DiscountAmount = evaluation.DiscountAmount,
            TotalAmount = evaluation.TotalAfter,
            PromotionCode = req.PromotionCode,
            AppliedPromotionId = evaluation.PromotionId,
            AppliedPromotionName = evaluation.PromotionName,
            CreatedAtUtc = VietnamTime.Now,
            Delivery = delivery,
        };

        var contract = await _persistence.EnsurePersistedAsync(payload, cancellationToken);
        await _draftCache.SaveAsync(command.UserId, draftId, payload, cancellationToken);

        return new PrepareOrganizationMealPeriodContractResponse
        {
            DraftId = draftId,
            ContractId = contract.Id,
            ContractNumber = contract.ContractNumber,
            ContractFileUrl = contract.ContractFileUrl,
            StartDate = req.StartDate,
            EndDate = req.EndDate,
            ExcludedDates = excluded,
            ServiceDays = serviceDays,
            MealsPerDay = req.MealsPerDay,
            MealUnitPrice = mealUnitPrice,
            SubtotalAmount = evaluation.Subtotal,
            DiscountAmount = evaluation.DiscountAmount,
            TotalAmount = evaluation.TotalAfter,
            AppliedPromotionId = evaluation.PromotionId,
            AppliedPromotionName = evaluation.PromotionName,
            Delivery = ToDeliverySummary(delivery),
        };
    }

    private static void ValidateRequest(PrepareOrganizationMealPeriodContractRequest req)
    {
        if (req.OrganizationId <= 0)
            throw new ArgumentException("OrganizationId is required.");
        if (req.EndDate < req.StartDate)
            throw new ArgumentException("EndDate must be on or after StartDate.");
        if (req.MealsPerDay < 1)
            throw new ArgumentException("MealsPerDay must be at least 1.");
        if (req.MealUnitPrice <= 0)
            throw new ArgumentException("MealUnitPrice must be greater than zero.");

        var spanDays = req.EndDate.DayNumber - req.StartDate.DayNumber + 1;
        if (spanDays > 62)
            throw new ArgumentException("Contract period must not exceed 62 days (typically one month).");
    }

    private static List<DateOnly> NormalizeExcludedDates(
        DateOnly start,
        DateOnly end,
        IEnumerable<DateOnly> excluded)
    {
        var list = new List<DateOnly>();
        foreach (var d in excluded.Distinct())
        {
            if (d < start || d > end)
                throw new ArgumentException($"Excluded date {d:yyyy-MM-dd} is outside contract period.");
            list.Add(d);
        }

        return list;
    }

    private static OrganizationMealDeliverySummaryDto ToDeliverySummary(OrganizationMealOrderDraftDelivery delivery) =>
        new()
        {
            RecipientName = delivery.RecipientName,
            RecipientPhone = delivery.RecipientPhone,
            RecipientEmail = delivery.RecipientEmail,
            DeliveryAddress = delivery.DeliveryAddress,
            DeliveryWardDistrict = delivery.DeliveryWardDistrict,
            DeliveryNotes = delivery.DeliveryNotes,
            PreferredDeliveryTime = delivery.PreferredDeliveryTime,
            FullAddress = OrganizationMealDeliveryValidator.BuildFullAddress(delivery),
        };
}
