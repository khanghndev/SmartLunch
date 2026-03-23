using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.IngredientInventory;

namespace SmartLunch.Backend.Service.Application.Queries.IngredientInventory.GetIngredientInventoryDetail;

public class GetIngredientInventoryDetailQuery : IRequest<GetIngredientInventoryDetailResponse>
{
    public Guid IngredientId { get; }
    public int RecentBatchTake { get; }

    public GetIngredientInventoryDetailQuery(Guid ingredientId, int recentBatchTake = 20)
    {
        IngredientId = ingredientId;
        RecentBatchTake = recentBatchTake;
    }
}
