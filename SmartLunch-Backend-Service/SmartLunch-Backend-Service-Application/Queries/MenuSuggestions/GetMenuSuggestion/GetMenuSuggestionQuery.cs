using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.MenuSuggestions;

namespace SmartLunch.Backend.Service.Application.Queries.MenuSuggestions.GetMenuSuggestion;

public class GetMenuSuggestionQuery : IRequest<GetMenuSuggestionResponse>
{
    public int MenuSuggestionId { get; set; }

    public GetMenuSuggestionQuery(int menuSuggestionId)
    {
        MenuSuggestionId = menuSuggestionId;
    }
}
