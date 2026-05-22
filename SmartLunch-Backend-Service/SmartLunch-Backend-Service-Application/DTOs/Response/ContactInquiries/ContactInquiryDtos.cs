namespace SmartLunch.Backend.Service.Application.DTOs.Response.ContactInquiries;

public sealed class CreateContactInquiryRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string InterestedService { get; set; } = string.Empty;
    public string? Message { get; set; }
}

public sealed class CreateContactInquiryResponse
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string Message { get; set; } = "Yêu cầu đã được ghi nhận. Chúng tôi sẽ liên hệ sớm.";
}

public sealed class ManagerContactInquiryListItemDto
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string InterestedService { get; set; } = string.Empty;
    public string? Message { get; set; }
    public string Status { get; set; } = "pending";
    public string? ManagerReply { get; set; }
    public DateTime? RepliedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsReplied => !string.IsNullOrWhiteSpace(ManagerReply);
}

public sealed class GetManagerContactInquiriesResponse
{
    public List<ManagerContactInquiryListItemDto> Data { get; set; } = new();
    public int TotalCount { get; set; }
    public int PendingCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public sealed class ReplyToContactInquiryRequest
{
    public string Reply { get; set; } = string.Empty;
}
