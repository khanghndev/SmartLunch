using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Contracts;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Contracts;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Contracts.CreateContract;

public class CreateContractCommand : IRequest<GetContractResponse>
{
    public CreateContractRequest Request { get; }

    public CreateContractCommand(CreateContractRequest request)
    {
        Request = request;
    }
}
