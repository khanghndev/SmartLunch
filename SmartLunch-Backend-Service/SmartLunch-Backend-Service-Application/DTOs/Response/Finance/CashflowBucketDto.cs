namespace SmartLunch.Backend.Service.Application.DTOs.Response.Finance;

public class CashflowBucketDto
{
    public string PeriodKey { get; set; } = string.Empty;
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public decimal CollectedPayments { get; set; }
    public decimal TransactionIncome { get; set; }
    public decimal TransactionExpense { get; set; }
    public decimal NetFlow { get; set; }
}
