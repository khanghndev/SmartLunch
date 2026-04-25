using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Contracts;

namespace SmartLunch.Backend.Service.Application.Queries.Contracts.GetContract;

public class GetContractQuery : IRequest<GetContractResponse>
{
    public int ContractId { get; set; }

    public GetContractQuery(int contractId)
    {
        ContractId = contractId;
    }
}
