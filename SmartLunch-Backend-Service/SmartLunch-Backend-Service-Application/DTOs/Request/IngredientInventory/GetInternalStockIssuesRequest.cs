namespace SmartLunch.Backend.Service.Application.DTOs.Request.IngredientInventory;

public class GetInternalStockIssuesRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public DateOnly? IssuedFrom { get; set; }
    public DateOnly? IssuedTo { get; set; }
}
