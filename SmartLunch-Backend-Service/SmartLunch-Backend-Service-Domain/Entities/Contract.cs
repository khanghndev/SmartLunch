namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Supplier contract
/// </summary>
public class Contract
{
    public Guid Id { get; set; }
    public Guid PartnerId { get; set; }
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? TotalValue { get; set; }
    public decimal? DepositAmount { get; set; }
    public string Status { get; set; } = "active"; // active | expired
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public virtual Partner Partner { get; set; } = null!;
    public virtual ICollection<PartnerPayment> PartnerPayments { get; set; } = new List<PartnerPayment>();
}
