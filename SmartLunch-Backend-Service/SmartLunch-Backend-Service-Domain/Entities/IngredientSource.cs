namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Ingredient batch/source
/// </summary>
public class IngredientSource
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public int IngredientId { get; set; }
    public int? PartnerId { get; set; }
    public string? BatchNumber { get; set; }
    public string? OriginDetails { get; set; }
    public DateTime? ProductionDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string? Certification { get; set; }
    public DateTime CreatedAt { get; set; } = VietnamTime.Now;

    public virtual Ingredient Ingredient { get; set; } = null!;
    public virtual Partner? Partner { get; set; }
}
