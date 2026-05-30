namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>Ảnh/video bằng chứng khiếu nại.</summary>
public class ComplaintEvidence
{
    public int Id { get; set; }
    public int ComplaintId { get; set; }
    public string Kind { get; set; } = string.Empty;
    public string MediaType { get; set; } = string.Empty; // image|video
    public string StorageObjectName { get; set; } = string.Empty;
    public string? ContentType { get; set; }
    public long? FileSizeBytes { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; } = VietnamTime.Now;

    public virtual Complaint Complaint { get; set; } = null!;
}
