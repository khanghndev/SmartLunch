using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.Finance;
using SmartLunch.Backend.Service.Application.DTOs.Response.Finance;

namespace SmartLunch.Backend.Service.Application.Queries.Finance.GetOrganizationReceivables;

public class GetOrganizationReceivablesQuery : IRequest<GetOrganizationReceivablesResponse>
{
    public GetOrganizationReceivablesRequest Request { get; }

    public GetOrganizationReceivablesQuery(GetOrganizationReceivablesRequest request)
    {
        Request = request;
    }
}
