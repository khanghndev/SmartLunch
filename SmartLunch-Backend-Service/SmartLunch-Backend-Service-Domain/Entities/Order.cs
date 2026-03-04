namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Meal order
/// </summary>
public class Order
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public Guid? UnitId { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public DateTime ScheduledDate { get; set; }
    public string Status { get; set; } = "pending"; // pending|confirmed|preparing|delivered|cancelled
    public decimal TotalAmount { get; set; }
    public string PaymentStatus { get; set; } = "unpaid"; // unpaid|partial|paid
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public virtual User? User { get; set; }
    public virtual Unit? Unit { get; set; }
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    public virtual ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
}
