using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.PartnerPayments;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.PartnerPayments.GetPartnerPayments;

public class GetPartnerPaymentsQueryHandler : IRequestHandler<GetPartnerPaymentsQuery, GetPartnerPaymentsResponse>
{
    private readonly IPartnerPaymentRepository _partnerPaymentRepository;
    private readonly ILogger<GetPartnerPaymentsQueryHandler> _logger;

    public GetPartnerPaymentsQueryHandler(IPartnerPaymentRepository partnerPaymentRepository, ILogger<GetPartnerPaymentsQueryHandler> logger)
    {
        _partnerPaymentRepository = partnerPaymentRepository;
        _logger = logger;
    }

    public async Task<GetPartnerPaymentsResponse> Handle(GetPartnerPaymentsQuery request, CancellationToken cancellationToken)
    {
        var (partnerPayments, totalCount) = await _partnerPaymentRepository.GetPartnerPaymentsAsync(
            request.Page,
            request.PageSize,
            request.SearchTerm);

        var partnerPaymentDtos = partnerPayments.Select(partnerPayment => new PartnerPaymentDto
        {
                Id = partnerPayment.Id,
                ContractId = partnerPayment.ContractId,
                PartnerId = partnerPayment.PartnerId,
                PaymentDate = partnerPayment.PaymentDate,
                Amount = partnerPayment.Amount,
                Method = partnerPayment.Method,
                Status = partnerPayment.Status,
                CreatedAt = partnerPayment.CreatedAt
        }).ToList();

        _logger.LogInformation("Retrieved {Count} partnerpayments (Page {Page}, PageSize {PageSize})",
            partnerPaymentDtos.Count, request.Page, request.PageSize);

        return new GetPartnerPaymentsResponse
        {
            Data = partnerPaymentDtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
