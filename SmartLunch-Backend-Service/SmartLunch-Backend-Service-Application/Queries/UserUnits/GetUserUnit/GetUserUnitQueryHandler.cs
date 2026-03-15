using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserUnits;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.UserUnits.GetUserUnit;

public class GetUserUnitQueryHandler : IRequestHandler<GetUserUnitQuery, GetUserUnitResponse>
{
    private readonly IUserUnitRepository _userUnitRepository;

    public GetUserUnitQueryHandler(IUserUnitRepository userUnitRepository)
    {
        _userUnitRepository = userUnitRepository;
    }

    public async Task<GetUserUnitResponse> Handle(GetUserUnitQuery request, CancellationToken cancellationToken)
    {
        var uu = await _userUnitRepository.GetByIdAsync(request.Id);
        if (uu == null)
            return new GetUserUnitResponse { UserUnit = new UserUnitDto() };

        return new GetUserUnitResponse
        {
            UserUnit = new UserUnitDto
            {
                Id = uu.Id,
                UserId = uu.UserId,
                UnitId = uu.UnitId,
                UserName = uu.User?.Username,
                UnitName = uu.Unit?.Name,
                JoinedAt = uu.JoinedAt,
                IsActive = uu.IsActive
            }
        };
    }
}
