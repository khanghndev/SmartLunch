namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Organizations;

public class OrganizationDto
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? ContactPerson { get; set; }
    public string? ContactEmail { get; set; }
    public string? TaxCode { get; set; }
    public string? LegalRepresentative { get; set; }
    public string? LogoUrl { get; set; }
    public string? Website { get; set; }
    public string? EducationLevel { get; set; }
    public string? Type { get; set; }
    public bool IsSubscriptionActive { get; set; }
    public int DefaultDailyMeals { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
