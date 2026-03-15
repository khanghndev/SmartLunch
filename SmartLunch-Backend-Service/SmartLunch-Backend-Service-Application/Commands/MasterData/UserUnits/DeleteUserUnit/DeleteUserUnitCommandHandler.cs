using MediatR;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.UserUnits.DeleteUserUnit;

public class DeleteUserUnitCommandHandler : IRequestHandler<DeleteUserUnitCommand, bool>
{
    private readonly IUserUnitRepository _userUnitRepository;

    public DeleteUserUnitCommandHandler(IUserUnitRepository userUnitRepository)
    {
        _userUnitRepository = userUnitRepository;
    }

    public async Task<bool> Handle(DeleteUserUnitCommand request, CancellationToken cancellationToken)
    {
        var deleted = await _userUnitRepository.DeleteAsync(request.Id);
        if (!deleted)
            throw new KeyNotFoundException($"UserUnit with ID {request.Id} not found");
        return true;
    }
}
