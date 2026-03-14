using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.WeeklyMenus;

namespace SmartLunch.Backend.Service.Application.Queries.WeeklyMenus.GetWeeklyMenu;

public class GetWeeklyMenuQuery : IRequest<GetWeeklyMenuResponse>
{
    public Guid WeeklyMenuId { get; set; }

    public GetWeeklyMenuQuery(Guid weeklyMenuId)
    {
        WeeklyMenuId = weeklyMenuId;
    }
}
