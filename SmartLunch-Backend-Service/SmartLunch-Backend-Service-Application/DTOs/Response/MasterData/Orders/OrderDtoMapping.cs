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
            ContractId = order.ContractId,
            ContractSummary = order.Contract == null ? null : ToContractSummary(order.Contract),
            OrganizationId = order.Contract?.OrganizationId,
            OrganizationName = ResolveOrganizationName(order),
            OrderDate = order.OrderDate,
            ScheduledDate = order.ScheduledDate,
            Status = order.Status,
            TotalAmount = order.TotalAmount,
            SubtotalAmount = order.SubtotalAmount,
            DiscountAmount = order.DiscountAmount,
            AppliedPromotion = order.PromotionApplications
                .OrderByDescending(a => a.AppliedAt)
                .Select(a => new OrderPromotionSummaryDto
                {
                    PromotionId = a.PromotionId,
                    PromotionCode = a.PromotionCode,
                    PromotionName = a.PromotionName,
                    DiscountAmount = a.DiscountAmount,
                })
                .FirstOrDefault(),
            PaymentStatus = order.PaymentStatus,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            InvoiceCode = order.InvoiceCode,
            CreatedBySalesUserId = order.CreatedBySalesUserId,
            CreatedBySalesDisplayName = FormatSalesStaffName(order.CreatedBySalesUser),
            AnnexPdfUrl = order.AnnexPdfUrl,
            AnnexSignedAt = order.AnnexSignedAt,
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

    private static OrderContractSummaryDto ToContractSummary(Contract c) => new()
    {
        Id = c.Id,
        ContractType = c.ContractType,
        ContractNumber = c.ContractNumber,
        Description = c.Description,
        SupplySchedule = c.SupplySchedule,
        StartDate = c.StartDate,
        EndDate = c.EndDate,
        TotalValue = c.TotalValue,
        MealUnitPrice = c.MealUnitPrice,
        Status = c.Status,
        PartnerLegalName = c.Partner?.LegalName,
        IsDigitallySigned = c.IsDigitallySigned,
        DigitallySignedAt = c.DigitallySignedAt,
        ContractFileUrl = c.ContractFileUrl
    };

    private static string? ResolveOrganizationName(Order order)
    {
        var fromContract = order.Contract?.Organization?.Name;
        if (!string.IsNullOrWhiteSpace(fromContract))
            return fromContract;

        return order.User?.UserOrganizations?
            .Where(uo => uo.IsActive)
            .Select(uo => uo.Organization?.Name)
            .FirstOrDefault(n => !string.IsNullOrWhiteSpace(n));
    }

    private static string? FormatSalesStaffName(User? u)
    {
        if (u == null)
            return null;

        var name = string.Join(" ", new[] { u.FirstName, u.LastName }.Where(s => !string.IsNullOrWhiteSpace(s))).Trim();
        return string.IsNullOrEmpty(name) ? u.Username : name;
    }
}
