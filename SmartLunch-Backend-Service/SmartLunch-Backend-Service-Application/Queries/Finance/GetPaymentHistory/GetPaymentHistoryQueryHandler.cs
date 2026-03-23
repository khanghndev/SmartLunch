using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Request.Finance;
using SmartLunch.Backend.Service.Application.DTOs.Response.Finance;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.Finance.GetPaymentHistory;

public class GetPaymentHistoryQueryHandler : IRequestHandler<GetPaymentHistoryQuery, GetPaymentHistoryResponse>
{
    private const int MaxRangeDays = 400;

    private readonly IDebtAnalyticsRepository _debtRepository;
    private readonly ILogger<GetPaymentHistoryQueryHandler> _logger;

    public GetPaymentHistoryQueryHandler(
        IDebtAnalyticsRepository debtRepository,
        ILogger<GetPaymentHistoryQueryHandler> logger)
    {
        _debtRepository = debtRepository;
        _logger = logger;
    }

    public async Task<GetPaymentHistoryResponse> Handle(GetPaymentHistoryQuery request, CancellationToken cancellationToken)
    {
        var req = request.Request;
        if (req.From == default || req.To == default)
            throw new ArgumentException("From and To are required.");
        if (req.To < req.From)
            throw new ArgumentException("'To' must be on or after 'From'.");
        if (req.To.DayNumber - req.From.DayNumber + 1 > MaxRangeDays)
            throw new ArgumentException($"Date range cannot exceed {MaxRangeDays} days.");
        if (req.Page < 1 || req.PageSize < 1 || req.PageSize > 200)
            throw new ArgumentException("Invalid pagination.");

        var start = req.From.ToDateTime(TimeOnly.MinValue);
        var endEx = req.To.ToDateTime(TimeOnly.MinValue).AddDays(1);

        var merged = new List<PaymentHistoryEntryDto>();

        if (req.Scope is PaymentHistoryScope.Customer or PaymentHistoryScope.All)
        {
            var payments = await _debtRepository.GetCustomerPaymentsInRangeAsync(
                start, endEx, req.UnitId, cancellationToken);
            merged.AddRange(payments.Select(p => new PaymentHistoryEntryDto
            {
                Source = "customer",
                EntryId = p.Id,
                PaymentDate = p.PaymentDate,
                Amount = p.Amount,
                Method = p.Method,
                Status = p.Status,
                OrderId = p.OrderId,
                UnitId = p.Order.UnitId,
                UnitName = p.Order.Unit?.Name,
                PartnerId = null,
                PartnerLegalName = null,
                ContractId = null,
                ContractNumber = null
            }));
        }

        if (req.Scope is PaymentHistoryScope.Supplier or PaymentHistoryScope.All)
        {
            var pp = await _debtRepository.GetSupplierPaymentsInRangeAsync(
                start, endEx, req.PartnerId, cancellationToken);
            merged.AddRange(pp.Select(x => new PaymentHistoryEntryDto
            {
                Source = "supplier",
                EntryId = x.Id,
                PaymentDate = x.PaymentDate,
                Amount = x.Amount,
                Method = x.Method,
                Status = x.Status,
                OrderId = null,
                UnitId = null,
                UnitName = null,
                PartnerId = x.PartnerId,
                PartnerLegalName = x.Partner?.LegalName,
                ContractId = x.ContractId,
                ContractNumber = x.Contract?.ContractNumber
            }));
        }

        var ordered = merged
            .OrderByDescending(e => e.PaymentDate)
            .ThenByDescending(e => e.EntryId)
            .ToList();

        var total = ordered.Count;
        var slice = ordered
            .Skip((req.Page - 1) * req.PageSize)
            .Take(req.PageSize)
            .ToList();

        _logger.LogInformation(
            "Payment history {Scope}: {Total} entries, page {Page}",
            req.Scope, total, req.Page);

        return new GetPaymentHistoryResponse
        {
            From = req.From,
            To = req.To,
            Scope = req.Scope.ToString(),
            TotalCount = total,
            Page = req.Page,
            PageSize = req.PageSize,
            Entries = slice
        };
    }
}
