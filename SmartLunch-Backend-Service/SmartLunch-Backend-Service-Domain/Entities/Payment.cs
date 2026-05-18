namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Order payment
/// </summary>
public class Payment
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public int OrderId { get; set; }
    public int? PayerId { get; set; }
    public DateTime PaymentDate { get; set; } = VietnamTime.Now;
    public decimal Amount { get; set; }
    public string Method { get; set; } = "cash"; // cash|card|bank_transfer|e_wallet
    public string Status { get; set; } = "pending"; // pending|paid|refunded
    public DateTime CreatedAt { get; set; } = VietnamTime.Now;

    public virtual Order Order { get; set; } = null!;
    public virtual User? Payer { get; set; }
}
