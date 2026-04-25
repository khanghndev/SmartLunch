using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserUnits;

namespace SmartLunch.Backend.Service.Application.Queries.UserUnits.GetUserUnits;

public class GetUserUnitsQuery : IRequest<GetUserUnitsResponse>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int? UserId { get; set; }
    public int? UnitId { get; set; }
    public bool? IsActive { get; set; }

    public GetUserUnitsQuery(int page, int pageSize, int? userId, int? unitId, bool? isActive)
    {
        Page = page;
        PageSize = pageSize;
        UserId = userId;
        UnitId = unitId;
        IsActive = isActive;
    }
}
