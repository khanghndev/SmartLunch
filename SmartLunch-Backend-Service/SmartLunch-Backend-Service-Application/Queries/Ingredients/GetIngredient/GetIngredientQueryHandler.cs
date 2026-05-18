using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Ingredients;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.Ingredients.GetIngredient;

public class GetIngredientQueryHandler : IRequestHandler<GetIngredientQuery, GetIngredientResponse>
{
    private readonly IIngredientRepository _ingredientRepository;
    private readonly ILogger<GetIngredientQueryHandler> _logger;

    public GetIngredientQueryHandler(IIngredientRepository ingredientRepository, ILogger<GetIngredientQueryHandler> logger)
    {
        _ingredientRepository = ingredientRepository;
        _logger = logger;
    }

    public async Task<GetIngredientResponse> Handle(GetIngredientQuery request, CancellationToken cancellationToken)
    {
        var ingredient = await _ingredientRepository.GetByIdWithDetailsAsync(request.IngredientId, cancellationToken);

        if (ingredient == null)
        {
            _logger.LogWarning("Ingredient not found with ID: {IngredientId}", request.IngredientId);
            throw new KeyNotFoundException($"Ingredient with ID {request.IngredientId} was not found.");
        }

        return new GetIngredientResponse
        {
            Ingredient = IngredientDtoMapping.ToDto(ingredient),
        };
    }
}
