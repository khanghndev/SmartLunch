using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.Commands.Auth;
using SmartLunch.Backend.Service.Application.DTOs.Response.Auth;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Commands.Auth.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    private readonly IJwtService _jwtService;
    private readonly IUserRepository _userRepository;
    private readonly IUserTokenRepository _userTokenRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;
    private readonly int _refreshExpireDays;

    public RefreshTokenCommandHandler(
        IJwtService jwtService,
        IUserRepository userRepository,
        IUserTokenRepository userTokenRepository,
        IConfiguration configuration,
        ILogger<RefreshTokenCommandHandler> logger)
    {
        _jwtService = jwtService;
        _userRepository = userRepository;
        _userTokenRepository = userTokenRepository;
        _configuration = configuration;
        _logger = logger;
        _refreshExpireDays = int.Parse(configuration["Jwt:RefreshTokenExpireDays"] ?? "7");
    }

    public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;

        if (string.IsNullOrWhiteSpace(req.RefreshToken))
            throw new ArgumentException("Refresh token is required");

        var storedToken = await _userTokenRepository.GetByRefreshTokenAsync(req.RefreshToken);
        if (storedToken == null || !storedToken.IsActive)
        {
            _logger.LogWarning("Invalid or inactive refresh token attempted");
            throw new UnauthorizedAccessException("Invalid or expired refresh token");
        }

        if (storedToken.ExpiresAt < VietnamTime.Now)
        {
            _logger.LogWarning("Expired refresh token attempted: {TokenId}", storedToken.Id);
            storedToken.IsActive = false;
            storedToken.RevokedAt = VietnamTime.Now;
            await _userTokenRepository.UpdateAsync(storedToken);
            throw new UnauthorizedAccessException("Refresh token has expired");
        }

        var user = await _userRepository.GetByIdAsync(storedToken.UserId);
        if (user == null || !user.IsActive)
        {
            _logger.LogWarning("User not found or inactive for refresh token: {UserId}", storedToken.UserId);
            throw new KeyNotFoundException("User not found or inactive");
        }

        var oldRefreshToken = storedToken.RefreshToken;

        storedToken.IsActive = false;
        storedToken.RevokedAt = VietnamTime.Now;
        storedToken.ReplacedByToken = oldRefreshToken;
        await _userTokenRepository.UpdateAsync(storedToken);

        var roles = user.UserRoles
            .Where(ur => ur.IsActive)
            .Select(ur => ur.Role.Name)
            .ToList();

        var newAccessToken = _jwtService.GenerateAccessToken(user, roles);
        var newRefreshToken = _jwtService.GenerateRefreshToken();
        var refreshTokenExpiresAt = VietnamTime.Now.AddDays(_refreshExpireDays);

        var jti = _jwtService.GetPrincipalFromToken(newAccessToken)?.Claims
            .FirstOrDefault(c => c.Type == "jti")?.Value;

        var newUserToken = new UserToken
        {
            UserId = user.Id,
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            IssuedAt = VietnamTime.Now,
            ExpiresAt = refreshTokenExpiresAt,
            IsActive = true,
            Jti = jti,
            ReplacedByToken = oldRefreshToken,
        };

        await _userTokenRepository.CreateAsync(newUserToken);

        _logger.LogInformation("Token refreshed successfully for user: {UserId}", user.Id);

        return new RefreshTokenResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            RefreshTokenExpiresAt = refreshTokenExpiresAt,
        };
    }
}
