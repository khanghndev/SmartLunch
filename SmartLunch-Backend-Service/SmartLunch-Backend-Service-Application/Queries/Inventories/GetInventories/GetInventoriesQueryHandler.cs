using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Inventories;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.Inventories.GetInventories;

public class GetInventoriesQueryHandler : IRequestHandler<GetInventoriesQuery, GetInventoriesResponse>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly ILogger<GetInventoriesQueryHandler> _logger;

    public GetInventoriesQueryHandler(IInventoryRepository inventoryRepository, ILogger<GetInventoriesQueryHandler> logger)
    {
        _inventoryRepository = inventoryRepository;
        _logger = logger;
    }

    public async Task<GetInventoriesResponse> Handle(GetInventoriesQuery request, CancellationToken cancellationToken)
    {
        var (inventories, totalCount) = await _inventoryRepository.GetInventoriesAsync(
            request.Page,
            request.PageSize,
            request.SearchTerm);

        var inventoryDtos = inventories.Select(inventory => new InventoryDto
        {
                IngredientId = inventory.IngredientId,
                QuantityAvailable = inventory.QuantityAvailable,
                ReorderLevel = inventory.ReorderLevel,
                LastUpdated = inventory.LastUpdated
        }).ToList();

        _logger.LogInformation("Retrieved {Count} inventories (Page {Page}, PageSize {PageSize})",
            inventoryDtos.Count, request.Page, request.PageSize);

        return new GetInventoriesResponse
        {
            Data = inventoryDtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
