using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.Finance;
using SmartLunch.Backend.Service.Application.DTOs.Response.Finance;

namespace SmartLunch.Backend.Service.Application.Queries.Finance.GetUnitReceivables;

public class GetUnitReceivablesQuery : IRequest<GetUnitReceivablesResponse>
{
    public GetUnitReceivablesRequest Request { get; }

    public GetUnitReceivablesQuery(GetUnitReceivablesRequest request)
    {
        Request = request;
    }
}
