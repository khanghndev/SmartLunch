using Khoa_Luan_KS_Web.Services;

namespace Khoa_Luan_KS_Web.Models;

public class MenuWeekGridViewModel
{
    public List<IGrouping<DateTime, WeeklyMenuScheduleDetailClientDto>> ByDate { get; set; } = new();
    public List<string> AllSlots { get; set; } = new();
    public WeeklyMenuClientDto Menu { get; set; } = new();
    public string DefaultDishImg { get; set; } = "";
}
