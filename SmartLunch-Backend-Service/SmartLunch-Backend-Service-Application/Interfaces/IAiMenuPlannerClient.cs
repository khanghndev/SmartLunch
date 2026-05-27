using SmartLunch.Backend.Service.Application.Models.AiMenuPlanner;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IAiMenuPlannerClient
{
    Task<AiIndustrialMenuPlansResponse> RecommendIndustrialMenusAsync(
        AiIndustrialMenuPlansRequest request,
        CancellationToken cancellationToken = default);

    Task<AiIndustrialIngredientPrepResponse> RecommendIndustrialIngredientPreparationAsync(
        AiIndustrialIngredientPrepRequest request,
        CancellationToken cancellationToken = default);
}

