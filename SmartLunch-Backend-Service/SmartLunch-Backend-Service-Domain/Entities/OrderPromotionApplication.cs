namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>Bản ghi KM đã áp dụng khi checkout (đối soát / báo cáo).</summary>
public class OrderPromotionApplication
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int PromotionId { get; set; }
    public string? PromotionCode { get; set; }
    public string PromotionName { get; set; } = string.Empty;
    public string ScopeType { get; set; } = string.Empty;
    public string DiscountType { get; set; } = string.Empty;
    public decimal DiscountValue { get; set; }
    public decimal SubtotalBefore { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAfter { get; set; }
    public string? SnapshotJson { get; set; }
    public DateTime AppliedAt { get; set; } = VietnamTime.Now;

    public virtual Order Order { get; set; } = null!;
    public virtual Promotion Promotion { get; set; } = null!;
}
