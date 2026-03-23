using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Contracts;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Contracts.DeleteContract;

public class DeleteContractCommand : IRequest<DeleteContractResponse>
{
    public Guid ContractId { get; }

    public DeleteContractCommand(Guid contractId)
    {
        ContractId = contractId;
    }
}
