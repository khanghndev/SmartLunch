namespace Khoa_Luan_KS_Web.Models;

/// <summary>Ràng buộc đặt hàng tối thiểu (tổng suất).</summary>
public static class CartOrderRules
{
    public const int MinimumPortions = 20;

    public static int CountPortions(CartState state)
    {
        var n = 0;
        foreach (var b in state.MenuBundles)
        {
            var perSet = b.Slots.Count(s => s.Selected);
            n += perSet * b.BundleQuantity;
        }

        foreach (var l in state.LooseLines)
            n += l.Quantity;

        return n;
    }

    /// <summary>Các ngày có suất đã chọn (theo lịch thực đơn), dùng cho bước nhập địa chỉ giao.</summary>
    public static List<DateTime> DistinctSelectedDates(CartState state)
    {
        var set = new HashSet<DateTime>();
        foreach (var b in state.MenuBundles)
        {
            foreach (var s in b.Slots.Where(x => x.Selected))
                set.Add(s.ScheduleDate.Date);
        }

        return set.OrderBy(d => d).ToList();
    }
}

public class CartDeliveryDayRowVm
{
    public string DateKey { get; set; } = string.Empty;
    public string DisplayLabel { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? DeliveryTime { get; set; }
}

/// <summary>Chỉ giờ giao theo ngày (địa chỉ nhập một lần chung).</summary>
public class CartDeliveryTimeRowVm
{
    public string DateKey { get; set; } = string.Empty;
    public string? DeliveryTime { get; set; }
}

public class CartDeliveryPageVm
{
    public List<CartDeliveryDayRowVm> Days { get; set; } = new();
    public CartIndexViewModel Cart { get; set; } = CartIndexViewModel.Create(new CartState());
    public string? PrefillCommonAddress { get; set; }
}

public class CartContractPageVm
{
    public CartIndexViewModel Cart { get; set; } = CartIndexViewModel.Create(new CartState());
    public List<CartDeliveryDayRowVm> Delivery { get; set; } = new();
    public DateTime ScheduledDateForApi { get; set; }
}

public class CartBundleSyncResponse
{
    public bool Ok { get; set; }
    public string? Error { get; set; }
    public decimal CartTotal { get; set; }
    public decimal BundleSubtotal { get; set; }
    public int TotalPortions { get; set; }
    public bool MeetsMinimum { get; set; }
}

public class OrderDeliveryNoteLine
{
    public string DateKey { get; set; } = string.Empty;
    public string DisplayLabel { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? DeliveryTime { get; set; }
}
