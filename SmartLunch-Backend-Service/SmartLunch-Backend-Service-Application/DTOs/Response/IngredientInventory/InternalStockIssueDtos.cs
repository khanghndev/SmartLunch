namespace SmartLunch.Backend.Service.Application.DTOs.Response.IngredientInventory;

public class InternalStockIssueLineDto
{
    public Guid Id { get; set; }
    public Guid IngredientId { get; set; }
    public string IngredientName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
}

public class InternalStockIssueSummaryDto
{
    public Guid Id { get; set; }
    public string IssueCode { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; }
    public string? Reason { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public string? CreatedByDisplayName { get; set; }
    public DateTime CreatedAt { get; set; }
    public int LineCount { get; set; }
}

public class InternalStockIssueDetailDto
{
    public Guid Id { get; set; }
    public string IssueCode { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; }
    public string? Reason { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public string? CreatedByDisplayName { get; set; }
    public DateTime CreatedAt { get; set; }
    public IReadOnlyList<InternalStockIssueLineDto> Lines { get; set; } = Array.Empty<InternalStockIssueLineDto>();
}

public class GetInternalStockIssuesResponse
{
    public IReadOnlyList<InternalStockIssueSummaryDto> Data { get; set; } = Array.Empty<InternalStockIssueSummaryDto>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class CreateInternalStockIssueResponse
{
    public InternalStockIssueDetailDto Issue { get; set; } = null!;
}
