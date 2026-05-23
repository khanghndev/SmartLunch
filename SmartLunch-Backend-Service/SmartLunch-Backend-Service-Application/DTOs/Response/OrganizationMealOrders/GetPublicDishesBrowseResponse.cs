namespace SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationMealOrders;

public sealed class PublicDishBrowseItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public string? PrimarySlotKey { get; set; }
    public string? CategoryLabel { get; set; }
    public string? DietaryLabel { get; set; }
}

public sealed class GetPublicDishesBrowseResponse
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public List<PublicDishBrowseItemDto> Items { get; set; } = new();
}
