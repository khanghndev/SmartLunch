using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.Finance;
using SmartLunch.Backend.Service.Application.DTOs.Response.Finance;

namespace SmartLunch.Backend.Service.Application.Queries.Finance.GetPaymentReconciliation;

public class GetPaymentReconciliationQuery : IRequest<GetPaymentReconciliationResponse>
{
    public GetPaymentReconciliationRequest Request { get; }

    public GetPaymentReconciliationQuery(GetPaymentReconciliationRequest request)
    {
        Request = request;
    }
}
