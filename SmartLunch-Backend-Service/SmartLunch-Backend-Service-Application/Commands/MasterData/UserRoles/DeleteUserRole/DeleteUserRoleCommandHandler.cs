using MediatR;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.UserRoles.DeleteUserRole;

public class DeleteUserRoleCommandHandler : IRequestHandler<DeleteUserRoleCommand, bool>
{
    private readonly IUserRoleRepository _userRoleRepository;

    public DeleteUserRoleCommandHandler(IUserRoleRepository userRoleRepository)
    {
        _userRoleRepository = userRoleRepository;
    }

    public async Task<bool> Handle(DeleteUserRoleCommand request, CancellationToken cancellationToken)
    {
        var deleted = await _userRoleRepository.DeleteAsync(request.Id);
        if (!deleted)
            throw new KeyNotFoundException($"UserRole with ID {request.Id} not found");
        return true;
    }
}
