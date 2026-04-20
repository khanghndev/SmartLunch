using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.MenuSuggestions;

namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.MenuSuggestions;

public class CreateMenuSuggestionResponse
{
    public MenuSuggestionDto MenuSuggestion { get; set; } = new();
}

