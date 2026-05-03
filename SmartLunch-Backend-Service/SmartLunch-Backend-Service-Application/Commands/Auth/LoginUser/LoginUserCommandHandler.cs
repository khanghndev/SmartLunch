using MediatR;
using SmartLunch.Backend.Service.Application.Commands.Auth.LoginUser;
using SmartLunch.Backend.Service.Application.DTOs.Response.Auth;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace SmartLunch.Backend.Service.Application.Handlers.Auth.LoginUser;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserTokenRepository _userTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _configuration;
    private readonly int _expireDays;
    private readonly ILogger<LoginUserCommandHandler> _logger;

    public LoginUserCommandHandler(
        IUserRepository userRepository,
        IUserTokenRepository userTokenRepository,
        IPasswordHasher passwordHasher,
        IJwtService jwtService,
        IConfiguration configuration,
        ILogger<LoginUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _userTokenRepository = userTokenRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _configuration = configuration;
        _expireDays = int.Parse(_configuration["Jwt:RefreshTokenExpireDays"] ?? "7");
        _logger = logger;
    }

    public async Task<LoginResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;

        // Validate input
        if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Password))
        {
            throw new ArgumentException("Email and password are required");
        }

        // Find user by email
        var user = await _userRepository.GetByEmailAsync(req.Email);
        if (user == null)
        {
            _logger.LogWarning("Login attempt with invalid email: {Email}", req.Email);
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Check if user is using system login (not social login)
        if (user.Provider != "system")
        {
            _logger.LogWarning("Login attempt with password for social login user: {Email}, Provider: {Provider}", req.Email, user.Provider);
            throw new UnauthorizedAccessException($"This account uses {user.Provider} authentication. Please use the appropriate login method.");
        }

        // Verify password
        if (!_passwordHasher.VerifyHashedPassword(user.PasswordHash, req.Password))
        {
            _logger.LogWarning("Login attempt with invalid password for user: {Email}", req.Email);
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Check if user is active
        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("User account is inactive");
        }

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        // Get user roles
        var roles = user.UserRoles
            .Where(ur => ur.IsActive)
            .Select(ur => ur.Role.Name)
            .ToList();

        // Generate tokens
        var accessToken = _jwtService.GenerateAccessToken(user, roles);
        var refreshToken = _jwtService.GenerateRefreshToken();
        var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(_expireDays);

        // Revoke all existing active tokens for this user
        await _userTokenRepository.RevokeAllUserTokensAsync(user.Id);

        // Store new token in database
        var userToken = new UserToken
        {

            UserId = user.Id,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            IssuedAt = DateTime.UtcNow,
            ExpiresAt = refreshTokenExpiresAt,
            IsActive = true
        };

        await _userTokenRepository.CreateAsync(userToken);

        _logger.LogInformation("User logged in successfully: {Email}", user.Email);

        return new LoginResponse
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            FullName = $"{user.FirstName} {user.LastName}".Trim(),
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            RefreshTokenExpiresAt = refreshTokenExpiresAt
        };
    }
}
