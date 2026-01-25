using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Roles;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Roles.GrantRoleToUser;

public class GrantRoleToUserCommandHandler : IRequestHandler<GrantRoleToUserCommand, GrantRoleToUserResponse>
{
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly ILogger<GrantRoleToUserCommandHandler> _logger;

    public GrantRoleToUserCommandHandler(
        IUserRoleRepository userRoleRepository,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        ILogger<GrantRoleToUserCommandHandler> logger)
    {
        _userRoleRepository = userRoleRepository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _logger = logger;
    }

    public async Task<GrantRoleToUserResponse> Handle(GrantRoleToUserCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;

        // Validate user exists
        var user = await _userRepository.GetByIdAsync(req.UserId);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {req.UserId} not found");
        }

        // Validate role exists
        var role = await _roleRepository.GetByIdAsync(req.RoleId);
        if (role == null)
        {
            throw new KeyNotFoundException($"Role with ID {req.RoleId} not found");
        }

        // Check if role is active
        if (!role.IsActive)
        {
            throw new InvalidOperationException($"Role {role.Name} is not active");
        }

        // Check if user already has this role
        var existingUserRole = await _userRoleRepository.GetByUserAndRoleAsync(req.UserId, req.RoleId);
        if (existingUserRole != null)
        {
            if (existingUserRole.IsActive)
            {
                throw new InvalidOperationException($"User already has role {role.Name}");
            }
            else
            {
                // Reactivate the role
                existingUserRole.IsActive = true;
                existingUserRole.AssignedAt = DateTime.UtcNow;
                await _userRoleRepository.UpdateAsync(existingUserRole);
                
                _logger.LogInformation("Reactivated role {RoleName} for user {UserId}", role.Name, req.UserId);
                
                return new GrantRoleToUserResponse
                {
                    UserId = req.UserId,
                    RoleId = req.RoleId,
                    Message = $"Role {role.Name} reactivated for user"
                };
            }
        }

        // Create new user role assignment
        var userRole = new UserRole
        {
            Id = Guid.NewGuid(),
            UserId = req.UserId,
            RoleId = req.RoleId,
            AssignedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _userRoleRepository.CreateAsync(userRole);

        _logger.LogInformation("Granted role {RoleName} to user {UserId}", role.Name, req.UserId);

        return new GrantRoleToUserResponse
        {
            UserId = req.UserId,
            RoleId = req.RoleId,
            Message = $"Role {role.Name} granted successfully"
        };
    }
}
