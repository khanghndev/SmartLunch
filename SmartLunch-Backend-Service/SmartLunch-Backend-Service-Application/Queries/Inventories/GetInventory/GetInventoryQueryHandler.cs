using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Inventories;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.Inventories.GetInventory;

public class GetInventoryQueryHandler : IRequestHandler<GetInventoryQuery, GetInventoryResponse>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly ILogger<GetInventoryQueryHandler> _logger;

    public GetInventoryQueryHandler(IInventoryRepository inventoryRepository, ILogger<GetInventoryQueryHandler> logger)
    {
        _inventoryRepository = inventoryRepository;
        _logger = logger;
    }

    public async Task<GetInventoryResponse> Handle(GetInventoryQuery request, CancellationToken cancellationToken)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(request.InventoryId);

        if (inventory == null)
        {
            _logger.LogWarning("Inventory not found with ID: {InventoryId}", request.InventoryId);
            return new GetInventoryResponse { Inventory = new InventoryDto() };
        }

        return new GetInventoryResponse
        {
            Inventory = new InventoryDto
            {
                IngredientId = inventory.IngredientId,
                QuantityAvailable = inventory.QuantityAvailable,
                ReorderLevel = inventory.ReorderLevel,
                LastUpdated = inventory.LastUpdated
            }
        };
    }
}
