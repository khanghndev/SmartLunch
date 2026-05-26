namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Dishes;

/// <summary>Định mức nguyên liệu theo từng mức giá suất ăn.</summary>
public class DishPriceTierDto
{
    public DishValueDto DishValue { get; set; } = new();
    /// <summary>Khối lượng tham chiếu 01 suất (gram), tính từ nguyên liệu chính.</summary>
    public decimal PortionWeightGrams { get; set; }
    public List<DishIngredientQuotaDto> IngredientQuotas { get; set; } = new();
}
