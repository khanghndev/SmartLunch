using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IIngredientIntakeProposalRepository
{
    Task<IngredientIntakeProposal?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<IngredientIntakeProposal> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        int? createdByUserIdFilter,
        CancellationToken cancellationToken = default);

    Task<IngredientIntakeProposal> CreateAsync(IngredientIntakeProposal proposal, CancellationToken cancellationToken = default);

    /// <summary>
    /// Quản lý duyệt / từ chối phiếu đang submitted.
    /// </summary>
    Task<IngredientIntakeProposal> ReviewProposalAsync(
        int proposalId,
        int reviewerUserId,
        bool approve,
        string? reviewNote,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Phiếu đã qua xử lý của quản lý (status khác submitted). Lọc theo người tạo nếu có.
    /// </summary>
    Task<(IReadOnlyList<IngredientIntakeProposal> Items, int TotalCount)> GetReviewHistoryPagedAsync(
        int page,
        int pageSize,
        int? createdByUserIdFilter,
        CancellationToken cancellationToken = default);
}
