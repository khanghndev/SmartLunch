namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>Tài liệu pháp lý do đơn vị khách hàng (Organization) tự tải lên.</summary>
public class OrganizationLegalDocument
{
    public int Id { get; set; }
    public int OrganizationId { get; set; }
    public string Title { get; set; } = string.Empty;
    /// <summary>business_license | tax | authorization | other</summary>
    public string DocumentType { get; set; } = "other";
    public string? Description { get; set; }
    public int MediaFileId { get; set; }
    public DateTime? IssuedDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int? UploadedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = VietnamTime.Now;
    public DateTime? UpdatedAt { get; set; }

    public virtual Organization Organization { get; set; } = null!;
    public virtual MediaFile MediaFile { get; set; } = null!;
}
