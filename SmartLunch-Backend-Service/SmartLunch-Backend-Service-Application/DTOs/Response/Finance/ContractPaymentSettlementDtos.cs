namespace SmartLunch.Backend.Service.Application.DTOs.Response.Finance;

/// <summary>Tiêu đề hợp đồng dùng cho các báo cáo finance theo contract.</summary>
public class ContractFinanceHeaderDto
{
    public int Id { get; set; }
    public string ContractType { get; set; } = string.Empty;
    public string? ContractNumber { get; set; }
    public string Status { get; set; } = string.Empty;
    public int PartnerId { get; set; }
    public string? PartnerLegalName { get; set; }
    public int? OrganizationId { get; set; }
    public string? OrganizationName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? TotalValue { get; set; }
    public string? ContractFileUrl { get; set; }
}

/// <summary>Thanh toán theo hợp đồng: tóm tắt đơn + chi NCC (không có logic đối soát lệch).</summary>
public class GetContractPaymentsResponse
{
    public ContractFinanceHeaderDto Contract { get; set; } = new();
    public CustomerContractPaymentsDto Customer { get; set; } = new();
    public SupplierContractPaymentsDto Supplier { get; set; } = new();
}

public class CustomerContractPaymentsDto
{
    public DateOnly? OrderScheduledFrom { get; set; }
    public DateOnly? OrderScheduledTo { get; set; }
    public bool IncludeCancelledOrders { get; set; }
    public int OrderCount { get; set; }
    public decimal TotalOrderAmount { get; set; }
    public decimal TotalPaidFromPayments { get; set; }
    public decimal TotalPendingFromPayments { get; set; }
    public decimal OutstandingReceivable { get; set; }
    public List<OrderContractPaymentLineDto> Orders { get; set; } = new();
}

/// <summary>Dòng theo đơn: số tiền thu/pending, không tính derived trạng thái đối soát.</summary>
public class OrderContractPaymentLineDto
{
    public int OrderId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string OrderStatus { get; set; } = string.Empty;
    public string? OrganizationName { get; set; }
    public string? InvoiceCode { get; set; }
    public decimal OrderTotal { get; set; }
    public string RecordedPaymentStatus { get; set; } = string.Empty;
    public decimal PaidAmount { get; set; }
    public decimal PendingPaymentAmount { get; set; }
}

public class SupplierContractPaymentsDto
{
    public DateOnly? PaymentFrom { get; set; }
    public DateOnly? PaymentTo { get; set; }
    public int PaymentLineCount { get; set; }
    public decimal TotalCompletedAmount { get; set; }
    public decimal TotalPendingAmount { get; set; }
    public decimal TotalFailedAmount { get; set; }
    public decimal? RemainingVsDeclaredContractValue { get; set; }
    public List<SupplierContractPaymentLineDto> PaymentLines { get; set; } = new();
}

public class SupplierContractPaymentLineDto
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

/// <summary>Đối soát đơn theo hợp đồng: so khớp Payment paid vs Order.Total &amp; Order.PaymentStatus.</summary>
public class GetContractPaymentReconciliationResponse
{
    public ContractFinanceHeaderDto Contract { get; set; } = new();
    public CustomerContractPaymentReconciliationDto Reconciliation { get; set; } = new();
}

public class CustomerContractPaymentReconciliationDto
{
    public DateOnly? OrderScheduledFrom { get; set; }
    public DateOnly? OrderScheduledTo { get; set; }
    public bool IncludeCancelledOrders { get; set; }
    public bool OnlyMismatches { get; set; }
    public int OrderCount { get; set; }
    public decimal TotalOrderAmount { get; set; }
    public decimal TotalPaidFromPayments { get; set; }
    public decimal TotalPendingFromPayments { get; set; }
    public decimal OutstandingReceivable { get; set; }
    public int MismatchCount { get; set; }
    public List<PaymentReconciliationLineDto> Lines { get; set; } = new();
}
