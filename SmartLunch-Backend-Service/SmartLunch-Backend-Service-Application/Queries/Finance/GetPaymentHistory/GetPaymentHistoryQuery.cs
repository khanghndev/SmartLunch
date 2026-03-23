using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.Finance;
using SmartLunch.Backend.Service.Application.DTOs.Response.Finance;

namespace SmartLunch.Backend.Service.Application.Queries.Finance.GetPaymentHistory;

public class GetPaymentHistoryQuery : IRequest<GetPaymentHistoryResponse>
{
    public GetPaymentHistoryRequest Request { get; }

    public GetPaymentHistoryQuery(GetPaymentHistoryRequest request)
    {
        Request = request;
    }
}
