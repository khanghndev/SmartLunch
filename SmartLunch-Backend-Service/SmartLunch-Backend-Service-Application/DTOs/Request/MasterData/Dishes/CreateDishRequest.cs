namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Dishes;

public class CreateDishRequest
{
    public string? Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameEnglish { get; set; }
    public string? Description { get; set; }

    /// <summary>Bắt buộc: ít nhất một khóa slot (dish_categories.SlotKey), ví dụ main, soup, noodle_soup.</summary>
    public List<string> DishSlotCategoryCodes { get; set; } = new();

    /// <summary>Bắt buộc: MethodKey trong cooking_methods (fried|stewed|boiled|stir_fried|grilled|steamed|raw).</summary>
    public string? CookingMethod { get; set; }

    public decimal Price { get; set; }
    public string? DietaryLabel { get; set; }
    public string? ImageUrl { get; set; }
    public decimal? Calories { get; set; }
    public decimal? Protein { get; set; }
    public decimal? Fat { get; set; }
    public decimal? Carbs { get; set; }
    public bool IsActive { get; set; } = true;

    public List<UpdateDishImageItemRequest>? Images { get; set; }
}
