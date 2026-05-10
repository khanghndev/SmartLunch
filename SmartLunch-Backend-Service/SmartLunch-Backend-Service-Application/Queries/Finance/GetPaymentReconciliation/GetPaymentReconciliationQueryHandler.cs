using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.DTOs.Response.Finance;
using SmartLunch.Backend.Service.Application.Helpers;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.Finance.GetPaymentReconciliation;

public class GetPaymentReconciliationQueryHandler : IRequestHandler<GetPaymentReconciliationQuery, GetPaymentReconciliationResponse>
{
    private const int MaxRangeDays = 800;

    private readonly IFinanceAnalyticsRepository _financeRepository;
    private readonly ILogger<GetPaymentReconciliationQueryHandler> _logger;

    public GetPaymentReconciliationQueryHandler(
        IFinanceAnalyticsRepository financeRepository,
        ILogger<GetPaymentReconciliationQueryHandler> logger)
    {
        _financeRepository = financeRepository;
        _logger = logger;
    }

    public async Task<GetPaymentReconciliationResponse> Handle(GetPaymentReconciliationQuery request, CancellationToken cancellationToken)
    {
        var req = request.Request;
        if (req.From == default || req.To == default)
            throw new ArgumentException("From and To are required.");
        if (req.To < req.From)
            throw new ArgumentException("'To' must be on or after 'From'.");

        var spanDays = req.To.DayNumber - req.From.DayNumber + 1;
        if (spanDays > MaxRangeDays)
            throw new ArgumentException($"Date range cannot exceed {MaxRangeDays} days.");

        var rangeStart = req.From.ToDateTime(TimeOnly.MinValue);
        var rangeEndExclusive = req.To.ToDateTime(TimeOnly.MinValue).AddDays(1);

        var orders = await _financeRepository.GetOrdersWithPaymentsForReconciliationAsync(
            rangeStart,
            rangeEndExclusive,
            req.IncludeCancelledOrders,
            cancellationToken);

        var lines = new List<PaymentReconciliationLineDto>();
        foreach (var order in orders)
        {
            var paidAmount = order.Payments
                .Where(p => string.Equals(p.Status, PaymentRecordStatus.Paid, StringComparison.OrdinalIgnoreCase))
                .Sum(p => p.Amount);
            var pendingAmount = order.Payments
                .Where(p => string.Equals(p.Status, PaymentRecordStatus.Pending, StringComparison.OrdinalIgnoreCase))
                .Sum(p => p.Amount);

            var derived = PaymentReconciliationDerivation.DerivePaymentStatus(order.TotalAmount, paidAmount);
            var recorded = order.PaymentStatus.Trim().ToLowerInvariant();
            var isAligned = string.Equals(recorded, derived, StringComparison.Ordinal);
            var difference = paidAmount - order.TotalAmount;

            var issue = PaymentReconciliationDerivation.BuildIssue(isAligned, recorded, derived, pendingAmount, difference);

            lines.Add(new PaymentReconciliationLineDto
            {
                OrderId = order.Id,
                ScheduledDate = order.ScheduledDate,
                OrganizationName = order.Contract?.Organization?.Name,
                OrderTotal = order.TotalAmount,
                RecordedPaymentStatus = order.PaymentStatus,
                PaidAmount = paidAmount,
                PendingPaymentAmount = pendingAmount,
                Difference = difference,
                DerivedPaymentStatus = derived,
                IsAligned = isAligned,
                Issue = issue
            });
        }

        var mismatchCount = lines.Count(l => !l.IsAligned);
        if (req.OnlyMismatches)
            lines = lines.Where(l => !l.IsAligned).ToList();

        _logger.LogInformation(
            "Payment reconciliation {From}–{To}: {OrderCount} orders, {MismatchCount} mismatches (before filter)",
            req.From, req.To, orders.Count, mismatchCount);

        return new GetPaymentReconciliationResponse
        {
            From = req.From,
            To = req.To,
            IncludeCancelledOrders = req.IncludeCancelledOrders,
            OnlyMismatches = req.OnlyMismatches,
            OrderCount = orders.Count,
            MismatchCount = mismatchCount,
            Lines = lines
        };
    }
}
