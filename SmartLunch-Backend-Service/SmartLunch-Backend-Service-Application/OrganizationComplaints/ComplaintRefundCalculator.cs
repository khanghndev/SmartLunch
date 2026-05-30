using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.OrganizationComplaints;

public static class ComplaintRefundCalculator
{
    public static decimal GetUnitPricePerPortion(Order order)
    {
        var pricedItems = order.OrderItems?.Where(i => i.UnitPrice > 0).ToList() ?? [];
        if (pricedItems.Count > 0)
            return pricedItems[0].UnitPrice;

        if (order.Contract?.MealUnitPrice is decimal contractPrice && contractPrice > 0)
            return contractPrice;

        return 0m;
    }

    public static decimal? SuggestRefund(Order order, int? portionCount)
    {
        if (portionCount is not > 0)
            return null;

        var unit = GetUnitPricePerPortion(order);
        if (unit <= 0)
            return null;

        return decimal.Round(unit * portionCount.Value, 2, MidpointRounding.AwayFromZero);
    }
}
