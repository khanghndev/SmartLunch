using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.Auth;
using SmartLunch.Backend.Service.Application.DTOs.Response.Auth;

namespace SmartLunch.Backend.Service.Application.Commands.Auth;

public class RegisterCommand : IRequest<RegisterResponse>
{
    public RegisterRequest Request { get; set; }

    public RegisterCommand(RegisterRequest request)
    {
        Request = request;
    }
}
