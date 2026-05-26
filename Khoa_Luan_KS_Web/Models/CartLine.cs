namespace Khoa_Luan_KS_Web.Models;

public class CartLine
{
    public int DishId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; } = 1;
    public string? ImageUrl { get; set; }
    /// <summary>main | side | soup | dessert | vegetable | noodle_soup | other</summary>
    public string? SlotKey { get; set; }
    public string? CategoryLabel { get; set; }
    public int? MenuScheduleId { get; set; }
    public DateTime? ScheduleDateUtc { get; set; }
    public string? MealSlot { get; set; }
}
