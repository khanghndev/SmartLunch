using MediatR;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.UserPermissions.DeleteUserPermission;

public class DeleteUserPermissionCommandHandler : IRequestHandler<DeleteUserPermissionCommand, bool>
{
    private readonly IUserPermissionRepository _userPermissionRepository;

    public DeleteUserPermissionCommandHandler(IUserPermissionRepository userPermissionRepository)
    {
        _userPermissionRepository = userPermissionRepository;
    }

    public async Task<bool> Handle(DeleteUserPermissionCommand request, CancellationToken cancellationToken)
    {
        var deleted = await _userPermissionRepository.DeleteAsync(request.Id);
        if (!deleted)
            throw new KeyNotFoundException($"UserPermission with ID {request.Id} not found");
        return true;
    }
}
