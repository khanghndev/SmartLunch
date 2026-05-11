namespace SmartLunch.Backend.Service.Application.Constants;

/// <summary>
/// Chuẩn hóa <c>meal_structure</c> từ form (tiếng Việt) sang khóa enum AI (tiếng Anh, snake_case).
/// Đồng bộ với SmartLunch-AI-Service <c>DishCategory</c>.
/// </summary>
public static class MealStructureEnglishNormalizer
{
    private static readonly Dictionary<string, string> VietnameseLabelToSlot = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Món chính"] = "main",
        ["Món canh"] = "soup",
        ["Món rau"] = "vegetable",
        ["Món phụ"] = "side",
        ["Tráng miệng"] = "dessert",
        ["Món nước"] = "noodle_soup",
    };

    /// <summary>
    /// Trả về danh sách slot tiếng Anh (duy nhất, giữ thứ tự xuất hiện).
    /// </summary>
    /// <exception cref="ArgumentException">Không nhận diện được token hoặc danh sách rỗng sau khi lọc.</exception>
    public static List<string> NormalizeOrThrow(IEnumerable<string?>? inputs)
    {
        if (inputs == null)
            throw new ArgumentException("MealStructure is required.");

        var ordered = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var position = 0;

        foreach (var raw in inputs)
        {
            position++;
            if (string.IsNullOrWhiteSpace(raw))
                continue;

            var t = raw.Trim();
            string? slot = null;
            if (VietnameseLabelToSlot.TryGetValue(t, out var mapped))
                slot = mapped;
            else if (DishAiEnglishCatalog.TryGetCanonicalCategorySlot(t, out var canonical))
                slot = canonical;

            if (slot == null)
            {
                var vi = string.Join(", ", VietnameseLabelToSlot.Keys.OrderBy(x => x, StringComparer.Ordinal));
                var en = string.Join(", ", DishAiEnglishCatalog.CategoryEnglishValues.OrderBy(x => x, StringComparer.Ordinal));
                throw new ArgumentException(
                    $"MealStructure[{position}] '{raw}' không hợp lệ. Dùng nhãn tiếng Việt ({vi}) hoặc khóa tiếng Anh ({en}).");
            }

            if (seen.Add(slot))
                ordered.Add(slot);
        }

        if (ordered.Count == 0)
            throw new ArgumentException(
                "MealStructure phải có ít nhất một slot hợp lệ (ví dụ: Món chính / main, Món canh / soup, Món rau / vegetable, Món phụ / side).");

        return ordered;
    }
}
