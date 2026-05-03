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
    public int Version { get; set; } = 1;
    public string? RulesKey { get; set; }
    public decimal? BudgetPerServing { get; set; }
    public int? TopK { get; set; }
    public decimal? TimeLimitSeconds { get; set; }
    public int? PlanCount { get; set; }
    public string SuggestionText { get; set; } = string.Empty;
    public string? AlgorithmVersion { get; set; }
    public int? CreatedBy { get; set; }

    public virtual User? CreatedByUser { get; set; }
    public virtual ICollection<MenuSuggestionPlan> Plans { get; set; } = new List<MenuSuggestionPlan>();
}
