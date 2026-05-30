namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Customer / organization complaint
/// </summary>
public class Complaint
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public int UserId { get; set; }
    public int? OrderId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public int? MissingPortionCount { get; set; }
    public string Status { get; set; } = "draft";
    public string? Resolution { get; set; }
    public string? ResolutionNote { get; set; }
    public int? AssignedTo { get; set; }
    public int? ResolvedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = VietnamTime.Now;
    public DateTime? SubmittedAt { get; set; }
    public DateTime? ComplaintDeadlineAt { get; set; }
    public int? RefundPortionCount { get; set; }
    public decimal? SuggestedRefundAmount { get; set; }
    public decimal? FinalRefundAmount { get; set; }
    public int? RefundPaymentId { get; set; }
    public DateTime? ResolvedAt { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual Order? Order { get; set; }
    public virtual Payment? RefundPayment { get; set; }
    public virtual User? AssignedToUser { get; set; }
    public virtual User? ResolvedByUser { get; set; }
    public virtual ICollection<ComplaintEvidence> Evidence { get; set; } = new List<ComplaintEvidence>();
}
