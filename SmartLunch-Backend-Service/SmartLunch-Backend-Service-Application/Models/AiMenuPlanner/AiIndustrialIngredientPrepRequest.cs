using System.Text.Json.Serialization;

namespace SmartLunch.Backend.Service.Application.Models.AiMenuPlanner;

public sealed class AiIndustrialIngredientPrepRequest
{
    [JsonPropertyName("start_date")]
    public string StartDate { get; set; } = string.Empty; // yyyy-MM-dd

    [JsonPropertyName("days")]
    public int Days { get; set; } = 7;

    [JsonPropertyName("order_items")]
    public List<AiUpcomingOrderItem> OrderItems { get; set; } = new();

    [JsonPropertyName("dish_boms")]
    public List<AiDishBom> DishBoms { get; set; } = new();

    [JsonPropertyName("available_ingredients")]
    public List<AiAvailableIngredient> AvailableIngredients { get; set; } = new();

    [JsonPropertyName("constraints")]
    public AiIngredientPrepConstraints Constraints { get; set; } = new();
}

public sealed class AiUpcomingOrderItem
{
    [JsonPropertyName("service_date")]
    public string ServiceDate { get; set; } = string.Empty; // yyyy-MM-dd

    [JsonPropertyName("dish_id")]
    public int DishId { get; set; }

    [JsonPropertyName("quantity_meals")]
    public int QuantityMeals { get; set; }
}

public sealed class AiIngredientBomLine
{
    [JsonPropertyName("ingredient_name")]
    public string IngredientName { get; set; } = string.Empty; // NameEnglish preferred

    [JsonPropertyName("quantity_kg_per_meal")]
    public decimal QuantityKgPerMeal { get; set; }

    [JsonPropertyName("cost_per_kg")]
    public decimal? CostPerKg { get; set; }
}

public sealed class AiDishBom
{
    [JsonPropertyName("dish_id")]
    public int DishId { get; set; }

    [JsonPropertyName("dish_name")]
    public string DishName { get; set; } = string.Empty;

    [JsonPropertyName("ingredients")]
    public List<AiIngredientBomLine> Ingredients { get; set; } = new();
}

public sealed class AiIngredientPrepConstraints
{
    [JsonPropertyName("lead_time_days")]
    public int LeadTimeDays { get; set; } = 0;

    [JsonPropertyName("safety_stock_kg")]
    public decimal SafetyStockKg { get; set; } = 0;

    [JsonPropertyName("max_inventory_kg")]
    public decimal? MaxInventoryKg { get; set; }

    [JsonPropertyName("holding_weight")]
    public decimal HoldingWeight { get; set; } = 0.02m;

    [JsonPropertyName("smooth_weight")]
    public decimal SmoothWeight { get; set; } = 0.05m;
}

