namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Orders;

/// <summary>Tóm tắt hợp đồng gắn đơn — đủ để hiển thị sau checkout; chi tiết đầy đủ / ký số dùng API company/contracts.</summary>
public class OrderContractSummaryDto
{
    public int Id { get; set; }
    public string ContractType { get; set; } = string.Empty;
    public string? ContractNumber { get; set; }
    public string? Description { get; set; }
    public string? SupplySchedule { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? TotalValue { get; set; }
    public decimal? MealUnitPrice { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? PartnerLegalName { get; set; }
    public bool IsDigitallySigned { get; set; }
    public DateTime? DigitallySignedAt { get; set; }
    public string? ContractFileUrl { get; set; }
}
