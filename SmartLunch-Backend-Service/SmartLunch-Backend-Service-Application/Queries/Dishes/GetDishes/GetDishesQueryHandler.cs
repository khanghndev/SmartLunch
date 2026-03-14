using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.Common.Caching;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Dishes;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.Dishes.GetDishes;

public class GetDishesQueryHandler : IRequestHandler<GetDishesQuery, GetDishesResponse>
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);
    private readonly ICacheService _cacheService;
    private readonly IDishRepository _dishRepository;
    private readonly ILogger<GetDishesQueryHandler> _logger;

    public GetDishesQueryHandler(
        ICacheService cacheService,
        IDishRepository dishRepository,
        ILogger<GetDishesQueryHandler> logger)
    {
        _cacheService = cacheService;
        _dishRepository = dishRepository;
        _logger = logger;
    }

    public async Task<GetDishesResponse> Handle(GetDishesQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = MasterDataCacheKeys.Dishes(
            request.Page,
            request.PageSize,
            request.SearchTerm,
            request.IsActive,
            request.Category);

        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            async _ =>
            {
                var (dishes, totalCount) = await _dishRepository.GetDishesAsync(
                    request.Page,
                    request.PageSize,
                    request.SearchTerm,
                    request.IsActive,
                    request.Category);

                var dishDtos = dishes.Select(d => new DishDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description,
                    Category = d.Category,
                    Price = d.Price,
                    DietaryLabel = d.DietaryLabel,
                    IsActive = d.IsActive,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt
                }).ToList();

                _logger.LogInformation("Retrieved {Count} dishes (Page {Page}, PageSize {PageSize})",
                    dishDtos.Count, request.Page, request.PageSize);

                return new GetDishesResponse
                {
                    Data = dishDtos,
                    TotalCount = totalCount,
                    Page = request.Page,
                    PageSize = request.PageSize
                };
            },
            CacheDuration,
            cancellationToken);
    }
}
