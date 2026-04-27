using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Contracts;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Contracts;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Contracts.UpdateContract;

public class UpdateContractCommand : IRequest<GetContractResponse>
{
    public int ContractId { get; }
    public UpdateContractRequest Request { get; }

    public UpdateContractCommand(int contractId, UpdateContractRequest request)
    {
        ContractId = contractId;
        Request = request;
    }
}
