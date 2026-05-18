using MediatR;
using SmartLunch.Backend.Service.Application.Common.Caching;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Partners;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Partners.UpdatePartner;

public class UpdatePartnerCommandHandler : IRequestHandler<UpdatePartnerCommand, GetPartnerResponse>
{
    private readonly IPartnerRepository _partnerRepository;
    private readonly ICacheService _cacheService;

    public UpdatePartnerCommandHandler(IPartnerRepository partnerRepository, ICacheService cacheService)
    {
        _partnerRepository = partnerRepository;
        _cacheService = cacheService;
    }

    public async Task<GetPartnerResponse> Handle(UpdatePartnerCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;
        if (string.IsNullOrWhiteSpace(req.LegalName))
            throw new ArgumentException("LegalName is required.");

        var entity = await _partnerRepository.GetByIdAsync(request.PartnerId);
        if (entity == null)
            throw new KeyNotFoundException($"Partner with ID {request.PartnerId} was not found.");

        if (!string.IsNullOrWhiteSpace(req.TaxId) &&
            await _partnerRepository.ExistsByTaxIdAsync(req.TaxId.Trim(), request.PartnerId))
            throw new InvalidOperationException($"Another partner already uses tax ID '{req.TaxId.Trim()}'.");

        entity.LegalName = req.LegalName.Trim();
        entity.BusinessRegistrationNumber = string.IsNullOrWhiteSpace(req.BusinessRegistrationNumber)
            ? null
            : req.BusinessRegistrationNumber.Trim();
        entity.TaxId = string.IsNullOrWhiteSpace(req.TaxId) ? null : req.TaxId.Trim();
        entity.LegalRepresentative = string.IsNullOrWhiteSpace(req.LegalRepresentative)
            ? null
            : req.LegalRepresentative.Trim();
        entity.Address = req.Address?.Trim();
        entity.ContactPerson = req.ContactPerson?.Trim();
        entity.Phone = req.Phone?.Trim();
        entity.Email = req.Email?.Trim();
        entity.PerformanceRating = req.PerformanceRating;
        entity.ComplianceInfo = req.ComplianceInfo?.Trim();
        entity.FinancialTerms = req.FinancialTerms?.Trim();
        entity.IsActive = req.IsActive;
        entity.UpdatedAt = VietnamTime.Now;

        await _partnerRepository.UpdateAsync(entity);

        await _cacheService.RemoveAsync(MasterDataCacheKeys.Partner(request.PartnerId), cancellationToken);

        var reloaded = await _partnerRepository.GetByIdWithContractsAsync(request.PartnerId);
        if (reloaded == null)
            return new GetPartnerResponse { Partner = PartnerDtoMapping.ToDto(entity) };

        return new GetPartnerResponse
        {
            Partner = PartnerDtoMapping.ToDto(reloaded),
            Contracts = reloaded.Contracts
                .OrderByDescending(c => c.StartDate)
                .Select(PartnerDtoMapping.ToContractSummary)
                .ToList()
        };
    }
}
