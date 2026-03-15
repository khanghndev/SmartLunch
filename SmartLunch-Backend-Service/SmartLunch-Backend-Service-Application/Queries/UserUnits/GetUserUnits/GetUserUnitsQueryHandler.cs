using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserUnits;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.UserUnits.GetUserUnits;

public class GetUserUnitsQueryHandler : IRequestHandler<GetUserUnitsQuery, GetUserUnitsResponse>
{
    private readonly IUserUnitRepository _userUnitRepository;

    public GetUserUnitsQueryHandler(IUserUnitRepository userUnitRepository)
    {
        _userUnitRepository = userUnitRepository;
    }

    public async Task<GetUserUnitsResponse> Handle(GetUserUnitsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _userUnitRepository.GetPagedAsync(
            request.Page, request.PageSize, request.UserId, request.UnitId, request.IsActive);

        var dtos = items.Select(uu => new UserUnitDto
        {
            Id = uu.Id,
            UserId = uu.UserId,
            UnitId = uu.UnitId,
            UserName = uu.User?.Username,
            UnitName = uu.Unit?.Name,
            JoinedAt = uu.JoinedAt,
            IsActive = uu.IsActive
        }).ToList();

        return new GetUserUnitsResponse
        {
            Data = dtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
