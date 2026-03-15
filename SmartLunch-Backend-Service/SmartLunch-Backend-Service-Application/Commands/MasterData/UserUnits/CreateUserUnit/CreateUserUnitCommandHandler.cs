using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserUnits;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.UserUnits.CreateUserUnit;

public class CreateUserUnitCommandHandler : IRequestHandler<CreateUserUnitCommand, CreateUserUnitResponse>
{
    private readonly IUserUnitRepository _userUnitRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitRepository _unitRepository;

    public CreateUserUnitCommandHandler(
        IUserUnitRepository userUnitRepository,
        IUserRepository userRepository,
        IUnitRepository unitRepository)
    {
        _userUnitRepository = userUnitRepository;
        _userRepository = userRepository;
        _unitRepository = unitRepository;
    }

    public async Task<CreateUserUnitResponse> Handle(CreateUserUnitCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;

        if (await _userRepository.GetByIdAsync(req.UserId) == null)
            throw new KeyNotFoundException($"User with ID {req.UserId} not found");
        if (await _unitRepository.GetByIdAsync(req.UnitId) == null)
            throw new KeyNotFoundException($"Unit with ID {req.UnitId} not found");
        if (await _userUnitRepository.ExistsByUserAndUnitAsync(req.UserId, req.UnitId))
            throw new InvalidOperationException("User is already assigned to this unit");

        var entity = new UserUnit
        {
            Id = Guid.NewGuid(),
            UserId = req.UserId,
            UnitId = req.UnitId,
            JoinedAt = DateTime.UtcNow,
            IsActive = true
        };
        await _userUnitRepository.CreateAsync(entity);

        return new CreateUserUnitResponse
        {
            Id = entity.Id,
            UserId = entity.UserId,
            UnitId = entity.UnitId,
            Message = "User unit created successfully"
        };
    }
}
