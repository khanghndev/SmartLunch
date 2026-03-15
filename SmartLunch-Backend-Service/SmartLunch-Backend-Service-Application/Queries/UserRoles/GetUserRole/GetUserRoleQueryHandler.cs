using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserRoles;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.UserRoles.GetUserRole;

public class GetUserRoleQueryHandler : IRequestHandler<GetUserRoleQuery, GetUserRoleResponse>
{
    private readonly IUserRoleRepository _userRoleRepository;

    public GetUserRoleQueryHandler(IUserRoleRepository userRoleRepository)
    {
        _userRoleRepository = userRoleRepository;
    }

    public async Task<GetUserRoleResponse> Handle(GetUserRoleQuery request, CancellationToken cancellationToken)
    {
        var userRole = await _userRoleRepository.GetByIdAsync(request.Id);
        if (userRole == null)
            return new GetUserRoleResponse { UserRole = new UserRoleDto() };

        return new GetUserRoleResponse
        {
            UserRole = new UserRoleDto
            {
                Id = userRole.Id,
                UserId = userRole.UserId,
                RoleId = userRole.RoleId,
                UserName = userRole.User?.Username,
                RoleName = userRole.Role?.Name,
                AssignedAt = userRole.AssignedAt,
                AssignedBy = userRole.AssignedBy,
                IsActive = userRole.IsActive
            }
        };
    }
}
