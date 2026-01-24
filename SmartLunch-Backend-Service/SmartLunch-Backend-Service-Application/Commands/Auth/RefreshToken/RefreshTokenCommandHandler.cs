using MediatR;
using SmartLunch.Backend.Service.Application.Commands.Auth;
using SmartLunch.Backend.Service.Application.DTOs.Response.Auth;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace SmartLunch.Backend.Service.Application.Handlers.Auth;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    private readonly IJwtService _jwtService;
    private readonly IUserRepository _userRepository;
    private readonly IUserTokenRepository _userTokenRepository;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;

    public RefreshTokenCommandHandler(
        IJwtService jwtService,
        IUserRepository userRepository,
        IUserTokenRepository userTokenRepository,
        ILogger<RefreshTokenCommandHandler> logger)
    {
        _jwtService = jwtService;
        _userRepository = userRepository;
        _userTokenRepository = userTokenRepository;
        _logger = logger;
    }

    public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;

        // Validate refresh token
        if (string.IsNullOrWhiteSpace(req.RefreshToken))
        {
            throw new ArgumentException("Refresh token is required");
        }

        // Get stored token from database
        var storedToken = await _userTokenRepository.GetByRefreshTokenAsync(req.RefreshToken);
        if (storedToken == null || !storedToken.IsActive)
        {
            _logger.LogWarning("Invalid or inactive refresh token attempted");
            throw new UnauthorizedAccessException("Invalid or expired refresh token");
        }

        // Check if token is expired
        if (storedToken.ExpiresAt < DateTime.UtcNow)
        {
            _logger.LogWarning("Expired refresh token attempted: {TokenId}", storedToken.Id);
            // Revoke expired token
            storedToken.IsActive = false;
            storedToken.RevokedAt = DateTime.UtcNow;
            await _userTokenRepository.UpdateAsync(storedToken);

            throw new UnauthorizedAccessException("Refresh token has expired");
        }

        // Get user
        var user = await _userRepository.GetByIdAsync(storedToken.UserId);
        if (user == null || !user.IsActive)
        {
            _logger.LogWarning("User not found or inactive for refresh token: {UserId}", storedToken.UserId);
            throw new KeyNotFoundException("User not found or inactive");
        }

        // Revoke old refresh token
        storedToken.IsActive = false;
        storedToken.RevokedAt = DateTime.UtcNow;
        await _userTokenRepository.UpdateAsync(storedToken);

        // Get user roles
        var roles = user.UserRoles
            .Where(ur => ur.IsActive)
            .Select(ur => ur.Role.Name)
            .ToList();

        // Generate new tokens
        var newAccessToken = _jwtService.GenerateAccessToken(user, roles);
        var newRefreshToken = _jwtService.GenerateRefreshToken();
        var expiresAt = DateTime.UtcNow.AddMinutes(60);
        var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);

        // Store new refresh token
        var newUserToken = new UserToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            IssuedAt = DateTime.UtcNow,
            ExpiresAt = refreshTokenExpiresAt,
            IsActive = true,
            ReplacedByToken = newRefreshToken
        };

        await _userTokenRepository.CreateAsync(newUserToken);

        _logger.LogInformation("Token refreshed successfully for user: {UserId}", user.Id);

        return new RefreshTokenResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            RefreshTokenExpiresAt = refreshTokenExpiresAt
        };
    }
}
