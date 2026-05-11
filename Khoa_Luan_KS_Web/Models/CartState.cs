namespace Khoa_Luan_KS_Web.Models;

/// <summary>Giỏ phiên bản mới: các bộ menu tuần (số bộ chung + chọn suất theo ngày) và các dòng món lẻ.</summary>
public class CartState
{
    public List<WeekMenuCartBundle> MenuBundles { get; set; } = new();
    public List<CartLine> LooseLines { get; set; } = new();
}

public class WeekMenuCartBundle
{
    public string BundleId { get; set; } = string.Empty;
    public int WeeklyMenuId { get; set; }
    public string Label { get; set; } = string.Empty;
    public int BundleQuantity { get; set; } = 1;
    public List<WeekMenuSlotSelection> Slots { get; set; } = new();
}

public class WeekMenuSlotSelection
{
    public int ScheduleId { get; set; }
    public int DishId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime ScheduleDate { get; set; }
    public string MealSlot { get; set; } = string.Empty;
    public bool Selected { get; set; } = true;
}

public class CartIndexViewModel
{
    public CartState State { get; set; } = new();
    public decimal TotalAmount { get; set; }

    public bool IsEmpty => State.MenuBundles.Count == 0 && State.LooseLines.Count == 0;

    public static CartIndexViewModel Create(CartState state)
    {
        decimal t = 0;
        foreach (var b in state.MenuBundles)
        {
            var sum = b.Slots.Where(s => s.Selected).Sum(s => s.Price);
            t += sum * b.BundleQuantity;
        }

        foreach (var l in state.LooseLines)
            t += l.Price * l.Quantity;

        return new CartIndexViewModel { State = state, TotalAmount = t };
    }

    /// <summary>Dùng cho đặt hàng: mỗi suất đã chọn × số bộ menu.</summary>
    public static List<CartLine> FlattenForOrder(CartState state)
    {
        var lines = new List<CartLine>();
        foreach (var b in state.MenuBundles)
        {
            foreach (var slot in b.Slots.Where(s => s.Selected))
            {
                lines.Add(new CartLine
                {
                    DishId = slot.DishId,
                    Name = slot.Name,
                    Price = slot.Price,
                    Quantity = b.BundleQuantity,
                    ImageUrl = slot.ImageUrl,
                    MenuScheduleId = slot.ScheduleId,
                    ScheduleDateUtc = slot.ScheduleDate.ToUniversalTime(),
                    MealSlot = slot.MealSlot,
                });
            }
        }

        foreach (var l in state.LooseLines)
            lines.Add(l);

        return lines;
    }
}
