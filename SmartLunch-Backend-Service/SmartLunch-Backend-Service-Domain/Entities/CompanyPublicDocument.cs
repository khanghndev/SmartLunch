namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>Hồ sơ công khai của HuitMeal (chứng nhận, VSATTP, biên bản kiểm tra...) — hiển thị trang Giới thiệu.</summary>
public class CompanyPublicDocument
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    /// <summary>food_safety | iso | business_license | inspection | contract | other</summary>
    public string DocumentType { get; set; } = "other";
    public string? Description { get; set; }
    public int MediaFileId { get; set; }
    public int SortOrder { get; set; }
    public bool IsPublished { get; set; } = true;
    public DateTime? IssuedDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int? CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = VietnamTime.Now;
    public DateTime? UpdatedAt { get; set; }

    public virtual MediaFile MediaFile { get; set; } = null!;
}
