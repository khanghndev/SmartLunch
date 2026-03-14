using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.MenuSuggestions;

namespace SmartLunch.Backend.Service.Application.Queries.MenuSuggestions.GetMenuSuggestion;

public class GetMenuSuggestionQuery : IRequest<GetMenuSuggestionResponse>
{
    public Guid MenuSuggestionId { get; set; }

    public GetMenuSuggestionQuery(Guid menuSuggestionId)
    {
        MenuSuggestionId = menuSuggestionId;
    }
}
