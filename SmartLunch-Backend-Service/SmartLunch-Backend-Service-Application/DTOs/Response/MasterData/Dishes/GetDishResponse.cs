namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Dishes;

public class GetDishResponse
{
    public DishDto Dish { get; set; } = new();
    /// <summary>Danh sách phẳng (legacy). Ưu tiên dùng <see cref="PriceTiers"/> khi hiển thị theo mức giá.</summary>
    public List<DishIngredientQuotaDto> IngredientQuotas { get; set; } = new();
    /// <summary>Định mức nguyên liệu theo từng mức giá suất ăn.</summary>
    public List<DishPriceTierDto> PriceTiers { get; set; } = new();
}
