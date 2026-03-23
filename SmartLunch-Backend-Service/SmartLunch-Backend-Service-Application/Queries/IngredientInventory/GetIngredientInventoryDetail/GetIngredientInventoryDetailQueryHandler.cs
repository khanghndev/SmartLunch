using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.IngredientInventory;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.IngredientInventory.GetIngredientInventoryDetail;

public class GetIngredientInventoryDetailQueryHandler
    : IRequestHandler<GetIngredientInventoryDetailQuery, GetIngredientInventoryDetailResponse>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IIngredientSourceRepository _ingredientSourceRepository;
    private readonly ILogger<GetIngredientInventoryDetailQueryHandler> _logger;

    public GetIngredientInventoryDetailQueryHandler(
        IInventoryRepository inventoryRepository,
        IIngredientSourceRepository ingredientSourceRepository,
        ILogger<GetIngredientInventoryDetailQueryHandler> logger)
    {
        _inventoryRepository = inventoryRepository;
        _ingredientSourceRepository = ingredientSourceRepository;
        _logger = logger;
    }

    public async Task<GetIngredientInventoryDetailResponse> Handle(
        GetIngredientInventoryDetailQuery request,
        CancellationToken cancellationToken)
    {
        var inv = await _inventoryRepository.GetByIngredientIdWithIngredientAsync(
            request.IngredientId,
            cancellationToken);

        if (inv == null)
            throw new KeyNotFoundException($"No inventory record for ingredient {request.IngredientId}.");

        var ing = inv.Ingredient;
        var take = request.RecentBatchTake is >= 1 and <= 100 ? request.RecentBatchTake : 20;
        var batches = await _ingredientSourceRepository.GetRecentByIngredientIdAsync(
            request.IngredientId,
            take,
            cancellationToken);

        var isLow = inv.ReorderLevel != null && inv.QuantityAvailable <= inv.ReorderLevel;

        _logger.LogInformation("Inventory detail for ingredient {IngredientId}", request.IngredientId);

        return new GetIngredientInventoryDetailResponse
        {
            IngredientId = ing.Id,
            IngredientName = ing.Name,
            Unit = ing.Unit,
            Description = ing.Description,
            IsActive = ing.IsActive,
            CostPerUnit = ing.CostPerUnit,
            DefaultSupplierId = ing.DefaultSupplierId,
            DefaultSupplierLegalName = ing.DefaultSupplier?.LegalName,
            QuantityAvailable = inv.QuantityAvailable,
            ReorderLevel = inv.ReorderLevel,
            IsLowStock = isLow,
            LastUpdated = inv.LastUpdated,
            RecentBatches = batches.Select(b => new IngredientSourceBatchDto
            {
                Id = b.Id,
                BatchNumber = b.BatchNumber,
                OriginDetails = b.OriginDetails,
                ProductionDate = b.ProductionDate,
                ExpirationDate = b.ExpirationDate,
                SupplierLegalName = b.Partner?.LegalName,
                CreatedAt = b.CreatedAt
            }).ToList()
        };
    }
}
