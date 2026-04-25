namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// User entity for RBAC system
/// </summary>
public class User
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string Provider { get; set; } = "system"; // "system", "google", "facebook", "firebase", etc.
    public bool IsActive { get; set; } = true;
    public bool IsEmailVerified { get; set; } = false;
    public DateTime? EmailVerifiedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public int? CreatedBy { get; set; }
    public int? UpdatedBy { get; set; }

    // Navigation properties
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public virtual ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
    public virtual ICollection<UserToken> UserTokens { get; set; } = new List<UserToken>();
    public virtual ICollection<MediaFile> MediaFiles { get; set; } = new List<MediaFile>();
    public virtual ICollection<UserUnit> UserUnits { get; set; } = new List<UserUnit>();
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual ICollection<Order> SalesOrdersCreated { get; set; } = new List<Order>();
    public virtual ICollection<Delivery> DeliveriesAssigned { get; set; } = new List<Delivery>();
    public virtual ICollection<Payment> PaymentsMade { get; set; } = new List<Payment>();
    public virtual ICollection<WeeklyMenu> WeeklyMenusCreated { get; set; } = new List<WeeklyMenu>();
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    public virtual ICollection<Complaint> ComplaintsRaised { get; set; } = new List<Complaint>();
    public virtual ICollection<Complaint> ComplaintsAssigned { get; set; } = new List<Complaint>();
    public virtual ICollection<ChatbotLog> ChatbotLogs { get; set; } = new List<ChatbotLog>();
    public virtual ICollection<MenuSuggestion> MenuSuggestionsCreated { get; set; } = new List<MenuSuggestion>();
    public virtual ICollection<InternalStockIssue> InternalStockIssuesCreated { get; set; } = new List<InternalStockIssue>();
    public virtual ICollection<IngredientIntakeProposal> IngredientIntakeProposalsCreated { get; set; } = new List<IngredientIntakeProposal>();
    public virtual ICollection<IngredientIntakeProposal> IngredientIntakeProposalsReviewed { get; set; } = new List<IngredientIntakeProposal>();
    public virtual ICollection<IngredientActualIntake> IngredientActualIntakesCreated { get; set; } = new List<IngredientActualIntake>();
}
