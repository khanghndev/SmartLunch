using MediatR;
using SmartLunch.Backend.Service.Application.Common.Caching;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Contracts;
using SmartLunch.Backend.Service.Application.Helpers;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Contracts.CreateContract;

public class CreateContractCommandHandler : IRequestHandler<CreateContractCommand, GetContractResponse>
{
    private readonly IContractRepository _contractRepository;
    private readonly IPartnerRepository _partnerRepository;
    private readonly ICacheService _cacheService;
    private readonly IContractPdfService _contractPdfService;

    public CreateContractCommandHandler(
        IContractRepository contractRepository,
        IPartnerRepository partnerRepository,
        ICacheService cacheService,
        IContractPdfService contractPdfService)
    {
        _contractRepository = contractRepository;
        _partnerRepository = partnerRepository;
        _cacheService = cacheService;
        _contractPdfService = contractPdfService;
    }

    public async Task<GetContractResponse> Handle(CreateContractCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;
        ValidateDateRange(req.StartDate, req.EndDate);
        var status = NormalizeStatus(req.Status);

        if (await _partnerRepository.GetByIdAsync(req.PartnerId) == null)
            throw new KeyNotFoundException($"Partner with ID {req.PartnerId} was not found.");

        if (!string.IsNullOrWhiteSpace(req.ContractNumber) &&
            await _contractRepository.ExistsContractNumberForPartnerAsync(req.PartnerId, req.ContractNumber.Trim()))
            throw new InvalidOperationException(
                $"Contract number '{req.ContractNumber.Trim()}' already exists for this partner.");

        var entity = new Contract
        {

            PartnerId = req.PartnerId,
            ContractNumber = string.IsNullOrWhiteSpace(req.ContractNumber) ? null : req.ContractNumber.Trim(),
            Description = string.IsNullOrWhiteSpace(req.Description) ? null : req.Description.Trim(),
            SupplySchedule = string.IsNullOrWhiteSpace(req.SupplySchedule) ? null : req.SupplySchedule.Trim(),
            StartDate = req.StartDate,
            EndDate = req.EndDate,
            TotalValue = req.TotalValue,
            DepositAmount = req.DepositAmount,
            Status = status,
            CreatedAt = DateTime.UtcNow
        };

        ContractLifecycleHelper.ApplyExpiryByEndDate(entity);

        await _contractRepository.CreateAsync(entity);
        await _cacheService.RemoveAsync(MasterDataCacheKeys.Partner(req.PartnerId), cancellationToken);

        Contract? reloaded = await _contractRepository.GetByIdAsync(entity.Id);
        if (reloaded?.Partner != null)
        {
            var pdfUrl = await _contractPdfService.GenerateUploadAndResolveUrlAsync(
                reloaded,
                reloaded.Partner,
                buyer: null,
                cancellationToken);
            reloaded.ContractFileUrl = pdfUrl;
            reloaded.UpdatedAt = DateTime.UtcNow;
            await _contractRepository.UpdateAsync(reloaded);
            reloaded = await _contractRepository.GetByIdAsync(entity.Id);
        }

        return new GetContractResponse
        {
            Contract = reloaded != null ? ContractDtoMapping.ToDto(reloaded) : ContractDtoMapping.ToDto(entity)
        };
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
