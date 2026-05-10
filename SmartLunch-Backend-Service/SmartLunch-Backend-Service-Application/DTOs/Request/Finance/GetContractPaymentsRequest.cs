namespace SmartLunch.Backend.Service.Application.DTOs.Request.Finance;

/// <summary>Thanh toán theo hợp đồng: đơn (thu khách) + PartnerPayment (chi NCC).</summary>
public class GetContractPaymentsRequest
{
    public DateOnly? OrderScheduledFrom { get; set; }
    public DateOnly? OrderScheduledTo { get; set; }

    public DateOnly? SupplierPaymentFrom { get; set; }
    public DateOnly? SupplierPaymentTo { get; set; }

    public bool IncludeCancelledOrders { get; set; }
}
