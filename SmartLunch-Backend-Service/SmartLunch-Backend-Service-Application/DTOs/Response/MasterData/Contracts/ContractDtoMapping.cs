using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Contracts;

public static class ContractDtoMapping
{
    public static ContractDto ToDto(Contract c) => new()
    {
        Id = c.Id,
        PartnerId = c.PartnerId,
        PartnerLegalName = c.Partner?.LegalName,
        OrganizationId = c.OrganizationId,
        ContractNumber = c.ContractNumber,
        Description = c.Description,
        SupplySchedule = c.SupplySchedule,
        StartDate = c.StartDate,
        EndDate = c.EndDate,
        TotalValue = c.TotalValue,
        DepositAmount = c.DepositAmount,
        ContractFileUrl = c.ContractFileUrl,
        IsDigitallySigned = c.IsDigitallySigned,
        DigitalSignature = c.DigitalSignature,
        DigitallySignedAt = c.DigitallySignedAt,
        SignatureImage = c.SignatureImage,
        Status = c.Status,
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt
    };
}
