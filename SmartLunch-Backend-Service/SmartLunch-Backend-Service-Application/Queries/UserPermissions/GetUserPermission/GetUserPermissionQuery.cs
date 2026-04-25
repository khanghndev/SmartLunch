using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserPermissions;

namespace SmartLunch.Backend.Service.Application.Queries.UserPermissions.GetUserPermission;

public class GetUserPermissionQuery : IRequest<GetUserPermissionResponse>
{
    public int Id { get; set; }

    public GetUserPermissionQuery(int id)
    {
        Id = id;
    }
}
