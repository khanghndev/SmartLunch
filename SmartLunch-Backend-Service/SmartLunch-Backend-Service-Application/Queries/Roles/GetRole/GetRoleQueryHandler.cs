using MediatR;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.Queries.Roles.GetRoles;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Roles;

namespace SmartLunch.Backend.Service.Application.Queries.Roles.GetRole;

public class GetRoleQueryHandler : IRequestHandler<GetRoleQuery, GetRoleResponse>
{
    private readonly IRoleRepository _roleRepository;
    private readonly ILogger<GetRoleQueryHandler> _logger;

    public GetRoleQueryHandler(IRoleRepository roleRepository, ILogger<GetRoleQueryHandler> logger)
    {
        _roleRepository = roleRepository;
        _logger = logger;
    }

    public async Task<GetRoleResponse> Handle(GetRoleQuery request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdAsync(request.RoleId);

        if (role == null)
        {
            _logger.LogWarning("Role not found with ID: {RoleId}", request.RoleId);
            return new GetRoleResponse { Role = new RoleDto() };
        }

        var response = new GetRoleResponse
        {
            Role = new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                IsActive = role.IsActive,
                IsSystemRole = role.IsSystemRole,
                CreatedAt = role.CreatedAt,
                UpdatedAt = role.UpdatedAt
            }
        };

        _logger.LogInformation("Retrieved role with ID: {RoleId}", request.RoleId);

        return response;
    }
}
