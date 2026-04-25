namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Media file metadata stored in DB. Actual bytes are stored in Firebase Storage (GCS).
/// </summary>
public class MediaFile
{
    public int Id { get; set; }
    public string? Code { get; set; }

    public int OwnerUserId { get; set; }
    public virtual User? OwnerUser { get; set; }

    public string Bucket { get; set; } = string.Empty;
    public string ObjectName { get; set; } = string.Empty;

    public string? OriginalFileName { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public string? Md5HashBase64 { get; set; }

    /// <summary>
    /// "image" | "video"
    /// </summary>
    public string MediaType { get; set; } = "unknown";

    public bool IsPublic { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

