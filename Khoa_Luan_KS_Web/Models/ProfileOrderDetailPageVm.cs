using Khoa_Luan_KS_Web.Services;

namespace Khoa_Luan_KS_Web.Models;

public class ProfileOrderDetailPageVm
{
    public OrderDetailClientDto Order { get; set; } = new();
    public bool IsPeriodBased { get; set; }
    public CustomerContractDto? Contract { get; set; }
    public GetContractWeeklySelectionsClientResponse? Weekly { get; set; }
}
