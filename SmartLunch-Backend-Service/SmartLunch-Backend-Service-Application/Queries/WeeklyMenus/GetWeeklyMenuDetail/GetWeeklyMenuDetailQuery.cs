using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.WeeklyMenus;

namespace SmartLunch.Backend.Service.Application.Queries.WeeklyMenus.GetWeeklyMenuDetail;

public class GetWeeklyMenuDetailQuery : IRequest<GetWeeklyMenuDetailResponse>
{
    public int WeeklyMenuId { get; set; }

    public GetWeeklyMenuDetailQuery(int weeklyMenuId)
    {
        WeeklyMenuId = weeklyMenuId;
    }
}
