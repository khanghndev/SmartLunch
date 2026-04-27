using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Systems;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Systems;

namespace SmartLunch.Backend.Service.Application.Commands.Systems.RestoreSystem;

public class RestoreSystemCommand : IRequest<RestoreSystemResponse>
{
    public RestoreSystemRequest Request { get; }
    public int ActorUserId { get; }

    public RestoreSystemCommand(RestoreSystemRequest request, int actorUserId)
    {
        Request = request;
        ActorUserId = actorUserId;
    }
}

