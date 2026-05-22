using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Contracts;

public static class ContractDtoMapping
{
    private static string? MaskDigitalSignature(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return null;
        var s = raw.Trim();
        if (s.StartsWith("data:image", StringComparison.OrdinalIgnoreCase))
            return "(chữ ký dạng ảnh đã lưu trong hệ thống)";
        return s.Length <= 160 ? s : s[..160] + "…";
    }

    public static ContractDto ToDto(Contract c) => new()
    {
        Id = c.Id,
        PartnerId = c.PartnerId,
        PartnerLegalName = c.Partner?.LegalName,
        OrganizationId = c.OrganizationId,
        OrganizationName = c.Organization?.Name,
        SourceOrderId = c.SourceOrderId,
        ContractType = c.ContractType,
        ContractNumber = c.ContractNumber,
        Description = c.Description,
        SupplySchedule = c.SupplySchedule,
        StartDate = c.StartDate,
        EndDate = c.EndDate,
        TotalValue = c.TotalValue,
        MealUnitPrice = c.MealUnitPrice,
        DepositAmount = c.DepositAmount,
        ContractFileUrl = c.ContractFileUrl,
        IsDigitallySigned = c.IsDigitallySigned,
        DigitalSignature = MaskDigitalSignature(c.DigitalSignature),
        DigitallySignedAt = c.DigitallySignedAt,
        SignatureImage = c.SignatureImage,
        Status = c.Status,
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt
    };
}
