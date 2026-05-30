namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Complaints;

public class ComplaintDto
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public int UserId { get; set; }
    public int? OrderId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Resolution { get; set; }
    public int? AssignedTo { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public DateTime? ComplaintDeadlineAt { get; set; }
    public int? RefundPortionCount { get; set; }
    public decimal? SuggestedRefundAmount { get; set; }
    public decimal? FinalRefundAmount { get; set; }
    public int? RefundPaymentId { get; set; }
    public DateTime? ResolvedAt { get; set; }
}
