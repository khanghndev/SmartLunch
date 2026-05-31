using Khoa_Luan_KS_Web.Services;

namespace Khoa_Luan_KS_Web.Models;

public class ProfileOrdersPageVm
{
    public GetOrdersClientResponse Orders { get; set; } = new();
    public int Page { get; set; } = 1;
    public string? FilterStatus { get; set; }
    public string? FilterPaymentStatus { get; set; }
    public string? Search { get; set; }
    public DateOnly? ScheduledOn { get; set; }
}
