namespace SmartLunch.Backend.Service.Application.DTOs.Response.IngredientIntake;

public class IngredientIntakeProposalLineDto
{
    public Guid Id { get; set; }
    public Guid IngredientId { get; set; }
    public string IngredientName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string? LineNote { get; set; }
}

public class IngredientIntakeProposalSummaryDto
{
    public Guid Id { get; set; }
    public string ProposalCode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? HeaderNote { get; set; }
    public Guid CreatedByUserId { get; set; }
    public string? CreatedByDisplayName { get; set; }
    public DateTime CreatedAt { get; set; }
    /// <summary>Đã có phiếu nhập kho thực tế (tồn đã cập nhật).</summary>
    public bool HasActualReceipt { get; set; }
}

public class IngredientIntakeProposalDetailDto
{
    public Guid Id { get; set; }
    public string ProposalCode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? HeaderNote { get; set; }
    public Guid CreatedByUserId { get; set; }
    public string? CreatedByDisplayName { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? ReviewedByUserId { get; set; }
    public string? ReviewedByDisplayName { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewNote { get; set; }
    public bool HasActualReceipt { get; set; }
    public string? ActualReceiptCode { get; set; }
    public IReadOnlyList<IngredientIntakeProposalLineDto> Lines { get; set; } = Array.Empty<IngredientIntakeProposalLineDto>();
}

public class GetIngredientIntakeProposalsResponse
{
    public IReadOnlyList<IngredientIntakeProposalSummaryDto> Data { get; set; } = Array.Empty<IngredientIntakeProposalSummaryDto>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class CreateIngredientIntakeProposalResponse
{
    public IngredientIntakeProposalDetailDto Proposal { get; set; } = null!;
}

/// <summary>
/// Một bản ghi lịch sử sau khi quản lý đã xử lý phiếu (duyệt / từ chối / hủy).
/// </summary>
public class IntakeProposalReviewHistoryEntryDto
{
    public Guid ProposalId { get; set; }
    public string ProposalCode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? HeaderNote { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedByUserId { get; set; }
    public string? CreatedByDisplayName { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public Guid? ReviewedByUserId { get; set; }
    public string? ReviewedByDisplayName { get; set; }
    public string? ReviewNote { get; set; }
}

public class GetIntakeProposalReviewHistoryResponse
{
    public IReadOnlyList<IntakeProposalReviewHistoryEntryDto> Data { get; set; } = Array.Empty<IntakeProposalReviewHistoryEntryDto>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class IngredientActualIntakeLineDto
{
    public Guid Id { get; set; }
    public Guid IngredientId { get; set; }
    public string IngredientName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
}

public class IngredientActualIntakeDetailDto
{
    public Guid Id { get; set; }
    public string ReceiptCode { get; set; } = string.Empty;
    public Guid ProposalId { get; set; }
    public string ProposalCode { get; set; } = string.Empty;
    public Guid CreatedByUserId { get; set; }
    public string? CreatedByDisplayName { get; set; }
    public DateTime ReceivedAt { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
    public IReadOnlyList<IngredientActualIntakeLineDto> Lines { get; set; } = Array.Empty<IngredientActualIntakeLineDto>();
}

public class CreateActualIntakeFromProposalResponse
{
    public IngredientActualIntakeDetailDto Receipt { get; set; } = null!;
}

public class ReviewIngredientIntakeProposalResponse
{
    public IngredientIntakeProposalDetailDto Proposal { get; set; } = null!;
}
