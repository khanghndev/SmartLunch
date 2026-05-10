using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.DTOs.Response.Finance;
using SmartLunch.Backend.Service.Application.Helpers;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Queries.Finance.GetContractPaymentReconciliation;

public class GetContractPaymentReconciliationQueryHandler
    : IRequestHandler<GetContractPaymentReconciliationQuery, GetContractPaymentReconciliationResponse>
{
    private readonly IFinanceAnalyticsRepository _financeRepository;
    private readonly ILogger<GetContractPaymentReconciliationQueryHandler> _logger;

    public GetContractPaymentReconciliationQueryHandler(
        IFinanceAnalyticsRepository financeRepository,
        ILogger<GetContractPaymentReconciliationQueryHandler> logger)
    {
        _financeRepository = financeRepository;
        _logger = logger;
    }

    public async Task<GetContractPaymentReconciliationResponse> Handle(
        GetContractPaymentReconciliationQuery request,
        CancellationToken cancellationToken)
    {
        var req = request.Request;
        FinanceContractSettlementQueryHelper.ValidateDatePair(req.OrderScheduledFrom, req.OrderScheduledTo, "OrderScheduled");
        if (req.OrderScheduledFrom.HasValue && req.OrderScheduledTo.HasValue)
            FinanceContractSettlementQueryHelper.AssertRangeDays(req.OrderScheduledFrom.Value, req.OrderScheduledTo.Value);

        var (scheduledFromUtc, scheduledToEx) =
            FinanceContractSettlementQueryHelper.ToUtcDayBounds(req.OrderScheduledFrom, req.OrderScheduledTo);

        var snapshot = await _financeRepository.GetContractFinanceSettlementSnapshotAsync(
            request.ContractId,
            scheduledFromUtc,
            scheduledToEx,
            supplierPaymentFromUtc: null,
            supplierPaymentToExclusiveUtc: null,
            req.IncludeCancelledOrders,
            loadSupplierPayments: false,
            cancellationToken);

        if (snapshot == null)
            throw new KeyNotFoundException($"Contract with ID {request.ContractId} was not found.");

        if (!scheduledFromUtc.HasValue && snapshot.Orders.Count > FinanceContractSettlementQueryHelper.MaxUnboundedOrders)
            throw new ArgumentException(
                $"Quá nhiều đơn ({snapshot.Orders.Count}). Hãy chỉ định orderScheduledFrom và orderScheduledTo (tối đa {FinanceContractSettlementQueryHelper.MaxUnboundedOrders} đơn khi không lọc ngày).");

        var orgNameFallback = snapshot.Contract.Organization?.Name;
        var lines = new List<PaymentReconciliationLineDto>();
        foreach (var order in snapshot.Orders)
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
                OrganizationName = order.Contract?.Organization?.Name ?? orgNameFallback,
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

        var activeNonCancelled = snapshot.Orders.Where(o => o.Status != OrderLifecycleStatus.Cancelled).ToList();
        var totalOrderAmt = activeNonCancelled.Sum(o => o.TotalAmount);
        var totalPaid = snapshot.Orders.Sum(o => o.Payments
            .Where(p => string.Equals(p.Status, PaymentRecordStatus.Paid, StringComparison.OrdinalIgnoreCase))
            .Sum(p => p.Amount));
        var totalPending = snapshot.Orders.Sum(o => o.Payments
            .Where(p => string.Equals(p.Status, PaymentRecordStatus.Pending, StringComparison.OrdinalIgnoreCase))
            .Sum(p => p.Amount));
        var outstanding = activeNonCancelled.Sum(o =>
        {
            var paid = o.Payments
                .Where(p => string.Equals(p.Status, PaymentRecordStatus.Paid, StringComparison.OrdinalIgnoreCase))
                .Sum(p => p.Amount);
            var remain = o.TotalAmount - paid;
            return remain > 0 ? remain : 0;
        });

        _logger.LogInformation(
            "Contract {Id} reconciliation: {Orders} orders, {Mm} mismatches.",
            snapshot.Contract.Id,
            snapshot.Orders.Count,
            mismatchCount);

        return new GetContractPaymentReconciliationResponse
        {
            Contract = MapHeader(snapshot.Contract),
            Reconciliation = new CustomerContractPaymentReconciliationDto
            {
                OrderScheduledFrom = req.OrderScheduledFrom,
                OrderScheduledTo = req.OrderScheduledTo,
                IncludeCancelledOrders = req.IncludeCancelledOrders,
                OnlyMismatches = req.OnlyMismatches,
                OrderCount = snapshot.Orders.Count,
                TotalOrderAmount = totalOrderAmt,
                TotalPaidFromPayments = totalPaid,
                TotalPendingFromPayments = totalPending,
                OutstandingReceivable = outstanding,
                MismatchCount = mismatchCount,
                Lines = lines
            }
        };
    }

    private static ContractFinanceHeaderDto MapHeader(Contract c) => new()
    {
        Id = c.Id,
        ContractType = c.ContractType,
        ContractNumber = c.ContractNumber,
        Status = c.Status,
        PartnerId = c.PartnerId,
        PartnerLegalName = c.Partner?.LegalName,
        OrganizationId = c.OrganizationId,
        OrganizationName = c.Organization?.Name,
        StartDate = c.StartDate,
        EndDate = c.EndDate,
        TotalValue = c.TotalValue,
        ContractFileUrl = c.ContractFileUrl
    };
}
