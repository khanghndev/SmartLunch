using System.Text.Json;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.Promotions;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Domain.Time;

namespace SmartLunch.Backend.Service.Application.Services;

public sealed class PromotionEngine : IPromotionEngine
{
    private readonly IPromotionRepository _promotionRepository;

    public PromotionEngine(IPromotionRepository promotionRepository)
    {
        _promotionRepository = promotionRepository;
    }

    public async Task<OrderPromotionEvaluateResult> EvaluateAsync(
        OrderPromotionEvaluateInput input,
        CancellationToken cancellationToken = default)
    {
        var subtotal = decimal.Round(input.Subtotal, 2, MidpointRounding.AwayFromZero);
        if (subtotal <= 0)
        {
            return NoDiscount(subtotal, "Subtotal must be greater than zero.");
        }

        var placedAt = input.OrderPlacedAt ?? VietnamTime.Now;
        var today = DateOnly.FromDateTime(placedAt);
        var promotions = await _promotionRepository.GetActiveForEvaluationAsync(today, cancellationToken);

        var eligible = new List<(Promotion Promo, decimal Discount)>();
        foreach (var promo in promotions)
        {
            if (!IsChannelMatch(promo.Channel, input.Channel))
                continue;

            if (!IsBookingWindowValid(promo, placedAt))
                continue;

            if (!await IsUsageLimitOkAsync(promo, input.UserId, cancellationToken))
                continue;

            if (!IsScopeEligible(promo, input))
                continue;

            var discount = ComputeDiscount(promo, input, subtotal);
            if (discount > 0)
                eligible.Add((promo, discount));
        }

        if (!string.IsNullOrWhiteSpace(input.PromotionCode))
        {
            var code = input.PromotionCode.Trim();
            var coded = eligible.FirstOrDefault(e =>
                e.Promo.Code != null &&
                string.Equals(e.Promo.Code, code, StringComparison.OrdinalIgnoreCase));

            if (coded.Promo == null)
            {
                var byCode = await _promotionRepository.GetByCodeAsync(code, cancellationToken);
                if (byCode == null)
                    return NoDiscount(subtotal, $"Promotion code '{code}' was not found.");

                return NoDiscount(subtotal, $"Promotion code '{code}' is not eligible for this order.");
            }

            return ToResult(subtotal, coded.Promo, coded.Discount);
        }

        if (eligible.Count == 0)
            return NoDiscount(subtotal, null);

        var mode = eligible[0].Promo.SelectionMode;
        (Promotion Promo, decimal Discount) chosen;
        if (string.Equals(mode, PromotionConstants.SelectionFirstMatch, StringComparison.OrdinalIgnoreCase))
        {
            chosen = eligible[0];
        }
        else
        {
            chosen = eligible.OrderByDescending(e => e.Discount).ThenByDescending(e => e.Promo.Priority).First();
        }

        return ToResult(subtotal, chosen.Promo, chosen.Discount);
    }

    public OrderPromotionApplication BuildApplication(
        Order order,
        Promotion promotion,
        OrderPromotionEvaluateResult evaluation)
    {
        var snapshot = JsonSerializer.Serialize(new
        {
            evaluation.PromotionId,
            evaluation.PromotionCode,
            evaluation.PromotionName,
            evaluation.ScopeType,
            evaluation.DiscountType,
            evaluation.DiscountValue,
            evaluation.Subtotal,
            evaluation.DiscountAmount,
            evaluation.TotalAfter,
            order.InvoiceCode,
        });

        return new OrderPromotionApplication
        {
            OrderId = order.Id,
            PromotionId = promotion.Id,
            PromotionCode = promotion.Code,
            PromotionName = promotion.Name,
            ScopeType = promotion.ScopeType,
            DiscountType = promotion.DiscountType,
            DiscountValue = promotion.DiscountValue,
            SubtotalBefore = evaluation.Subtotal,
            DiscountAmount = evaluation.DiscountAmount,
            TotalAfter = evaluation.TotalAfter,
            SnapshotJson = snapshot,
            AppliedAt = VietnamTime.Now,
        };
    }

    private static bool IsChannelMatch(string promoChannel, string orderChannel) =>
        string.Equals(promoChannel, PromotionConstants.ChannelAll, StringComparison.OrdinalIgnoreCase) ||
        string.Equals(promoChannel, orderChannel, StringComparison.OrdinalIgnoreCase);

    private static bool IsBookingWindowValid(Promotion promo, DateTime placedAt)
    {
        if (promo.BookingTimeStart == null || promo.BookingTimeEnd == null)
            return true;

        var local = placedAt; // VietnamTime.Now is already VN wall clock in this project
        var time = TimeOnly.FromDateTime(local);
        var start = promo.BookingTimeStart.Value;
        var end = promo.BookingTimeEnd.Value;

        if (start <= end)
            return time >= start && time <= end;

        return time >= start || time <= end;
    }

    private async Task<bool> IsUsageLimitOkAsync(Promotion promo, int? userId, CancellationToken ct)
    {
        if (promo.MaxTotalUses is int maxTotal)
        {
            var used = await _promotionRepository.CountTotalApplicationsAsync(promo.Id, ct);
            if (used >= maxTotal)
                return false;
        }

        if (promo.MaxUsesPerUser is int maxPerUser && userId is int uid)
        {
            var used = await _promotionRepository.CountUserApplicationsAsync(promo.Id, uid, ct);
            if (used >= maxPerUser)
                return false;
        }

        return true;
    }

    private static bool IsScopeEligible(Promotion promo, OrderPromotionEvaluateInput input)
    {
        if (promo.MinOrderAmount is decimal minAmt && input.Subtotal < minAmt)
            return false;

        return promo.ScopeType switch
        {
            PromotionConstants.ScopeOrderQuantity => IsOrderQuantityEligible(promo, input),
            PromotionConstants.ScopeDish => IsDishEligible(promo, input),
            PromotionConstants.ScopeCustomer => IsCustomerEligible(promo, input),
            _ => false,
        };
    }

    private static bool IsOrderQuantityEligible(Promotion promo, OrderPromotionEvaluateInput input)
    {
        var minQty = promo.MinOrderQuantity ?? 1;
        return input.TotalQuantity >= minQty;
    }

    private static bool IsDishEligible(Promotion promo, OrderPromotionEvaluateInput input)
    {
        var dishTargets = promo.Targets
            .Where(t => string.Equals(t.TargetType, PromotionConstants.TargetDish, StringComparison.OrdinalIgnoreCase))
            .Select(t => t.TargetId)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .ToHashSet();

        if (dishTargets.Count == 0)
            return input.Lines.Count > 0;

        return input.Lines.Any(l => dishTargets.Contains(l.DishId));
    }

    private static bool IsCustomerEligible(Promotion promo, OrderPromotionEvaluateInput input)
    {
        var orgTargets = promo.Targets
            .Where(t => string.Equals(t.TargetType, PromotionConstants.TargetOrganization, StringComparison.OrdinalIgnoreCase))
            .Select(t => t.TargetId)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .ToHashSet();

        var contractTypes = promo.Targets
            .Where(t => string.Equals(t.TargetType, PromotionConstants.TargetContractType, StringComparison.OrdinalIgnoreCase))
            .Select(t => t.TargetKey)
            .Where(k => !string.IsNullOrWhiteSpace(k))
            .Select(k => k!.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (orgTargets.Count == 0 && contractTypes.Count == 0)
            return input.OrganizationId.HasValue || input.ContractId.HasValue;

        var orgOk = orgTargets.Count == 0 ||
                    (input.OrganizationId is int orgId && orgTargets.Contains(orgId));

        var contractOk = contractTypes.Count == 0 ||
                         (!string.IsNullOrWhiteSpace(input.ContractType) &&
                          contractTypes.Contains(input.ContractType.Trim()));

        return orgOk && contractOk;
    }

    private static decimal ComputeDiscount(Promotion promo, OrderPromotionEvaluateInput input, decimal subtotal)
    {
        decimal basis = subtotal;

        if (string.Equals(promo.ScopeType, PromotionConstants.ScopeDish, StringComparison.OrdinalIgnoreCase))
        {
            var dishTargets = promo.Targets
                .Where(t => string.Equals(t.TargetType, PromotionConstants.TargetDish, StringComparison.OrdinalIgnoreCase))
                .Select(t => t.TargetId)
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .ToHashSet();

            if (dishTargets.Count > 0)
            {
                basis = input.Lines
                    .Where(l => dishTargets.Contains(l.DishId))
                    .Sum(l => l.LineTotal);
            }
        }

        basis = decimal.Round(basis, 2, MidpointRounding.AwayFromZero);
        if (basis <= 0)
            return 0;

        decimal raw = string.Equals(promo.DiscountType, PromotionConstants.DiscountPercent, StringComparison.OrdinalIgnoreCase)
            ? basis * (promo.DiscountValue / 100m)
            : promo.DiscountValue;

        raw = decimal.Round(raw, 2, MidpointRounding.AwayFromZero);
        return raw > subtotal ? subtotal : raw;
    }

    private static OrderPromotionEvaluateResult NoDiscount(decimal subtotal, string? message) => new()
    {
        Subtotal = subtotal,
        DiscountAmount = 0,
        TotalAfter = subtotal,
        Applied = false,
        Message = message,
    };

    private static OrderPromotionEvaluateResult ToResult(decimal subtotal, Promotion promo, decimal discount)
    {
        discount = decimal.Round(discount, 2, MidpointRounding.AwayFromZero);
        var total = decimal.Round(subtotal - discount, 2, MidpointRounding.AwayFromZero);
        if (total < 0)
            total = 0;

        return new OrderPromotionEvaluateResult
        {
            Subtotal = subtotal,
            DiscountAmount = discount,
            TotalAfter = total,
            Applied = true,
            PromotionId = promo.Id,
            PromotionCode = promo.Code,
            PromotionName = promo.Name,
            ScopeType = promo.ScopeType,
            DiscountType = promo.DiscountType,
            DiscountValue = promo.DiscountValue,
        };
    }
}
