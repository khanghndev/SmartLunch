namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Supplier contract
/// </summary>
public class Contract
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public int PartnerId { get; set; }
    public int? OrganizationId { get; set; }

    /// <summary>Đơn hàng tạo ra hợp đồng Order-Based (đặt suất đơn vị).</summary>
    public int? SourceOrderId { get; set; }

    /// <summary>Số hợp đồng (ký hiệu nội bộ hoặc theo văn bản pháp lý).</summary>
    public string? ContractNumber { get; set; }
    public string ContractType { get; set; } = "Framework"; // Framework | Order-Based
    public string? Description { get; set; }
    /// <summary>Mô tả thời gian / khung cung cấp suất ăn (vd. Thứ 2–6, bữa trưa).</summary>
    public string? SupplySchedule { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? TotalValue { get; set; }

    /// <summary>Mức giá suất ăn của hợp đồng (FK dish_values).</summary>
    public int? DishValueId { get; set; }

    /// <summary>Giá / suất thỏa thuận với đơn vị (không lấy từ Dish.Price).</summary>
    public decimal? MealUnitPrice { get; set; }

    /// <summary>Số suất ăn mỗi ngày phục vụ (HĐ Period-Based).</summary>
    public int? MealsPerDay { get; set; }

    public decimal? DepositAmount { get; set; }

    /// <summary>Tuần (Thứ 2) đã gửi email nhắc đặt món.</summary>
    public DateOnly? LastWeeklyReminderWeekStart { get; set; }

    /// <summary>Tuần (Thứ 2) đã chạy auto random món chính.</summary>
    public DateOnly? WeeklyAutoFillWeekStart { get; set; }
    public string? ContractFileUrl { get; set; }
    
    // Digital Signature Info
    public bool IsDigitallySigned { get; set; } = false;
    public string? DigitalSignature { get; set; }
    public DateTime? DigitallySignedAt { get; set; }
    public string? SignatureImage { get; set; }
    
    public string Status { get; set; } = "active"; // active | expired | cancelled
    public DateTime CreatedAt { get; set; } = VietnamTime.Now;
    public DateTime? UpdatedAt { get; set; }

    public virtual Partner Partner { get; set; } = null!;
    public virtual Organization? Organization { get; set; }
    public virtual DishValue? DishValue { get; set; }
    public virtual ICollection<PartnerPayment> PartnerPayments { get; set; } = new List<PartnerPayment>();
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual ICollection<ContractExcludedDate> ExcludedDates { get; set; } = new List<ContractExcludedDate>();
    public virtual ICollection<ContractDailyMealPortion> DailyMealPortions { get; set; } = new List<ContractDailyMealPortion>();
    public virtual ICollection<ContractWeeklySelection> WeeklySelections { get; set; } = new List<ContractWeeklySelection>();
}
