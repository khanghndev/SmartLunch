using MediatR;
using SmartLunch.Backend.Service.Application.Commands.Auth;
using SmartLunch.Backend.Service.Application.DTOs.Response.Auth;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace SmartLunch.Backend.Service.Application.Handlers.Auth;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserTokenRepository _userTokenRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IUserPermissionRepository _userPermissionRepository;
    private readonly IRolePermissionRepository _rolePermissionRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly ILogger<RegisterCommandHandler> _logger;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IUserTokenRepository userTokenRepository,
        IUserRoleRepository userRoleRepository,
        IUserPermissionRepository userPermissionRepository,
        IRolePermissionRepository rolePermissionRepository,
        IPasswordHasher passwordHasher,
        IJwtService jwtService,
        ILogger<RegisterCommandHandler> logger)
    {
        _userRepository = userRepository;
        _userTokenRepository = userTokenRepository;
        _userRoleRepository = userRoleRepository;
        _userPermissionRepository = userPermissionRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _logger = logger;
    }

    public async Task<RegisterResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;

        // Validate input
        if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Password))
        {
            throw new ArgumentException("Email and password are required");
        }

        // Validate password confirmation
        if (req.Password != req.ConfirmPassword)
        {
            throw new ArgumentException("Password and confirm password do not match");
        }

        // Check if email already exists (using email as username)
        if (await _userRepository.GetByEmailAsync(req.Email) != null)
        {
            throw new InvalidOperationException("Email already exists");
        }

        // Create user - use email as username if username is not provided
        var user = new User
        {

            Username = req.Email, // Use email as username
            Email = req.Email,
            PasswordHash = _passwordHasher.HashPassword(req.Password),
            Provider = "system",
            IsActive = true,
            IsEmailVerified = false,
            CreatedAt = DateTime.UtcNow
        };
        await _userRepository.CreateAsync(user);

        var userRole = new UserRole
        {

            UserId = user.Id,
            RoleId = req.RoleId ?? 0,
            AssignedAt = DateTime.UtcNow,
            IsActive = true
        };
        await _userRoleRepository.CreateAsync(userRole);

        var rolePermissionList = await _rolePermissionRepository.GetByRoleIdAsync(req.RoleId ?? 0);
        foreach (var rolePermission in rolePermissionList)
        {
            var userPermission = new UserPermission
            {

                UserId = user.Id,
                PermissionId = rolePermission.PermissionId,
            };
            await _userPermissionRepository.CreateAsync(userPermission);
        }

        // Generate tokens
        var roles = new List<string> { "User" }; // Default role
        var accessToken = _jwtService.GenerateAccessToken(user, roles);
        var refreshToken = _jwtService.GenerateRefreshToken();
        var expiresAt = DateTime.UtcNow.AddMinutes(60);
        var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);

        // Store token in database
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

        _logger.LogInformation("User registered successfully: {Email}", user.Email);

        return new RegisterResponse
        {
            Username = user.Username,
            Email = user.Email
        };
    }
}
