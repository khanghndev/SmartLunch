namespace SmartLunch.Backend.Service.Application.DTOs.Request.Shipper.Deliveries;

public class UploadDeliveryProofRequest
{
    public string? Notes { get; set; }
    /// <summary>Tên người nhận xác nhận (bắt buộc khi upload proof).</summary>
    public string RecipientConfirmedName { get; set; } = string.Empty;
    /// <summary>Mã OTP / mã xác nhận do người nhận cung cấp.</summary>
    public string RecipientConfirmationCode { get; set; } = string.Empty;
}

