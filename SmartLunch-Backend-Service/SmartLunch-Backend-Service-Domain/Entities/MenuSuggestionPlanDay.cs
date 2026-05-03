namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// A day entry inside a specific suggested plan.
/// </summary>
public class MenuSuggestionPlanDay
{
    public int Id { get; set; }
    public int MenuSuggestionPlanId { get; set; }
    public byte DayIndex { get; set; }
    public string DayName { get; set; } = string.Empty;

    public virtual MenuSuggestionPlan? MenuSuggestionPlan { get; set; }
    public virtual ICollection<MenuSuggestionPlanItem> Items { get; set; } = new List<MenuSuggestionPlanItem>();
}

