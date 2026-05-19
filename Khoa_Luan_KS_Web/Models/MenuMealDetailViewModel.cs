using Khoa_Luan_KS_Web.Services;

namespace Khoa_Luan_KS_Web.Models;

public class MenuMealDetailViewModel
{
    public WeeklyMenuClientDto WeeklyMenu { get; set; } = new();
    public WeeklyMenuScheduleDetailClientDto Schedule { get; set; } = new();
    public DishDetailResponse? DishDetail { get; set; }
    public string? DishLoadWarning { get; set; }
}
