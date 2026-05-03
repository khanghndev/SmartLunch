namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Customer unit (school/company) placing meal orders
/// </summary>
public class Organization
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
    
    /// <summary>Văn phòng (Office) / Xí nghiệp (Factory) / Trường học (School)</summary>
    public string Type { get; set; } = "Office";
    
    /// <summary>Bật tính năng khóa & lên đơn tự động dựa vào hợp đồng (Subscription).</summary>
    public bool IsSubscriptionActive { get; set; } = false;
    
    /// <summary>Số suất mặc định đặt mỗi ngày khi cơ chế định kỳ chạy tự động.</summary>
    public int DefaultDailyMeals { get; set; } = 0;

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public int? CreatedBy { get; set; }
    public int? UpdatedBy { get; set; }

    public virtual ICollection<UserOrganization> UserOrganizations { get; set; } = new List<UserOrganization>();
    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();
}
