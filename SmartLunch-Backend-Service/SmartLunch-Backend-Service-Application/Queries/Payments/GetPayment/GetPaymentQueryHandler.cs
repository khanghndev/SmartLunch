using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Payments;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.Payments.GetPayment;

public class GetPaymentQueryHandler : IRequestHandler<GetPaymentQuery, GetPaymentResponse>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ILogger<GetPaymentQueryHandler> _logger;

    public GetPaymentQueryHandler(IPaymentRepository paymentRepository, ILogger<GetPaymentQueryHandler> logger)
    {
        _paymentRepository = paymentRepository;
        _logger = logger;
    }

    public async Task<GetPaymentResponse> Handle(GetPaymentQuery request, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(request.PaymentId);

        if (payment == null)
        {
            _logger.LogWarning("Payment not found with ID: {PaymentId}", request.PaymentId);
            return new GetPaymentResponse { Payment = new PaymentDto() };
        }

        return new GetPaymentResponse
        {
            Payment = new PaymentDto
            {
                Id = payment.Id,
                OrderId = payment.OrderId,
                PayerId = payment.PayerId,
                PaymentDate = payment.PaymentDate,
                Amount = payment.Amount,
                Method = payment.Method,
                Status = payment.Status,
                CreatedAt = payment.CreatedAt
            }
        };
    }
}
