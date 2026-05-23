using SmartLunch.Backend.Service.Application.DTOs.Response.Manager.Deliveries;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.ManagerDeliveries;

public static class ManagerDeliveryMapper
{
    public static string StatusLabel(string? status)
    {
        var s = (status ?? "pending").Trim().ToLowerInvariant();
        return s switch
        {
            "pending" => "Chờ điều phối",
            "received" => "Đã giao shipper",
            "in_transit" => "Đang giao",
            "completed" => "Đã giao",
            "failed" => "Thất bại",
            "rejected" => "Từ chối",
            _ => status ?? "—",
        };
    }

    public static ManagerDeliveryListItemDto ToListItem(Delivery d)
    {
        var order = d.Order;
        var orgName = order?.Contract?.Organization?.Name ?? "—";
        var mealCount = order?.OrderItems?.Sum(i => i.Quantity) ?? 0;
        var staffName = d.AssignedStaff == null
            ? null
            : $"{d.AssignedStaff.FirstName} {d.AssignedStaff.LastName}".Trim();
        if (string.IsNullOrWhiteSpace(staffName))
            staffName = d.AssignedStaff.Username;

        return new ManagerDeliveryListItemDto
        {
            DeliveryId = d.Id,
            DeliveryCode = d.Code,
            OrderId = d.OrderId,
            OrderCode = order?.Code,
            InvoiceCode = order?.InvoiceCode,
            OrganizationName = orgName,
            DeliveryAddress = d.DeliveryAddress,
            DeliveryStatus = d.DeliveryStatus,
            DeliveryStatusLabel = StatusLabel(d.DeliveryStatus),
            ScheduledDate = order?.ScheduledDate ?? d.CreatedAt,
            PreferredDeliveryTime = order?.PreferredDeliveryTime,
            MealCount = mealCount,
            AssignedStaffId = d.AssignedStaffId,
            AssignedStaffName = staffName,
            AssignedStaffPhone = d.AssignedStaff?.PhoneNumber,
            Notes = d.Notes,
            ProofImageUrl = d.ProofImageUrl,
            DeliveredAt = d.DeliveredAt,
            CreatedAt = d.CreatedAt,
        };
    }
}
