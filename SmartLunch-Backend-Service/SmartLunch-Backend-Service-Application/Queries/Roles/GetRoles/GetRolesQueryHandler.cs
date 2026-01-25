using MediatR;
using SmartLunch.Backend.Service.Application.Interfaces;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Roles;

namespace SmartLunch.Backend.Service.Application.Queries.Roles.GetRoles;

public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, GetRolesResponse>
{
    private readonly IRoleRepository _roleRepository;
    private readonly ILogger<GetRolesQueryHandler> _logger;

    public GetRolesQueryHandler(IRoleRepository roleRepository, ILogger<GetRolesQueryHandler> logger)
    {
        _roleRepository = roleRepository;
        _logger = logger;
    }

    public async Task<GetRolesResponse> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = request.IsActive.HasValue && request.IsActive.Value
            ? await _roleRepository.GetActiveRolesAsync()
            : await _roleRepository.GetAllAsync();

        var response = new GetRolesResponse
        {
            Data = roles.Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                IsActive = r.IsActive,
                IsSystemRole = r.IsSystemRole,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            }).ToList()
        };

        return response;
    }
}
