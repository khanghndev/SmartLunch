namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Dishes;

public class UpdateDishRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public decimal Price { get; set; }
    public string? DietaryLabel { get; set; }

    /// <summary>Ẩn món: đặt false (không xóa bản ghi).</summary>
    public bool IsActive { get; set; } = true;
}
