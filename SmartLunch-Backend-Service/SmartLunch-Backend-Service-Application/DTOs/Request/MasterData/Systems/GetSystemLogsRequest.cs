namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Systems;

public class GetSystemLogsRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

