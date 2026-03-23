using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.IngredientSources;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.IngredientSources.GetIngredientSource;

public class GetIngredientSourceQueryHandler : IRequestHandler<GetIngredientSourceQuery, GetIngredientSourceResponse>
{
    private readonly IIngredientSourceRepository _ingredientSourceRepository;
    private readonly ILogger<GetIngredientSourceQueryHandler> _logger;

    public GetIngredientSourceQueryHandler(IIngredientSourceRepository ingredientSourceRepository, ILogger<GetIngredientSourceQueryHandler> logger)
    {
        _ingredientSourceRepository = ingredientSourceRepository;
        _logger = logger;
    }

    public async Task<GetIngredientSourceResponse> Handle(GetIngredientSourceQuery request, CancellationToken cancellationToken)
    {
        var ingredientSource = await _ingredientSourceRepository.GetByIdAsync(request.IngredientSourceId);

        if (ingredientSource == null)
        {
            _logger.LogWarning("IngredientSource not found with ID: {IngredientSourceId}", request.IngredientSourceId);
            return new GetIngredientSourceResponse { IngredientSource = new IngredientSourceDto() };
        }

        return new GetIngredientSourceResponse
        {
            IngredientSource = IngredientSourceDtoMapping.ToDto(ingredientSource)
        };
    }
}
