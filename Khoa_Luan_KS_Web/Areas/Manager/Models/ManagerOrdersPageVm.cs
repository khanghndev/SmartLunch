using Khoa_Luan_KS_Web.Services;

namespace Khoa_Luan_KS_Web.Areas.Manager.Models;

public class ManagerOrdersPageVm
{
    public GetOrdersClientResponse Orders { get; set; } = new();
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 15;
    public string? FilterStatus { get; set; }
    public string? Search { get; set; }
    public DateOnly? ScheduledOn { get; set; }

    public int CountPending { get; set; }
    public int CountConfirmed { get; set; }
    public int CountPreparing { get; set; }
    public int CountDelivered { get; set; }
    public int CountDeliveredToday { get; set; }

    public int TotalCount => Orders.TotalCount;

    public int TotalPages => PageSize > 0
        ? Math.Max(1, (int)Math.Ceiling(Orders.TotalCount / (double)PageSize))
        : 1;

    public ManagerPaginationVm Pagination => new()
    {
        Page = Page,
        PageSize = PageSize,
        TotalCount = Orders.TotalCount,
        ItemCount = Orders.Items.Count,
        TotalPages = TotalPages,
        Status = FilterStatus,
        Search = Search,
        ScheduledOn = ScheduledOn?.ToString("yyyy-MM-dd")
    };
}
