using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Contracts;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Contracts;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Contracts.SignContract;

public record SignContractCommand(int ContractId, int ActorUserId, SignContractRequest Request) : IRequest<GetContractResponse>;

