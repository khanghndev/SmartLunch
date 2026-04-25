using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.MenuSchedules;

namespace SmartLunch.Backend.Service.Application.Queries.MenuSchedules.GetMenuSchedule;

public class GetMenuScheduleQuery : IRequest<GetMenuScheduleResponse>
{
    public int MenuScheduleId { get; set; }

    public GetMenuScheduleQuery(int menuScheduleId)
    {
        MenuScheduleId = menuScheduleId;
    }
}
