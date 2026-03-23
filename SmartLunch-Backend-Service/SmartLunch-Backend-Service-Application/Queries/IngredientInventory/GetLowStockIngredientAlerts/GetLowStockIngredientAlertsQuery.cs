using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.IngredientInventory;

namespace SmartLunch.Backend.Service.Application.Queries.IngredientInventory.GetLowStockIngredientAlerts;

public class GetLowStockIngredientAlertsQuery : IRequest<GetLowStockIngredientAlertsResponse>
{
}
