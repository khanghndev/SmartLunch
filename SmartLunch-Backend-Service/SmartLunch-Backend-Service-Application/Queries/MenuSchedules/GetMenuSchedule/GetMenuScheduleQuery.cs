using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.MenuSchedules;

namespace SmartLunch.Backend.Service.Application.Queries.MenuSchedules.GetMenuSchedule;

public class GetMenuScheduleQuery : IRequest<GetMenuScheduleResponse>
{
    public Guid MenuScheduleId { get; set; }

    public GetMenuScheduleQuery(Guid menuScheduleId)
    {
        MenuScheduleId = menuScheduleId;
    }
}
