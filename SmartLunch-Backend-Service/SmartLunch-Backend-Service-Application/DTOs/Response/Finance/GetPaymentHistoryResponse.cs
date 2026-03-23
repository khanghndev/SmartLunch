namespace SmartLunch.Backend.Service.Application.DTOs.Response.Finance;

public class GetPaymentHistoryResponse
{
    public DateOnly From { get; set; }
    public DateOnly To { get; set; }
    public string Scope { get; set; } = string.Empty;
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<PaymentHistoryEntryDto> Entries { get; set; } = new();
}
