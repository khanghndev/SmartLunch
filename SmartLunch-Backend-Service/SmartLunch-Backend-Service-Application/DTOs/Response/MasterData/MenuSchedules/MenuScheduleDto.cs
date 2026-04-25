namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.MenuSchedules;

public class MenuScheduleDto
{
    public int Id { get; set; }
    public int MenuId { get; set; }
    public DateTime Date { get; set; }
    public string MealSlot { get; set; } = string.Empty;
    public int DishId { get; set; }
    public DateTime CreatedAt { get; set; }
}
