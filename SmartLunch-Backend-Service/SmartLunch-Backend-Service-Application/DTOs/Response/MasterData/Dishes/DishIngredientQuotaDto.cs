namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Dishes;

/// <summary>Định mức nguyên liệu cho một món (DishIngredient).</summary>
public class DishIngredientQuotaDto
{
    public Guid Id { get; set; }
    public Guid IngredientId { get; set; }
    public string IngredientName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string? Unit { get; set; }
}
