using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.UserRoles;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserRoles;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.UserRoles.CreateUserRole;

public class CreateUserRoleCommandHandler : IRequestHandler<CreateUserRoleCommand, CreateUserRoleResponse>
{
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;

    public CreateUserRoleCommandHandler(
        IUserRoleRepository userRoleRepository,
        IUserRepository userRepository,
        IRoleRepository roleRepository)
    {
        _userRoleRepository = userRoleRepository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
    }

    public async Task<CreateUserRoleResponse> Handle(CreateUserRoleCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;

        if (await _userRepository.GetByIdAsync(req.UserId) == null)
            throw new KeyNotFoundException($"User with ID {req.UserId} not found");
        if (await _roleRepository.GetByIdAsync(req.RoleId) == null)
            throw new KeyNotFoundException($"Role with ID {req.RoleId} not found");
        if (await _userRoleRepository.ExistsByUserAndRoleAsync(req.UserId, req.RoleId))
            throw new InvalidOperationException("User already has this role");

        var entity = new UserRole
        {

            UserId = req.UserId,
            RoleId = req.RoleId,
            AssignedAt = DateTime.UtcNow,
            AssignedBy = req.AssignedBy,
            IsActive = true
        };
        await _userRoleRepository.CreateAsync(entity);

        return new CreateUserRoleResponse
        {
            Id = entity.Id,
            UserId = entity.UserId,
            RoleId = entity.RoleId,
            Message = "User role created successfully"
        };
    }
}
