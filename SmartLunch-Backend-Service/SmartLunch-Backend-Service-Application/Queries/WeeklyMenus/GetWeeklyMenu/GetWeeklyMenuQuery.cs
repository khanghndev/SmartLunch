using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.WeeklyMenus;

namespace SmartLunch.Backend.Service.Application.Queries.WeeklyMenus.GetWeeklyMenu;

public class GetWeeklyMenuQuery : IRequest<GetWeeklyMenuResponse>
{
    public int WeeklyMenuId { get; set; }

    public GetWeeklyMenuQuery(int weeklyMenuId)
    {
        WeeklyMenuId = weeklyMenuId;
    }
}
