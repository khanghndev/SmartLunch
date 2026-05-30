namespace SmartLunch.Backend.Service.Application.DTOs.Request.OrganizationComplaints;

public class CreateOrganizationComplaintRequest
{
    public int OrderId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? MissingPortionCount { get; set; }
    public int? RefundPortionCount { get; set; }
}

public class UploadComplaintEvidenceRequest
{
    public string Kind { get; set; } = string.Empty;
}

public class ResolveManagerComplaintRequest
{
    public string Resolution { get; set; } = string.Empty;
    public string? ResolutionNote { get; set; }
    public decimal? FinalRefundAmount { get; set; }
    public int? RefundPortionCount { get; set; }
}
