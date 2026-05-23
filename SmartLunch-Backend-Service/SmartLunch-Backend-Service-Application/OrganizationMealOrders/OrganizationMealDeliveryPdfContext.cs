using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.OrganizationMealOrders;

/// <summary>Thông tin giao nhận đưa vào PDF hợp đồng / phụ lục.</summary>
public sealed class OrganizationMealDeliveryPdfContext
{
    public string RecipientName { get; init; } = string.Empty;
    public string RecipientPhone { get; init; } = string.Empty;
    public string RecipientEmail { get; init; } = string.Empty;
    public string DeliveryAddress { get; init; } = string.Empty;
    public string? DeliveryWardDistrict { get; init; }
    public string? DeliveryNotes { get; init; }
    public string? PreferredDeliveryTime { get; init; }

    public string FullAddress =>
        OrganizationMealDeliveryValidator.BuildFullAddress(new OrganizationMealOrderDraftDelivery
        {
            DeliveryAddress = DeliveryAddress,
            DeliveryWardDistrict = DeliveryWardDistrict,
        });

    public bool HasData =>
        !string.IsNullOrWhiteSpace(DeliveryAddress) ||
        !string.IsNullOrWhiteSpace(RecipientName);

    public static OrganizationMealDeliveryPdfContext? FromOrder(Order? order)
    {
        if (order == null || string.IsNullOrWhiteSpace(order.DeliveryAddress))
            return null;

        return new OrganizationMealDeliveryPdfContext
        {
            RecipientName = order.RecipientName ?? string.Empty,
            RecipientPhone = order.RecipientPhone ?? string.Empty,
            RecipientEmail = order.RecipientEmail ?? string.Empty,
            DeliveryAddress = order.DeliveryAddress,
            DeliveryWardDistrict = order.DeliveryWardDistrict,
            DeliveryNotes = order.DeliveryNotes,
            PreferredDeliveryTime = order.PreferredDeliveryTime,
        };
    }

    public static OrganizationMealDeliveryPdfContext? FromDraft(OrganizationMealOrderDraftDelivery? delivery)
    {
        if (delivery == null || string.IsNullOrWhiteSpace(delivery.DeliveryAddress))
            return null;

        return new OrganizationMealDeliveryPdfContext
        {
            RecipientName = delivery.RecipientName,
            RecipientPhone = delivery.RecipientPhone,
            RecipientEmail = delivery.RecipientEmail,
            DeliveryAddress = delivery.DeliveryAddress,
            DeliveryWardDistrict = delivery.DeliveryWardDistrict,
            DeliveryNotes = delivery.DeliveryNotes,
            PreferredDeliveryTime = delivery.PreferredDeliveryTime,
        };
    }
}
