namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Payments;

public class GetPaymentsRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
}
