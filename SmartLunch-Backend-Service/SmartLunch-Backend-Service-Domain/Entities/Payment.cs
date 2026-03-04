namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Order payment
/// </summary>
public class Payment
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid? PayerId { get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public decimal Amount { get; set; }
    public string Method { get; set; } = "cash"; // cash|card|bank_transfer|e_wallet
    public string Status { get; set; } = "pending"; // pending|paid|refunded
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual Order Order { get; set; } = null!;
    public virtual User? Payer { get; set; }
}
