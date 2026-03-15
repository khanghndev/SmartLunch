using MediatR;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.UserRoles.DeleteUserRole;

public class DeleteUserRoleCommand : IRequest<bool>
{
    public Guid Id { get; set; }

    public DeleteUserRoleCommand(Guid id)
    {
        Id = id;
    }
}
