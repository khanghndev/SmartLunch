namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Ingredients;

public class GetIngredientsRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public bool? IsActive { get; set; }
}
