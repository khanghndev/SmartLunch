using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.Common.Caching;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Organizations;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.Organizations.GetOrganizations;

public class GetOrganizationsQueryHandler : IRequestHandler<GetOrganizationsQuery, GetOrganizationsResponse>
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);
    private readonly ICacheService _cacheService;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly ILogger<GetOrganizationsQueryHandler> _logger;

    public GetOrganizationsQueryHandler(
        ICacheService cacheService,
        IOrganizationRepository organizationRepository,
        ILogger<GetOrganizationsQueryHandler> logger)
    {
        _cacheService = cacheService;
        _organizationRepository = organizationRepository;
        _logger = logger;
    }

    public async Task<GetOrganizationsResponse> Handle(GetOrganizationsQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = MasterDataCacheKeys.Organizations(
            request.Page,
            request.PageSize,
            request.SearchTerm,
            request.IsActive);

        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            async _ =>
            {
                var (organizations, totalCount) = await _organizationRepository.GetOrganizationsAsync(
                    request.Page,
                    request.PageSize,
                    request.SearchTerm,
                    request.IsActive);

                var organizationDtos = organizations.Select(u => new OrganizationDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Address = u.Address,
                    Phone = u.Phone,
                    ContactPerson = u.ContactPerson,
                    ContactEmail = u.ContactEmail,
                    Type = u.Type,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                }).ToList();

                _logger.LogInformation("Retrieved {Count} organizations (Page {Page}, PageSize {PageSize})",
                    organizationDtos.Count, request.Page, request.PageSize);

                return new GetOrganizationsResponse
                {
                    Data = organizationDtos,
                    TotalCount = totalCount,
                    Page = request.Page,
                    PageSize = request.PageSize
                };
            },
            CacheDuration,
            cancellationToken);
    }
}
