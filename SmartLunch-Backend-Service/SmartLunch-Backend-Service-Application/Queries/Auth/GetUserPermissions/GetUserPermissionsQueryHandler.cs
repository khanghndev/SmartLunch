using MediatR;
using SmartLunch.Backend.Service.Application.Queries.Auth;
using SmartLunch.Backend.Service.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace SmartLunch.Backend.Service.Application.Handlers.Queries.Auth;

public class GetUserPermissionsQueryHandler : IRequestHandler<GetUserPermissionsQuery, List<string>>
{
    private readonly ILogger<GetUserPermissionsQueryHandler> _logger;
    private readonly IUserPermissionRepository _userPermissionRepository;

    public GetUserPermissionsQueryHandler(
        ILogger<GetUserPermissionsQueryHandler> logger,
        IUserPermissionRepository userPermissionRepository)
    {
        _logger = logger;
        _userPermissionRepository = userPermissionRepository;
    }

    public async Task<List<string>> Handle(GetUserPermissionsQuery request, CancellationToken cancellationToken)
    {
        var userPermissions = await _userPermissionRepository.GetByUserIdAsync(request.UserId);

        _logger.LogInformation("Retrieved {Count} permissions for user: {UserId}", userPermissions.Count(), request.UserId);

        return userPermissions.Select(up => up.Permission.Name).ToList();
    }
}
