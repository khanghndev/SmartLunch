using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.UserPermissions;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserPermissions;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.UserPermissions.UpdateUserPermission;

public class UpdateUserPermissionCommandHandler : IRequestHandler<UpdateUserPermissionCommand, GetUserPermissionResponse>
{
    private readonly IUserPermissionRepository _userPermissionRepository;

    public UpdateUserPermissionCommandHandler(IUserPermissionRepository userPermissionRepository)
    {
        _userPermissionRepository = userPermissionRepository;
    }

    public async Task<GetUserPermissionResponse> Handle(UpdateUserPermissionCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;
        var entity = await _userPermissionRepository.GetByIdAsync(req.Id);
        if (entity == null)
            throw new KeyNotFoundException($"UserPermission with ID {req.Id} not found");

        entity.IsActive = req.IsActive;
        await _userPermissionRepository.UpdateAsync(entity);

        return new GetUserPermissionResponse
        {
            UserPermission = new UserPermissionDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                PermissionId = entity.PermissionId,
                UserName = entity.User?.Username,
                PermissionName = entity.Permission?.Name,
                AssignedAt = entity.AssignedAt,
                AssignedBy = entity.AssignedBy,
                IsActive = entity.IsActive
            }
        };
    }
}
