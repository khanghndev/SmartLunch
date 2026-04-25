using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Payments;

namespace SmartLunch.Backend.Service.Application.Queries.Payments.GetPayment;

public class GetPaymentQuery : IRequest<GetPaymentResponse>
{
    public int PaymentId { get; set; }

    public GetPaymentQuery(int paymentId)
    {
        PaymentId = paymentId;
    }
}
