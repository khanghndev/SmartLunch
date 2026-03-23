namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Partners;

public class PartnerDto
{
    public Guid Id { get; set; }
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
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
