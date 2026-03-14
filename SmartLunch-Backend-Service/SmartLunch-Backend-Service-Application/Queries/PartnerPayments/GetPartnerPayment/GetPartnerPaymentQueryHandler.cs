using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.PartnerPayments;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.PartnerPayments.GetPartnerPayment;

public class GetPartnerPaymentQueryHandler : IRequestHandler<GetPartnerPaymentQuery, GetPartnerPaymentResponse>
{
    private readonly IPartnerPaymentRepository _partnerPaymentRepository;
    private readonly ILogger<GetPartnerPaymentQueryHandler> _logger;

    public GetPartnerPaymentQueryHandler(IPartnerPaymentRepository partnerPaymentRepository, ILogger<GetPartnerPaymentQueryHandler> logger)
    {
        _partnerPaymentRepository = partnerPaymentRepository;
        _logger = logger;
    }

    public async Task<GetPartnerPaymentResponse> Handle(GetPartnerPaymentQuery request, CancellationToken cancellationToken)
    {
        var partnerPayment = await _partnerPaymentRepository.GetByIdAsync(request.PartnerPaymentId);

        if (partnerPayment == null)
        {
            _logger.LogWarning("PartnerPayment not found with ID: {PartnerPaymentId}", request.PartnerPaymentId);
            return new GetPartnerPaymentResponse { PartnerPayment = new PartnerPaymentDto() };
        }

        return new GetPartnerPaymentResponse
        {
            PartnerPayment = new PartnerPaymentDto
            {
                Id = partnerPayment.Id,
                ContractId = partnerPayment.ContractId,
                PartnerId = partnerPayment.PartnerId,
                PaymentDate = partnerPayment.PaymentDate,
                Amount = partnerPayment.Amount,
                Method = partnerPayment.Method,
                Status = partnerPayment.Status,
                CreatedAt = partnerPayment.CreatedAt
            }
        };
    }
}
