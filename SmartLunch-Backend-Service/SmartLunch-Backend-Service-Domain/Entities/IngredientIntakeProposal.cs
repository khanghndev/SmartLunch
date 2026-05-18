namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Phiếu đề xuất nhập nguyên liệu (nhân viên kho lập, chờ quản lý xử lý sau).
/// </summary>
public class IngredientIntakeProposal
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string ProposalCode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? HeaderNote { get; set; }
    public int CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = VietnamTime.Now;

    public int? ReviewedByUserId { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewNote { get; set; }

    public virtual User CreatedByUser { get; set; } = null!;
    public virtual User? ReviewedByUser { get; set; }
    public virtual ICollection<IngredientIntakeProposalLine> Lines { get; set; } = new List<IngredientIntakeProposalLine>();
    public virtual IngredientActualIntake? ActualIntake { get; set; }
}
