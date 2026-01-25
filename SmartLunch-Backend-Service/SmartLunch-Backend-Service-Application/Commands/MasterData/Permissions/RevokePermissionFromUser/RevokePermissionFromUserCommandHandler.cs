using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Permissions;
using SmartLunch.Backend.Service.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Permissions.RevokePermissionFromUser;

public class RevokePermissionFromUserCommandHandler : IRequestHandler<RevokePermissionFromUserCommand, RevokePermissionFromUserResponse>
{
    private readonly IUserPermissionRepository _userPermissionRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly ILogger<RevokePermissionFromUserCommandHandler> _logger;

    public RevokePermissionFromUserCommandHandler(
        IUserPermissionRepository userPermissionRepository,
        IUserRepository userRepository,
        IPermissionRepository permissionRepository,
        ILogger<RevokePermissionFromUserCommandHandler> logger)
    {
        _userPermissionRepository = userPermissionRepository;
        _userRepository = userRepository;
        _permissionRepository = permissionRepository;
        _logger = logger;
    }

    public async Task<RevokePermissionFromUserResponse> Handle(RevokePermissionFromUserCommand request, CancellationToken cancellationToken)
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

        // Check if user has this permission
        var userPermission = await _userPermissionRepository.GetByUserAndPermissionAsync(req.UserId, req.PermissionId);
        if (userPermission == null || !userPermission.IsActive)
        {
            throw new InvalidOperationException($"User does not have active permission {permission.Name}");
        }

        // Deactivate the permission assignment (soft delete)
        userPermission.IsActive = false;
        await _userPermissionRepository.UpdateAsync(userPermission);

        _logger.LogInformation("Revoked permission {PermissionName} from user {UserId}", permission.Name, req.UserId);

        return new RevokePermissionFromUserResponse
        {
            UserId = req.UserId,
            PermissionId = req.PermissionId,
            Message = $"Permission {permission.Name} revoked successfully"
        };
    }
}
