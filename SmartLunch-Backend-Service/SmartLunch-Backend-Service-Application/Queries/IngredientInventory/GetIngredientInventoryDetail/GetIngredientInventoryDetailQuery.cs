using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.IngredientInventory;

namespace SmartLunch.Backend.Service.Application.Queries.IngredientInventory.GetIngredientInventoryDetail;

public class GetIngredientInventoryDetailQuery : IRequest<GetIngredientInventoryDetailResponse>
{
    public int IngredientId { get; }
    public int RecentBatchTake { get; }

    public GetIngredientInventoryDetailQuery(int ingredientId, int recentBatchTake = 20)
    {
        IngredientId = ingredientId;
        RecentBatchTake = recentBatchTake;
    }
}
