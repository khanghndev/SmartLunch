namespace SmartLunch.Backend.Service.Application.DTOs.Request.Media;

public class CreateMediaUploadUrlRequest
{
    /// <summary>
    /// Original file name from client (used for metadata only).
    /// </summary>
    public string? FileName { get; set; }

    /// <summary>
    /// MIME type of the upload (e.g. image/png, video/mp4).
    /// This will be signed into the URL and must match the upload request header.
    /// </summary>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// Expected size in bytes (server will enforce limits before issuing URL).
    /// </summary>
    public long SizeBytes { get; set; }

    /// <summary>
    /// "image" | "video"
    /// </summary>
    public string MediaType { get; set; } = "image";

    public bool IsPublic { get; set; } = false;
}

