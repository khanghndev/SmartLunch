namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.MenuSuggestions;

public class MenuSuggestionDto
{
    public int Id { get; set; }
    public DateTime WeekStart { get; set; }
    public DateTime GeneratedAt { get; set; }
    public string SuggestionText { get; set; } = string.Empty;
    public string? AlgorithmVersion { get; set; }
    public int? CreatedBy { get; set; }
}
