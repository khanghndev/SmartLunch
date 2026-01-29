namespace SmartLunch.Backend.Service.Application.DTOs.Request.Media;

public class ConfirmMediaUploadRequest
{
    /// <summary>
    /// The object path returned from the upload-url endpoint.
    /// </summary>
    public string ObjectName { get; set; } = string.Empty;

    public string? OriginalFileName { get; set; }

    /// <summary>
    /// "image" | "video"
    /// </summary>
    public string MediaType { get; set; } = "image";

    public bool IsPublic { get; set; } = false;
}

