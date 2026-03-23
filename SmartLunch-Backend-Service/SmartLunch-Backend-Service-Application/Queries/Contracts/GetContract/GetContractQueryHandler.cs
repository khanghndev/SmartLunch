using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Contracts;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.Contracts.GetContract;

public class GetContractQueryHandler : IRequestHandler<GetContractQuery, GetContractResponse>
{
    private readonly IContractRepository _contractRepository;
    private readonly ILogger<GetContractQueryHandler> _logger;

    public GetContractQueryHandler(IContractRepository contractRepository, ILogger<GetContractQueryHandler> logger)
    {
        _contractRepository = contractRepository;
        _logger = logger;
    }

    public async Task<GetContractResponse> Handle(GetContractQuery request, CancellationToken cancellationToken)
    {
        var contract = await _contractRepository.GetByIdAsync(request.ContractId);

        if (contract == null)
        {
            _logger.LogWarning("Contract not found with ID: {ContractId}", request.ContractId);
            return new GetContractResponse { Contract = new ContractDto() };
        }

        return new GetContractResponse
        {
            Contract = ContractDtoMapping.ToDto(contract)
        };
    }
}
