namespace SmartLunch.Backend.Service.Application.DTOs.Request.Finance;

public class GetCashflowSummaryRequest
{
    public DateOnly From { get; set; }
    public DateOnly To { get; set; }
    public CashflowGranularity Granularity { get; set; } = CashflowGranularity.Day;
}
