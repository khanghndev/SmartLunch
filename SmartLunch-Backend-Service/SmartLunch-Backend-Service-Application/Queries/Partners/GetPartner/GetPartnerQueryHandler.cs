using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.Common.Caching;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Partners;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.Partners.GetPartner;

public class GetPartnerQueryHandler : IRequestHandler<GetPartnerQuery, GetPartnerResponse>
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);
    private readonly ICacheService _cacheService;
    private readonly IPartnerRepository _partnerRepository;
    private readonly ILogger<GetPartnerQueryHandler> _logger;

    public GetPartnerQueryHandler(
        ICacheService cacheService,
        IPartnerRepository partnerRepository,
        ILogger<GetPartnerQueryHandler> logger)
    {
        _cacheService = cacheService;
        _partnerRepository = partnerRepository;
        _logger = logger;
    }

    public async Task<GetPartnerResponse> Handle(GetPartnerQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = MasterDataCacheKeys.Partner(request.PartnerId);

        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            async _ =>
            {
                var partner = await _partnerRepository.GetByIdAsync(request.PartnerId);

                if (partner == null)
                {
                    _logger.LogWarning("Partner not found with ID: {PartnerId}", request.PartnerId);
                    return new GetPartnerResponse { Partner = new PartnerDto() };
                }

                return new GetPartnerResponse
                {
                    Partner = new PartnerDto
                    {
                        Id = partner.Id,
                        LegalName = partner.LegalName,
                        TaxId = partner.TaxId,
                        Address = partner.Address,
                        ContactPerson = partner.ContactPerson,
                        Phone = partner.Phone,
                        Email = partner.Email,
                        PerformanceRating = partner.PerformanceRating,
                        ComplianceInfo = partner.ComplianceInfo,
                        FinancialTerms = partner.FinancialTerms,
                        IsActive = partner.IsActive,
                        CreatedAt = partner.CreatedAt,
                        UpdatedAt = partner.UpdatedAt
                    }
                };
            },
            CacheDuration,
            cancellationToken);
    }
}
