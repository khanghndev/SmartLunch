namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Supplier/partner entity
/// </summary>
public class Partner
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string LegalName { get; set; } = string.Empty;
    /// <summary>Số đăng ký kinh doanh (nếu khác mã số thuế).</summary>
    public string? BusinessRegistrationNumber { get; set; }
    public string? TaxId { get; set; }
    /// <summary>Người đại diện theo pháp luật.</summary>
    public string? LegalRepresentative { get; set; }
    public string? Address { get; set; }
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public decimal? PerformanceRating { get; set; }
    public string? ComplianceInfo { get; set; }
    public string? FinancialTerms { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();
    public virtual ICollection<PartnerPayment> PartnerPayments { get; set; } = new List<PartnerPayment>();
    public virtual ICollection<Ingredient> IngredientsAsDefaultSupplier { get; set; } = new List<Ingredient>();
    public virtual ICollection<IngredientSource> IngredientSources { get; set; } = new List<IngredientSource>();
}
