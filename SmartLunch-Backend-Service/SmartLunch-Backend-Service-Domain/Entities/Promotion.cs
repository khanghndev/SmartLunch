namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>Cấu hình khuyến mãi (quản lý trước, áp dụng khi tạo đơn).</summary>
public class Promotion
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    /// <summary>dish | order_quantity | customer</summary>
    public string ScopeType { get; set; } = "order_quantity";

    /// <summary>percent | fixed_amount</summary>
    public string DiscountType { get; set; } = "percent";

    public decimal DiscountValue { get; set; }
    public int Priority { get; set; }

    /// <summary>first_match | best_discount</summary>
    public string SelectionMode { get; set; } = "best_discount";

    /// <summary>all | b2c | b2b_org</summary>
    public string Channel { get; set; } = "all";

    public int? MinOrderQuantity { get; set; }
    public decimal? MinOrderAmount { get; set; }
    public DateOnly ValidFrom { get; set; }
    public DateOnly ValidTo { get; set; }
    public TimeOnly? BookingTimeStart { get; set; }
    public TimeOnly? BookingTimeEnd { get; set; }
    public int? MaxTotalUses { get; set; }
    public int? MaxUsesPerUser { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = VietnamTime.Now;
    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<PromotionTarget> Targets { get; set; } = new List<PromotionTarget>();
    public virtual ICollection<OrderPromotionApplication> Applications { get; set; } = new List<OrderPromotionApplication>();
}
