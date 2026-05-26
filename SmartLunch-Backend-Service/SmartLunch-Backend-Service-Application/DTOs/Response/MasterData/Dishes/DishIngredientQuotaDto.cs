namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Dishes;

/// <summary>Định mức nguyên liệu cho một món (DishIngredient).</summary>
public class DishIngredientQuotaDto
{
    public int Id { get; set; }
    public int IngredientId { get; set; }
    public string IngredientName { get; set; } = string.Empty;
    public int DishValueId { get; set; }
    public decimal DishValueAmount { get; set; }
    public string? DishValueLabel { get; set; }
    public decimal Quantity { get; set; }
    public string? Unit { get; set; }
}
