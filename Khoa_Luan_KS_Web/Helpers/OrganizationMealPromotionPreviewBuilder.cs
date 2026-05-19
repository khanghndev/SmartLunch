using Khoa_Luan_KS_Web.Services;

namespace Khoa_Luan_KS_Web.Helpers;

public static class OrganizationMealPromotionPreviewBuilder
{
    public static PreviewPromotionClientRequest ToPreviewRequest(
        PrepareOrganizationMealContractClientRequest request,
        int organizationId,
        string? promotionCode = null)
    {
        var price = request.Price;
        var lines = new List<PreviewPromotionLineClientRequest>();
        var totalQty = 0;

        foreach (var day in request.MealDays)
        {
            foreach (var line in day.MealPlan?.Main ?? [])
            {
                var qty = Math.Max(1, line.Quantity);
                totalQty += qty;
                lines.Add(new PreviewPromotionLineClientRequest
                {
                    DishId = line.DishId,
                    Quantity = qty,
                    LineTotal = decimal.Round(price * qty, 2, MidpointRounding.AwayFromZero),
                });
            }
        }

        return new PreviewPromotionClientRequest
        {
            Channel = "b2b_org",
            OrganizationId = organizationId,
            PromotionCode = promotionCode,
            Subtotal = decimal.Round(price * totalQty, 2, MidpointRounding.AwayFromZero),
            TotalQuantity = totalQty,
            Lines = lines,
        };
    }
}
