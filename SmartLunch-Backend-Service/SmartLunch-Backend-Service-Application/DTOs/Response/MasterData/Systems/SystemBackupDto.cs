namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Systems;

public class SystemBackupDto
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? RestoredAtUtc { get; set; }
    public bool IsDeleted { get; set; }
}

