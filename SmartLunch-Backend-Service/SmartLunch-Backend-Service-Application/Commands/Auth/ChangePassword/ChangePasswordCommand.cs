using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.Auth;
using SmartLunch.Backend.Service.Application.DTOs.Response.Auth;

namespace SmartLunch.Backend.Service.Application.Commands.Auth.ChangePassword;

public class ChangePasswordCommand : IRequest<ResetPasswordResponse>
{
    public int UserId { get; }
    public ChangePasswordRequest Request { get; }

    public ChangePasswordCommand(int userId, ChangePasswordRequest request)
    {
        UserId = userId;
        Request = request;
    }
}
