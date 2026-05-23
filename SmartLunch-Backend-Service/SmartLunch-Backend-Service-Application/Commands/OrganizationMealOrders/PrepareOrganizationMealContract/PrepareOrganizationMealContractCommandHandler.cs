using MediatR;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.DTOs.Request.OrganizationMealOrders;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationMealOrders;
using SmartLunch.Backend.Service.Application.Helpers;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.OrganizationMealOrders;
using SmartLunch.Backend.Service.Application.Promotions;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Commands.OrganizationMealOrders.PrepareOrganizationMealContract;

public sealed class PrepareOrganizationMealContractCommandHandler
    : IRequestHandler<PrepareOrganizationMealContractCommand, PrepareOrganizationMealContractResponse>
{
    private readonly IUserOrganizationRepository _userOrganizationRepository;
    private readonly IDishRepository _dishRepository;
    private readonly IOrganizationMealOrderDraftCache _draftCache;
    private readonly IPromotionEngine _promotionEngine;
    private readonly IContractRepository _contractRepository;
    private readonly OrganizationMealContractDraftPersistence _contractPersistence;

    public PrepareOrganizationMealContractCommandHandler(
        IUserOrganizationRepository userOrganizationRepository,
        IDishRepository dishRepository,
        IOrganizationMealOrderDraftCache draftCache,
        IPromotionEngine promotionEngine,
        IContractRepository contractRepository,
        OrganizationMealContractDraftPersistence contractPersistence)
    {
        _userOrganizationRepository = userOrganizationRepository;
        _dishRepository = dishRepository;
        _draftCache = draftCache;
        _promotionEngine = promotionEngine;
        _contractRepository = contractRepository;
        _contractPersistence = contractPersistence;
    }

    public async Task<PrepareOrganizationMealContractResponse> Handle(
        PrepareOrganizationMealContractCommand command,
        CancellationToken cancellationToken)
    {
        var req = command.Request;
        if (req.OrganizationId <= 0)
            throw new ArgumentException("OrganizationId is required.");

        if (req.Price <= 0)
            throw new ArgumentException("Price must be greater than zero.");

        if (req.MealDays == null || req.MealDays.Count == 0)
            throw new ArgumentException("At least one meal day is required.");

        var membership = await _userOrganizationRepository.GetByUserAndOrganizationAsync(
            command.UserId,
            req.OrganizationId);
        if (membership == null || !membership.IsActive)
            throw new UnauthorizedAccessException("You do not have access to this organization.");

        var delivery = OrganizationMealDeliveryValidator.Normalize(req.Delivery);

        var utcNow = VietnamTime.Now;
        //var (allowedFirst, allowedLast) = OrganizationMealOrderDateWindow.GetAllowedServiceDateRange(utcNow);

        var mergedByDate = new Dictionary<DateOnly, OrganizationMealOrderDraftDay>();
        var dishSlot = new Dictionary<int, string>();
        var totalMainQty = 0;

        foreach (var day in req.MealDays)
        {
            if (day.MealPlan == null)
                throw new ArgumentException($"Meal day {day.ServiceDate} must include mealPlan.");

            var main = day.MealPlan.Main ?? new List<OrganizationMealLineRequest>();
            var side = day.MealPlan.Side ?? new List<OrganizationMealLineRequest>();
            var soup = day.MealPlan.Soup ?? new List<OrganizationMealLineRequest>();

            if (main.Count == 0)
                throw new ArgumentException($"Meal day {day.ServiceDate}: mealPlan.main must have at least one line.");

            // if (!OrganizationMealOrderDateWindow.IsDateInWindow(day.ServiceDate, utcNow))
            // {
            //     throw new ArgumentException(
            //         $"Ngày {day.ServiceDate:yyyy-MM-dd} nằm ngoài cửa sổ đặt suất ({allowedFirst:yyyy-MM-dd} – {allowedLast:yyyy-MM-dd}, múi giờ Việt Nam).");
            // }

            if (!mergedByDate.TryGetValue(day.ServiceDate, out var draftDay))
            {
                draftDay = new OrganizationMealOrderDraftDay { ServiceDate = day.ServiceDate };
                mergedByDate[day.ServiceDate] = draftDay;
            }

            void AddSlot(string slot, List<OrganizationMealLineRequest> lines, List<OrganizationMealOrderDraftLine> target)
            {
                foreach (var line in lines)
                {
                    if (line.DishId <= 0)
                        throw new ArgumentException("Each line must include a valid DishId.");
                    if (line.Quantity < 1)
                        throw new ArgumentException("Quantity must be at least 1 for each line.");

                    if (dishSlot.TryGetValue(line.DishId, out var existingSlot) &&
                        !string.Equals(existingSlot, slot, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new ArgumentException(
                            $"Dish {line.DishId} cannot appear in more than one slot (was '{existingSlot}', now '{slot}').");
                    }

                    dishSlot[line.DishId] = slot;
                    if (string.Equals(slot, "main", StringComparison.OrdinalIgnoreCase))
                        totalMainQty += line.Quantity;

                    target.Add(new OrganizationMealOrderDraftLine { DishId = line.DishId, Quantity = line.Quantity });
                }
            }

            AddSlot("main", main, draftDay.Main);
            AddSlot("side", side, draftDay.Side);
            AddSlot("soup", soup, draftDay.Soup);
        }

        OrganizationMealSlotBalance.ValidateDraftDays(mergedByDate.Values);

        var dishIds = dishSlot.Keys.ToList();
        var dishes = await _dishRepository.GetByIdsWithIngredientsAsync(dishIds, cancellationToken);
        if (dishes.Count != dishIds.Count)
            throw new ArgumentException("One or more dishes were not found.");

        foreach (var d in dishes)
        {
            if (!d.IsActive)
                throw new ArgumentException($"Dish '{d.Name}' is not active.");
        }

        var dishById = dishes.ToDictionary(d => d.Id);
        foreach (var (dishId, slot) in dishSlot)
        {
            var dish = dishById[dishId];
            if (!DishHasSlot(dish, slot))
                throw new ArgumentException(
                    $"Dish '{dish.Name}' (Id={dishId}) is not assigned to slot '{slot}' in dish_categories.");
        }

        var price = decimal.Round(req.Price, 2, MidpointRounding.AwayFromZero);
        var total = decimal.Round(price * totalMainQty, 2, MidpointRounding.AwayFromZero);

        var draftDays = mergedByDate.Values.OrderBy(d => d.ServiceDate).ToList();
        var minDate = draftDays.Min(d => d.ServiceDate);

        var draftId = Guid.NewGuid().ToString("N");

        var promoLines = new List<OrderPromotionLineInput>();
        foreach (var day in draftDays)
        {
            foreach (var line in day.Main)
            {
                promoLines.Add(new OrderPromotionLineInput
                {
                    DishId = line.DishId,
                    Quantity = line.Quantity,
                    LineTotal = decimal.Round(price * line.Quantity, 2, MidpointRounding.AwayFromZero),
                });
            }
        }

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
            Subtotal = total,
            TotalQuantity = totalMainQty,
            Lines = promoLines,
        }, cancellationToken);

        var payload = new OrganizationMealOrderDraftPayload
        {
            UserId = command.UserId,
            OrganizationId = req.OrganizationId,
            PricePerPortion = price,
            TotalMainQuantity = totalMainQty,
            Days = draftDays,
            SubtotalAmount = evaluation.Subtotal,
            DiscountAmount = evaluation.DiscountAmount,
            TotalAmount = evaluation.TotalAfter,
            PromotionCode = req.PromotionCode,
            AppliedPromotionId = evaluation.PromotionId,
            AppliedPromotionName = evaluation.PromotionName,
            MinServiceDate = minDate,
            CreatedAtUtc = utcNow,
            Delivery = delivery,
        };

        var persistedContract = await _contractPersistence.EnsurePersistedAsync(payload, cancellationToken);

        await _draftCache.SaveAsync(command.UserId, draftId, payload, cancellationToken);

        var lineSummaries = new List<OrganizationMealDraftLineSummaryDto>();
        foreach (var day in draftDays)
        {
            void Append(string slot, List<OrganizationMealOrderDraftLine> lines)
            {
                var isMain = string.Equals(slot, "main", StringComparison.OrdinalIgnoreCase);
                foreach (var g in lines.GroupBy(l => l.DishId))
                {
                    var qty = g.Sum(x => x.Quantity);
                    var dish = dishById[g.Key];
                    var unit = isMain ? price : 0m;
                    var lineTotal = decimal.Round(unit * qty, 2, MidpointRounding.AwayFromZero);
                    lineSummaries.Add(new OrganizationMealDraftLineSummaryDto
                    {
                        ServiceDate = day.ServiceDate,
                        Slot = slot,
                        DishId = g.Key,
                        DishName = dish.Name,
                        Quantity = qty,
                        UnitPrice = unit,
                        LineTotal = lineTotal,
                    });
                }
            }

            Append("main", day.Main);
            Append("side", day.Side);
            Append("soup", day.Soup);
        }

        lineSummaries = lineSummaries
            .OrderBy(x => x.ServiceDate)
            .ThenBy(x => x.Slot)
            .ThenBy(x => x.DishName)
            .ToList();

        return new PrepareOrganizationMealContractResponse
        {
            DraftId = draftId,
            ContractId = persistedContract.Id,
            ContractNumber = persistedContract.ContractNumber,
            ContractFileUrl = persistedContract.ContractFileUrl,
            // AllowedFirstServiceDate = allowedFirst,
            // AllowedLastServiceDate = allowedLast,
            PricePerPortion = price,
            TotalMainQuantity = totalMainQty,
            SubtotalAmount = evaluation.Subtotal,
            DiscountAmount = evaluation.DiscountAmount,
            TotalAmount = evaluation.TotalAfter,
            AppliedPromotionId = evaluation.PromotionId,
            AppliedPromotionName = evaluation.PromotionName,
            PromotionCode = req.PromotionCode,
            Lines = lineSummaries,
            Delivery = ToDeliverySummary(delivery),
        };
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

    private static bool DishHasSlot(Dish d, string slot)
    {
        var want = slot.Trim().ToLowerInvariant();
        return d.DishDishCategories.Any(ddc =>
            ddc.DishCategory != null &&
            string.Equals(ddc.DishCategory.SlotKey, want, StringComparison.OrdinalIgnoreCase));
    }
}
