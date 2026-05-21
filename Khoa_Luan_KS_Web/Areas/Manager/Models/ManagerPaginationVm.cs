namespace Khoa_Luan_KS_Web.Areas.Manager.Models;

/// <summary>View model cho thanh phân trang dùng chung khu vực Manager.</summary>
public class ManagerPaginationVm
{
    public string Area { get; set; } = "Manager";
    public string Controller { get; set; } = "Order";
    public string Action { get; set; } = "Index";

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 15;
    public int TotalCount { get; set; }
    public int ItemCount { get; set; }
    public int TotalPages { get; set; }

    public string? Status { get; set; }
    public string? Search { get; set; }
    public string? ScheduledOn { get; set; }
    public string? SearchTerm { get; set; }
    public string? RoleName { get; set; }

    public int FromRecord => TotalCount == 0 ? 0 : (Page - 1) * PageSize + 1;
    public int ToRecord => TotalCount == 0 ? 0 : Math.Min(Page * PageSize, TotalCount);

    public object RouteAt(int targetPage)
    {
        if (!string.IsNullOrEmpty(SearchTerm) || !string.IsNullOrEmpty(RoleName))
        {
            return new
            {
                page = targetPage,
                pageSize = PageSize,
                searchTerm = SearchTerm,
                roleName = RoleName
            };
        }

        return new
        {
            page = targetPage,
            pageSize = PageSize,
            status = Status,
            search = Search,
            scheduledOn = ScheduledOn
        };
    }
}
