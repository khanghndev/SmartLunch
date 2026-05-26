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
        var subtotal = decimal.Round(serviceDays * meals * unitPrice, 2, MidpointRounding.AwayFromZero);
        var totalQty = serviceDays * meals;

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
}
