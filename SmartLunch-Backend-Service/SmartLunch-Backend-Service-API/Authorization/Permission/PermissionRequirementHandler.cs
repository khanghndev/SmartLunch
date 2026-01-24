using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.Queries.Auth;

namespace SmartLunch.Backend.Service.API.Authorization.Permission;

public class PermissionRequirementHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IMediator _mediator;
    private readonly ILogger<PermissionRequirementHandler> _logger;

    public PermissionRequirementHandler(
        IMediator mediator,
        ILogger<PermissionRequirementHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        // Get UserId from claims
        var userIdClaim = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim))
        {
            _logger.LogWarning("User ID claim not found in token");
            return;
        }

        // Parse UserId
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            _logger.LogWarning("Invalid user ID format: {UserId}", userIdClaim);
            return;
        }

        // Get user permissions using MediatR Command
        var userPermissions = await _mediator.Send(new GetUserPermissionsQuery(userId));

        if (userPermissions == null || !userPermissions.Any())
        {
            _logger.LogWarning("No permissions found for user: {UserId}", userId);
            return;
        }

        // Check if user has any of the required permissions
        var hasRequiredPermission = requirement.AllowedPermissions.Any(allowedPermission =>
            userPermissions.Contains(allowedPermission, StringComparer.OrdinalIgnoreCase));

        if (hasRequiredPermission)
        {
            _logger.LogDebug("User {UserId} has required permission. Permissions: {UserPermissions}, Required: {RequiredPermissions}",
                userId, string.Join(", ", userPermissions), string.Join(", ", requirement.AllowedPermissions));
            context.Succeed(requirement);
        }
        else
        {
            _logger.LogWarning("User {UserId} does not have required permission. User permissions: {UserPermissions}, Required: {RequiredPermissions}",
                userId, string.Join(", ", userPermissions), string.Join(", ", requirement.AllowedPermissions));
        }
    }
}
