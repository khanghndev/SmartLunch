using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Ingredients;
using SmartLunch.Backend.Service.Application.Helpers;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Ingredients.UpdateIngredient;

public class UpdateIngredientCommandHandler : IRequestHandler<UpdateIngredientCommand, GetIngredientResponse>
{
    private readonly IIngredientRepository _ingredientRepository;
    private readonly IPartnerRepository _partnerRepository;

    public UpdateIngredientCommandHandler(
        IIngredientRepository ingredientRepository,
        IPartnerRepository partnerRepository)
    {
        _ingredientRepository = ingredientRepository;
        _partnerRepository = partnerRepository;
    }

    public async Task<GetIngredientResponse> Handle(UpdateIngredientCommand command, CancellationToken cancellationToken)
    {
        var req = command.Request;
        IngredientRequestValidation.ValidateUpdate(req);

        var entity = await _ingredientRepository.GetByIdAsync(command.IngredientId);
        if (entity == null)
            throw new KeyNotFoundException($"Ingredient with ID {command.IngredientId} was not found.");

        var name = req.Name.Trim();
        if (await _ingredientRepository.ExistsByNameAsync(name, command.IngredientId, cancellationToken))
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

        entity.Name = name;
        entity.NameEnglish = string.IsNullOrWhiteSpace(req.NameEnglish) ? null : req.NameEnglish.Trim();
        entity.Unit = req.Unit.Trim();
        entity.Description = string.IsNullOrWhiteSpace(req.Description) ? null : req.Description.Trim();
        entity.DefaultSupplierId = req.DefaultSupplierId;
        entity.CostPerUnit = req.CostPerUnit;
        entity.CategoryId = req.CategoryId;
        entity.IsActive = req.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;

        await _ingredientRepository.UpdateAsync(entity, cancellationToken);
        var reloaded = await _ingredientRepository.GetByIdWithDetailsAsync(entity.Id, cancellationToken);
        return new GetIngredientResponse
        {
            Ingredient = IngredientDtoMapping.ToDto(reloaded ?? entity),
        };
    }
}
