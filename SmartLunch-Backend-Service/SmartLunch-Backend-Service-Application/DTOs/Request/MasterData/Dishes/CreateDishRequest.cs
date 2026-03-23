namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Dishes;

public class CreateDishRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    /// <summary>Một trong: man, xao, canh, trang_mieng (xem GET /catalog/dish-categories).</summary>
    public string? Category { get; set; }

    public decimal Price { get; set; }
    public string? DietaryLabel { get; set; }
    public bool IsActive { get; set; } = true;
}
