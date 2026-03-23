namespace SmartLunch.Backend.Service.Application.DTOs.Response.Finance;

public class GetCashflowSummaryResponse
{
    public DateOnly From { get; set; }
    public DateOnly To { get; set; }
    public string Granularity { get; set; } = string.Empty;
    public decimal TotalCollectedPayments { get; set; }
    public decimal TotalTransactionIncome { get; set; }
    public decimal TotalTransactionExpense { get; set; }
    public decimal TotalNet { get; set; }
    public List<CashflowBucketDto> Buckets { get; set; } = new();
}
