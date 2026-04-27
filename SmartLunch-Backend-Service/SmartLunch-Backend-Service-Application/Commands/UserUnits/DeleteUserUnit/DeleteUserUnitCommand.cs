using MediatR;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.UserUnits.DeleteUserUnit;

public class DeleteUserUnitCommand : IRequest<bool>
{
    public int Id { get; set; }

    public DeleteUserUnitCommand(int id)
    {
        Id = id;
    }
}
