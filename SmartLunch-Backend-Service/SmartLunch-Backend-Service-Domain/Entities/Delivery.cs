namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Delivery record
/// </summary>
public class Delivery
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public int OrderId { get; set; }
    public int? AssignedStaffId { get; set; }
    public string DeliveryAddress { get; set; } = string.Empty;
    public string DeliveryStatus { get; set; } = "pending"; // pending|in_transit|completed|failed
    public DateTime? DeliveredAt { get; set; }
    public string? ProofImageUrl { get; set; }
    public DateTime? ProofCapturedAt { get; set; }
    public string? Notes { get; set; }
    public string? RecipientConfirmedName { get; set; }
    public string? RecipientConfirmationCode { get; set; }
    /// <summary>Ảnh chữ ký người nhận (object storage URL).</summary>
    public string? RecipientSignatureUrl { get; set; }
    public DateTime? RecipientConfirmedAt { get; set; }
    public string? DeliveryOtp { get; set; }
    public DateTime? DeliveryOtpExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = VietnamTime.Now;

    public virtual Order Order { get; set; } = null!;
    public virtual User? AssignedStaff { get; set; }
}
