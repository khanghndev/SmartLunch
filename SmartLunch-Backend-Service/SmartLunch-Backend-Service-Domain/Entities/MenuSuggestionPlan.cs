namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// A ranked plan (top-K) within a menu suggestion run.
/// </summary>
public class MenuSuggestionPlan
{
    public int Id { get; set; }
    public int MenuSuggestionId { get; set; }
    public int Rank { get; set; }
    public decimal PlanScore { get; set; }
    public decimal ObjectiveValue { get; set; }

    public virtual MenuSuggestion? MenuSuggestion { get; set; }
    public virtual ICollection<MenuSuggestionPlanDay> Days { get; set; } = new List<MenuSuggestionPlanDay>();
}

