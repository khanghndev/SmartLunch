using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Ingredients;

namespace SmartLunch.Backend.Service.Application.Queries.Ingredients.GetIngredients;

public class GetIngredientsQuery : IRequest<GetIngredientsResponse>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public bool? IsActive { get; set; }

    public GetIngredientsQuery(int page = 1, int pageSize = 10, string? searchTerm = null, bool? isActive = null)
    {
        Page = page;
        PageSize = pageSize;
        SearchTerm = searchTerm;
        IsActive = isActive;
    }
}
