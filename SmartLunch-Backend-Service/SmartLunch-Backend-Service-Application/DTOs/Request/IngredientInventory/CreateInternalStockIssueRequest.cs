namespace SmartLunch.Backend.Service.Application.DTOs.Request.IngredientInventory;

public class CreateInternalStockIssueRequest
{
    public DateTime? IssuedAtUtc { get; set; }
    public string? Reason { get; set; }
    public List<CreateInternalStockIssueLineRequest> Lines { get; set; } = new();
}

public class CreateInternalStockIssueLineRequest
{
    public Guid IngredientId { get; set; }
    public decimal Quantity { get; set; }
}
