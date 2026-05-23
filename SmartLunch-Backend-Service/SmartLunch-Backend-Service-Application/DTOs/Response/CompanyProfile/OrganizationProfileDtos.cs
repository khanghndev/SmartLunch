namespace SmartLunch.Backend.Service.Application.DTOs.Response.CompanyProfile;

public sealed class OrganizationProfileResponse
{
    public int OrganizationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? TaxCode { get; set; }
    public string? LegalRepresentative { get; set; }
    public string? ContactPerson { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? ContactEmail { get; set; }
    public string? Website { get; set; }
    public string UnitType { get; set; } = "Office";
    public bool IsActive { get; set; }
    public string? LogoUrl { get; set; }
    public List<OrganizationLegalDocumentDto> Documents { get; set; } = new();
}

public sealed class UploadOrganizationLogoResponse
{
    public string LogoUrl { get; set; } = string.Empty;
}

public sealed class OrganizationLegalDocumentDto
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
    public DateTime? IssuedDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DateTime CreatedAt { get; set; }
}

public sealed class UpdateOrganizationProfileRequest
{
    public string Name { get; set; } = string.Empty;
    public string? TaxCode { get; set; }
    public string? LegalRepresentative { get; set; }
    public string? ContactPerson { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? ContactEmail { get; set; }
    public string? Website { get; set; }
}
