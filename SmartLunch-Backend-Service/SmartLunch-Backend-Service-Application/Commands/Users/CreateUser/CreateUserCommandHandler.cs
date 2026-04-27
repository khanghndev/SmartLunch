using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Users;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Users.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, GetUserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository,
        IPasswordHasher passwordHasher,
        ILogger<CreateUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<GetUserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;
        var actor = await _userRepository.GetByIdAsync(request.ActorUserId)
            ?? throw new UnauthorizedAccessException("Actor user not found.");

        if (string.IsNullOrWhiteSpace(req.Username))
            throw new ArgumentException("Username is required.");
        if (string.IsNullOrWhiteSpace(req.Email))
            throw new ArgumentException("Email is required.");
        if (string.IsNullOrWhiteSpace(req.Password) || req.Password.Length < 6)
            throw new ArgumentException("Password is required and must be at least 6 characters.");

        if (await _userRepository.ExistsByUsernameAsync(req.Username.Trim()))
            throw new InvalidOperationException("Username already exists.");
        if (await _userRepository.ExistsByEmailAsync(req.Email.Trim()))
            throw new InvalidOperationException("Email already exists.");

        Role? initialRole = null;
        if (req.InitialRoleId.HasValue && req.InitialRoleId.Value != 0)
        {
            initialRole = await _roleRepository.GetByIdAsync(req.InitialRoleId.Value)
                ?? throw new ArgumentException("Initial role was not found.");
            if (!initialRole.IsActive)
                throw new InvalidOperationException($"Role {initialRole.Name} is not active.");

            if (string.Equals(initialRole.Name, BuiltinRoles.Admin, StringComparison.OrdinalIgnoreCase)
                && !ActorIsAdmin(actor))
            {
                throw new UnauthorizedAccessException("Only Admin can assign the Admin role.");
            }
        }

        var user = new User
        {

            Username = req.Username.Trim(),
            Email = req.Email.Trim(),
            PasswordHash = _passwordHasher.HashPassword(req.Password),
            FirstName = string.IsNullOrWhiteSpace(req.FirstName) ? null : req.FirstName.Trim(),
            LastName = string.IsNullOrWhiteSpace(req.LastName) ? null : req.LastName.Trim(),
            PhoneNumber = string.IsNullOrWhiteSpace(req.PhoneNumber) ? null : req.PhoneNumber.Trim(),
            Provider = "system",
            IsActive = req.IsActive,
            IsEmailVerified = false,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.CreateAsync(user);

        if (initialRole != null)
        {
            await _userRoleRepository.CreateAsync(new UserRole
            {

                UserId = user.Id,
                RoleId = initialRole.Id,
                AssignedAt = DateTime.UtcNow,
                AssignedBy = request.ActorUserId,
                IsActive = true
            });
        }

        var reloaded = await _userRepository.GetByIdAsync(user.Id);
        if (reloaded == null)
        {
            _logger.LogWarning("User missing after create: {UserId}", user.Id);
            return new GetUserResponse { User = new UserDto() };
        }

        return MapResponse(reloaded);
    }

    private static bool ActorIsAdmin(User actor)
    {
        return actor.UserRoles.Any(ur =>
            ur.IsActive && ur.Role != null
            && string.Equals(ur.Role.Name, BuiltinRoles.Admin, StringComparison.OrdinalIgnoreCase));
    }

    private static GetUserResponse MapResponse(User user)
    {
        return new GetUserResponse
        {
            User = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                IsEmailVerified = user.IsEmailVerified,
                LastLoginAt = user.LastLoginAt,
                CreatedAt = user.CreatedAt,
                RoleNames = user.UserRoles
                    .Where(ur => ur.Role != null && ur.IsActive)
                    .Select(ur => ur.Role!.Name)
                    .Distinct()
                    .OrderBy(n => n)
                    .ToList()
            }
        };
    }
}
