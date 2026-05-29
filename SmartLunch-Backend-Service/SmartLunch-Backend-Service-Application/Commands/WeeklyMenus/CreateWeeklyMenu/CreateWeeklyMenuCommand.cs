using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.WeeklyMenus;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.WeeklyMenus;

namespace SmartLunch.Backend.Service.Application.Commands.WeeklyMenus.CreateWeeklyMenu;

public class CreateWeeklyMenuCommand : IRequest<GetWeeklyMenuDetailResponse>
{
    public CreateWeeklyMenuRequest Request { get; }
    public int CreatedByUserId { get; }

    public CreateWeeklyMenuCommand(CreateWeeklyMenuRequest request, int createdByUserId)
    {
        Request = request;
        CreatedByUserId = createdByUserId;
    }
}
