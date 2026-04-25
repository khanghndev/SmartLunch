namespace SmartLunch.Backend.Service.Application.DTOs.Response.IngredientInventory;

public class GetIngredientInventoryDetailResponse
{
    public int IngredientId { get; set; }
    public string IngredientName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public decimal? CostPerUnit { get; set; }
    public int? DefaultSupplierId { get; set; }
    public string? DefaultSupplierLegalName { get; set; }

    public decimal QuantityAvailable { get; set; }
    public decimal? ReorderLevel { get; set; }
    public bool IsLowStock { get; set; }
    public DateTime LastUpdated { get; set; }

    public IReadOnlyList<IngredientSourceBatchDto> RecentBatches { get; set; } = Array.Empty<IngredientSourceBatchDto>();
}
