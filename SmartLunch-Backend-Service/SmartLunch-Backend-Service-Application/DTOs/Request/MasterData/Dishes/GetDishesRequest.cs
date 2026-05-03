namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Dishes;

public class GetDishesRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public bool? IsActive { get; set; }
    /// <summary>Lọc món có slot dish_categories.SlotKey trùng (vd main, soup).</summary>
    public string? Category { get; set; }
}
