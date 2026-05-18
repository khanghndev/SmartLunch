namespace SmartLunch.Backend.Service.Domain.Entities;

public class PromotionTarget
{
    public int Id { get; set; }
    public int PromotionId { get; set; }

    /// <summary>dish | organization | contract_type</summary>
    public string TargetType { get; set; } = string.Empty;

    public int? TargetId { get; set; }
    public string? TargetKey { get; set; }

    public virtual Promotion Promotion { get; set; } = null!;
}
