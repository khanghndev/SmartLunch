using Khoa_Luan_KS_Web.Services;

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

    /// <summary>Ghi chú pháp lý / kỳ vọng khách hàng trên trang chi tiết món.</summary>
    public static class ServingNotice
    {
        public const string ImageCaption =
            "Hình ảnh chỉ mang tính minh họa. Món thực tế có thể khác nhẹ về màu sắc và cách trình bày tùy ngày chế biến và nguyên liệu tươi theo mùa — vẫn đảm bảo đúng công thức và chất lượng.";

        public const string IngredientTitle = "Về định mức nguyên liệu & giá trị suất ăn";

        public const string IngredientBody =
            "Bảng định lượng bên dưới là mức tham chiếu chuẩn cho 01 suất, giúp bạn hình dung cơ cấu món ăn. " +
            "Khối lượng nguyên liệu thực tế được điều chỉnh linh hoạt theo mức giá trị từng suất mà doanh nghiệp đã chọn " +
            "(gói suất / ngân sách bữa ăn), nhằm cân bằng dinh dưỡng, khẩu phần và chi phí — luôn tuân thủ quy trình an toàn thực phẩm của bếp trung tâm.";

        public const string NutritionNote =
            "Thông tin dinh dưỡng (nếu có) mang tính ước tính theo định mức tham chiếu, có thể thay đổi nhẹ khi điều chỉnh khẩu phần theo gói suất.";

        /// <summary>Ghi chú cuối khối nguyên liệu (cột phải).</summary>
        public const string DetailFooterNote =
            "Hình ảnh chỉ mang tính minh họa. Định lượng nguyên liệu là mức tham chiếu cho 01 suất — khối lượng thực tế sẽ linh hoạt theo giá trị từng suất ăn mà doanh nghiệp chọn (gói suất / ngân sách bữa ăn), vẫn đảm bảo đúng công thức, dinh dưỡng và chất lượng.";
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
        "raw" => "Sống / trộn",
        _ => string.IsNullOrWhiteSpace(method) ? "" : method!
    };

  private static readonly HashSet<string> SpiceIngredientNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "Nước mắm", "Đường cát trắng", "Dầu thực vật", "Muối", "Tiêu xay", "Tỏi", "Hành tím", "Gừng",
        "Hạt nêm", "Mắm ruốc", "Nước tương", "Dầu hào", "Giấm ăn", "Bột ngọt", "Sả băm", "Ớt hiểm",
        "Hành lá", "Hành phi", "Tương ớt", "Đường phèn", "Nước dừa tươi"
    };

    public static bool IsSpiceIngredient(string? ingredientName) =>
        !string.IsNullOrWhiteSpace(ingredientName) && SpiceIngredientNames.Contains(ingredientName.Trim());

    /// <summary>Hiển thị định lượng dễ đọc (tránh 0 kg với gia vị nhỏ).</summary>
    public static string FormatQuotaQuantity(decimal quantity, string? unit)
    {
        var u = (unit ?? "").Trim().ToLowerInvariant();
        if (quantity <= 0)
            return "vừa đủ";

        if (u is "kg" or "kilogram")
        {
            if (quantity < 0.01m)
                return $"{quantity * 1000m:0.#} g";
            return $"{quantity:0.##} kg";
        }

        if (u is "lít" or "lit" or "liter" or "litre")
        {
            if (quantity < 0.01m)
                return $"{quantity * 1000m:0.#} ml";
            return $"{quantity:0.##} lít";
        }

        if (u is "quả")
            return $"{quantity:0.##} quả";

        if (u is "miếng")
            return $"{quantity:0.##} miếng";

        return string.IsNullOrEmpty(u)
            ? quantity.ToString("0.##")
            : $"{quantity:0.##} {unit}";
    }

    /// <summary>Ánh xạ nhóm món sang slot đặt suất doanh nghiệp (main/side/soup). null = không dùng trong wizard 3 slot.</summary>
    public static string? ResolveOrganizationSlotKey(string? categoryOrSlot)
    {
        var key = (categoryOrSlot ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(key))
            return null;

        if (key is "main" or "man" or "món chính")
            return "main";
        if (key is "side" or "món phụ" or "phụ")
            return "side";
        if (key is "soup" or "canh" or "canh / súp")
            return "soup";
        if (key is "vegetable" or "rau" or "rau / món xanh" or "món xanh")
            return "side";
        if (key is "noodle_soup" or "món nước")
            return "main";
        if (key.Contains("canh") || key.Contains("súp") || key.Contains("sup"))
            return "soup";
        if (key.Contains("phụ"))
            return "side";
        if (key.Contains("chính") || key.Contains("chinh"))
            return "main";
        if (key.Contains("rau") || key.Contains("xanh"))
            return "side";
        if (key is "dessert" or "trang_mieng" or "tráng miệng")
            return null;

        return null;
    }

    public static Dictionary<string, int> SlotSortOrder() => new(StringComparer.OrdinalIgnoreCase)
    {
        ["main"] = 0,
        ["side"] = 1,
        ["soup"] = 2,
        ["vegetable"] = 3,
        ["noodle_soup"] = 4,
        ["dessert"] = 5,
        ["other"] = 6,
    };

    public static (List<DishIngredientQuotaDto> Main, List<DishIngredientQuotaDto> Spices) SplitIngredientQuotas(
        IEnumerable<DishIngredientQuotaDto>? quotas)
    {
        var main = new List<DishIngredientQuotaDto>();
        var spices = new List<DishIngredientQuotaDto>();
        foreach (var q in quotas ?? Enumerable.Empty<DishIngredientQuotaDto>())
        {
            if (IsSpiceIngredient(q.IngredientName))
                spices.Add(q);
            else
                main.Add(q);
        }

        static int Compare(DishIngredientQuotaDto a, DishIngredientQuotaDto b) =>
            string.Compare(a.IngredientName, b.IngredientName, StringComparison.OrdinalIgnoreCase);

        main.Sort(Compare);
        spices.Sort(Compare);
        return (main, spices);
    }

    public static List<DishIngredientQuotaDto> ResolveIngredientQuotas(DishDetailResponse? detail, int? dishValueId = null)
    {
        if (detail == null)
            return new List<DishIngredientQuotaDto>();

        var tiers = detail.PriceTiers ?? new List<DishPriceTierClientDto>();
        if (tiers.Count > 0)
        {
            var tier = dishValueId.HasValue
                ? tiers.FirstOrDefault(t => t.DishValue.Id == dishValueId.Value)
                : tiers.OrderBy(t => t.DishValue.SortOrder).ThenBy(t => t.DishValue.Amount).FirstOrDefault();

            if (tier?.IngredientQuotas?.Count > 0)
                return tier.IngredientQuotas;
        }

        if (dishValueId.HasValue && detail.IngredientQuotas.Count > 0)
        {
            var filtered = detail.IngredientQuotas
                .Where(q => q.DishValueId == dishValueId.Value)
                .ToList();
            if (filtered.Count > 0)
                return filtered;
        }

        return detail.IngredientQuotas ?? new List<DishIngredientQuotaDto>();
    }

    public static DishPriceTierClientDto? ResolveDefaultPriceTier(DishDetailResponse? detail) =>
        detail?.PriceTiers?
            .OrderBy(t => t.DishValue.SortOrder)
            .ThenBy(t => t.DishValue.Amount)
            .FirstOrDefault();

    public static string FormatPortionWeight(decimal grams)
    {
        if (grams <= 0)
            return "—";

        if (grams >= 1000)
            return $"{grams / 1000m:0.##} kg/suất";

        return $"{grams:0.#} g/suất";
    }

    public static string FormatPriceTierLabel(DishValueClientDto value) =>
        !string.IsNullOrWhiteSpace(value.Label)
            ? value.Label!
            : $"{value.Amount:N0}đ/suất";

    public static decimal ComputePortionWeightGrams(IEnumerable<DishIngredientQuotaDto>? quotas)
    {
        decimal grams = 0;
        foreach (var q in quotas ?? Enumerable.Empty<DishIngredientQuotaDto>())
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
