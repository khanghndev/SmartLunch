namespace SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationMealContractOrders;

public sealed class MealPortionPriceOptionDto
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string? Label { get; set; }
    public string? Code { get; set; }
}

public sealed class GetMealPortionPricesResponse
{
    public List<MealPortionPriceOptionDto> Items { get; set; } = new();
}
