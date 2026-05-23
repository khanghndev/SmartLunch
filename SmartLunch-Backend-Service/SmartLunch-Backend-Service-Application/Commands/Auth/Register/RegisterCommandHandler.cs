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
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IUserTokenRepository userTokenRepository,
        IUserRoleRepository userRoleRepository,
        IUserPermissionRepository userPermissionRepository,
        IRolePermissionRepository rolePermissionRepository,
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
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _logger = logger;
        _unitOfWork = unitOfWork;
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

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Create user - use email as username if username is not provided
            var user = new User
            {
                Email = req.Email,
                PasswordHash = _passwordHasher.HashPassword(req.Password),
                Provider = "system",
                IsActive = true,
                IsEmailVerified = false,
                CreatedAt = VietnamTime.Now
            };
            var userCreate = await _userRepository.CreateAsync(user);

            _logger.LogInformation("User created successfully: {UserId}", userCreate.Id);
            
            var userRole = new UserRole
            {
                UserId = userCreate.Id,
                RoleId = req.RoleId ?? 0,
                AssignedAt = VietnamTime.Now,
                IsActive = true
            };
            await _userRoleRepository.CreateAsync(userRole);

            var rolePermissionList = await _rolePermissionRepository.GetByRoleIdAsync(req.RoleId ?? 0);
            foreach (var rolePermission in rolePermissionList)
            {
                var userPermission = new UserPermission
                {

                    UserId = userCreate.Id,
                    PermissionId = rolePermission.PermissionId,
                };
                await _userPermissionRepository.CreateAsync(userPermission);
            }

            // Generate tokens
            var roles = new List<string> { "Customer" }; // Default role
            var accessToken = _jwtService.GenerateAccessToken(userCreate, roles);
            var refreshToken = _jwtService.GenerateRefreshToken();
            var refreshTokenExpiresAt = VietnamTime.Now.AddDays(7);
            var jti = _jwtService.GetPrincipalFromToken(accessToken)?.Claims
                .FirstOrDefault(c => c.Type == "jti")?.Value;

            // Store token in database
            var userToken = new UserToken
            {
                UserId = userCreate.Id,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                IssuedAt = VietnamTime.Now,
                ExpiresAt = refreshTokenExpiresAt,
                IsActive = true,
                Jti = jti,
            };

            await _userTokenRepository.CreateAsync(userToken);

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
}
