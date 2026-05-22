namespace SmartLunch.Backend.Service.Application.DTOs.Response.Customer.Reviews;

public sealed class PublicReviewDto
{
    public int Id { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string? OrganizationName { get; set; }
    public string? OrderCode { get; set; }
    public string? ManagerReply { get; set; }
    public DateTime? RepliedAt { get; set; }
}

public sealed class GetPublicReviewsResponse
{
    public List<PublicReviewDto> Reviews { get; set; } = new();
    public double AverageRating { get; set; }
    public int TotalCount { get; set; }
}

public sealed class ReviewableOrderDto
{
    public int OrderId { get; set; }
    public string? OrderCode { get; set; }
    public string? InvoiceCode { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? OrganizationName { get; set; }
    public bool AlreadyReviewed { get; set; }
}

public sealed class GetReviewMeContextResponse
{
    public bool CanSubmitReview { get; set; }
    public bool IsEnterpriseMember { get; set; }
    public string? Message { get; set; }
    public List<ReviewableOrderDto> ReviewableOrders { get; set; } = new();
}

public sealed class ManagerReviewListItemDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int? OrderId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? OrganizationName { get; set; }
    public string? OrderCode { get; set; }
    public string? InvoiceCode { get; set; }
    public string? ManagerReply { get; set; }
    public DateTime? RepliedAt { get; set; }
    public bool IsReplied => !string.IsNullOrWhiteSpace(ManagerReply);
}

public sealed class GetManagerReviewsResponse
{
    public List<ManagerReviewListItemDto> Data { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public double AverageRating { get; set; }
}

public sealed class ReplyToReviewRequest
{
    public string Reply { get; set; } = string.Empty;
}
