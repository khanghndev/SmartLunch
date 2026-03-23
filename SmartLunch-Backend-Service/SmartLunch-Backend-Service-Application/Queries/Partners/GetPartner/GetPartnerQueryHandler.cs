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
        if (request.IncludeContracts)
        {
            var partner = await _partnerRepository.GetByIdWithContractsAsync(request.PartnerId);
            if (partner == null)
            {
                _logger.LogWarning("Partner not found with ID: {PartnerId}", request.PartnerId);
                return new GetPartnerResponse { Partner = new PartnerDto() };
            }

            var contracts = partner.Contracts
                .OrderByDescending(c => c.StartDate)
                .Select(PartnerDtoMapping.ToContractSummary)
                .ToList();

            return new GetPartnerResponse
            {
                Partner = PartnerDtoMapping.ToDto(partner),
                Contracts = contracts
            };
        }

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
                    Partner = PartnerDtoMapping.ToDto(partner)
                };
            },
            CacheDuration,
            cancellationToken);
    }
}
