namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.IngredientSources;

public class GetIngredientSourcesRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public Guid? PartnerId { get; set; }
    public Guid? IngredientId { get; set; }
}
