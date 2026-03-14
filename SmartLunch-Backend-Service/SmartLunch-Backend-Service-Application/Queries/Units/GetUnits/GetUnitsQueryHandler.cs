using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.Common.Caching;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Units;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.Units.GetUnits;

public class GetUnitsQueryHandler : IRequestHandler<GetUnitsQuery, GetUnitsResponse>
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);
    private readonly ICacheService _cacheService;
    private readonly IUnitRepository _unitRepository;
    private readonly ILogger<GetUnitsQueryHandler> _logger;

    public GetUnitsQueryHandler(
        ICacheService cacheService,
        IUnitRepository unitRepository,
        ILogger<GetUnitsQueryHandler> logger)
    {
        _cacheService = cacheService;
        _unitRepository = unitRepository;
        _logger = logger;
    }

    public async Task<GetUnitsResponse> Handle(GetUnitsQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = MasterDataCacheKeys.Units(
            request.Page,
            request.PageSize,
            request.SearchTerm,
            request.IsActive);

        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            async _ =>
            {
                var (units, totalCount) = await _unitRepository.GetUnitsAsync(
                    request.Page,
                    request.PageSize,
                    request.SearchTerm,
                    request.IsActive);

                var unitDtos = units.Select(u => new UnitDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Address = u.Address,
                    Phone = u.Phone,
                    ContactPerson = u.ContactPerson,
                    ContactEmail = u.ContactEmail,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                }).ToList();

                _logger.LogInformation("Retrieved {Count} units (Page {Page}, PageSize {PageSize})",
                    unitDtos.Count, request.Page, request.PageSize);

                return new GetUnitsResponse
                {
                    Data = unitDtos,
                    TotalCount = totalCount,
                    Page = request.Page,
                    PageSize = request.PageSize
                };
            },
            CacheDuration,
            cancellationToken);
    }
}
