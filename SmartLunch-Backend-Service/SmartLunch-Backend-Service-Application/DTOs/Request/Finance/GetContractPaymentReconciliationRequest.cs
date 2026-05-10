namespace SmartLunch.Backend.Service.Application.DTOs.Request.Finance;

/// <summary>Đối soát thanh toán đơn gắn hợp đồng (theo ngày giao dự kiến).</summary>
public class GetContractPaymentReconciliationRequest
{
    public DateOnly? OrderScheduledFrom { get; set; }
    public DateOnly? OrderScheduledTo { get; set; }

    public bool IncludeCancelledOrders { get; set; }

    public bool OnlyMismatches { get; set; }
}
