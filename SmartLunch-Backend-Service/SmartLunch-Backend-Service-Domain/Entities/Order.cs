namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Meal order
/// </summary>
public class Order
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public int? UserId { get; set; }
    public int? UnitId { get; set; }
    public int? ContractId { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public DateTime ScheduledDate { get; set; }
    public string Status { get; set; } = "pending"; // pending|confirmed|preparing|delivered|cancelled
    public decimal TotalAmount { get; set; }
    public string PaymentStatus { get; set; } = "unpaid"; // unpaid|partial|paid
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    /// <summary>Mã hóa đơn hiển thị (VD: HD-20250324-AB12CD34).</summary>
    public string? InvoiceCode { get; set; }

    /// <summary>Nhân viên bán tạo hóa đơn (POS).</summary>
    public int? CreatedBySalesUserId { get; set; }

    public virtual User? User { get; set; }
    public virtual Unit? Unit { get; set; }
    public virtual Contract? Contract { get; set; }
    public virtual User? CreatedBySalesUser { get; set; }
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    public virtual ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
}
