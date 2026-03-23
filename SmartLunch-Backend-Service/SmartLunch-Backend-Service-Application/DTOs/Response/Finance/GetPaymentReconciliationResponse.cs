namespace SmartLunch.Backend.Service.Application.DTOs.Response.Finance;

public class GetPaymentReconciliationResponse
{
    public DateOnly From { get; set; }
    public DateOnly To { get; set; }
    public bool IncludeCancelledOrders { get; set; }
    public bool OnlyMismatches { get; set; }
    public int OrderCount { get; set; }
    public int MismatchCount { get; set; }
    public List<PaymentReconciliationLineDto> Lines { get; set; } = new();
}
