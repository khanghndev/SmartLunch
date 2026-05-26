using Khoa_Luan_KS_Web.Models;
using Khoa_Luan_KS_Web.Services;

namespace Khoa_Luan_KS_Web.Helpers;

/// <summary>Điền sẵn bước giao hàng từ hồ sơ người dùng + đơn vị (doanh nghiệp).</summary>
public static class OrganizationMealDeliveryDefaultsBuilder
{
    public static OrganizationMealDeliveryDefaultsVm Build(
        UserProfileResponse user,
        OrganizationProfileClientResponse? organization)
    {
        var unit = user.Unit;
        var recipientName = FirstNonEmpty(
            organization?.ContactPerson,
            user.FullName,
            JoinName(user.FirstName, user.LastName),
            organization?.LegalRepresentative,
            unit?.LegalRepresentative);

        var recipientPhone = FirstNonEmpty(
            organization?.Phone,
            user.PhoneNumber,
            unit?.Phone);

        var recipientEmail = FirstNonEmpty(
            organization?.ContactEmail,
            user.Email,
            unit?.ContactEmail);

        var fullAddress = FirstNonEmpty(
            organization?.Address,
            unit?.Address,
            user.Address);

        var (street, ward) = SplitAddress(fullAddress);

        var sources = new List<string>();
        if (!string.IsNullOrWhiteSpace(organization?.ContactPerson) ||
            !string.IsNullOrWhiteSpace(organization?.Phone) ||
            !string.IsNullOrWhiteSpace(organization?.Address))
            sources.Add("hồ sơ đơn vị");
        if (!string.IsNullOrWhiteSpace(user.FullName) ||
            !string.IsNullOrWhiteSpace(user.PhoneNumber) ||
            !string.IsNullOrWhiteSpace(user.Email))
            sources.Add("tài khoản đăng nhập");

        var sourceHint = sources.Count > 0
            ? "Đã điền sẵn từ " + string.Join(" và ", sources.Distinct()) + ". Kiểm tra và chỉnh sửa nếu cần."
            : "Vui lòng nhập thông tin người nhận và địa chỉ giao hàng.";

        return new OrganizationMealDeliveryDefaultsVm
        {
            RecipientName = recipientName,
            RecipientPhone = recipientPhone,
            RecipientEmail = recipientEmail,
            DeliveryAddress = street,
            DeliveryWardDistrict = ward,
            PreferredDeliveryTime = "11:30",
            SourceHint = sourceHint,
            OrganizationName = organization?.Name ?? unit?.Name,
        };
    }

    private static string? FirstNonEmpty(params string?[] values)
    {
        foreach (var v in values)
        {
            if (!string.IsNullOrWhiteSpace(v))
                return v.Trim();
        }
        return null;
    }

    private static string? JoinName(string? first, string? last)
    {
        var s = $"{first} {last}".Trim();
        return string.IsNullOrWhiteSpace(s) ? null : s;
    }

    /// <summary>Tách địa chỉ dài: phần sau dấu phẩy cuối → phường/quận/TP.</summary>
    private static (string? Street, string? WardDistrict) SplitAddress(string? full)
    {
        if (string.IsNullOrWhiteSpace(full))
            return (null, null);

        var text = full.Trim();
        var lastComma = text.LastIndexOf(',');
        if (lastComma <= 0 || lastComma >= text.Length - 2)
            return (text, null);

        var street = text[..lastComma].Trim().TrimEnd(',');
        var ward = text[(lastComma + 1)..].Trim();
        if (street.Length < 5)
            return (text, null);

        return (street, ward);
    }
}
