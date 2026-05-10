using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Contracts;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Contracts;

namespace SmartLunch.Backend.Service.Application.Commands.CompanyContracts.SignOrganizationContract;

public record SignOrganizationContractCommand(int ContractId, int UserId, SignContractRequest Request)
    : IRequest<GetContractResponse>;
