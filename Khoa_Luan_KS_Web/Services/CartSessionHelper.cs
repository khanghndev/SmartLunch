using System.Text.Json;
using Khoa_Luan_KS_Web.Models;

namespace Khoa_Luan_KS_Web.Services;

public static class CartSessionHelper
{
    public const string SessionKey = "huit_cart_v1";
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public static CartState GetState(ISession session)
    {
        var raw = session.GetString(SessionKey);
        if (string.IsNullOrEmpty(raw))
            return new CartState();

        try
        {
            var st = JsonSerializer.Deserialize<CartState>(raw, JsonOpts);
            if (st != null)
                return st;
        }
        catch
        {
            // legacy
        }

        try
        {
            var legacy = JsonSerializer.Deserialize<List<CartLine>>(raw, JsonOpts);
            if (legacy is { Count: > 0 })
                return new CartState { LooseLines = legacy };
        }
        catch
        {
            // ignored
        }

        return new CartState();
    }

    public static void SaveState(ISession session, CartState state) =>
        session.SetString(SessionKey, JsonSerializer.Serialize(state, JsonOpts));

    /// <summary>Số dòng / suất lẻ hiển thị trên badge (không tính menu tuần).</summary>
    public static int GetBadgeCount(ISession session)
    {
        var state = GetState(session);
        var n = state.LooseLines.Sum(l => l.Quantity);
        if (n > 0)
            return n;
        return state.MenuBundles.Count > 0 ? 1 : 0;
    }

    public static List<CartLooseGroupVm> GroupLooseLines(IEnumerable<CartLine> lines)
    {
        var order = MenuDishDisplayHelper.SlotSortOrder();
        return lines
            .GroupBy(l => NormalizeSlotKey(l.SlotKey))
            .OrderBy(g => order.GetValueOrDefault(g.Key, 99))
            .Select(g => new CartLooseGroupVm
            {
                SlotKey = g.Key,
                Label = g.First().CategoryLabel ?? MenuDishDisplayHelper.CategoryLabel(g.Key),
                Lines = g.OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase).ToList(),
            })
            .ToList();
    }

    public static List<CartImportLineDto> ToImportDtos(IEnumerable<CartLine> lines) =>
        lines
            .Where(l => l.DishId > 0 && !string.IsNullOrWhiteSpace(l.Name))
            .Select(l => new CartImportLineDto
            {
                DishId = l.DishId,
                Name = l.Name,
                Quantity = Math.Max(1, l.Quantity),
                SlotKey = NormalizeSlotKey(l.SlotKey),
                CategoryLabel = l.CategoryLabel ?? MenuDishDisplayHelper.CategoryLabel(l.SlotKey),
                ImageUrl = l.ImageUrl,
            })
            .ToList();

    private static string NormalizeSlotKey(string? slotKey)
    {
        var resolved = MenuDishDisplayHelper.ResolveOrganizationSlotKey(slotKey);
        if (!string.IsNullOrEmpty(resolved))
            return resolved;
        return string.IsNullOrWhiteSpace(slotKey) ? "other" : slotKey.Trim().ToLowerInvariant();
    }
}

public class CartLooseGroupVm
{
    public string SlotKey { get; set; } = "other";
    public string Label { get; set; } = string.Empty;
    public List<CartLine> Lines { get; set; } = new();
}

public class CartImportLineDto
{
    public int DishId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
    public string SlotKey { get; set; } = string.Empty;
    public string CategoryLabel { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
}
