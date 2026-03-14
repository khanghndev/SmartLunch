namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.MenuSchedules;

public class MenuScheduleDto
{
    public Guid Id { get; set; }
    public Guid MenuId { get; set; }
    public DateTime Date { get; set; }
    public string MealSlot { get; set; } = string.Empty;
    public Guid DishId { get; set; }
    public DateTime CreatedAt { get; set; }
}
