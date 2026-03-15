using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserUnits;

namespace SmartLunch.Backend.Service.Application.Queries.UserUnits.GetUserUnit;

public class GetUserUnitQuery : IRequest<GetUserUnitResponse>
{
    public Guid Id { get; set; }

    public GetUserUnitQuery(Guid id)
    {
        Id = id;
    }
}
