using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.Queries.Auth;

namespace SmartLunch.Backend.Service.API.Authorization.Role;

public class RoleRequirementHandler : AuthorizationHandler<RoleRequirement>
{
    private readonly IMediator _mediator;
    private readonly ILogger<RoleRequirementHandler> _logger;

    public RoleRequirementHandler(
        IMediator mediator,
        ILogger<RoleRequirementHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        RoleRequirement requirement)
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

        // Get user roles using MediatR Command
        var userRoles = await _mediator.Send(new GetUserRolesQuery(userId));

        if (userRoles == null || !userRoles.Any())
        {
            _logger.LogWarning("No roles found for user: {UserId}", userId);
            return;
        }

        // Check if user has any of the required roles
        var hasRequiredRole = requirement.AllowedRoles.Any(allowedRole =>
            userRoles.Contains(allowedRole, StringComparer.OrdinalIgnoreCase));

        if (hasRequiredRole)
        {
            _logger.LogDebug("User {UserId} has required role. Roles: {UserRoles}, Required: {RequiredRoles}",
                userId, string.Join(", ", userRoles), string.Join(", ", requirement.AllowedRoles));
            context.Succeed(requirement);
        }
        else
        {
            _logger.LogWarning("User {UserId} does not have required role. User roles: {UserRoles}, Required: {RequiredRoles}",
                userId, string.Join(", ", userRoles), string.Join(", ", requirement.AllowedRoles));
        }
    }
}
