namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Payment to supplier
/// </summary>
public class PartnerPayment
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public int ContractId { get; set; }
    public int PartnerId { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; } = "bank_transfer"; // bank_transfer | cash | card
    public string Status { get; set; } = "pending"; // pending | completed | failed
    public DateTime CreatedAt { get; set; } = VietnamTime.Now;

    public virtual Contract Contract { get; set; } = null!;
    public virtual Partner Partner { get; set; } = null!;
}
