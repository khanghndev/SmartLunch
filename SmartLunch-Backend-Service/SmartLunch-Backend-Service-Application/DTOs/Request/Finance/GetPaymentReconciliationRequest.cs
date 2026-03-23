namespace SmartLunch.Backend.Service.Application.DTOs.Request.Finance;

public class GetPaymentReconciliationRequest
{
    public DateOnly From { get; set; }
    public DateOnly To { get; set; }

    /// <summary>Include orders in cancelled state.</summary>
    public bool IncludeCancelledOrders { get; set; }

    /// <summary>If true, return only rows where totals do not match recorded payment status.</summary>
    public bool OnlyMismatches { get; set; } = true;
}
