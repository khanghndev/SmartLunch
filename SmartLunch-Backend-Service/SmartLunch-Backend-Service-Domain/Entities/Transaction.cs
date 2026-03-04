namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Income/expense transaction (ReferenceId is app-managed: order/payment/contract etc.)
/// </summary>
public class Transaction
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public string? Description { get; set; }
    public decimal Amount { get; set; } // +income, -expense
    public string? Category { get; set; }
    public string? Method { get; set; }
    public Guid? ReferenceId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
