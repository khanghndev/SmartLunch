using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.UserRoles;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserRoles;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.UserRoles.UpdateUserRole;

public class UpdateUserRoleCommandHandler : IRequestHandler<UpdateUserRoleCommand, GetUserRoleResponse>
{
    private readonly IUserRoleRepository _userRoleRepository;

    public UpdateUserRoleCommandHandler(IUserRoleRepository userRoleRepository)
    {
        _userRoleRepository = userRoleRepository;
    }

    public async Task<GetUserRoleResponse> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;
        var entity = await _userRoleRepository.GetByIdAsync(req.Id);
        if (entity == null)
            throw new KeyNotFoundException($"UserRole with ID {req.Id} not found");

        entity.IsActive = req.IsActive;
        await _userRoleRepository.UpdateAsync(entity);

        return new GetUserRoleResponse
        {
            UserRole = new UserRoleDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                RoleId = entity.RoleId,
                UserName = entity.User?.Username,
                RoleName = entity.Role?.Name,
                AssignedAt = entity.AssignedAt,
                AssignedBy = entity.AssignedBy,
                IsActive = entity.IsActive
            }
        };
    }
}
