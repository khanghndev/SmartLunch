using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Contracts;

namespace SmartLunch.Backend.Service.Application.Queries.CompanyContracts.GetOrganizationContract;

public record GetOrganizationContractQuery(int ContractId, int UserId) : IRequest<GetContractResponse>;
