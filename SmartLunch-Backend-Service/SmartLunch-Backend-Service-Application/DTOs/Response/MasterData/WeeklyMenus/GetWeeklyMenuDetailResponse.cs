namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.WeeklyMenus;

public class GetWeeklyMenuDetailResponse
{
    public WeeklyMenuDto WeeklyMenu { get; set; } = new();

    /// <summary>
    /// Các dòng lịch món (MenuSchedule) của weekly menu này, sắp xếp theo ngày và buổi.
    /// </summary>
    public List<WeeklyMenuScheduleDetailDto> Schedules { get; set; } = new();
}

public class WeeklyMenuScheduleDetailDto
{
    public int Id { get; set; }
    public string? Code { get; set; }
    /// <summary>
    /// Trùng với WeeklyMenu.Id (foreign key MenuId trên MenuSchedule).
    /// </summary>
    public int MenuId { get; set; }

    public DateTime Date { get; set; }
    public string MealSlot { get; set; } = string.Empty;
    public int DishId { get; set; }
    public DateTime CreatedAt { get; set; }
    public WeeklyMenuScheduleDishSummaryDto Dish { get; set; } = new();
}

public class WeeklyMenuScheduleDishSummaryDto
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Tên nhóm danh mục (<c>dish_categories.Name</c>), nối bằng · — cùng logic với luồng customer weekly menu.
    /// </summary>
    public string? Category { get; set; }

    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
}
