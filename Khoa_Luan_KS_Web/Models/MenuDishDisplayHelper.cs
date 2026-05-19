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
        if (key.Contains("tráng") || key == "trang_mieng")
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
        if (key.Contains("rau") || key.Contains("món xanh"))
            return "bg-emerald-100 text-emerald-800 border-emerald-200";
        if (key.Contains("tráng") || key == "trang_mieng")
            return "bg-pink-100 text-pink-800 border-pink-200";
        return "bg-slate-100 text-slate-700 border-slate-200";
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
