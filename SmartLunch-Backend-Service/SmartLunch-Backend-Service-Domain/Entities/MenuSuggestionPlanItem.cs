namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// A chosen dish for a slot (main/side/soup/dessert) within a plan day.
/// </summary>
public class MenuSuggestionPlanItem
{
    public int Id { get; set; }
    public int MenuSuggestionPlanDayId { get; set; }
    public string SlotCategory { get; set; } = string.Empty;
    public int? DishId { get; set; }
    public string DishName { get; set; } = string.Empty;
    public string? DishSourceCategory { get; set; }
    public decimal Score { get; set; }
    public decimal CostPerServing { get; set; }
    public string? ReasonsJson { get; set; }

    public virtual MenuSuggestionPlanDay? MenuSuggestionPlanDay { get; set; }
}

