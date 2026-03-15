using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.UserUnits;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserUnits;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.UserUnits.UpdateUserUnit;

public class UpdateUserUnitCommandHandler : IRequestHandler<UpdateUserUnitCommand, GetUserUnitResponse>
{
    private readonly IUserUnitRepository _userUnitRepository;

    public UpdateUserUnitCommandHandler(IUserUnitRepository userUnitRepository)
    {
        _userUnitRepository = userUnitRepository;
    }

    public async Task<GetUserUnitResponse> Handle(UpdateUserUnitCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;
        var entity = await _userUnitRepository.GetByIdAsync(req.Id);
        if (entity == null)
            throw new KeyNotFoundException($"UserUnit with ID {req.Id} not found");

        entity.IsActive = req.IsActive;
        await _userUnitRepository.UpdateAsync(entity);

        return new GetUserUnitResponse
        {
            UserUnit = new UserUnitDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                UnitId = entity.UnitId,
                UserName = entity.User?.Username,
                UnitName = entity.Unit?.Name,
                JoinedAt = entity.JoinedAt,
                IsActive = entity.IsActive
            }
        };
    }
}
