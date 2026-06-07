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
    private const string DefaultCustomerRoleName = "Customer";

    private readonly IUserRepository _userRepository;
    private readonly IUserTokenRepository _userTokenRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IUserPermissionRepository _userPermissionRepository;
    private readonly IRolePermissionRepository _rolePermissionRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly ILogger<RegisterCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IUserTokenRepository userTokenRepository,
        IUserRoleRepository userRoleRepository,
        IUserPermissionRepository userPermissionRepository,
        IRolePermissionRepository rolePermissionRepository,
        IRoleRepository roleRepository,
        IPasswordHasher passwordHasher,
        IJwtService jwtService,
        IUnitOfWork unitOfWork,
        ILogger<RegisterCommandHandler> logger)
    {
        _userRepository = userRepository;
        _userTokenRepository = userTokenRepository;
        _userRoleRepository = userRoleRepository;
        _userPermissionRepository = userPermissionRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _roleRepository = roleRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<RegisterResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;
        var email = req.Email?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(req.Password))
            throw new ArgumentException("Email and password are required");

        if (req.Password.Length < 6)
            throw new ArgumentException("Password must be at least 6 characters");

        if (req.Password != req.ConfirmPassword)
            throw new ArgumentException("Password and confirm password do not match");

        if (await _userRepository.GetByEmailAsync(email) != null)
            throw new InvalidOperationException("Email already exists");

        if (await _userRepository.ExistsByUsernameAsync(email))
            throw new InvalidOperationException("Email already exists");

        var role = await ResolveRoleAsync(req.RoleId);
        if (role == null || !role.IsActive)
            throw new InvalidOperationException($"Role '{DefaultCustomerRoleName}' is not configured");

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var (firstName, lastName) = SplitFullName(req.FullName);

            var user = new User
            {
                Username = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = string.IsNullOrWhiteSpace(req.PhoneNumber) ? null : req.PhoneNumber.Trim(),
                PasswordHash = _passwordHasher.HashPassword(req.Password),
                Provider = "system",
                IsActive = true,
                IsEmailVerified = false,
                CreatedAt = VietnamTime.Now
            };
            var userCreate = await _userRepository.CreateAsync(user);

            _logger.LogInformation("User created successfully: {UserId}", userCreate.Id);

            await _userRoleRepository.CreateAsync(new UserRole
            {
                UserId = userCreate.Id,
                RoleId = role.Id,
                AssignedAt = VietnamTime.Now,
                IsActive = true
            });

            var rolePermissionList = await _rolePermissionRepository.GetByRoleIdAsync(role.Id);
            foreach (var rolePermission in rolePermissionList)
            {
                await _userPermissionRepository.CreateAsync(new UserPermission
                {
                    UserId = userCreate.Id,
                    PermissionId = rolePermission.PermissionId,
                    IsActive = true
                });
            }

            var roles = new List<string> { role.Name };
            var accessToken = _jwtService.GenerateAccessToken(userCreate, roles);
            var refreshToken = _jwtService.GenerateRefreshToken();
            var refreshTokenExpiresAt = VietnamTime.Now.AddDays(7);
            var jti = _jwtService.GetPrincipalFromToken(accessToken)?.Claims
                .FirstOrDefault(c => c.Type == "jti")?.Value;

            await _userTokenRepository.CreateAsync(new UserToken
            {
                UserId = userCreate.Id,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                IssuedAt = VietnamTime.Now,
                ExpiresAt = refreshTokenExpiresAt,
                IsActive = true,
                Jti = jti,
            });

            await _unitOfWork.CommitAsync();

            _logger.LogInformation("User registered successfully: {Email}", userCreate.Email);

            return new RegisterResponse
            {
                Username = userCreate.Username,
                Email = userCreate.Email
            };
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    private async Task<Role?> ResolveRoleAsync(int? roleId)
    {
        if (roleId.HasValue && roleId.Value > 0)
            return await _roleRepository.GetByIdAsync(roleId.Value);

        return await _roleRepository.GetByNameAsync(DefaultCustomerRoleName);
    }

    private static (string? FirstName, string? LastName) SplitFullName(string? fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            return (null, null);

        var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
            return (null, null);
        if (parts.Length == 1)
            return (parts[0], null);

        return (parts[^1], string.Join(' ', parts[..^1]));
    }
}
