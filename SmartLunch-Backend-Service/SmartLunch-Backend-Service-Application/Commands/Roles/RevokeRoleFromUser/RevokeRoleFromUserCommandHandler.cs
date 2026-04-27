using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Roles;
using SmartLunch.Backend.Service.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Roles.RevokeRoleFromUser;

public class RevokeRoleFromUserCommandHandler : IRequestHandler<RevokeRoleFromUserCommand, RevokeRoleFromUserResponse>
{
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly ILogger<RevokeRoleFromUserCommandHandler> _logger;

    public RevokeRoleFromUserCommandHandler(
        IUserRoleRepository userRoleRepository,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        ILogger<RevokeRoleFromUserCommandHandler> logger)
    {
        _userRoleRepository = userRoleRepository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _logger = logger;
    }

    public async Task<RevokeRoleFromUserResponse> Handle(RevokeRoleFromUserCommand request, CancellationToken cancellationToken)
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

        // Check if user has this role
        var userRole = await _userRoleRepository.GetByUserAndRoleAsync(req.UserId, req.RoleId);
        if (userRole == null || !userRole.IsActive)
        {
            throw new InvalidOperationException($"User does not have active role {role.Name}");
        }

        // Check if it's a system role (optional: prevent revoking system roles)
        if (role.IsSystemRole)
        {
            _logger.LogWarning("Attempt to revoke system role {RoleName} from user {UserId}", role.Name, req.UserId);
        }

        // Deactivate the role assignment (soft delete)
        userRole.IsActive = false;
        await _userRoleRepository.UpdateAsync(userRole);

        _logger.LogInformation("Revoked role {RoleName} from user {UserId}", role.Name, req.UserId);

        return new RevokeRoleFromUserResponse
        {
            UserId = req.UserId,
            RoleId = req.RoleId,
            Message = $"Role {role.Name} revoked successfully"
        };
    }
}
