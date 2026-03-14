using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Payments;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.Payments.GetPayments;

public class GetPaymentsQueryHandler : IRequestHandler<GetPaymentsQuery, GetPaymentsResponse>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ILogger<GetPaymentsQueryHandler> _logger;

    public GetPaymentsQueryHandler(IPaymentRepository paymentRepository, ILogger<GetPaymentsQueryHandler> logger)
    {
        _paymentRepository = paymentRepository;
        _logger = logger;
    }

    public async Task<GetPaymentsResponse> Handle(GetPaymentsQuery request, CancellationToken cancellationToken)
    {
        var (payments, totalCount) = await _paymentRepository.GetPaymentsAsync(
            request.Page,
            request.PageSize,
            request.SearchTerm);

        var paymentDtos = payments.Select(payment => new PaymentDto
        {
                Id = payment.Id,
                OrderId = payment.OrderId,
                PayerId = payment.PayerId,
                PaymentDate = payment.PaymentDate,
                Amount = payment.Amount,
                Method = payment.Method,
                Status = payment.Status,
                CreatedAt = payment.CreatedAt
        }).ToList();

        _logger.LogInformation("Retrieved {Count} payments (Page {Page}, PageSize {PageSize})",
            paymentDtos.Count, request.Page, request.PageSize);

        return new GetPaymentsResponse
        {
            Data = paymentDtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
