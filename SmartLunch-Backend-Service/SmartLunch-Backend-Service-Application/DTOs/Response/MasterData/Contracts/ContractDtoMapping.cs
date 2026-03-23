using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Contracts;

public static class ContractDtoMapping
{
    public static ContractDto ToDto(Contract c) => new()
    {
        Id = c.Id,
        PartnerId = c.PartnerId,
        PartnerLegalName = c.Partner?.LegalName,
        ContractNumber = c.ContractNumber,
        Description = c.Description,
        SupplySchedule = c.SupplySchedule,
        StartDate = c.StartDate,
        EndDate = c.EndDate,
        TotalValue = c.TotalValue,
        DepositAmount = c.DepositAmount,
        Status = c.Status,
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt
    };
}
