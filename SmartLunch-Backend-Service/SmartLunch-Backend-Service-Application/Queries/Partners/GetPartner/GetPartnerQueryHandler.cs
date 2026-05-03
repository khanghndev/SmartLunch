using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.Common.Caching;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Partners;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using SmartLunch.Backend.Service.Application.Interfaces;
using System.Net.Http;

namespace SmartLunch.Backend.Service.Application.Queries.Partners.GetPartner;

public class GetPartnerQueryHandler : IRequestHandler<GetPartnerQuery, GetPartnerResponse>
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);
    private readonly ICacheService _cacheService;
    private readonly IPartnerRepository _partnerRepository;
    private readonly ILogger<GetPartnerQueryHandler> _logger;
    private readonly IStorageService _storage;
    private readonly IConfiguration _configuration;

    public GetPartnerQueryHandler(
        ICacheService cacheService,
        IPartnerRepository partnerRepository,
        ILogger<GetPartnerQueryHandler> logger,
        IStorageService storage,
        IConfiguration configuration)
    {
        _cacheService = cacheService;
        _partnerRepository = partnerRepository;
        _logger = logger;
        _storage = storage;
        _configuration = configuration;
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

            var dto = PartnerDtoMapping.ToDto(partner);
            // Include private documents with signed URLs (do not cache).
            dto.Documents = await BuildDocumentDtosAsync(partner.PartnerDocuments);

            return new GetPartnerResponse
            {
                Partner = dto,
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

    private async Task<List<PartnerDocumentDto>> BuildDocumentDtosAsync(ICollection<SmartLunch.Backend.Service.Domain.Entities.PartnerDocument> docs)
    {
        if (docs == null || docs.Count == 0) return new List<PartnerDocumentDto>();

        var expiresMinutes = int.TryParse(_configuration["Media:DownloadUrlExpireMinutes"], out var m) ? m : 15;
        var expiresIn = TimeSpan.FromMinutes(Math.Clamp(expiresMinutes, 1, 60));

        var result = new List<PartnerDocumentDto>(docs.Count);
        foreach (var d in docs.OrderByDescending(x => x.CreatedAt))
        {
            var objectName = d.MediaFile?.ObjectName ?? string.Empty;
            var url = string.IsNullOrWhiteSpace(objectName)
                ? string.Empty
                : (await _storage.CreateSignedUrlAsync(objectName, HttpMethod.Get, contentType: null, expiresIn: expiresIn)).Url;

            result.Add(new PartnerDocumentDto
            {
                Id = d.Id,
                MediaFileId = d.MediaFileId,
                DocumentType = d.DocumentType,
                IsVerified = d.IsVerified,
                Url = url
            });
        }

        return result;
    }
}
