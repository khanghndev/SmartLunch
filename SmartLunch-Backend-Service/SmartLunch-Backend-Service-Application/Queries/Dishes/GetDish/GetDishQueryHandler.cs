using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.Common.Caching;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Dishes;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.Dishes.GetDish;

public class GetDishQueryHandler : IRequestHandler<GetDishQuery, GetDishResponse>
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);
    private readonly ICacheService _cacheService;
    private readonly IDishRepository _dishRepository;
    private readonly ILogger<GetDishQueryHandler> _logger;

    public GetDishQueryHandler(
        ICacheService cacheService,
        IDishRepository dishRepository,
        ILogger<GetDishQueryHandler> logger)
    {
        _cacheService = cacheService;
        _dishRepository = dishRepository;
        _logger = logger;
    }

    public async Task<GetDishResponse> Handle(GetDishQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = MasterDataCacheKeys.Dish(request.DishId);

        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            async _ =>
            {
                var dish = await _dishRepository.GetByIdAsync(request.DishId);

                if (dish == null)
                {
                    _logger.LogWarning("Dish not found with ID: {DishId}", request.DishId);
                    return new GetDishResponse { Dish = new DishDto() };
                }

                return new GetDishResponse
                {
                    Dish = new DishDto
                    {
                        Id = dish.Id,
                        Name = dish.Name,
                        Description = dish.Description,
                        Category = dish.Category,
                        Price = dish.Price,
                        DietaryLabel = dish.DietaryLabel,
                        IsActive = dish.IsActive,
                        CreatedAt = dish.CreatedAt,
                        UpdatedAt = dish.UpdatedAt
                    }
                };
            },
            CacheDuration,
            cancellationToken);
    }
}
