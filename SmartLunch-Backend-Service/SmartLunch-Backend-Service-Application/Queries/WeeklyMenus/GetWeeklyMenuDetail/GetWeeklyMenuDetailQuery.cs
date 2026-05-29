using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.WeeklyMenus;

namespace SmartLunch.Backend.Service.Application.Queries.WeeklyMenus.GetWeeklyMenuDetail;

public class GetWeeklyMenuDetailQuery : IRequest<GetWeeklyMenuDetailResponse>
{
    public int WeeklyMenuId { get; set; }

    /// <summary>Chỉ trả lịch từ ngày này trở đi (date-only). Null = trả tất cả.</summary>
    public DateTime? ScheduleFrom { get; set; }

    public GetWeeklyMenuDetailQuery(int weeklyMenuId, DateTime? scheduleFrom = null)
    {
        WeeklyMenuId = weeklyMenuId;
        ScheduleFrom = scheduleFrom;
    }
}
