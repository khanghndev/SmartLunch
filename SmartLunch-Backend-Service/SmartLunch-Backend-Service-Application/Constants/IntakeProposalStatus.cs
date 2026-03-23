namespace SmartLunch.Backend.Service.Application.Constants;

/// <summary>Trạng thái phiếu đề xuất nhập nguyên liệu.</summary>
public static class IntakeProposalStatus
{
    public const string Submitted = "submitted";
    public const string Approved = "approved";
    public const string Rejected = "rejected";
    public const string Cancelled = "cancelled";
    /// <summary>Đã nhập kho thực tế (phiếu nhập đã ghi tồn).</summary>
    public const string Fulfilled = "fulfilled";
}
