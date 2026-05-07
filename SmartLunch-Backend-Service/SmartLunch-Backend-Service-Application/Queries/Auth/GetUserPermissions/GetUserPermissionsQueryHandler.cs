using MediatR;
using SmartLunch.Backend.Service.Application.Queries.Auth;
using SmartLunch.Backend.Service.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace SmartLunch.Backend.Service.Application.Handlers.Queries.Auth;

public class GetUserPermissionsQueryHandler : IRequestHandler<GetUserPermissionsQuery, List<string>>
{
    private readonly ILogger<GetUserPermissionsQueryHandler> _logger;
    private readonly IUserPermissionRepository _userPermissionRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IRolePermissionRepository _rolePermissionRepository;

    public GetUserPermissionsQueryHandler(
        ILogger<GetUserPermissionsQueryHandler> logger,
        IUserPermissionRepository userPermissionRepository,
        IUserRoleRepository userRoleRepository,
        IRolePermissionRepository rolePermissionRepository)
    {
        _logger = logger;
        _userPermissionRepository = userPermissionRepository;
        _userRoleRepository = userRoleRepository;
        _rolePermissionRepository = rolePermissionRepository;
    }

    public async Task<List<string>> Handle(GetUserPermissionsQuery request, CancellationToken cancellationToken)
    {
        var permissions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // Get direct user permissions
        var userPermissions = await _userPermissionRepository.GetByUserIdAsync(request.UserId);
        foreach (var up in userPermissions)
        {
            if (up.Permission != null && up.Permission.IsActive && up.IsActive)
            {
                permissions.Add(up.Permission.Name);
            }
        }

        // Get role-based permissions
        var userRoles = await _userRoleRepository.GetByUserIdAsync(request.UserId);
        foreach (var ur in userRoles.Where(r => r.IsActive && (r.Role == null || r.Role.IsActive)))
        {
            var rolePermissions = await _rolePermissionRepository.GetByRoleIdAsync(ur.RoleId);
            foreach (var rp in rolePermissions)
            {
                if (rp.Permission != null && rp.Permission.IsActive && rp.IsActive)
                {
                    permissions.Add(rp.Permission.Name);
                }
            }
        }

        _logger.LogInformation("Retrieved {Count} total permissions for user: {UserId}", permissions.Count, request.UserId);

        return permissions.ToList();
    }
}
