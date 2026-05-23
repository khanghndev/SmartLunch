namespace SmartLunch.Backend.Service.Application.OrganizationMealOrders;

/// <summary>Dữ liệu nháp đặt suất đơn vị (Redis), chưa ghi DB.</summary>
public sealed class OrganizationMealOrderDraftPayload
{
    public int UserId { get; set; }
    public int OrganizationId { get; set; }

    /// <summary>Hợp đồng đã ghi vào bảng contracts khi Prepare.</summary>
    public int ContractId { get; set; }

    /// <summary>Giá / suất thỏa thuận (VND).</summary>
    public decimal PricePerPortion { get; set; }

    /// <summary>Tổng quantity các dòng main (mọi ngày) — nhân với PricePerPortion = tổng tiền.</summary>
    public int TotalMainQuantity { get; set; }

    public List<OrganizationMealOrderDraftDay> Days { get; set; } = new();
    public decimal TotalAmount { get; set; }

    public decimal? SubtotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public string? PromotionCode { get; set; }
    public int? AppliedPromotionId { get; set; }
    public string? AppliedPromotionName { get; set; }

    public DateOnly MinServiceDate { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public OrganizationMealOrderDraftDelivery? Delivery { get; set; }
}

public sealed class OrganizationMealOrderDraftDay
{
    public DateOnly ServiceDate { get; set; }
    public List<OrganizationMealOrderDraftLine> Main { get; set; } = new();
    public List<OrganizationMealOrderDraftLine> Side { get; set; } = new();
    public List<OrganizationMealOrderDraftLine> Soup { get; set; } = new();
}

public sealed class OrganizationMealOrderDraftLine
{
    public int DishId { get; set; }
    public int Quantity { get; set; }
}
