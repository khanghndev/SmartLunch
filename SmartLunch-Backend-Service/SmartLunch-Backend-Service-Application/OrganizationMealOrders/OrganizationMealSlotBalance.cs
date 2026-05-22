namespace SmartLunch.Backend.Service.Application.OrganizationMealOrders;

/// <summary>Mỗi ngày: tổng món phụ và tổng canh phải bằng tổng món chính (1 suất = chính + phụ + canh).</summary>
public static class OrganizationMealSlotBalance
{
    public static void ValidateDraftDays(IEnumerable<OrganizationMealOrderDraftDay> days)
    {
        foreach (var day in days)
            ValidateDay(day);
    }

    public static void ValidateDay(OrganizationMealOrderDraftDay day)
    {
        var main = SumQty(day.Main);
        var side = SumQty(day.Side);
        var soup = SumQty(day.Soup);

        if (main < 1)
            throw new ArgumentException($"Ngày {day.ServiceDate:dd/MM/yyyy}: cần ít nhất một món chính (suất > 0).");

        if (side != main)
        {
            throw new ArgumentException(
                $"Ngày {day.ServiceDate:dd/MM/yyyy}: tổng món phụ ({side}) phải bằng món chính ({main} suất).");
        }

        if (soup != main)
        {
            throw new ArgumentException(
                $"Ngày {day.ServiceDate:dd/MM/yyyy}: tổng canh ({soup}) phải bằng món chính ({main} suất).");
        }
    }

    private static int SumQty(IReadOnlyList<OrganizationMealOrderDraftLine> lines) =>
        lines.Sum(l => l.Quantity);
}
