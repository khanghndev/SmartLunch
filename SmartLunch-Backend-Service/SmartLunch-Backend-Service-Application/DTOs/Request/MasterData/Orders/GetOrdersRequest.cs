namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Orders;

public class GetOrdersRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
}
