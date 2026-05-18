using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.IngredientSources;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.IngredientSources.CreateIngredientSource;

public class CreateIngredientSourceCommandHandler : IRequestHandler<CreateIngredientSourceCommand, GetIngredientSourceResponse>
{
    private readonly IIngredientSourceRepository _ingredientSourceRepository;
    private readonly IIngredientRepository _ingredientRepository;
    private readonly IPartnerRepository _partnerRepository;

    public CreateIngredientSourceCommandHandler(
        IIngredientSourceRepository ingredientSourceRepository,
        IIngredientRepository ingredientRepository,
        IPartnerRepository partnerRepository)
    {
        _ingredientSourceRepository = ingredientSourceRepository;
        _ingredientRepository = ingredientRepository;
        _partnerRepository = partnerRepository;
    }

    public async Task<GetIngredientSourceResponse> Handle(CreateIngredientSourceCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;
        if (await _ingredientRepository.GetByIdAsync(req.IngredientId) == null)
            throw new KeyNotFoundException($"Ingredient with ID {req.IngredientId} was not found.");
        if (req.PartnerId.HasValue && await _partnerRepository.GetByIdAsync(req.PartnerId.Value) == null)
            throw new KeyNotFoundException($"Partner with ID {req.PartnerId} was not found.");

        var entity = new IngredientSource
        {

            IngredientId = req.IngredientId,
            PartnerId = req.PartnerId,
            BatchNumber = string.IsNullOrWhiteSpace(req.BatchNumber) ? null : req.BatchNumber.Trim(),
            OriginDetails = string.IsNullOrWhiteSpace(req.OriginDetails) ? null : req.OriginDetails.Trim(),
            ProductionDate = req.ProductionDate,
            ExpirationDate = req.ExpirationDate,
            Certification = string.IsNullOrWhiteSpace(req.Certification) ? null : req.Certification.Trim(),
            CreatedAt = VietnamTime.Now
        };

        await _ingredientSourceRepository.CreateAsync(entity);
        var reloaded = await _ingredientSourceRepository.GetByIdAsync(entity.Id);
        return new GetIngredientSourceResponse
        {
            IngredientSource = reloaded != null
                ? IngredientSourceDtoMapping.ToDto(reloaded)
                : IngredientSourceDtoMapping.ToDto(entity)
        };
    }
}
