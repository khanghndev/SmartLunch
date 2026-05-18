using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserPermissions;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.UserPermissions.CreateUserPermission;

public class CreateUserPermissionCommandHandler : IRequestHandler<CreateUserPermissionCommand, CreateUserPermissionResponse>
{
    private readonly IUserPermissionRepository _userPermissionRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPermissionRepository _permissionRepository;

    public CreateUserPermissionCommandHandler(
        IUserPermissionRepository userPermissionRepository,
        IUserRepository userRepository,
        IPermissionRepository permissionRepository)
    {
        _userPermissionRepository = userPermissionRepository;
        _userRepository = userRepository;
        _permissionRepository = permissionRepository;
    }

    public async Task<CreateUserPermissionResponse> Handle(CreateUserPermissionCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;

        if (await _userRepository.GetByIdAsync(req.UserId) == null)
            throw new KeyNotFoundException($"User with ID {req.UserId} not found");
        if (await _permissionRepository.GetByIdAsync(req.PermissionId) == null)
            throw new KeyNotFoundException($"Permission with ID {req.PermissionId} not found");
        if (await _userPermissionRepository.ExistsByUserAndPermissionAsync(req.UserId, req.PermissionId))
            throw new InvalidOperationException("User already has this permission");

        var entity = new UserPermission
        {

            UserId = req.UserId,
            PermissionId = req.PermissionId,
            AssignedAt = VietnamTime.Now,
            AssignedBy = req.AssignedBy,
            IsActive = true
        };
        await _userPermissionRepository.CreateAsync(entity);

        return new CreateUserPermissionResponse
        {
            Id = entity.Id,
            UserId = entity.UserId,
            PermissionId = entity.PermissionId,
            Message = "User permission created successfully"
        };
    }
}
