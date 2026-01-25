using MediatR;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Permissions;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Queries.Permissions.GetPermissions;

public class GetPermissionsQueryHandler : IRequestHandler<GetPermissionsQuery, GetPermissionsResponse>
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly ILogger<GetPermissionsQueryHandler> _logger;

    public GetPermissionsQueryHandler(IPermissionRepository permissionRepository, ILogger<GetPermissionsQueryHandler> logger)
    {
        _permissionRepository = permissionRepository;
        _logger = logger;
    }

    public async Task<GetPermissionsResponse> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Permission> permissions;

        if (!string.IsNullOrWhiteSpace(request.Resource))
        {
            permissions = await _permissionRepository.GetByResourceAsync(request.Resource);
        }
        else if (!string.IsNullOrWhiteSpace(request.Action))
        {
            permissions = await _permissionRepository.GetByActionAsync(request.Action);
        }
        else if (request.IsActive.HasValue && request.IsActive.Value)
        {
            permissions = await _permissionRepository.GetActivePermissionsAsync();
        }
        else
        {
            permissions = await _permissionRepository.GetAllAsync();
        }

        var response = new GetPermissionsResponse
        {
            Data = permissions.Select(p => new PermissionDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Resource = p.Resource,
                Action = p.Action,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            }).ToList()
        };

        return response;
    }
}
