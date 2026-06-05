using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Contracts;

namespace SmartLunch.Backend.Service.Application.Queries.CompanyContracts.GetOrganizationContractWeeklySelections;

public sealed record GetOrganizationContractWeeklySelectionsQuery(int ContractId, int UserId)
    : IRequest<GetContractWeeklySelectionsResponse>;
