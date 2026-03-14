using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Units;

namespace SmartLunch.Backend.Service.Application.Queries.Units.GetUnit;

/// <summary>
/// Query to get a unit by ID
/// </summary>
public class GetUnitQuery : IRequest<GetUnitResponse>
{
    public Guid UnitId { get; set; }

    public GetUnitQuery(Guid unitId)
    {
        UnitId = unitId;
    }
}
