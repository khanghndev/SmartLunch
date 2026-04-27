using System.Linq;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Orders;

public static class OrderDtoMapping
{
    public static OrderDto ToDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            UserId = order.UserId,
            OrganizationId = order.OrganizationId,
            OrganizationName = order.Organization?.Name,
            OrderDate = order.OrderDate,
            ScheduledDate = order.ScheduledDate,
            Status = order.Status,
            TotalAmount = order.TotalAmount,
            PaymentStatus = order.PaymentStatus,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            InvoiceCode = order.InvoiceCode,
            CreatedBySalesUserId = order.CreatedBySalesUserId,
            CreatedBySalesDisplayName = FormatSalesStaffName(order.CreatedBySalesUser),
            Items = order.OrderItems
                .OrderBy(i => i.Dish?.Name)
                .Select(i => new OrderItemLineDto
                {
                    Id = i.Id,
                    DishId = i.DishId,
                    DishName = i.Dish?.Name ?? string.Empty,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    TotalPrice = i.TotalPrice
                })
                .ToList()
        };
    }

    private static string? FormatSalesStaffName(User? u)
    {
        if (u == null)
            return null;

        var name = string.Join(" ", new[] { u.FirstName, u.LastName }.Where(s => !string.IsNullOrWhiteSpace(s))).Trim();
        return string.IsNullOrEmpty(name) ? u.Username : name;
    }
}
