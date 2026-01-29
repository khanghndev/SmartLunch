namespace SmartLunch.Backend.Service.Application.DTOs.Response.Media;

public class GetMediaDownloadUrlResponse
{
    public string DownloadUrl { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
}

