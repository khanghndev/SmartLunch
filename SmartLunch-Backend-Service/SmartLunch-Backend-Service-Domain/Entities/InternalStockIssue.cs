namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Phiếu xuất kho nguyên liệu nội bộ (bếp / nội bộ).
/// </summary>
public class InternalStockIssue
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string IssueCode { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; }
    public string? Reason { get; set; }
    public int? CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = VietnamTime.Now;

    public virtual User? CreatedByUser { get; set; }
    public virtual ICollection<InternalStockIssueLine> Lines { get; set; } = new List<InternalStockIssueLine>();
}
