using Khoa_Luan_KS_Web.Services;

namespace Khoa_Luan_KS_Web.Helpers;

public static class OrganizationMealPeriodPromotionPreviewBuilder
{
    public static PreviewPromotionClientRequest ToPreviewRequest(
        PrepareOrganizationMealPeriodContractClientRequest request,
        int organizationId,
        string? promotionCode = null,
        int? promotionId = null)
    {
        if (!DateOnly.TryParse(request.StartDate, out var start) ||
            !DateOnly.TryParse(request.EndDate, out var end))
        {
            return new PreviewPromotionClientRequest
            {
                Channel = "b2b_org",
                OrganizationId = organizationId,
            };
        }

        var excluded = request.ExcludedDates
            .Select(d => DateOnly.TryParse(d, out var x) ? x : (DateOnly?)null)
            .Where(d => d.HasValue)
            .Select(d => d!.Value)
            .ToList();

        var serviceDays = OrganizationMealContractDateRules.CountServiceDays(start, end, excluded);
        var meals = Math.Max(1, request.MealsPerDay);
        var unitPrice = decimal.Round(request.MealUnitPrice, 2, MidpointRounding.AwayFromZero);
        var overrides = BuildOverrideMap(request);
        var totalQty = OrganizationMealContractDateRules.CountTotalMeals(start, end, excluded, meals, overrides);
        var subtotal = decimal.Round(totalQty * unitPrice, 2, MidpointRounding.AwayFromZero);

        return new PreviewPromotionClientRequest
        {
            Channel = "b2b_org",
            OrganizationId = organizationId,
            PromotionCode = promotionCode,
            PromotionId = promotionId,
            Subtotal = subtotal,
            TotalQuantity = totalQty,
            Lines = new List<PreviewPromotionLineClientRequest>(),
        };
    }

    private static Dictionary<string, int>? BuildOverrideMap(PrepareOrganizationMealPeriodContractClientRequest request)
    {
        if (request.DailyMealPortions == null || request.DailyMealPortions.Count == 0)
            return null;

        var map = new Dictionary<string, int>();
        foreach (var p in request.DailyMealPortions)
        {
            if (string.IsNullOrWhiteSpace(p.ServiceDate) || p.MealCount < 1)
                continue;
            map[p.ServiceDate] = p.MealCount;
        }

        return map.Count == 0 ? null : map;
    }
}
