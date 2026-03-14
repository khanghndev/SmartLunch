namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Transactions;

public class TransactionDto
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public string? Category { get; set; }
    public string? Method { get; set; }
    public Guid? ReferenceId { get; set; }
    public DateTime CreatedAt { get; set; }
}
