namespace SmartLunch.Backend.Service.Application.DTOs.Response.IngredientInventory;

public class IngredientSourceBatchDto
{
    public int Id { get; set; }
    public string? BatchNumber { get; set; }
    public string? OriginDetails { get; set; }
    public DateTime? ProductionDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string? SupplierLegalName { get; set; }
    public DateTime CreatedAt { get; set; }
}
