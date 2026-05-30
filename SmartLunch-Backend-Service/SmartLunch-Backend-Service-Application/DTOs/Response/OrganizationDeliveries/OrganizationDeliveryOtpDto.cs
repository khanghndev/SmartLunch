namespace SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationDeliveries;

public class OrganizationDeliveryOtpResponse
{
    public int OrderId { get; set; }
    public int DeliveryId { get; set; }
    public string? DeliveryOtp { get; set; }
    public DateTime? DeliveryOtpExpiresAt { get; set; }
    public string DeliveryStatus { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
