namespace SmartLunch.Backend.Service.Application.DTOs.Request.Finance;

public class GetPaymentHistoryRequest
{
    public DateOnly From { get; set; }
    public DateOnly To { get; set; }
    public PaymentHistoryScope Scope { get; set; } = PaymentHistoryScope.All;
    public Guid? UnitId { get; set; }
    public Guid? PartnerId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
