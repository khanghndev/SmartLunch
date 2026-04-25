using MediatR;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.UserRoles.DeleteUserRole;

public class DeleteUserRoleCommand : IRequest<bool>
{
    public int Id { get; set; }

    public DeleteUserRoleCommand(int id)
    {
        Id = id;
    }
}
