using MediatR;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.UserUnits.DeleteUserUnit;

public class DeleteUserUnitCommand : IRequest<bool>
{
    public Guid Id { get; set; }

    public DeleteUserUnitCommand(Guid id)
    {
        Id = id;
    }
}
