using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Ingredients;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Ingredients.DeleteIngredient;

public class DeleteIngredientCommandHandler : IRequestHandler<DeleteIngredientCommand, DeleteIngredientResponse>
{
    private readonly IIngredientRepository _ingredientRepository;

    public DeleteIngredientCommandHandler(IIngredientRepository ingredientRepository)
    {
        _ingredientRepository = ingredientRepository;
    }

    public async Task<DeleteIngredientResponse> Handle(DeleteIngredientCommand request, CancellationToken cancellationToken)
    {
        var entity = await _ingredientRepository.GetByIdAsync(request.IngredientId);
        if (entity == null)
            throw new KeyNotFoundException($"Ingredient with ID {request.IngredientId} was not found.");

        var hasReferences = await _ingredientRepository.HasBlockingReferencesAsync(request.IngredientId, cancellationToken);
        if (hasReferences)
        {
            entity.IsActive = false;
            entity.UpdatedAt = DateTime.UtcNow;
            await _ingredientRepository.UpdateAsync(entity, cancellationToken);
            return new DeleteIngredientResponse
            {
                Id = entity.Id,
                DeactivatedOnly = true,
                Message =
                    "Nguyên liệu đang được dùng trong món ăn hoặc phiếu nhập — đã chuyển sang trạng thái ngừng hoạt động (xóa mềm).",
            };
        }

        await _ingredientRepository.DeleteAsync(entity, cancellationToken);
        return new DeleteIngredientResponse
        {
            Id = request.IngredientId,
            DeactivatedOnly = false,
            Message = "Ingredient deleted successfully.",
        };
    }
}
