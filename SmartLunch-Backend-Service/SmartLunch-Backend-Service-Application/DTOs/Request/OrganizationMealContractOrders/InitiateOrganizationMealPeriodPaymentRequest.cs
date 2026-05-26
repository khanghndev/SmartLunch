namespace SmartLunch.Backend.Service.Application.DTOs.Request.OrganizationMealContractOrders;

public sealed class InitiateOrganizationMealPeriodPaymentRequest
{
    public int OrderId { get; set; }
    public string? ReturnUrl { get; set; }
    public string? CancelUrl { get; set; }
}
