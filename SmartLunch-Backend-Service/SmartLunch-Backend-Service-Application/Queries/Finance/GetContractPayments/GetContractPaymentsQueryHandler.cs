using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.DTOs.Response.Finance;
using SmartLunch.Backend.Service.Application.Helpers;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Queries.Finance.GetContractPayments;

public class GetContractPaymentsQueryHandler : IRequestHandler<GetContractPaymentsQuery, GetContractPaymentsResponse>
{
    private readonly IFinanceAnalyticsRepository _financeRepository;
    private readonly ILogger<GetContractPaymentsQueryHandler> _logger;

    public GetContractPaymentsQueryHandler(
        IFinanceAnalyticsRepository financeRepository,
        ILogger<GetContractPaymentsQueryHandler> logger)
    {
        _financeRepository = financeRepository;
        _logger = logger;
    }

    public async Task<GetContractPaymentsResponse> Handle(
        GetContractPaymentsQuery request,
        CancellationToken cancellationToken)
    {
        var req = request.Request;
        FinanceContractSettlementQueryHelper.ValidateDatePair(req.OrderScheduledFrom, req.OrderScheduledTo, "OrderScheduled");
        FinanceContractSettlementQueryHelper.ValidateDatePair(req.SupplierPaymentFrom, req.SupplierPaymentTo, "SupplierPayment");

        if (req.OrderScheduledFrom.HasValue && req.OrderScheduledTo.HasValue)
            FinanceContractSettlementQueryHelper.AssertRangeDays(req.OrderScheduledFrom.Value, req.OrderScheduledTo.Value);
        if (req.SupplierPaymentFrom.HasValue && req.SupplierPaymentTo.HasValue)
            FinanceContractSettlementQueryHelper.AssertRangeDays(req.SupplierPaymentFrom.Value, req.SupplierPaymentTo.Value);

        var (scheduledFromUtc, scheduledToEx) =
            FinanceContractSettlementQueryHelper.ToUtcDayBounds(req.OrderScheduledFrom, req.OrderScheduledTo);
        var (supplierFromUtc, supplierToEx) =
            FinanceContractSettlementQueryHelper.ToUtcDayBounds(req.SupplierPaymentFrom, req.SupplierPaymentTo);

        var snapshot = await _financeRepository.GetContractFinanceSettlementSnapshotAsync(
            request.ContractId,
            scheduledFromUtc,
            scheduledToEx,
            supplierFromUtc,
            supplierToEx,
            req.IncludeCancelledOrders,
            loadSupplierPayments: true,
            cancellationToken);

        if (snapshot == null)
            throw new KeyNotFoundException($"Contract with ID {request.ContractId} was not found.");

        if (!scheduledFromUtc.HasValue && snapshot.Orders.Count > FinanceContractSettlementQueryHelper.MaxUnboundedOrders)
            throw new ArgumentException(
                $"Quá nhiều đơn ({snapshot.Orders.Count}). Hãy chỉ định orderScheduledFrom và orderScheduledTo (tối đa {FinanceContractSettlementQueryHelper.MaxUnboundedOrders} đơn khi không lọc ngày).");

        var orgNameFallback = snapshot.Contract.Organization?.Name;
        var orderDtos = snapshot.Orders.Select(o =>
        {
            var paid = SumPaid(o);
            var pend = SumPending(o);
            return new OrderContractPaymentLineDto
            {
                OrderId = o.Id,
                ScheduledDate = o.ScheduledDate,
                OrderStatus = o.Status,
                OrganizationName = o.Contract?.Organization?.Name ?? orgNameFallback,
                InvoiceCode = o.InvoiceCode,
                OrderTotal = o.TotalAmount,
                RecordedPaymentStatus = o.PaymentStatus,
                PaidAmount = paid,
                PendingPaymentAmount = pend
            };
        }).ToList();

        var activeNonCancelled = snapshot.Orders.Where(o => o.Status != OrderLifecycleStatus.Cancelled).ToList();
        var totalOrderAmt = activeNonCancelled.Sum(o => o.TotalAmount);
        var totalPaid = snapshot.Orders.Sum(SumPaid);
        var totalPending = snapshot.Orders.Sum(SumPending);
        var outstanding = activeNonCancelled.Sum(o =>
        {
            var p = SumPaid(o);
            var r = o.TotalAmount - p;
            return r > 0 ? r : 0;
        });

        var completedSup = snapshot.SupplierPayments
            .Where(p => string.Equals(p.Status, PartnerPaymentStatus.Completed, StringComparison.OrdinalIgnoreCase))
            .Sum(p => p.Amount);
        var pendingSup = snapshot.SupplierPayments
            .Where(p => string.Equals(p.Status, PartnerPaymentStatus.Pending, StringComparison.OrdinalIgnoreCase))
            .Sum(p => p.Amount);
        var failedSup = snapshot.SupplierPayments
            .Where(p => string.Equals(p.Status, PartnerPaymentStatus.Failed, StringComparison.OrdinalIgnoreCase))
            .Sum(p => p.Amount);

        decimal? remainingDeclared = snapshot.Contract.TotalValue.HasValue
            ? snapshot.Contract.TotalValue.Value - completedSup
            : null;

        var supplierLines = snapshot.SupplierPayments.Select(p => new SupplierContractPaymentLineDto
        {
            Id = p.Id,
            Code = p.Code,
            PaymentDate = p.PaymentDate,
            Amount = p.Amount,
            Method = p.Method,
            Status = p.Status
        }).ToList();

        _logger.LogInformation(
            "Contract {Id} payments: {Orders} orders, {Pp} supplier rows.",
            snapshot.Contract.Id,
            snapshot.Orders.Count,
            supplierLines.Count);

        return new GetContractPaymentsResponse
        {
            Contract = MapHeader(snapshot.Contract),
            Customer = new CustomerContractPaymentsDto
            {
                OrderScheduledFrom = req.OrderScheduledFrom,
                OrderScheduledTo = req.OrderScheduledTo,
                IncludeCancelledOrders = req.IncludeCancelledOrders,
                OrderCount = snapshot.Orders.Count,
                TotalOrderAmount = totalOrderAmt,
                TotalPaidFromPayments = totalPaid,
                TotalPendingFromPayments = totalPending,
                OutstandingReceivable = outstanding,
                Orders = orderDtos
            },
            Supplier = new SupplierContractPaymentsDto
            {
                PaymentFrom = req.SupplierPaymentFrom,
                PaymentTo = req.SupplierPaymentTo,
                PaymentLineCount = supplierLines.Count,
                TotalCompletedAmount = completedSup,
                TotalPendingAmount = pendingSup,
                TotalFailedAmount = failedSup,
                RemainingVsDeclaredContractValue = remainingDeclared,
                PaymentLines = supplierLines
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

    private static decimal SumPaid(Order o) => o.Payments
        .Where(p => string.Equals(p.Status, PaymentRecordStatus.Paid, StringComparison.OrdinalIgnoreCase))
        .Sum(p => p.Amount);

    private static decimal SumPending(Order o) => o.Payments
        .Where(p => string.Equals(p.Status, PaymentRecordStatus.Pending, StringComparison.OrdinalIgnoreCase))
        .Sum(p => p.Amount);
}
