using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.IngredientSources;

namespace SmartLunch.Backend.Service.Application.Queries.IngredientSources.GetIngredientSources;

public class GetIngredientSourcesQuery : IRequest<GetIngredientSourcesResponse>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public Guid? PartnerId { get; set; }
    public Guid? IngredientId { get; set; }

    public GetIngredientSourcesQuery(
        int page = 1,
        int pageSize = 10,
        string? searchTerm = null,
        Guid? partnerId = null,
        Guid? ingredientId = null)
    {
        Page = page;
        PageSize = pageSize;
        SearchTerm = searchTerm;
        PartnerId = partnerId;
        IngredientId = ingredientId;
    }
}
