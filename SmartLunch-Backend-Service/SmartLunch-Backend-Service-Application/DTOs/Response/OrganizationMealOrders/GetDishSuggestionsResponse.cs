namespace SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationMealOrders;

public sealed class SuggestedDishItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? PrimarySlotKey { get; set; }
    public string? CategoryLabel { get; set; }
    public string? DietaryLabel { get; set; }
    /// <summary>Lý do gợi ý (hiển thị tooltip / subtitle).</summary>
    public string? MatchReason { get; set; }
}

public sealed class GetDishSuggestionsResponse
{
    public int AnchorDishId { get; set; }
    public string? AnchorPrimarySlotKey { get; set; }
    public List<SuggestedDishItemDto> Items { get; set; } = new();
}
