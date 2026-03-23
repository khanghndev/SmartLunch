using MediatR;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Dishes;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Dishes.CreateDish;

public class CreateDishCommandHandler : IRequestHandler<CreateDishCommand, GetDishResponse>
{
    private readonly IDishRepository _dishRepository;

    public CreateDishCommandHandler(IDishRepository dishRepository)
    {
        _dishRepository = dishRepository;
    }

    public async Task<GetDishResponse> Handle(CreateDishCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;
        if (string.IsNullOrWhiteSpace(req.Name))
            throw new ArgumentException("Name is required.");
        if (!DishCatalogCategory.IsValidOrEmpty(req.Category))
            throw new ArgumentException(
                "Invalid category. Use one of: man, xao, canh, trang_mieng, or leave empty.");
        if (req.Price < 0)
            throw new ArgumentException("Price cannot be negative.");

        var entity = new Dish
        {
            Id = Guid.NewGuid(),
            Name = req.Name.Trim(),
            Description = string.IsNullOrWhiteSpace(req.Description) ? null : req.Description.Trim(),
            Category = DishCatalogCategory.Normalize(req.Category),
            Price = req.Price,
            DietaryLabel = string.IsNullOrWhiteSpace(req.DietaryLabel) ? null : req.DietaryLabel.Trim(),
            IsActive = req.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        await _dishRepository.CreateAsync(entity);

        var reloaded = await _dishRepository.GetByIdWithIngredientsAsync(entity.Id);
        var dish = reloaded ?? entity;
        return new GetDishResponse
        {
            Dish = DishDtoMapping.ToDto(dish),
            IngredientQuotas = reloaded?.DishIngredients.OrderBy(di => di.Ingredient?.Name).Select(DishDtoMapping.ToQuotaDto).ToList()
                ?? new List<DishIngredientQuotaDto>()
        };
    }
}
