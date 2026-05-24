namespace Khoa_Luan_KS_Web.Models;

public static class MenuDishDisplayHelper
{
    public static string CategoryLabel(string? categoryOrSlot)
    {
        var key = (categoryOrSlot ?? "").Trim().ToLowerInvariant();
        if (key.Contains("món chính") || key == "man" || key == "main")
            return "Món chính";
        if (key.Contains("xào") || key == "xao")
            return "Món xào";
        if (key.Contains("canh") || key == "soup")
            return "Canh / súp";
        if (key.Contains("phụ") || key == "side")
            return "Món phụ";
        if (key.Contains("nước") || key == "noodle_soup")
            return "Món nước";
        if (key.Contains("tráng") || key == "trang_mieng" || key == "dessert")
            return "Tráng miệng";
        if (key.Contains("rau") || key.Contains("món xanh"))
            return "Rau / món xanh";
        return string.IsNullOrWhiteSpace(categoryOrSlot) ? "Món ăn" : categoryOrSlot!;
    }

    public static string CategoryBadgeClass(string? categoryOrSlot)
    {
        var key = (categoryOrSlot ?? "").Trim().ToLowerInvariant();
        if (key.Contains("món chính") || key == "man" || key == "main")
            return "bg-orange-100 text-orange-800 border-orange-200";
        if (key.Contains("canh") || key == "soup")
            return "bg-blue-100 text-blue-800 border-blue-200";
        if (key.Contains("phụ") || key == "side")
            return "bg-violet-100 text-violet-800 border-violet-200";
        if (key.Contains("nước") || key == "noodle_soup")
            return "bg-cyan-100 text-cyan-800 border-cyan-200";
        if (key.Contains("tráng") || key == "trang_mieng" || key == "dessert")
            return "bg-pink-100 text-pink-800 border-pink-200";
        if (key.Contains("rau") || key.Contains("món xanh") || key == "vegetable")
            return "bg-emerald-100 text-emerald-800 border-emerald-200";
        return "bg-slate-100 text-slate-700 border-slate-200";
    }

    /// <summary>Chống cache trình duyệt khi ảnh món vừa đổi (dùng UpdatedAt hoặc CreatedAt).</summary>
    public static string ImageSrc(string? url, DateTime? updatedAt, DateTime createdAt)
    {
        if (string.IsNullOrWhiteSpace(url))
            return string.Empty;

        var version = (updatedAt ?? createdAt).Ticks;
        return url.Contains('?', StringComparison.Ordinal)
            ? $"{url}&v={version}"
            : $"{url}?v={version}";
    }

    public static string CookingMethodLabel(string? method) => (method ?? "").Trim().ToLowerInvariant() switch
    {
        "fried" => "Chiên / rán",
        "stewed" => "Kho / hầm",
        "steamed" => "Hấp",
        "boiled" => "Luộc",
        "grilled" => "Nướng",
        "stir_fry" or "stir-fry" or "stir_fried" => "Xào",
        _ => string.IsNullOrWhiteSpace(method) ? "" : method!
    };
}
