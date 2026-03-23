namespace SmartLunch.Backend.Service.Application.DTOs.Response.IngredientInventory;

public class GetLowStockIngredientAlertsResponse
{
    public IReadOnlyList<LowStockIngredientAlertDto> Alerts { get; set; } = Array.Empty<LowStockIngredientAlertDto>();
    public int TotalCount { get; set; }
}
