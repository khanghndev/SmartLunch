namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.IngredientSources;

public class UpdateIngredientSourceRequest
{
    public Guid IngredientId { get; set; }
    public Guid? PartnerId { get; set; }
    public string? BatchNumber { get; set; }
    public string? OriginDetails { get; set; }
    public DateTime? ProductionDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string? Certification { get; set; }
}
