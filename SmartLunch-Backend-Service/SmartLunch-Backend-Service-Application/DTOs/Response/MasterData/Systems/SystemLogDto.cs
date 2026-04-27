namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Systems;

public class SystemLogDto
{
    public int Id { get; set; }
    public DateTime? Timestamp { get; set; }
    public string? Level { get; set; }
    public string? Template { get; set; }
    public string? Message { get; set; }
    public string? Exception { get; set; }
    public string? Properties { get; set; }
}

