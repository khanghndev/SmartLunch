using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Ingredients;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.Ingredients.GetIngredients;

public class GetIngredientsQueryHandler : IRequestHandler<GetIngredientsQuery, GetIngredientsResponse>
{
    private readonly IIngredientRepository _ingredientRepository;
    private readonly ILogger<GetIngredientsQueryHandler> _logger;

    public GetIngredientsQueryHandler(IIngredientRepository ingredientRepository, ILogger<GetIngredientsQueryHandler> logger)
    {
        _ingredientRepository = ingredientRepository;
        _logger = logger;
    }

    public async Task<GetIngredientsResponse> Handle(GetIngredientsQuery request, CancellationToken cancellationToken)
    {
        var (ingredients, totalCount) = await _ingredientRepository.GetIngredientsAsync(
            request.Page,
            request.PageSize,
            request.SearchTerm,
            request.IsActive);

        var ingredientDtos = ingredients.Select(IngredientDtoMapping.ToDto).ToList();

        _logger.LogInformation("Retrieved {Count} ingredients (Page {Page}, PageSize {PageSize})",
            ingredientDtos.Count, request.Page, request.PageSize);

        return new GetIngredientsResponse
        {
            Data = ingredientDtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
