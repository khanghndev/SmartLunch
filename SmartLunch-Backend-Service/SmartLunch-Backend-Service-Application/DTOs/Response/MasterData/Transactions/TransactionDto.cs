namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Transactions;

public class TransactionDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public string? Category { get; set; }
    public string? Method { get; set; }
    public int? ReferenceId { get; set; }
    public DateTime CreatedAt { get; set; }
}
