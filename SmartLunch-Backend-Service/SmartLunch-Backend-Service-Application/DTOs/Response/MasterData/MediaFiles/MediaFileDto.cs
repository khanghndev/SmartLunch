namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.MediaFiles;

public class MediaFileDto
{
    public Guid Id { get; set; }
    public Guid OwnerUserId { get; set; }
    public string Bucket { get; set; } = string.Empty;
    public string ObjectName { get; set; } = string.Empty;
    public string? OriginalFileName { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public string? Md5HashBase64 { get; set; }
    public string MediaType { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
