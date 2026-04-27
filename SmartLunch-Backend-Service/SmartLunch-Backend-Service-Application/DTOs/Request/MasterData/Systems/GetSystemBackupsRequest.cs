namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Systems;

public class GetSystemBackupsRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public bool IncludeDeleted { get; set; } = false;
}

