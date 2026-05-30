namespace SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationComplaints;

public class ComplaintEligibilityDto
{
    public int OrderId { get; set; }
    public bool CanComplain { get; set; }
    public string? BlockReason { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? ComplaintDeadlineAt { get; set; }
    public int OrderedMainPortionCount { get; set; }
    public int ExistingComplaintCount { get; set; }
}

public class ComplaintEvidenceDto
{
    public int Id { get; set; }
    public string Kind { get; set; } = string.Empty;
    public string MediaType { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class OrganizationComplaintSummaryDto
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public int? OrderId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Resolution { get; set; }
    public decimal? FinalRefundAmount { get; set; }
    public int? RefundPaymentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public DateTime? ComplaintDeadlineAt { get; set; }
}

public class OrganizationComplaintDetailDto : OrganizationComplaintSummaryDto
{
    public string Description { get; set; } = string.Empty;
    public int? MissingPortionCount { get; set; }
    public int? RefundPortionCount { get; set; }
    public decimal? SuggestedRefundAmount { get; set; }
    public string? ResolutionNote { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public List<ComplaintEvidenceDto> Evidence { get; set; } = new();
}

public class DeliveryProofContextDto
{
    public int DeliveryId { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public string? ProofImageUrl { get; set; }
    public string? RecipientConfirmedName { get; set; }
    public DateTime? RecipientConfirmedAt { get; set; }
    public string? ShipperName { get; set; }
}

public class ManagerComplaintDetailDto : OrganizationComplaintDetailDto
{
    public string? OrganizationName { get; set; }
    public string? OrderInvoiceCode { get; set; }
    public decimal OrderTotalAmount { get; set; }
    public int MealCount { get; set; }
    public decimal UnitPricePerPortion { get; set; }
    public DeliveryProofContextDto? ShipperDelivery { get; set; }
    public string? ComplainantName { get; set; }
}
