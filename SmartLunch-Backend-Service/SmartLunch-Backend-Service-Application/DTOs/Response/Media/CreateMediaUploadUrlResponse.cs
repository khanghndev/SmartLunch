namespace SmartLunch.Backend.Service.Application.DTOs.Response.Media;

public class CreateMediaUploadUrlResponse
{
    public string Bucket { get; set; } = string.Empty;
    public string ObjectName { get; set; } = string.Empty;

    public string UploadUrl { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }

    /// <summary>
    /// Headers that MUST be sent with the upload request.
    /// At minimum this will include Content-Type.
    /// </summary>
    public Dictionary<string, string> RequiredHeaders { get; set; } = new();
}

