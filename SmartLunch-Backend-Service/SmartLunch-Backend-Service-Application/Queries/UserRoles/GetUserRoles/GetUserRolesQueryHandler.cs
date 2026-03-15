using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserRoles;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.UserRoles.GetUserRoles;

public class GetUserRolesQueryHandler : IRequestHandler<GetUserRolesQuery, GetUserRolesResponse>
{
    private readonly IUserRoleRepository _userRoleRepository;

    public GetUserRolesQueryHandler(IUserRoleRepository userRoleRepository)
    {
        _userRoleRepository = userRoleRepository;
    }

    public async Task<GetUserRolesResponse> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _userRoleRepository.GetPagedAsync(
            request.Page, request.PageSize, request.UserId, request.RoleId, request.IsActive);

        var dtos = items.Select(ur => new UserRoleDto
        {
            Id = ur.Id,
            UserId = ur.UserId,
            RoleId = ur.RoleId,
            UserName = ur.User?.Username,
            RoleName = ur.Role?.Name,
            AssignedAt = ur.AssignedAt,
            AssignedBy = ur.AssignedBy,
            IsActive = ur.IsActive
        }).ToList();

        return new GetUserRolesResponse
        {
            Data = dtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
