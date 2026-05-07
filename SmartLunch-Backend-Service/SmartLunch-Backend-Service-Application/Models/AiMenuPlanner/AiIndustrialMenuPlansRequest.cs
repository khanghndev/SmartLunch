using System.Text.Json.Serialization;

namespace SmartLunch.Backend.Service.Application.Models.AiMenuPlanner;

public class AiIndustrialMenuPlansRequest
{
    [JsonPropertyName("budget_per_serving")]
    public decimal BudgetPerServing { get; set; }

    [JsonPropertyName("days")]
    public List<string> Days { get; set; } = new();

    [JsonPropertyName("meal_structure")]
    public List<string> MealStructure { get; set; } = new();

    [JsonPropertyName("top_k")]
    public int TopK { get; set; } = 3;

    [JsonPropertyName("time_limit_seconds")]
    public decimal TimeLimitSeconds { get; set; } = 5;

    [JsonPropertyName("dishes")]
    public List<AiIndustrialDish> Dishes { get; set; } = new();

    [JsonPropertyName("available_ingredients")]
    public List<AiAvailableIngredient> AvailableIngredients { get; set; } = new();

    [JsonPropertyName("constraints")]
    public AiConstraintsOverride? Constraints { get; set; }

    [JsonPropertyName("rules_key")]
    public string RulesKey { get; set; } = "industrial";

    [JsonPropertyName("ingredient_groups")]
    public Dictionary<string, List<string>> IngredientGroups { get; set; } = new();
}

public class AiConstraintsOverride
{
    [JsonPropertyName("group_frequencies")]
    public List<AiGroupFrequencyConstraint> GroupFrequencies { get; set; } = new();

    [JsonPropertyName("no_repeat_main_ingredient_consecutive_days")]
    public bool NoRepeatMainIngredientConsecutiveDays { get; set; } = true;

    [JsonPropertyName("alternate_cooking_methods")]
    public bool AlternateCookingMethods { get; set; } = true;

    [JsonPropertyName("prefer_ingredient_reuse")]
    public bool PreferIngredientReuse { get; set; } = true;

    [JsonPropertyName("max_consecutive_same_main_dish")]
    public int MaxConsecutiveSameMainDish { get; set; } = 3;
}

public class AiGroupFrequencyConstraint
{
    [JsonPropertyName("group_name")]
    public string GroupName { get; set; } = string.Empty;

    [JsonPropertyName("min_count")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? MinCount { get; set; }

    [JsonPropertyName("max_count")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? MaxCount { get; set; }
}

public class AiIndustrialDish
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("name_english")]
    public string NameEnglish { get; set; } = string.Empty;

    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    [JsonPropertyName("covers_categories")]
    public List<string> CoversCategories { get; set; } = new();

    [JsonPropertyName("main_ingredient")]
    public string MainIngredient { get; set; } = string.Empty;

    [JsonPropertyName("sub_ingredients")]
    public List<string> SubIngredients { get; set; } = new();

    [JsonPropertyName("cooking_method")]
    public string CookingMethod { get; set; } = string.Empty;

    [JsonPropertyName("cost_per_serving")]
    public decimal CostPerServing { get; set; }

    [JsonPropertyName("popularity")]
    public int Popularity { get; set; } = 3;

    [JsonPropertyName("max_per_week")]
    public int MaxPerWeek { get; set; } = 2;

    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = new();
}

public class AiAvailableIngredient
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("quantity_kg")]
    public decimal QuantityKg { get; set; }
}
