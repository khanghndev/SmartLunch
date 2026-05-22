using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.OrganizationReviews;

public static class OrganizationReviewRules
{
    public static bool IsReviewableOrderStatus(Order order) =>
        string.Equals(order.Status, OrderLifecycleStatus.Delivered, StringComparison.OrdinalIgnoreCase)
        || (string.Equals(order.Status, OrderLifecycleStatus.Confirmed, StringComparison.OrdinalIgnoreCase)
            && string.Equals(order.PaymentStatus, "paid", StringComparison.OrdinalIgnoreCase));

    public static async Task<IReadOnlyList<int>> GetActiveOrganizationIdsAsync(
        IUserOrganizationRepository userOrganizations,
        int userId,
        CancellationToken cancellationToken = default)
    {
        var memberships = await userOrganizations.GetActiveByUserIdAsync(userId);
        return memberships.Select(m => m.OrganizationId).Distinct().ToList();
    }

    public static bool UserCanAccessOrder(Order order, int userId, IReadOnlyList<int> organizationIds)
    {
        if (order.UserId != userId)
            return false;
        if (order.ContractId == null || order.Contract == null)
            return false;
        return order.Contract.OrganizationId.HasValue
               && organizationIds.Contains(order.Contract.OrganizationId.Value);
    }
}
