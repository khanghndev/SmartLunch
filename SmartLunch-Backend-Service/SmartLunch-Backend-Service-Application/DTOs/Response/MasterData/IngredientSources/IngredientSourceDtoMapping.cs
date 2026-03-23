using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.IngredientSources;

public static class IngredientSourceDtoMapping
{
    public static IngredientSourceDto ToDto(IngredientSource e) => new()
    {
        Id = e.Id,
        IngredientId = e.IngredientId,
        IngredientName = e.Ingredient?.Name,
        PartnerId = e.PartnerId,
        PartnerLegalName = e.Partner?.LegalName,
        BatchNumber = e.BatchNumber,
        OriginDetails = e.OriginDetails,
        ProductionDate = e.ProductionDate,
        ExpirationDate = e.ExpirationDate,
        Certification = e.Certification,
        CreatedAt = e.CreatedAt
    };
}
