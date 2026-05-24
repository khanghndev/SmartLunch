namespace Khoa_Luan_KS_Web.Models;

public static class MealFormOptions
{
    /// <summary>Giá mặc định lưu DB khi không nhập trên form (tính tiền theo suất đơn hàng).</summary>
    public const decimal DefaultStoredPrice = 0m;

    public static readonly (string Value, string Label)[] SlotCategories =
    {
        ("main", "Món chính"),
        ("side", "Món phụ"),
        ("soup", "Canh / súp"),
        ("vegetable", "Rau / món xanh"),
        ("noodle_soup", "Món nước"),
        ("dessert", "Tráng miệng"),
    };

    public static readonly (string Value, string Label)[] CookingMethods =
    {
        ("fried", "Chiên / rán"),
        ("stewed", "Kho / hầm"),
        ("boiled", "Luộc"),
        ("stir_fried", "Xào"),
        ("grilled", "Nướng"),
        ("steamed", "Hấp"),
        ("raw", "Ăn sống / salad"),
    };

    public static string SlotLabel(string? slot)
    {
        if (string.IsNullOrWhiteSpace(slot)) return "—";
        foreach (var (value, label) in SlotCategories)
        {
            if (string.Equals(value, slot, StringComparison.OrdinalIgnoreCase))
                return label;
        }
        return MenuDishDisplayHelper.CategoryLabel(slot);
    }
}
