using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.Auth;
using SmartLunch.Backend.Service.Application.DTOs.Response.Auth;

namespace SmartLunch.Backend.Service.Application.Commands.Auth;

public class RefreshTokenCommand : IRequest<RefreshTokenResponse>
{
    public RefreshTokenRequest Request { get; set; }

    public RefreshTokenCommand(RefreshTokenRequest request)
    {
        Request = request;
    }
}
