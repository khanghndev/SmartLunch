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
        var ingredient = await _ingredientRepository.GetByIdAsync(request.IngredientId);

        if (ingredient == null)
        {
            _logger.LogWarning("Ingredient not found with ID: {IngredientId}", request.IngredientId);
            return new GetIngredientResponse { Ingredient = new IngredientDto() };
        }

        return new GetIngredientResponse
        {
            Ingredient = new IngredientDto
            {
                Id = ingredient.Id,
                Name = ingredient.Name,
                Unit = ingredient.Unit,
                Description = ingredient.Description,
                DefaultSupplierId = ingredient.DefaultSupplierId,
                CostPerUnit = ingredient.CostPerUnit,
                IsActive = ingredient.IsActive,
                CreatedAt = ingredient.CreatedAt,
                UpdatedAt = ingredient.UpdatedAt
            }
        };
    }
}
