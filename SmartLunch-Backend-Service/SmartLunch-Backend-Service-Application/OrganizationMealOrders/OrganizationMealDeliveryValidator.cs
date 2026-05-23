using System.Net.Mail;
using System.Text.RegularExpressions;
using SmartLunch.Backend.Service.Application.DTOs.Request.OrganizationMealOrders;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.OrganizationMealOrders;

public static class OrganizationMealDeliveryValidator
{
    private static readonly Regex PhoneRegex = new(@"^(\+?84|0)[0-9]{8,10}$", RegexOptions.Compiled);
    private static readonly Regex TimeRegex = new(@"^([01]?\d|2[0-3]):[0-5]\d$", RegexOptions.Compiled);

    public static OrganizationMealOrderDraftDelivery Normalize(OrganizationMealDeliveryRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var name = (request.RecipientName ?? string.Empty).Trim();
        if (name.Length < 2)
            throw new ArgumentException("RecipientName must be at least 2 characters.");

        var phone = NormalizePhone(request.RecipientPhone);
        if (!PhoneRegex.IsMatch(phone))
            throw new ArgumentException("RecipientPhone is not valid.");

        var email = (request.RecipientEmail ?? string.Empty).Trim();
        if (!MailAddress.TryCreate(email, out _))
            throw new ArgumentException("RecipientEmail is not valid.");

        var address = (request.DeliveryAddress ?? string.Empty).Trim();
        if (address.Length < 10)
            throw new ArgumentException("DeliveryAddress must be at least 10 characters.");

        var ward = string.IsNullOrWhiteSpace(request.DeliveryWardDistrict)
            ? null
            : request.DeliveryWardDistrict.Trim();

        var notes = string.IsNullOrWhiteSpace(request.DeliveryNotes)
            ? null
            : request.DeliveryNotes.Trim();

        string? time = null;
        if (!string.IsNullOrWhiteSpace(request.PreferredDeliveryTime))
        {
            time = request.PreferredDeliveryTime.Trim();
            if (!TimeRegex.IsMatch(time))
                throw new ArgumentException("PreferredDeliveryTime must be HH:mm.");
        }

        return new OrganizationMealOrderDraftDelivery
        {
            RecipientName = name,
            RecipientPhone = phone,
            RecipientEmail = email,
            DeliveryAddress = address,
            DeliveryWardDistrict = ward,
            DeliveryNotes = notes,
            PreferredDeliveryTime = time,
        };
    }

    public static void ApplyToOrder(Order order, OrganizationMealOrderDraftDelivery delivery)
    {
        order.RecipientName = delivery.RecipientName;
        order.RecipientPhone = delivery.RecipientPhone;
        order.RecipientEmail = delivery.RecipientEmail;
        order.DeliveryAddress = delivery.DeliveryAddress;
        order.DeliveryWardDistrict = delivery.DeliveryWardDistrict;
        order.DeliveryNotes = delivery.DeliveryNotes;
        order.PreferredDeliveryTime = delivery.PreferredDeliveryTime;
    }

    public static string BuildFullAddress(OrganizationMealOrderDraftDelivery delivery)
    {
        if (!string.IsNullOrWhiteSpace(delivery.DeliveryWardDistrict))
            return $"{delivery.DeliveryAddress}, {delivery.DeliveryWardDistrict}";
        return delivery.DeliveryAddress;
    }

    private static string NormalizePhone(string? raw)
    {
        var p = (raw ?? string.Empty).Trim().Replace(" ", "").Replace("-", "");
        if (p.StartsWith("+84", StringComparison.Ordinal))
            p = "0" + p[3..];
        return p;
    }
}
