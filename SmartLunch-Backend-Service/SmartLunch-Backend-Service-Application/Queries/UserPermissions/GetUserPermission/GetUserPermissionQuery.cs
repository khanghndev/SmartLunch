using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserPermissions;

namespace SmartLunch.Backend.Service.Application.Queries.UserPermissions.GetUserPermission;

public class GetUserPermissionQuery : IRequest<GetUserPermissionResponse>
{
    public Guid Id { get; set; }

    public GetUserPermissionQuery(Guid id)
    {
        Id = id;
    }
}
