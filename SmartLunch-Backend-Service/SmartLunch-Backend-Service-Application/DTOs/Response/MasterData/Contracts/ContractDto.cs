namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Contracts;

public class ContractDto
{
    public Guid Id { get; set; }
    public Guid PartnerId { get; set; }
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? TotalValue { get; set; }
    public decimal? DepositAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
