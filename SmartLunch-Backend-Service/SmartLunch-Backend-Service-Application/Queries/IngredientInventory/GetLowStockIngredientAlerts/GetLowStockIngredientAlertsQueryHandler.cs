using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.IngredientInventory;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.IngredientInventory.GetLowStockIngredientAlerts;

public class GetLowStockIngredientAlertsQueryHandler
    : IRequestHandler<GetLowStockIngredientAlertsQuery, GetLowStockIngredientAlertsResponse>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly ILogger<GetLowStockIngredientAlertsQueryHandler> _logger;

    public GetLowStockIngredientAlertsQueryHandler(
        IInventoryRepository inventoryRepository,
        ILogger<GetLowStockIngredientAlertsQueryHandler> logger)
    {
        _inventoryRepository = inventoryRepository;
        _logger = logger;
    }

    public async Task<GetLowStockIngredientAlertsResponse> Handle(
        GetLowStockIngredientAlertsQuery request,
        CancellationToken cancellationToken)
    {
        var rows = await _inventoryRepository.GetLowStockForActiveIngredientsAsync(cancellationToken);

        var alerts = rows.Select(i =>
        {
            var reorder = i.ReorderLevel ?? 0m;
            return new LowStockIngredientAlertDto
            {
                IngredientId = i.IngredientId,
                IngredientName = i.Ingredient.Name,
                Unit = i.Ingredient.Unit,
                QuantityAvailable = i.QuantityAvailable,
                ReorderLevel = i.ReorderLevel,
                Shortage = reorder > i.QuantityAvailable ? reorder - i.QuantityAvailable : 0m,
                LastUpdated = i.LastUpdated
            };
        }).ToList();

        _logger.LogInformation("Low stock alerts: {Count} ingredients", alerts.Count);

        return new GetLowStockIngredientAlertsResponse
        {
            Alerts = alerts,
            TotalCount = alerts.Count
        };
    }
}
