using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.Finance;
using SmartLunch.Backend.Service.Application.DTOs.Response.Finance;

namespace SmartLunch.Backend.Service.Application.Queries.Finance.GetCashflowSummary;

public class GetCashflowSummaryQuery : IRequest<GetCashflowSummaryResponse>
{
    public GetCashflowSummaryRequest Request { get; }

    public GetCashflowSummaryQuery(GetCashflowSummaryRequest request)
    {
        Request = request;
    }
}
