namespace SmartLunch.Backend.Service.Application.DTOs.Response.Catalog;

public class DishCategoryOptionDto
{
    public string Code { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}

public class GetDishCategoriesResponse
{
    public List<DishCategoryOptionDto> Categories { get; set; } = new();
}
