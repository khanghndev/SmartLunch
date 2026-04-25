using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.PartnerPayments;

namespace SmartLunch.Backend.Service.Application.Queries.PartnerPayments.GetPartnerPayment;

public class GetPartnerPaymentQuery : IRequest<GetPartnerPaymentResponse>
{
    public int PartnerPaymentId { get; set; }

    public GetPartnerPaymentQuery(int partnerPaymentId)
    {
        PartnerPaymentId = partnerPaymentId;
    }
}
