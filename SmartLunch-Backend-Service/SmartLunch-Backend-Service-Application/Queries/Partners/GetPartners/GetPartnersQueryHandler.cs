using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.Common.Caching;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Partners;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.Partners.GetPartners;

public class GetPartnersQueryHandler : IRequestHandler<GetPartnersQuery, GetPartnersResponse>
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);
    private readonly ICacheService _cacheService;
    private readonly IPartnerRepository _partnerRepository;
    private readonly ILogger<GetPartnersQueryHandler> _logger;

    public GetPartnersQueryHandler(
        ICacheService cacheService,
        IPartnerRepository partnerRepository,
        ILogger<GetPartnersQueryHandler> logger)
    {
        _cacheService = cacheService;
        _partnerRepository = partnerRepository;
        _logger = logger;
    }

    public async Task<GetPartnersResponse> Handle(GetPartnersQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = MasterDataCacheKeys.Partners(
            request.Page,
            request.PageSize,
            request.SearchTerm,
            request.IsActive);

        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            async _ =>
            {
                var (partners, totalCount) = await _partnerRepository.GetPartnersAsync(
                    request.Page,
                    request.PageSize,
                    request.SearchTerm,
                    request.IsActive);

                var partnerDtos = partners.Select(p => new PartnerDto
                {
                    Id = p.Id,
                    LegalName = p.LegalName,
                    TaxId = p.TaxId,
                    Address = p.Address,
                    ContactPerson = p.ContactPerson,
                    Phone = p.Phone,
                    Email = p.Email,
                    PerformanceRating = p.PerformanceRating,
                    ComplianceInfo = p.ComplianceInfo,
                    FinancialTerms = p.FinancialTerms,
                    IsActive = p.IsActive,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                }).ToList();

                _logger.LogInformation("Retrieved {Count} partners (Page {Page}, PageSize {PageSize})",
                    partnerDtos.Count, request.Page, request.PageSize);

                return new GetPartnersResponse
                {
                    Data = partnerDtos,
                    TotalCount = totalCount,
                    Page = request.Page,
                    PageSize = request.PageSize
                };
            },
            CacheDuration,
            cancellationToken);
    }
}
