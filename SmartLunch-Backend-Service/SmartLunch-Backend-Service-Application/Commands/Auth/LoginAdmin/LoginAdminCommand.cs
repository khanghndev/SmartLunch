using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.Auth;
using SmartLunch.Backend.Service.Application.DTOs.Response.Auth;

namespace SmartLunch.Backend.Service.Application.Commands.Auth.LoginAdmin;

public class LoginAdminCommand : IRequest<LoginResponse>
{
    public LoginAdminRequest Request { get; set; }

    public LoginAdminCommand(LoginAdminRequest request)
    {
        Request = request;
    }
}
