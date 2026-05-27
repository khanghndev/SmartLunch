namespace SmartLunch.Backend.Service.Application.DTOs.Request.IngredientIntake;

public sealed class GenerateIngredientPrepFromAiRequest
{
    /// <summary>Planning start date (local calendar), yyyy-MM-dd.</summary>
    public DateOnly StartDate { get; set; }

    /// <summary>Planning horizon.</summary>
    public int Days { get; set; } = 7;

    /// <summary>
    /// If set, only consider orders for this contract. Otherwise uses all upcoming orders.
    /// </summary>
    public int? ContractId { get; set; }

    /// <summary>
    /// If set, overrides dish value tier (BOM) for all dishes.
    /// If null, uses Contract.DishValueId when available; otherwise falls back to the smallest DishValueId found per dish.
    /// </summary>
    public int? DishValueIdOverride { get; set; }

    public int LeadTimeDays { get; set; } = 0;
    public decimal SafetyStockKg { get; set; } = 0;
    public decimal? MaxInventoryKg { get; set; }

    /// <summary>Persist aggregated buy quantities into an IngredientIntakeProposal.</summary>
    public bool PersistAsProposal { get; set; } = true;

    public string? ProposalHeaderNote { get; set; }
}

