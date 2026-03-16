using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.Auth;
using SmartLunch.Backend.Service.Application.DTOs.Response.Auth;

namespace SmartLunch.Backend.Service.Application.Commands.Auth.LoginUser;

public class LoginUserCommand : IRequest<LoginResponse>
{
    public LoginUserRequest Request { get; set; }

    public LoginUserCommand(LoginUserRequest request)
    {
        Request = request;
    }
}
