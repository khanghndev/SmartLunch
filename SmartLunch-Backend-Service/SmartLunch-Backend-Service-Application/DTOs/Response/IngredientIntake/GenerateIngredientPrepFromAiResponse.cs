using SmartLunch.Backend.Service.Application.Models.AiMenuPlanner;

namespace SmartLunch.Backend.Service.Application.DTOs.Response.IngredientIntake;

public sealed class GenerateIngredientPrepFromAiResponse
{
    public AiIndustrialIngredientPrepResponse Plan { get; set; } = new();

    /// <summary>Created proposal (optional, only when PersistAsProposal=true).</summary>
    public IngredientIntakeProposalDetailDto? Proposal { get; set; }
}

