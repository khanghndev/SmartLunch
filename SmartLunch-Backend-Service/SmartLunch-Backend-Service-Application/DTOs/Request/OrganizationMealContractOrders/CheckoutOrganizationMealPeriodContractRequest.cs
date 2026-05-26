namespace SmartLunch.Backend.Service.Application.DTOs.Request.OrganizationMealContractOrders;

public sealed class CheckoutOrganizationMealPeriodContractRequest
{
    public string DraftId { get; set; } = string.Empty;

    /// <summary>20, 25, 30, 35 hoặc 50.</summary>
    public int DepositPercent { get; set; } = 30;
}
