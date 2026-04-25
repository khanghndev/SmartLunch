using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.IngredientSources;

namespace SmartLunch.Backend.Service.Application.Queries.IngredientSources.GetIngredientSources;

public class GetIngredientSourcesQuery : IRequest<GetIngredientSourcesResponse>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public int? PartnerId { get; set; }
    public int? IngredientId { get; set; }

    public GetIngredientSourcesQuery(
        int page = 1,
        int pageSize = 10,
        string? searchTerm = null,
        int? partnerId = null,
        int? ingredientId = null)
    {
        Page = page;
        PageSize = pageSize;
        SearchTerm = searchTerm;
        PartnerId = partnerId;
        IngredientId = ingredientId;
    }
}
