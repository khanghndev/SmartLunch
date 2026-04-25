namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Partners;

/// <summary>Hợp đồng gắn đối tác (thời gian cung cấp theo khung Start/End + SupplySchedule).</summary>
public class PartnerContractSummaryDto
{
    public int Id { get; set; }
    public string? ContractNumber { get; set; }
    public string? Description { get; set; }
    public string? SupplySchedule { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal? TotalValue { get; set; }
}
