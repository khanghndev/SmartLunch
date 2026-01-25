using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Permissions;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Permissions.GrantPermissionToUser;

public class GrantPermissionToUserCommandHandler : IRequestHandler<GrantPermissionToUserCommand, GrantPermissionToUserResponse>
{
    private readonly IUserPermissionRepository _userPermissionRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly ILogger<GrantPermissionToUserCommandHandler> _logger;

    public GrantPermissionToUserCommandHandler(
        IUserPermissionRepository userPermissionRepository,
        IUserRepository userRepository,
        IPermissionRepository permissionRepository,
        ILogger<GrantPermissionToUserCommandHandler> logger)
    {
        _userPermissionRepository = userPermissionRepository;
        _userRepository = userRepository;
        _permissionRepository = permissionRepository;
        _logger = logger;
    }

    public async Task<GrantPermissionToUserResponse> Handle(GrantPermissionToUserCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;

        // Validate user exists
        var user = await _userRepository.GetByIdAsync(req.UserId);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {req.UserId} not found");
        }

        // Validate permission exists
        var permission = await _permissionRepository.GetByIdAsync(req.PermissionId);
        if (permission == null)
        {
            throw new KeyNotFoundException($"Permission with ID {req.PermissionId} not found");
        }

        // Check if permission is active
        if (!permission.IsActive)
        {
            throw new InvalidOperationException($"Permission {permission.Name} is not active");
        }

        // Check if user already has this permission
        var existingUserPermission = await _userPermissionRepository.GetByUserAndPermissionAsync(req.UserId, req.PermissionId);
        if (existingUserPermission != null)
        {
            if (existingUserPermission.IsActive)
            {
                throw new InvalidOperationException($"User already has permission {permission.Name}");
            }
            else
            {
                // Reactivate the permission
                existingUserPermission.IsActive = true;
                existingUserPermission.AssignedAt = DateTime.UtcNow;
                await _userPermissionRepository.UpdateAsync(existingUserPermission);
                
                _logger.LogInformation("Reactivated permission {PermissionName} for user {UserId}", permission.Name, req.UserId);
                
                return new GrantPermissionToUserResponse
                {
                    UserId = req.UserId,
                    PermissionId = req.PermissionId,
                    Message = $"Permission {permission.Name} reactivated for user"
                };
            }
        }

        // Create new user permission assignment
        var userPermission = new UserPermission
        {
            Id = Guid.NewGuid(),
            UserId = req.UserId,
            PermissionId = req.PermissionId,
            AssignedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _userPermissionRepository.CreateAsync(userPermission);

        _logger.LogInformation("Granted permission {PermissionName} to user {UserId}", permission.Name, req.UserId);

        return new GrantPermissionToUserResponse
        {
            UserId = req.UserId,
            PermissionId = req.PermissionId,
            Message = $"Permission {permission.Name} granted successfully"
        };
    }
}
