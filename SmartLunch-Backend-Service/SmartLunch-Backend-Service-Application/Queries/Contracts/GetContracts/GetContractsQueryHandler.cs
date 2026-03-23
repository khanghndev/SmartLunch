using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Contracts;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.Contracts.GetContracts;

public class GetContractsQueryHandler : IRequestHandler<GetContractsQuery, GetContractsResponse>
{
    private readonly IContractRepository _contractRepository;
    private readonly ILogger<GetContractsQueryHandler> _logger;

    public GetContractsQueryHandler(IContractRepository contractRepository, ILogger<GetContractsQueryHandler> logger)
    {
        _contractRepository = contractRepository;
        _logger = logger;
    }

    public async Task<GetContractsResponse> Handle(GetContractsQuery request, CancellationToken cancellationToken)
    {
        var (contracts, totalCount) = await _contractRepository.GetContractsAsync(
            request.Page,
            request.PageSize,
            request.SearchTerm,
            request.PartnerId);

        var contractDtos = contracts.Select(ContractDtoMapping.ToDto).ToList();

        _logger.LogInformation("Retrieved {Count} contracts (Page {Page}, PageSize {PageSize})",
            contractDtos.Count, request.Page, request.PageSize);

        return new GetContractsResponse
        {
            Data = contractDtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
