namespace SmartLunch.Backend.Service.Application.DTOs.Request.OrganizationMealOrders;

public sealed class InitiateOrganizationMealPaymentRequest
{
    public int OrderId { get; set; }
    public string? ReturnUrl { get; set; }
    public string? CancelUrl { get; set; }
}
