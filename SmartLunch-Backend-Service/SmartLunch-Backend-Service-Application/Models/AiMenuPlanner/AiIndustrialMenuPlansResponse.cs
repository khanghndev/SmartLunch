using System.Text.Json.Serialization;

namespace SmartLunch.Backend.Service.Application.Models.AiMenuPlanner;

public class AiIndustrialMenuPlansResponse
{
    [JsonPropertyName("plans")]
    public List<AiIndustrialMenuPlan> Plans { get; set; } = new();
}

public class AiIndustrialMenuPlan
{
    [JsonPropertyName("rank")]
    public int Rank { get; set; }

    [JsonPropertyName("plan_score")]
    public decimal PlanScore { get; set; }

    [JsonPropertyName("objective_value")]
    public decimal ObjectiveValue { get; set; }

    [JsonPropertyName("week_menu")]
    public List<AiIndustrialDayMenu> WeekMenu { get; set; } = new();
}

public class AiIndustrialDayMenu
{
    [JsonPropertyName("day")]
    public string Day { get; set; } = string.Empty;

    [JsonPropertyName("dishes")]
    public List<AiDishRecommendation> Dishes { get; set; } = new();
}

public class AiDishRecommendation
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    [JsonPropertyName("score")]
    public decimal Score { get; set; }

    [JsonPropertyName("reasons")]
    public List<string> Reasons { get; set; } = new();

    [JsonPropertyName("cost_per_serving")]
    public decimal CostPerServing { get; set; }
}

