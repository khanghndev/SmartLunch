using MediatR;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.Queries.Permissions.GetPermissions;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Permissions;

namespace SmartLunch.Backend.Service.Application.Queries.Permissions.GetPermission;

public class GetPermissionQueryHandler : IRequestHandler<GetPermissionQuery, GetPermissionResponse>
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly ILogger<GetPermissionQueryHandler> _logger;

    public GetPermissionQueryHandler(IPermissionRepository permissionRepository, ILogger<GetPermissionQueryHandler> logger)
    {
        _permissionRepository = permissionRepository;
        _logger = logger;
    }

    public async Task<GetPermissionResponse> Handle(GetPermissionQuery request, CancellationToken cancellationToken)
    {
        var permission = await _permissionRepository.GetByIdAsync(request.PermissionId);

        if (permission == null)
        {
            _logger.LogWarning("Permission not found with ID: {PermissionId}", request.PermissionId);
            return new GetPermissionResponse { Permission = new PermissionDto() };
        }

        var permissionDto = new GetPermissionResponse
        {
            Permission = new PermissionDto
            {
                Id = permission.Id,
                Name = permission.Name,
                Description = permission.Description,
                Resource = permission.Resource,
                Action = permission.Action,
                IsActive = permission.IsActive,
                CreatedAt = permission.CreatedAt,
                UpdatedAt = permission.UpdatedAt
            }
        };

        _logger.LogInformation("Retrieved permission with ID: {PermissionId}", request.PermissionId);

        return permissionDto;
    }
}
