namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Systems;

public class RestoreSystemResponse
{
    public int InsertedCount { get; set; }
    public int SkippedCount { get; set; }
    public DateTime RestoredAt { get; set; }
    public int ActorUserId { get; set; }
    public string? RestoredFromFilePath { get; set; }
}

