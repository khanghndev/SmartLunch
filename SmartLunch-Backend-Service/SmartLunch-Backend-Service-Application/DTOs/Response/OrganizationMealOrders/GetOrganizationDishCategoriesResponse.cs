namespace SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationMealOrders;

public sealed class OrganizationDishCategoryListItemDto
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string SlotKey { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}

public sealed class GetOrganizationDishCategoriesResponse
{
    public List<OrganizationDishCategoryListItemDto> Categories { get; set; } = new();

    /// <summary>Ngày phục vụ tối thiểu / tối đa (VN) để FE hiển thị chọn ngày.</summary>
    public DateOnly AllowedFirstServiceDate { get; set; }
    public DateOnly AllowedLastServiceDate { get; set; }
}
