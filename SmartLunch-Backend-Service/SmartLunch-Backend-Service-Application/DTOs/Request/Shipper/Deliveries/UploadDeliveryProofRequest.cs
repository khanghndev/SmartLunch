namespace SmartLunch.Backend.Service.Application.DTOs.Request.Shipper.Deliveries;

public class UploadDeliveryProofRequest
{
    public string? Notes { get; set; }
    /// <summary>Tên người nhận xác nhận (bắt buộc khi upload proof).</summary>
    public string RecipientConfirmedName { get; set; } = string.Empty;
}
