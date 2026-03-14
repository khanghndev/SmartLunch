using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.Common.Caching;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Units;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.Units.GetUnit;

public class GetUnitQueryHandler : IRequestHandler<GetUnitQuery, GetUnitResponse>
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);
    private readonly ICacheService _cacheService;
    private readonly IUnitRepository _unitRepository;
    private readonly ILogger<GetUnitQueryHandler> _logger;

    public GetUnitQueryHandler(
        ICacheService cacheService,
        IUnitRepository unitRepository,
        ILogger<GetUnitQueryHandler> logger)
    {
        _cacheService = cacheService;
        _unitRepository = unitRepository;
        _logger = logger;
    }

    public async Task<GetUnitResponse> Handle(GetUnitQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = MasterDataCacheKeys.Unit(request.UnitId);

        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            async _ =>
            {
                var unit = await _unitRepository.GetByIdAsync(request.UnitId);

                if (unit == null)
                {
                    _logger.LogWarning("Unit not found with ID: {UnitId}", request.UnitId);
                    return new GetUnitResponse { Unit = new UnitDto() };
                }

                return new GetUnitResponse
                {
                    Unit = new UnitDto
                    {
                        Id = unit.Id,
                        Name = unit.Name,
                        Address = unit.Address,
                        Phone = unit.Phone,
                        ContactPerson = unit.ContactPerson,
                        ContactEmail = unit.ContactEmail,
                        IsActive = unit.IsActive,
                        CreatedAt = unit.CreatedAt,
                        UpdatedAt = unit.UpdatedAt
                    }
                };
            },
            CacheDuration,
            cancellationToken);
    }
}
