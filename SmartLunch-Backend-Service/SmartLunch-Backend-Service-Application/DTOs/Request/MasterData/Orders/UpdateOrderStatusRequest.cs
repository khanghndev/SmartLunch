namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Orders;

public class UpdateOrderStatusRequest
{
    /// <summary>Target status: pending | confirmed | delivered (workflow).</summary>
    public string Status { get; set; } = string.Empty;
}
