using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.OrganizationReviews;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Domain.Time;

namespace SmartLunch.Backend.Service.Application.OrganizationComplaints;

public static class OrganizationComplaintRules
{
    /// <summary>
    /// Thời điểm đơn được coi là đã giao — trùng lúc chuyển <c>orders.status = delivered</c>
    /// (shipper POST /proof hoặc manager cập nhật trạng thái đều ghi <c>deliveries.delivered_at</c>).
    /// </summary>
    public static DateTime? GetDeliveredAt(Order order)
    {
        if (!string.Equals(order.Status, OrderLifecycleStatus.Delivered, StringComparison.OrdinalIgnoreCase))
            return null;

        var delivery = order.Deliveries?
            .Where(d => string.Equals(d.DeliveryStatus, "completed", StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(d => d.DeliveredAt)
            .FirstOrDefault();

        return delivery?.DeliveredAt
            ?? order.Deliveries?.Where(d => d.DeliveredAt != null).MaxBy(d => d.DeliveredAt)?.DeliveredAt
            ?? order.UpdatedAt;
    }

    public static DateTime? GetComplaintDeadline(Order order)
    {
        var deliveredAt = GetDeliveredAt(order);
        return deliveredAt?.AddHours(ComplaintMediaLimits.ComplaintWindowHours);
    }

    public static bool IsWithinComplaintWindow(Order order, DateTime now)
    {
        var deadline = GetComplaintDeadline(order);
        return deadline.HasValue && now <= deadline.Value;
    }

    public static bool CanComplainAboutOrder(Order order, DateTime now)
    {
        if (!string.Equals(order.Status, OrderLifecycleStatus.Delivered, StringComparison.OrdinalIgnoreCase))
            return false;
        return IsWithinComplaintWindow(order, now);
    }

    public static int CountMainPortions(Order order) =>
        order.OrderItems?.Where(i => i.UnitPrice > 0).Sum(i => i.Quantity) ?? 0;

    public static async Task<IReadOnlyList<int>> GetActiveOrganizationIdsAsync(
        IUserOrganizationRepository userOrganizations,
        int userId,
        CancellationToken cancellationToken = default) =>
        await OrganizationReviewRules.GetActiveOrganizationIdsAsync(userOrganizations, userId, cancellationToken);

    public static bool UserCanAccessOrder(Order order, int userId, IReadOnlyList<int> organizationIds) =>
        OrganizationReviewRules.UserCanAccessOrder(order, userId, organizationIds);

    public static bool IsEditable(Complaint complaint) =>
        string.Equals(complaint.Status, ComplaintStatus.Draft, StringComparison.OrdinalIgnoreCase);

    public static string TitleForReason(string reason) => reason.ToLowerInvariant() switch
    {
        ComplaintReason.MissingPortions => "Thiếu suất ăn",
        ComplaintReason.SpoiledRice => "Cơm bị hỏng",
        ComplaintReason.WrongDish => "Giao sai món",
        ComplaintReason.FoodQuality => "Đồ ăn có vấn đề",
        _ => "Khiếu nại đơn hàng"
    };
}
