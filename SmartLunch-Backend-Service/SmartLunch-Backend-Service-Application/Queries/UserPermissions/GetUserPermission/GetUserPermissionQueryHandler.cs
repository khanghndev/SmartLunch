using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserPermissions;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.UserPermissions.GetUserPermission;

public class GetUserPermissionQueryHandler : IRequestHandler<GetUserPermissionQuery, GetUserPermissionResponse>
{
    private readonly IUserPermissionRepository _userPermissionRepository;

    public GetUserPermissionQueryHandler(IUserPermissionRepository userPermissionRepository)
    {
        _userPermissionRepository = userPermissionRepository;
    }

    public async Task<GetUserPermissionResponse> Handle(GetUserPermissionQuery request, CancellationToken cancellationToken)
    {
        var up = await _userPermissionRepository.GetByIdAsync(request.Id);
        if (up == null)
            return new GetUserPermissionResponse { UserPermission = new UserPermissionDto() };

        return new GetUserPermissionResponse
        {
            UserPermission = new UserPermissionDto
            {
                Id = up.Id,
                UserId = up.UserId,
                PermissionId = up.PermissionId,
                UserName = up.User?.Username,
                PermissionName = up.Permission?.Name,
                AssignedAt = up.AssignedAt,
                AssignedBy = up.AssignedBy,
                IsActive = up.IsActive
            }
        };
    }
}
