namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Phiếu đề xuất nhập nguyên liệu (nhân viên kho lập, chờ quản lý xử lý sau).
/// </summary>
public class IngredientIntakeProposal
{
    public Guid Id { get; set; }
    public string ProposalCode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? HeaderNote { get; set; }
    public Guid CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid? ReviewedByUserId { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewNote { get; set; }

    public virtual User CreatedByUser { get; set; } = null!;
    public virtual User? ReviewedByUser { get; set; }
    public virtual ICollection<IngredientIntakeProposalLine> Lines { get; set; } = new List<IngredientIntakeProposalLine>();
    public virtual IngredientActualIntake? ActualIntake { get; set; }
}
