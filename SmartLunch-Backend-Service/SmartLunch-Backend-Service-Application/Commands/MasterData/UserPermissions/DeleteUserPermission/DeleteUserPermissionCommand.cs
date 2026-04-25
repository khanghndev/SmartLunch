using MediatR;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.UserPermissions.DeleteUserPermission;

public class DeleteUserPermissionCommand : IRequest<bool>
{
    public int Id { get; set; }

    public DeleteUserPermissionCommand(int id)
    {
        Id = id;
    }
}
