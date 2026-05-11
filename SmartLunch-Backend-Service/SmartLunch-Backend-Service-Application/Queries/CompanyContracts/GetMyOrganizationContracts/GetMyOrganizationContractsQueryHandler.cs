using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Contracts;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.CompanyContracts.GetMyOrganizationContracts;

public class GetMyOrganizationContractsQueryHandler
    : IRequestHandler<GetMyOrganizationContractsQuery, GetMyOrganizationContractsResponse>
{
    private readonly IContractRepository _contractRepository;
    private readonly IUserOrganizationRepository _userOrganizationRepository;

    public GetMyOrganizationContractsQueryHandler(
        IContractRepository contractRepository,
        IUserOrganizationRepository userOrganizationRepository)
    {
        _contractRepository = contractRepository;
        _userOrganizationRepository = userOrganizationRepository;
    }

    public async Task<GetMyOrganizationContractsResponse> Handle(
        GetMyOrganizationContractsQuery request,
        CancellationToken cancellationToken)
    {
        var memberships = await _userOrganizationRepository.GetActiveByUserIdAsync(request.UserId);
        var orgIds = memberships.Select(m => m.OrganizationId).Distinct().ToList();
        if (orgIds.Count == 0)
            return new GetMyOrganizationContractsResponse();

        var contracts = await _contractRepository.GetByOrganizationIdsAsync(orgIds, cancellationToken);
        return new GetMyOrganizationContractsResponse
        {
            Contracts = contracts.Select(ContractDtoMapping.ToDto).ToList(),
        };
    }
}
