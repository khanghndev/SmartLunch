using Microsoft.AspNetCore.Authorization;

namespace SmartLunch.Backend.Service.API.Authorization.Permission;

public class PermissionRequirement : IAuthorizationRequirement
{
    public PermissionRequirement(params string[] allowedPermissions)
    {
        AllowedPermissions = (allowedPermissions ?? Array.Empty<string>()).ToArray();
    }

    public IReadOnlyCollection<string> AllowedPermissions { get; }
}