namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.WeeklyMenus;

public class GetWeeklyMenusRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public int? CustomerTypeId { get; set; }

    /// <summary>Lọc theo <c>customer_types.ProfileKey</c> (ví dụ org_primary_school, industrial, org_company).</summary>
    public string? CustomerProfileKey { get; set; }

    /// <summary>Ngày tham chiếu (date-only): chỉ trả weekly menu có StartDate–EndDate bao ngày này.</summary>
    public DateTime? EffectiveDate { get; set; }
}
