using MediatR;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Contracts;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.OrganizationMealContractOrders;

namespace SmartLunch.Backend.Service.Application.Queries.CompanyContracts.GetOrganizationContract;

public class GetOrganizationContractQueryHandler : IRequestHandler<GetOrganizationContractQuery, GetContractResponse>
{
    private readonly IContractRepository _contractRepository;
    private readonly IUserOrganizationRepository _userOrganizationRepository;
    private readonly OrganizationMealContractWeeklySelectionService _weeklySelectionService;
    private readonly IContractWeeklySelectionRepository _weeklyRepository;

    public GetOrganizationContractQueryHandler(
        IContractRepository contractRepository,
        IUserOrganizationRepository userOrganizationRepository,
        OrganizationMealContractWeeklySelectionService weeklySelectionService,
        IContractWeeklySelectionRepository weeklyRepository)
    {
        _contractRepository = contractRepository;
        _userOrganizationRepository = userOrganizationRepository;
        _weeklySelectionService = weeklySelectionService;
        _weeklyRepository = weeklyRepository;
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

        if (OrganizationMealContractTypes.IsPeriodBased(contract.ContractType)
            && contract.SourceOrderId.HasValue
            && contract.EndDate.HasValue)
        {
            var full = await _contractRepository.GetPeriodBasedWithExcludedDatesAsync(contract.Id, cancellationToken)
                ?? contract;
            await _weeklySelectionService.EnsureWeeksSeededAsync(full, cancellationToken);
        }

        var dto = ContractDtoMapping.ToDto(contract);
        if (OrganizationMealContractTypes.IsPeriodBased(contract.ContractType) && contract.SourceOrderId.HasValue)
        {
            var (total, filled) = await _weeklyRepository.GetProgressAsync(contract.Id, cancellationToken);
            dto.TotalServiceWeeks = total;
            dto.FilledServiceWeeks = filled;
        }

        return new GetContractResponse { Contract = dto };
    }
}
