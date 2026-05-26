using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Dishes;

namespace SmartLunch.Backend.Service.Application.Helpers;

public static class DishPortionHelper
{
    private static readonly HashSet<string> SpiceIngredientNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "Nước mắm", "Đường cát trắng", "Dầu thực vật", "Muối", "Tiêu xay", "Tỏi", "Hành tím", "Gừng",
        "Hạt nêm", "Mắm ruốc", "Nước tương", "Dầu hào", "Giấm ăn", "Bột ngọt", "Sả băm", "Ớt hiểm",
        "Hành lá", "Hành phi", "Tương ớt", "Đường phèn", "Nước dừa tươi"
    };

    public static bool IsSpiceIngredient(string? ingredientName) =>
        !string.IsNullOrWhiteSpace(ingredientName) && SpiceIngredientNames.Contains(ingredientName.Trim());

    /// <summary>Tổng khối lượng nguyên liệu chính (gram) làm định lượng tham chiếu 01 suất.</summary>
    public static decimal ComputePortionWeightGrams(IEnumerable<DishIngredientQuotaDto> quotas)
    {
        decimal grams = 0;
        foreach (var q in quotas)
        {
            if (IsSpiceIngredient(q.IngredientName) || q.Quantity <= 0)
                continue;

            var unit = (q.Unit ?? "").Trim().ToLowerInvariant();
            grams += unit switch
            {
                "kg" or "kilogram" => q.Quantity * 1000m,
                "g" or "gram" => q.Quantity,
                "quả" => q.Quantity * 55m,
                "miếng" => q.Quantity * 40m,
                _ => 0m
            };
        }

        if (grams <= 0)
            return 0;

        return Math.Round(grams / 10m, MidpointRounding.AwayFromZero) * 10m;
    }
}
