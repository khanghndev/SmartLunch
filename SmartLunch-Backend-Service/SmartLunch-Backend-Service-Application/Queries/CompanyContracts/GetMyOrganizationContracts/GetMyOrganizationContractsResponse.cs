using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Contracts;

namespace SmartLunch.Backend.Service.Application.Queries.CompanyContracts.GetMyOrganizationContracts;

public class GetMyOrganizationContractsResponse
{
    public List<ContractDto> Contracts { get; set; } = new();
}
