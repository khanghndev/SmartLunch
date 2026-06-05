using MediatR;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Contracts;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.OrganizationMealContractOrders;

namespace SmartLunch.Backend.Service.Application.Queries.CompanyContracts.GetMyOrganizationContracts;

public class GetMyOrganizationContractsQueryHandler
    : IRequestHandler<GetMyOrganizationContractsQuery, GetMyOrganizationContractsResponse>
{
    private readonly IContractRepository _contractRepository;
    private readonly IUserOrganizationRepository _userOrganizationRepository;
    private readonly IContractWeeklySelectionRepository _weeklyRepository;
    private readonly OrganizationMealContractWeeklySelectionService _weeklySelectionService;

    public GetMyOrganizationContractsQueryHandler(
        IContractRepository contractRepository,
        IUserOrganizationRepository userOrganizationRepository,
        IContractWeeklySelectionRepository weeklyRepository,
        OrganizationMealContractWeeklySelectionService weeklySelectionService)
    {
        _contractRepository = contractRepository;
        _userOrganizationRepository = userOrganizationRepository;
        _weeklyRepository = weeklyRepository;
        _weeklySelectionService = weeklySelectionService;
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
        var dtos = new List<ContractDto>();

        foreach (var c in contracts)
        {
            if (OrganizationMealContractTypes.IsPeriodBased(c.ContractType)
                && !c.SourceOrderId.HasValue
                && !c.IsDigitallySigned)
            {
                continue;
            }

            var dto = ContractDtoMapping.ToDto(c);

            if (OrganizationMealContractTypes.IsPeriodBased(c.ContractType) && c.SourceOrderId.HasValue)
            {
                var full = await _contractRepository.GetPeriodBasedWithExcludedDatesAsync(c.Id, cancellationToken);
                if (full != null)
                {
                    await _weeklySelectionService.EnsureWeeksSeededAsync(full, cancellationToken);
                    var (total, filled) = await _weeklyRepository.GetProgressAsync(c.Id, cancellationToken);
                    dto.TotalServiceWeeks = total;
                    dto.FilledServiceWeeks = filled;
                }
            }

            dtos.Add(dto);
        }

        return new GetMyOrganizationContractsResponse { Contracts = dtos };
    }
}
