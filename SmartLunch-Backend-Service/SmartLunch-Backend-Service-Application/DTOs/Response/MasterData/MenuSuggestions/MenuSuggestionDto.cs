namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.MenuSuggestions;

public class MenuSuggestionDto
{
    public Guid Id { get; set; }
    public DateTime WeekStart { get; set; }
    public DateTime GeneratedAt { get; set; }
    public string SuggestionText { get; set; } = string.Empty;
    public string? AlgorithmVersion { get; set; }
    public Guid? CreatedBy { get; set; }
}
