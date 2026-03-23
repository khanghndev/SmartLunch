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
        if (request.IncludeIngredientQuotas)
        {
            var dish = await _dishRepository.GetByIdWithIngredientsAsync(request.DishId);
            if (dish == null)
            {
                _logger.LogWarning("Dish not found with ID: {DishId}", request.DishId);
                return new GetDishResponse { Dish = new DishDto() };
            }

            var quotas = dish.DishIngredients
                .OrderBy(di => di.Ingredient?.Name)
                .Select(DishDtoMapping.ToQuotaDto)
                .ToList();

            return new GetDishResponse
            {
                Dish = DishDtoMapping.ToDto(dish),
                IngredientQuotas = quotas
            };
        }

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
                    Dish = DishDtoMapping.ToDto(dish)
                };
            },
            CacheDuration,
            cancellationToken);
    }
}
