using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Partners;

public static class PartnerDtoMapping
{
    public static PartnerDto ToDto(Partner p) => new()
    {
        Id = p.Id,
        LegalName = p.LegalName,
        BusinessRegistrationNumber = p.BusinessRegistrationNumber,
        TaxId = p.TaxId,
        LegalRepresentative = p.LegalRepresentative,
        Address = p.Address,
        ContactPerson = p.ContactPerson,
        Phone = p.Phone,
        Email = p.Email,
        PerformanceRating = p.PerformanceRating,
        ComplianceInfo = p.ComplianceInfo,
        FinancialTerms = p.FinancialTerms,
        IsActive = p.IsActive,
        CreatedAt = p.CreatedAt,
        UpdatedAt = p.UpdatedAt
    };

    public static PartnerContractSummaryDto ToContractSummary(Contract c) => new()
    {
        Id = c.Id,
        ContractNumber = c.ContractNumber,
        Description = c.Description,
        SupplySchedule = c.SupplySchedule,
        StartDate = c.StartDate,
        EndDate = c.EndDate,
        Status = c.Status,
        TotalValue = c.TotalValue
    };
}
