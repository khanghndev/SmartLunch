namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Supplier contract
/// </summary>
public class Contract
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public int PartnerId { get; set; }
    /// <summary>Số hợp đồng (ký hiệu nội bộ hoặc theo văn bản pháp lý).</summary>
    public string? ContractNumber { get; set; }
    public string? Description { get; set; }
    /// <summary>Mô tả thời gian / khung cung cấp suất ăn (vd. Thứ 2–6, bữa trưa).</summary>
    public string? SupplySchedule { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? TotalValue { get; set; }
    public decimal? DepositAmount { get; set; }
    public string Status { get; set; } = "active"; // active | expired | cancelled
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public virtual Partner Partner { get; set; } = null!;
    public virtual ICollection<PartnerPayment> PartnerPayments { get; set; } = new List<PartnerPayment>();
}
