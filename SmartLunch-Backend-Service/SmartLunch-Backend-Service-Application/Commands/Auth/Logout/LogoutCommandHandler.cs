using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.Commands.Auth;
using SmartLunch.Backend.Service.Application.DTOs.Response.Auth;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Handlers.Auth;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, LogoutResponse>
{
    private readonly IJwtService _jwtService;
    private readonly IUserTokenRepository _userTokenRepository;
    private readonly ILogger<LogoutCommandHandler> _logger;

    public LogoutCommandHandler(
        IJwtService jwtService,
        IUserTokenRepository userTokenRepository,
        ILogger<LogoutCommandHandler> logger)
    {
        _jwtService = jwtService;
        _userTokenRepository = userTokenRepository;
        _logger = logger;
    }

    public async Task<LogoutResponse> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;

        // Validate token
        if (string.IsNullOrWhiteSpace(req.Token))
        {
            throw new ArgumentException("Token is required");
        }

        // Find token in database
        var userToken = await _userTokenRepository.GetByAccessTokenAsync(req.Token);
        if (userToken == null || !userToken.IsActive)
        {
            _logger.LogWarning("Logout attempted with invalid or inactive token");
            throw new UnauthorizedAccessException("Invalid token");
        }

        // Revoke the token
        userToken.IsActive = false;
        userToken.RevokedAt = DateTime.UtcNow;
        await _userTokenRepository.UpdateAsync(userToken);

        // Also revoke associated refresh token if exists
        if (!string.IsNullOrEmpty(userToken.RefreshToken))
        {
            var refreshToken = await _userTokenRepository.GetByRefreshTokenAsync(userToken.RefreshToken);
            if (refreshToken != null && refreshToken.IsActive)
            {
                refreshToken.IsActive = false;
                refreshToken.RevokedAt = DateTime.UtcNow;
                await _userTokenRepository.UpdateAsync(refreshToken);
            }
        }

        _logger.LogInformation("User logged out successfully. UserId: {UserId}", userToken.UserId);

        return new LogoutResponse
        {
            Token = userToken.AccessToken
        };
    }
}
