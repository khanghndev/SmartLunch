namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Inventories;

public class GetInventoriesRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
}
