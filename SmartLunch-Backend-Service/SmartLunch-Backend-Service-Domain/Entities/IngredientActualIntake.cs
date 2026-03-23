namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Phiếu nhập kho thực tế (nguyên liệu đạt chuẩn), gắn 1-1 với phiếu đề xuất đã duyệt.
/// </summary>
public class IngredientActualIntake
{
    public Guid Id { get; set; }
    public string ReceiptCode { get; set; } = string.Empty;
    public Guid ProposalId { get; set; }
    public Guid CreatedByUserId { get; set; }
    public DateTime ReceivedAt { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual IngredientIntakeProposal Proposal { get; set; } = null!;
    public virtual User CreatedByUser { get; set; } = null!;
    public virtual ICollection<IngredientActualIntakeLine> Lines { get; set; } = new List<IngredientActualIntakeLine>();
}
