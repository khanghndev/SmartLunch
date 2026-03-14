using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.MenuSuggestions;

namespace SmartLunch.Backend.Service.Application.Queries.MenuSuggestions.GetMenuSuggestions;

public class GetMenuSuggestionsQuery : IRequest<GetMenuSuggestionsResponse>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }

    public GetMenuSuggestionsQuery(int page = 1, int pageSize = 10, string? searchTerm = null)
    {
        Page = page;
        PageSize = pageSize;
        SearchTerm = searchTerm;
    }
}
