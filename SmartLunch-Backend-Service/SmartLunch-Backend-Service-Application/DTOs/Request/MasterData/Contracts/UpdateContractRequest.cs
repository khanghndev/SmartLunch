using SmartLunch.Backend.Service.Application.Constants;

namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Contracts;

public class UpdateContractRequest
{
    public string? ContractNumber { get; set; }
    public string? Description { get; set; }
    public string? SupplySchedule { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? TotalValue { get; set; }
    public decimal? DepositAmount { get; set; }
    public string Status { get; set; } = ContractStatus.Active;
}
