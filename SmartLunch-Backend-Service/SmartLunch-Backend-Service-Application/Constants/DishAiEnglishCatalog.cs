using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Constants;

/// <summary>Giá trị AI enum đồng bộ với SmartLunch-AI-Service (DishCategory / CookingMethod).</summary>
public static class DishAiEnglishCatalog
{
    private static readonly HashSet<string> CategorySlots = new(StringComparer.OrdinalIgnoreCase)
    {
        "main", "side", "soup", "vegetable", "noodle_soup", "dessert",
    };

    private static readonly HashSet<string> CookingMethods = new(StringComparer.OrdinalIgnoreCase)
    {
        "fried", "stewed", "boiled", "stir_fried", "grilled", "steamed", "raw",
    };

    public static IReadOnlyCollection<string> CategoryEnglishValues => CategorySlots;

    public static IReadOnlyCollection<string> CookingMethodValues => CookingMethods;

    public static void ValidateCategoryEnglishOrThrow(string? value, string fieldName = "CategoryEnglish")
    {
        if (string.IsNullOrWhiteSpace(value))
            return;
        var t = value.Trim();
        if (!CategorySlots.Contains(t))
            throw new ArgumentException(
                $"{fieldName} must be one of: {string.Join(", ", CategorySlots.OrderBy(x => x, StringComparer.Ordinal))}.");
    }

    public static void ValidateCookingMethodOrThrow(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;
        var t = value.Trim();
        if (!CookingMethods.Contains(t))
            throw new ArgumentException(
                $"CookingMethod must be one of: {string.Join(", ", CookingMethods.OrderBy(x => x, StringComparer.Ordinal))}.");
    }

    public static void ValidateSlotCategoryListOrThrow(IReadOnlyList<string>? codes)
    {
        if (codes == null || codes.Count == 0)
            return;
        foreach (var c in codes)
            ValidateCategoryEnglishOrThrow(c, "DishSlotCategoryCodes");
    }

    /// <summary>Slot keys từ junction dish_dish_categories → dish_categories, thứ tự SortOrder (cho AI covers_categories).</summary>
    public static List<string> BuildSlotKeysFromJunction(Dish d)
    {
        return (d.DishDishCategories ?? Enumerable.Empty<DishDishCategory>())
            .Where(x => x.DishCategory != null && !string.IsNullOrWhiteSpace(x.DishCategory.SlotKey))
            .OrderBy(x => x.DishCategory!.SortOrder)
            .Select(x => x.DishCategory!.SlotKey)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    /// <summary>
    /// Một giá trị <c>category</c> cho IndustrialDish (Python dishes_by_cat + enum).
    /// Ưu tiên <c>noodle_soup</c> nếu có (món nước composite), sau đó main → soup → vegetable → side → dessert.
    /// </summary>
    public static string PickPrimaryCategoryForAi(IReadOnlyList<string> covers)
    {
        if (covers.Count == 0)
            throw new ArgumentException("covers must not be empty.", nameof(covers));

        var set = new HashSet<string>(covers, StringComparer.OrdinalIgnoreCase);
        foreach (var p in new[] { "noodle_soup", "main", "soup", "vegetable", "side", "dessert" })
        {
            if (set.Contains(p))
                return p;
        }

        return covers[0];
    }
}
