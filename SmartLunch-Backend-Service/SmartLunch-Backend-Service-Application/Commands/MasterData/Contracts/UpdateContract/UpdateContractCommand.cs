using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Contracts;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Contracts;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Contracts.UpdateContract;

public class UpdateContractCommand : IRequest<GetContractResponse>
{
    public Guid ContractId { get; }
    public UpdateContractRequest Request { get; }

    public UpdateContractCommand(Guid contractId, UpdateContractRequest request)
    {
        ContractId = contractId;
        Request = request;
    }
}
