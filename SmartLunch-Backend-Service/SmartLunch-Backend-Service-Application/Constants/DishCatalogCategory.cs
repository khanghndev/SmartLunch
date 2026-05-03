namespace SmartLunch.Backend.Service.Application.Constants;

/// <summary>Phân loại hiển thị tĩnh cho catalog (không lưu trên Dish; dùng UI nếu cần).</summary>
public static class DishCatalogCategory
{
    public const string Man = "man";
    public const string Xao = "xao";
    public const string Canh = "canh";
    public const string TrangMieng = "trang_mieng";

    private static readonly string[] Codes =
    {
        Man, Xao, Canh, TrangMieng
    };

    private static readonly Dictionary<string, string> DisplayVi = new(StringComparer.OrdinalIgnoreCase)
    {
        [Man] = "Món mặn",
        [Xao] = "Món xào",
        [Canh] = "Canh",
        [TrangMieng] = "Tráng miệng"
    };

    /// <summary>Danh sách cố định cho UI / API catalog.</summary>
    public static IReadOnlyList<(string Code, string DisplayName)> All =>
        Codes.Select(c => (c, DisplayVi[c])).ToList();

    /// <summary>Cho phép null/empty (chưa phân loại). Nếu có giá trị thì phải là một trong các mã chuẩn.</summary>
    public static bool IsValidOrEmpty(string? category)
    {
        if (string.IsNullOrWhiteSpace(category))
            return true;
        return Codes.Any(c => string.Equals(c, category.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public static string? Normalize(string? category)
    {
        if (string.IsNullOrWhiteSpace(category))
            return null;
        var t = category.Trim();
        var match = Codes.FirstOrDefault(c => string.Equals(c, t, StringComparison.OrdinalIgnoreCase));
        return match;
    }
}
