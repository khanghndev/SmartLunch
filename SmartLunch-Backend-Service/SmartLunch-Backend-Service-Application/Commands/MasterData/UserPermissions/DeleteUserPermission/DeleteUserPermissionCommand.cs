using MediatR;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.UserPermissions.DeleteUserPermission;

public class DeleteUserPermissionCommand : IRequest<bool>
{
    public Guid Id { get; set; }

    public DeleteUserPermissionCommand(Guid id)
    {
        Id = id;
    }
}
