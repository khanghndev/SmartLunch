namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.MenuSuggestions;

public class CreateMenuSuggestionRequest
{
    public DateTime WeekStart { get; set; }
    public string SuggestionText { get; set; } = string.Empty;
    public string? AlgorithmVersion { get; set; }
}

