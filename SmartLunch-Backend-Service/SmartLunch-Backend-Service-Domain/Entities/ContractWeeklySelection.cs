namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>Bản ghi chọn món theo tuần thuộc một HĐ Period-Based (một HĐ — nhiều tuần).</summary>
public class ContractWeeklySelection
{
    public int Id { get; set; }
    public int ContractId { get; set; }

    /// <summary>Thứ 2 của tuần phục vụ.</summary>
    public DateOnly WeekMonday { get; set; }

    /// <summary>pending | selected | auto_filled</summary>
    public string Status { get; set; } = ContractWeeklySelectionStatuses.Pending;

    /// <summary>Đơn fulfillment tuần (kitchen / giao hàng).</summary>
    public int? FulfillmentOrderId { get; set; }

    public DateTime? SelectedAt { get; set; }
    public DateTime CreatedAt { get; set; } = VietnamTime.Now;
    public DateTime? UpdatedAt { get; set; }

    public virtual Contract Contract { get; set; } = null!;
    public virtual Order? FulfillmentOrder { get; set; }
    public virtual ICollection<ContractWeeklySelectionItem> Items { get; set; } = new List<ContractWeeklySelectionItem>();
}

public static class ContractWeeklySelectionStatuses
{
    public const string Pending = "pending";
    public const string Selected = "selected";
    public const string AutoFilled = "auto_filled";

    public static bool IsFilled(string status) =>
        string.Equals(status, Selected, StringComparison.OrdinalIgnoreCase)
        || string.Equals(status, AutoFilled, StringComparison.OrdinalIgnoreCase);
}
