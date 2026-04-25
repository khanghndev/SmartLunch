namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Income/expense transaction (ReferenceId is app-managed: order/payment/contract etc.)
/// </summary>
public class Transaction
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public string? Description { get; set; }
    public decimal Amount { get; set; } // +income, -expense
    public string? Category { get; set; }
    public string? Method { get; set; }
    public int? ReferenceId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
