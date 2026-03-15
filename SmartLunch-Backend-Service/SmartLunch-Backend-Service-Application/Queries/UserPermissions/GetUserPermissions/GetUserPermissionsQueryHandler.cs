using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserPermissions;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.UserPermissions.GetUserPermissions;

public class GetUserPermissionsQueryHandler : IRequestHandler<GetUserPermissionsQuery, GetUserPermissionsResponse>
{
    private readonly IUserPermissionRepository _userPermissionRepository;

    public GetUserPermissionsQueryHandler(IUserPermissionRepository userPermissionRepository)
    {
        _userPermissionRepository = userPermissionRepository;
    }

    public async Task<GetUserPermissionsResponse> Handle(GetUserPermissionsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _userPermissionRepository.GetPagedAsync(
            request.Page, request.PageSize, request.UserId, request.PermissionId, request.IsActive);

        var dtos = items.Select(up => new UserPermissionDto
        {
            Id = up.Id,
            UserId = up.UserId,
            PermissionId = up.PermissionId,
            UserName = up.User?.Username,
            PermissionName = up.Permission?.Name,
            AssignedAt = up.AssignedAt,
            AssignedBy = up.AssignedBy,
            IsActive = up.IsActive
        }).ToList();

        return new GetUserPermissionsResponse
        {
            Data = dtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
