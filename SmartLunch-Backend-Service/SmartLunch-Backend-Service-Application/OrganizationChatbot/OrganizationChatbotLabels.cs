using System.Globalization;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.OrganizationChatbot;

/// <summary>Label/format helpers dùng chung cho RAG context và rule fallback.</summary>
public static class OrganizationChatbotLabels
{
    public static bool IsAwaitingPayment(string paymentStatus) =>
        string.Equals(paymentStatus, OrderPaymentStatus.Unpaid, StringComparison.OrdinalIgnoreCase) ||
        string.Equals(paymentStatus, OrderPaymentStatus.AwaitingPayment, StringComparison.OrdinalIgnoreCase) ||
        string.Equals(paymentStatus, OrderPaymentStatus.Partial, StringComparison.OrdinalIgnoreCase) ||
        string.Equals(paymentStatus, OrderPaymentStatus.DepositPaid, StringComparison.OrdinalIgnoreCase);

    public static string FormatMoney(decimal amount) =>
        amount.ToString("N0", CultureInfo.GetCultureInfo("vi-VN")) + " đ";

    public static string FormatOrderStatus(string status) => status.ToLowerInvariant() switch
    {
        OrderLifecycleStatus.Pending => "Chờ xác nhận",
        OrderLifecycleStatus.Confirmed => "Đã xác nhận",
        OrderLifecycleStatus.Preparing => "Đang chuẩn bị",
        OrderLifecycleStatus.Delivered => "Đã giao",
        OrderLifecycleStatus.Cancelled => "Đã hủy",
        _ => status,
    };

    public static string FormatPaymentStatus(string status) => status.ToLowerInvariant() switch
    {
        OrderPaymentStatus.Unpaid => "Chưa thanh toán",
        OrderPaymentStatus.AwaitingPayment => "Chờ thanh toán",
        OrderPaymentStatus.DepositPaid => "Đã cọc",
        OrderPaymentStatus.Partial => "Thanh toán một phần",
        OrderPaymentStatus.Paid => "Đã thanh toán",
        _ => status,
    };

    public static string FormatComplaintStatus(string status) => status.ToLowerInvariant() switch
    {
        ComplaintStatus.Draft => "Nháp",
        ComplaintStatus.PendingReview => "Chờ duyệt",
        ComplaintStatus.Resolved => "Đã xử lý",
        ComplaintStatus.Rejected => "Từ chối",
        ComplaintStatus.LegacyNew => "Mới",
        ComplaintStatus.LegacyInProgress => "Đang xử lý",
        _ => status,
    };

    public static string FormatOrganizationType(string type) => type.ToLowerInvariant() switch
    {
        "office" => "Văn phòng",
        "factory" => "Xí nghiệp",
        "school" => "Trường học",
        _ => type,
    };

    public static string GetCutoffRuleText(string organizationType) => organizationType.ToLowerInvariant() switch
    {
        "office" => "Đặt trước ít nhất 1 ngày so với ngày phục vụ. Đơn cho ngày mai phải chốt trước 17h00 cùng ngày.",
        "factory" => "Đặt trước ít nhất 2 ngày so với ngày phục vụ.",
        "school" => "Đặt trước ít nhất 3 ngày so với ngày phục vụ.",
        _ => "Đặt trước ít nhất 1 ngày so với ngày phục vụ.",
    };

    public static string GetOrderHowToText(string organizationType, string? orgName)
    {
        var unit = orgName ?? "Quý khách";
        var cutoff = GetCutoffRuleText(organizationType);
        return $"""
            {unit} đặt món trên web HuitMeal theo các bước:
            • Bước 1: Đăng nhập → mục Thực đơn hoặc Đặt món tuần (theo hợp đồng đang hiệu lực).
            • Bước 2: Chọn món trong thực đơn tuần phù hợp loại hình {FormatOrganizationType(organizationType)}.
            • Bước 3: Xác nhận đơn trước hạn chốt — {cutoff}
            • Bước 4: Theo dõi trạng thái tại Đơn hàng; thanh toán PayOS khi hệ thống yêu cầu.
            """;
    }

    public static int? ResolveCustomerTypeId(string? organizationType, List<CustomerType> types)
    {
        var key = organizationType?.ToLowerInvariant() switch
        {
            "school" => "org_primary_school",
            "factory" => "industrial",
            _ => "org_company",
        };
        return types.FirstOrDefault(t =>
            string.Equals(t.ProfileKey, key, StringComparison.OrdinalIgnoreCase))?.Id;
    }
}
