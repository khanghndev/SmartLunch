namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.WeeklyMenus;

public class CreateWeeklyMenuRequest
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string MenuType { get; set; } = "General";
    public int? CustomerTypeId { get; set; }
    public string? Description { get; set; }
    public List<CreateMenuScheduleItemRequest> Schedules { get; set; } = new();
}

public class CreateMenuScheduleItemRequest
{
    public DateTime Date { get; set; }
    public string MealSlot { get; set; } = "lunch";
    public int DishId { get; set; }
}
