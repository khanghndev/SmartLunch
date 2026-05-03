namespace SmartLunch.Backend.Service.Application.DTOs.Response.Customer.WeeklyMenu;

public class CustomerDishDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    /// <summary>Tên nhóm slot (dish_categories.Name), nối bằng · — thay cho Category tiếng Việt trên dishes.</summary>
    public string? Category { get; set; }
    public decimal Price { get; set; }
    public string? DietaryLabel { get; set; }
    public string? ImageUrl { get; set; }
    public decimal? Calories { get; set; }
}
