using SmartLunch.Backend.Service.Application.Constants;

namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Contracts;

public class CreateContractRequest
{
    public Guid PartnerId { get; set; }
    public string? ContractNumber { get; set; }
    public string? Description { get; set; }

    /// <summary>Mô tả khung thời gian cung cấp suất ăn (vd. các ngày trong tuần, ca).</summary>
    public string? SupplySchedule { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? TotalValue { get; set; }
    public decimal? DepositAmount { get; set; }

    /// <summary>active | expired | cancelled (mặc định active; hết hạn có thể được cập nhật tự động theo EndDate).</summary>
    public string Status { get; set; } = ContractStatus.Active;
}
