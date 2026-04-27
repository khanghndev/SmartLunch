using MediatR;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.RolePermissions.DeleteRolePermission;

public class DeleteRolePermissionCommand : IRequest<bool>
{
    public int Id { get; set; }

    public DeleteRolePermissionCommand(int id)
    {
        Id = id;
    }
}
