namespace SmartLunch.Backend.Service.Application.OrganizationChatbot;

/// <summary>Snapshot dữ liệu DB đưa vào LLM — nguồn duy nhất để trả lời.</summary>
public sealed class OrganizationChatbotKnowledgePack
{
    public DateTime GeneratedAt { get; set; }
    public OrganizationKnowledge? Organization { get; set; }
    public AccountSummary Summary { get; set; } = new();
    public List<ContractKnowledge> Contracts { get; set; } = new();
    public List<OrderKnowledge> Orders { get; set; } = new();
    public List<OrderKnowledge> PendingPaymentOrders { get; set; } = new();
    public WeeklyMenuKnowledge? WeeklyMenu { get; set; }
    public WeeklyMenuKnowledge? WeeklyMenuNext { get; set; }
    public List<ComplaintKnowledge> Complaints { get; set; } = new();
    public string CutoffRulesText { get; set; } = string.Empty;
    public SupportContactKnowledge? SupportContact { get; set; }
    public Dictionary<string, string> Policies { get; set; } = new();
}

public sealed class OrganizationKnowledge
{
    public string Name { get; set; } = string.Empty;
    /// <summary>Hiển thị: Văn phòng, Xí nghiệp...</summary>
    public string Type { get; set; } = string.Empty;
    /// <summary>Key DB: office, factory, school</summary>
    public string TypeKey { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? ContactPerson { get; set; }
    public string? ContactEmail { get; set; }
    public string? TaxCode { get; set; }
}

public sealed class AccountSummary
{
    public int TotalOrders { get; set; }
    public int ActiveContracts { get; set; }
    public int OrdersNeedingPayment { get; set; }
    public decimal TotalOutstandingVnd { get; set; }
    public int UpcomingOrders { get; set; }
    public int OpenComplaints { get; set; }
}

public sealed class ContractKnowledge
{
    public string Code { get; set; } = string.Empty;
    public string? ContractNumber { get; set; }
    public string ContractType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? MealUnitPriceVnd { get; set; }
    public int? MealsPerDay { get; set; }
    public bool IsDigitallySigned { get; set; }
}

public sealed class OrderKnowledge
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? InvoiceCode { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string StatusVi { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public string PaymentStatusVi { get; set; } = string.Empty;
    public decimal TotalAmountVnd { get; set; }
    public int MainPortionCount { get; set; }
    public string? DeliveryStatus { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? ComplaintDeadlineAt { get; set; }
    public List<string> DishNames { get; set; } = new();
}

public sealed class WeeklyMenuKnowledge
{
    public DateTime WeekStart { get; set; }
    public DateTime WeekEnd { get; set; }
    public string? CustomerTypeName { get; set; }
    /// <summary>current | next</summary>
    public string PeriodKey { get; set; } = "current";
    /// <summary>Chưa có bản ghi tuần sau — suy từ lịch cố định theo thứ.</summary>
    public bool IsProjected { get; set; }
    public List<DailyMenuKnowledge> Days { get; set; } = new();
}

public sealed class DailyMenuKnowledge
{
    public DateTime Date { get; set; }
    public string DayLabel { get; set; } = string.Empty;
    public List<string> Dishes { get; set; } = new();
}

public sealed class ComplaintKnowledge
{
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string StatusVi { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? OrderCode { get; set; }
}

public sealed class SupportContactKnowledge
{
    public string? SupplierName { get; set; }
    public string? Hotline { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
}
