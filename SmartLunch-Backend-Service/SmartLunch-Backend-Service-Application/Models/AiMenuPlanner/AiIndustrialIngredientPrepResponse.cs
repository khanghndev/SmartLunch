using System.Text.Json.Serialization;

namespace SmartLunch.Backend.Service.Application.Models.AiMenuPlanner;

public sealed class AiIndustrialIngredientPrepResponse
{
    [JsonPropertyName("start_date")]
    public string StartDate { get; set; } = string.Empty;

    [JsonPropertyName("days")]
    public int Days { get; set; }

    [JsonPropertyName("summary_by_day")]
    public List<AiIngredientPrepPlanDay> SummaryByDay { get; set; } = new();

    [JsonPropertyName("ingredients")]
    public List<AiIngredientPrepItem> Ingredients { get; set; } = new();
}

public sealed class AiIngredientPrepPlanDay
{
    [JsonPropertyName("date")]
    public string Date { get; set; } = string.Empty;

    [JsonPropertyName("total_demand_kg")]
    public decimal TotalDemandKg { get; set; }

    [JsonPropertyName("total_buy_kg")]
    public decimal TotalBuyKg { get; set; }

    [JsonPropertyName("total_end_inventory_kg")]
    public decimal TotalEndInventoryKg { get; set; }
}

public sealed class AiIngredientPrepItem
{
    [JsonPropertyName("ingredient_name")]
    public string IngredientName { get; set; } = string.Empty;

    [JsonPropertyName("unit")]
    public string Unit { get; set; } = "kg";

    [JsonPropertyName("total_demand_kg")]
    public decimal TotalDemandKg { get; set; }

    [JsonPropertyName("total_buy_kg")]
    public decimal TotalBuyKg { get; set; }

    [JsonPropertyName("start_inventory_kg")]
    public decimal StartInventoryKg { get; set; }

    [JsonPropertyName("end_inventory_kg")]
    public decimal EndInventoryKg { get; set; }

    [JsonPropertyName("daily")]
    public List<AiIngredientPrepDailyRow> Daily { get; set; } = new();
}

public sealed class AiIngredientPrepDailyRow
{
    [JsonPropertyName("date")]
    public string Date { get; set; } = string.Empty;

    [JsonPropertyName("demand_kg")]
    public decimal DemandKg { get; set; }

    [JsonPropertyName("buy_kg")]
    public decimal BuyKg { get; set; }

    [JsonPropertyName("end_inventory_kg")]
    public decimal EndInventoryKg { get; set; }
}

