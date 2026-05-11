using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.Common.Caching;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Organizations;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.Organizations.GetOrganization;

public class GetOrganizationQueryHandler : IRequestHandler<GetOrganizationQuery, GetOrganizationResponse>
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);
    private readonly ICacheService _cacheService;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly ILogger<GetOrganizationQueryHandler> _logger;

    public GetOrganizationQueryHandler(
        ICacheService cacheService,
        IOrganizationRepository organizationRepository,
        ILogger<GetOrganizationQueryHandler> logger)
    {
        _cacheService = cacheService;
        _organizationRepository = organizationRepository;
        _logger = logger;
    }

    public async Task<GetOrganizationResponse> Handle(GetOrganizationQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = MasterDataCacheKeys.Organization(request.OrganizationId);

        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            async _ =>
            {
                var organization = await _organizationRepository.GetByIdAsync(request.OrganizationId);

                if (organization == null)
                {
                    _logger.LogWarning("Organization not found with ID: {OrganizationId}", request.OrganizationId);
                    throw new KeyNotFoundException($"Organization with ID {request.OrganizationId} was not found.");
                }

                return new GetOrganizationResponse
                {
                    Organization = new OrganizationDto
                    {
                        Id = organization.Id,
                        Code = organization.Code,
                        Name = organization.Name,
                        Address = organization.Address,
                        Phone = organization.Phone,
                        ContactPerson = organization.ContactPerson,
                        ContactEmail = organization.ContactEmail,
                        TaxCode = organization.TaxCode,
                        LegalRepresentative = organization.LegalRepresentative,
                        LogoUrl = organization.LogoUrl,
                        Website = organization.Website,
                        EducationLevel = organization.EducationLevel,
                        Type = organization.Type,
                        IsSubscriptionActive = organization.IsSubscriptionActive,
                        DefaultDailyMeals = organization.DefaultDailyMeals,
                        IsActive = organization.IsActive,
                        CreatedAt = organization.CreatedAt,
                        UpdatedAt = organization.UpdatedAt
                    }
                };
            },
            CacheDuration,
            cancellationToken);
    }
}
