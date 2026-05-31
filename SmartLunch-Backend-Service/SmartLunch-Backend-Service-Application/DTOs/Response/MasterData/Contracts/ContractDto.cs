namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Contracts;

public class ContractDto
{
    public int Id { get; set; }
    public int PartnerId { get; set; }
    public string? PartnerLegalName { get; set; }
    public int? OrganizationId { get; set; }
    public string? OrganizationName { get; set; }
    public int? SourceOrderId { get; set; }
    public string ContractType { get; set; } = "Framework";
    public int? MealsPerDay { get; set; }
    public List<ContractDailyMealPortionDto> DailyMealPortions { get; set; } = new();
    public string? ContractNumber { get; set; }
    public string? Description { get; set; }
    public string? SupplySchedule { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? TotalValue { get; set; }
    public decimal? MealUnitPrice { get; set; }
    public decimal? DepositAmount { get; set; }
    public string? ContractFileUrl { get; set; }
    public bool IsDigitallySigned { get; set; }
    public string? DigitalSignature { get; set; }
    public DateTime? DigitallySignedAt { get; set; }
    public string? SignatureImage { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
