using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.Finance;
using SmartLunch.Backend.Service.Application.DTOs.Response.Finance;

namespace SmartLunch.Backend.Service.Application.Queries.Finance.GetPartnerPayables;

public class GetPartnerPayablesQuery : IRequest<GetPartnerPayablesResponse>
{
    public GetPartnerPayablesRequest Request { get; }

    public GetPartnerPayablesQuery(GetPartnerPayablesRequest request)
    {
        Request = request;
    }
}
