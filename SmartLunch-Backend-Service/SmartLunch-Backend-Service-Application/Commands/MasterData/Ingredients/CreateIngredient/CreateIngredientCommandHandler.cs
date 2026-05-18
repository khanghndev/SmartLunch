using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Ingredients;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Ingredients;
using SmartLunch.Backend.Service.Application.Helpers;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Ingredients.CreateIngredient;

public class CreateIngredientCommandHandler : IRequestHandler<CreateIngredientCommand, GetIngredientResponse>
{
    private readonly IIngredientRepository _ingredientRepository;
    private readonly IPartnerRepository _partnerRepository;

    public CreateIngredientCommandHandler(
        IIngredientRepository ingredientRepository,
        IPartnerRepository partnerRepository)
    {
        _ingredientRepository = ingredientRepository;
        _partnerRepository = partnerRepository;
    }

    public async Task<GetIngredientResponse> Handle(CreateIngredientCommand command, CancellationToken cancellationToken)
    {
        var req = command.Request;
        IngredientRequestValidation.ValidateCreate(req);

        var name = req.Name.Trim();
        if (await _ingredientRepository.ExistsByNameAsync(name, excludeId: null, cancellationToken))
            throw new ArgumentException($"Ingredient name '{name}' already exists.");

        if (req.CategoryId is int categoryId &&
            !await _ingredientRepository.CategoryExistsAsync(categoryId, cancellationToken))
        {
            throw new KeyNotFoundException($"Ingredient category with ID {categoryId} was not found.");
        }

        if (req.DefaultSupplierId is int supplierId &&
            await _partnerRepository.GetByIdAsync(supplierId) == null)
        {
            throw new KeyNotFoundException($"Partner (supplier) with ID {supplierId} was not found.");
        }

        var entity = new Ingredient
        {
            Name = name,
            NameEnglish = string.IsNullOrWhiteSpace(req.NameEnglish) ? null : req.NameEnglish.Trim(),
            Unit = req.Unit.Trim(),
            Description = string.IsNullOrWhiteSpace(req.Description) ? null : req.Description.Trim(),
            DefaultSupplierId = req.DefaultSupplierId,
            CostPerUnit = req.CostPerUnit,
            CategoryId = req.CategoryId,
            IsActive = req.IsActive,
            CreatedAt = DateTime.UtcNow,
        };

        var created = await _ingredientRepository.CreateAsync(entity, req.ReorderLevel, cancellationToken);
        var reloaded = await _ingredientRepository.GetByIdWithDetailsAsync(created.Id, cancellationToken);
        return new GetIngredientResponse
        {
            Ingredient = IngredientDtoMapping.ToDto(reloaded ?? created),
        };
    }
}
