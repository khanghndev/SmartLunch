namespace SmartLunch.Backend.Service.Application.DTOs.Response.Finance;

public class PaymentReconciliationLineDto
{
    public int OrderId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string? UnitName { get; set; }
    public decimal OrderTotal { get; set; }
    public string RecordedPaymentStatus { get; set; } = string.Empty;
    public decimal PaidAmount { get; set; }
    public decimal PendingPaymentAmount { get; set; }
    public decimal Difference { get; set; }
    public string DerivedPaymentStatus { get; set; } = string.Empty;
    public bool IsAligned { get; set; }
    public string? Issue { get; set; }
}
