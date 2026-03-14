using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.IngredientSources;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.IngredientSources.GetIngredientSources;

public class GetIngredientSourcesQueryHandler : IRequestHandler<GetIngredientSourcesQuery, GetIngredientSourcesResponse>
{
    private readonly IIngredientSourceRepository _ingredientSourceRepository;
    private readonly ILogger<GetIngredientSourcesQueryHandler> _logger;

    public GetIngredientSourcesQueryHandler(IIngredientSourceRepository ingredientSourceRepository, ILogger<GetIngredientSourcesQueryHandler> logger)
    {
        _ingredientSourceRepository = ingredientSourceRepository;
        _logger = logger;
    }

    public async Task<GetIngredientSourcesResponse> Handle(GetIngredientSourcesQuery request, CancellationToken cancellationToken)
    {
        var (ingredientSources, totalCount) = await _ingredientSourceRepository.GetIngredientSourcesAsync(
            request.Page,
            request.PageSize,
            request.SearchTerm);

        var ingredientSourceDtos = ingredientSources.Select(ingredientSource => new IngredientSourceDto
        {
                Id = ingredientSource.Id,
                IngredientId = ingredientSource.IngredientId,
                PartnerId = ingredientSource.PartnerId,
                BatchNumber = ingredientSource.BatchNumber,
                OriginDetails = ingredientSource.OriginDetails,
                ProductionDate = ingredientSource.ProductionDate,
                ExpirationDate = ingredientSource.ExpirationDate,
                Certification = ingredientSource.Certification,
                CreatedAt = ingredientSource.CreatedAt
        }).ToList();

        _logger.LogInformation("Retrieved {Count} ingredientsources (Page {Page}, PageSize {PageSize})",
            ingredientSourceDtos.Count, request.Page, request.PageSize);

        return new GetIngredientSourcesResponse
        {
            Data = ingredientSourceDtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
