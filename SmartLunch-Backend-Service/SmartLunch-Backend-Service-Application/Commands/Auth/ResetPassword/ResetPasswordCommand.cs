using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.Auth;
using SmartLunch.Backend.Service.Application.DTOs.Response.Auth;

namespace SmartLunch.Backend.Service.Application.Commands.Auth;

public class ResetPasswordCommand : IRequest<ResetPasswordResponse>
{
    public ResetPasswordRequest Request { get; }

    public ResetPasswordCommand(ResetPasswordRequest request)
    {
        Request = request;
    }
}

