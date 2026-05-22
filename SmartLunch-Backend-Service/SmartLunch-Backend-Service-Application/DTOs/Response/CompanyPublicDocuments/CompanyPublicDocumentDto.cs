using SmartLunch.Backend.Service.Domain.Time;

namespace SmartLunch.Backend.Service.Application.DTOs.Response.CompanyPublicDocuments;

public sealed class CompanyPublicDocumentDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string DocumentType { get; set; } = "other";
    public string DocumentTypeLabel { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string FileUrl { get; set; } = string.Empty;
    public string? ContentType { get; set; }
    public string? OriginalFileName { get; set; }
    public long SizeBytes { get; set; }
    public int SortOrder { get; set; }
    public bool IsPublished { get; set; }
    public DateTime? IssuedDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsExpired =>
        ExpiryDate.HasValue && DateOnly.FromDateTime(ExpiryDate.Value.Date) < VietnamTime.Today;
}

public sealed class CompanyPublicDocumentListResponse
{
    public List<CompanyPublicDocumentDto> Documents { get; set; } = new();
}
