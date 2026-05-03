namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Partners;

public class PartnerDto
{
    public int Id { get; set; }
    public string LegalName { get; set; } = string.Empty;
    public string? BusinessRegistrationNumber { get; set; }
    public string? TaxId { get; set; }
    public string? LegalRepresentative { get; set; }
    public string? Address { get; set; }
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public decimal? PerformanceRating { get; set; }
    public string? ComplianceInfo { get; set; }
    public string? FinancialTerms { get; set; }
    public List<PartnerDocumentDto> Documents { get; set; } = new();
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class PartnerDocumentDto
{
    public int Id { get; set; }
    public int MediaFileId { get; set; }
    public string DocumentType { get; set; } = "other";
    public bool IsVerified { get; set; }
    public string Url { get; set; } = string.Empty;
}
