using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.Auth;
using SmartLunch.Backend.Service.Application.DTOs.Response.Auth;

namespace SmartLunch.Backend.Service.Application.Commands.Auth;

public class LogoutCommand : IRequest<LogoutResponse>
{
    public LogoutRequest Request { get; set; }

    public LogoutCommand(LogoutRequest request)
    {
        Request = request;
    }
}
