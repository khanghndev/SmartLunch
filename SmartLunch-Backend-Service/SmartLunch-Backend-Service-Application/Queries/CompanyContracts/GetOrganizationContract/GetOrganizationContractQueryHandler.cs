using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Contracts;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.CompanyContracts.GetOrganizationContract;

public class GetOrganizationContractQueryHandler : IRequestHandler<GetOrganizationContractQuery, GetContractResponse>
{
    private readonly IContractRepository _contractRepository;
    private readonly IUserOrganizationRepository _userOrganizationRepository;

    public GetOrganizationContractQueryHandler(
        IContractRepository contractRepository,
        IUserOrganizationRepository userOrganizationRepository)
    {
        _contractRepository = contractRepository;
        _userOrganizationRepository = userOrganizationRepository;
    }

    public async Task<GetContractResponse> Handle(GetOrganizationContractQuery request, CancellationToken cancellationToken)
    {
        var contract = await _contractRepository.GetPeriodBasedWithExcludedDatesAsync(request.ContractId, cancellationToken)
            ?? await _contractRepository.GetByIdAsync(request.ContractId);
        if (contract == null)
            throw new KeyNotFoundException($"Contract with ID {request.ContractId} was not found.");

        if (!contract.OrganizationId.HasValue)
            throw new UnauthorizedAccessException("This contract is not available for organization self-service.");

        var membership = await _userOrganizationRepository.GetByUserAndOrganizationAsync(
            request.UserId,
            contract.OrganizationId.Value);
        if (membership == null || !membership.IsActive)
            throw new UnauthorizedAccessException("You do not have access to this contract.");

        return new GetContractResponse { Contract = ContractDtoMapping.ToDto(contract) };
    }
}
