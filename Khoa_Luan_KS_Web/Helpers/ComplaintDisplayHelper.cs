namespace Khoa_Luan_KS_Web.Helpers;

public static class ComplaintDisplayHelper
{
    public static string ReasonLabel(string? reason) => (reason ?? "").ToLowerInvariant() switch
    {
        "missing_portions" => "Thiếu suất",
        "spoiled_rice" => "Cơm hỏng / ôi",
        "wrong_dish" => "Sai món",
        "food_quality" => "Chất lượng món ăn",
        _ => reason ?? "—"
    };

    public static string StatusLabel(string? status) => (status ?? "").ToLowerInvariant() switch
    {
        "draft" => "Nháp",
        "pending_review" => "Chờ duyệt",
        "resolved" => "Đã xử lý",
        "rejected" => "Từ chối",
        "new" => "Mới",
        "in_progress" => "Đang xử lý",
        _ => status ?? "—"
    };

    public static string StatusBadgeClass(string? status) => (status ?? "").ToLowerInvariant() switch
    {
        "draft" => "bg-slate-100 text-slate-700 border-slate-200",
        "pending_review" => "bg-amber-50 text-amber-800 border-amber-200",
        "resolved" => "bg-emerald-50 text-emerald-800 border-emerald-200",
        "rejected" => "bg-red-50 text-red-800 border-red-200",
        _ => "bg-gray-100 text-gray-700 border-gray-200"
    };

    public static string ResolutionLabel(string? resolution) => (resolution ?? "").ToLowerInvariant() switch
    {
        "refund" => "Hoàn tiền",
        "rejected" => "Từ chối",
        _ => resolution ?? "—"
    };

    public static string EvidenceKindLabel(string? kind) => (kind ?? "").ToLowerInvariant() switch
    {
        "receipt_photo" => "Ảnh nhận hàng",
        "unboxing_video" => "Video mở thùng",
        "portion_count_video" => "Video đếm suất",
        "food_condition_video" => "Video tình trạng món",
        "other" => "Khác",
        _ => kind ?? "—"
    };
}
