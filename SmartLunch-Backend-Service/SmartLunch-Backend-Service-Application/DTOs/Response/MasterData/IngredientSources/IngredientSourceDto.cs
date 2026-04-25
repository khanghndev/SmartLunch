namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.IngredientSources;

public class IngredientSourceDto
{
    public int Id { get; set; }
    public int IngredientId { get; set; }
    public string? IngredientName { get; set; }
    public int? PartnerId { get; set; }
    public string? PartnerLegalName { get; set; }
    public string? BatchNumber { get; set; }
    public string? OriginDetails { get; set; }
    public DateTime? ProductionDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string? Certification { get; set; }
    public DateTime CreatedAt { get; set; }
}
