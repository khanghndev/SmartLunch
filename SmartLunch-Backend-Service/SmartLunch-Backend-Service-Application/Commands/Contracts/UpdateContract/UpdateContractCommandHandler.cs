using MediatR;
using SmartLunch.Backend.Service.Application.Common.Caching;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Contracts;
using SmartLunch.Backend.Service.Application.Helpers;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Contracts.UpdateContract;

public class UpdateContractCommandHandler : IRequestHandler<UpdateContractCommand, GetContractResponse>
{
    private readonly IContractRepository _contractRepository;
    private readonly ICacheService _cacheService;

    public UpdateContractCommandHandler(IContractRepository contractRepository, ICacheService cacheService)
    {
        _contractRepository = contractRepository;
        _cacheService = cacheService;
    }

    public async Task<GetContractResponse> Handle(UpdateContractCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;
        ValidateDateRange(req.StartDate, req.EndDate);
        var status = NormalizeStatus(req.Status);

        var entity = await _contractRepository.GetByIdAsync(request.ContractId);
        if (entity == null)
            throw new KeyNotFoundException($"Contract with ID {request.ContractId} was not found.");

        if (!string.IsNullOrWhiteSpace(req.ContractNumber) &&
            await _contractRepository.ExistsContractNumberForPartnerAsync(
                entity.PartnerId,
                req.ContractNumber.Trim(),
                request.ContractId))
            throw new InvalidOperationException(
                $"Contract number '{req.ContractNumber.Trim()}' already exists for this partner.");

        entity.ContractNumber = string.IsNullOrWhiteSpace(req.ContractNumber) ? null : req.ContractNumber.Trim();
        entity.Description = string.IsNullOrWhiteSpace(req.Description) ? null : req.Description.Trim();
        entity.SupplySchedule = string.IsNullOrWhiteSpace(req.SupplySchedule) ? null : req.SupplySchedule.Trim();
        entity.StartDate = req.StartDate;
        entity.EndDate = req.EndDate;
        entity.TotalValue = req.TotalValue;
        entity.DepositAmount = req.DepositAmount;
        entity.Status = status;
        entity.UpdatedAt = DateTime.UtcNow;

        ContractLifecycleHelper.ApplyExpiryByEndDate(entity);

        await _contractRepository.UpdateAsync(entity);
        await _cacheService.RemoveAsync(MasterDataCacheKeys.Partner(entity.PartnerId), cancellationToken);

        var reloaded = await _contractRepository.GetByIdAsync(entity.Id);
        return new GetContractResponse { Contract = ContractDtoMapping.ToDto(reloaded ?? entity) };
    }

    private static void ValidateDateRange(DateTime start, DateTime? end)
    {
        if (end.HasValue && end.Value.Date < start.Date)
            throw new ArgumentException("EndDate cannot be before StartDate.");
    }

    private static string NormalizeStatus(string status)
    {
        var s = (status ?? ContractStatus.Active).Trim().ToLowerInvariant();
        if (s is not (ContractStatus.Active or ContractStatus.Expired or ContractStatus.Cancelled))
            throw new ArgumentException("Status must be active, expired, or cancelled.");
        return s;
    }
}
