namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// AI menu suggestion
/// </summary>
public class MenuSuggestion
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public DateTime WeekStart { get; set; }
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public string SuggestionText { get; set; } = string.Empty;
    public string? AlgorithmVersion { get; set; }
    public int? CreatedBy { get; set; }

    public virtual User? CreatedByUser { get; set; }
}
