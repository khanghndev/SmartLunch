using MediatR;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.RolePermissions.DeleteRolePermission;

public class DeleteRolePermissionCommand : IRequest<bool>
{
    public Guid Id { get; set; }

    public DeleteRolePermissionCommand(Guid id)
    {
        Id = id;
    }
}
