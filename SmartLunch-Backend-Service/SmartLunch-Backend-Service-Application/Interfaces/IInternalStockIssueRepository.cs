using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IInternalStockIssueRepository
{
    Task<InternalStockIssue?> GetByIdWithLinesAsync(int id, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<InternalStockIssue> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        DateTime? issuedFromUtc,
        DateTime? issuedToUtcExclusive,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Tạo phiếu và trừ tồn kho trong một transaction.
    /// </summary>
    Task<InternalStockIssue> CreateAndDeductStockAsync(
        InternalStockIssue issue,
        IReadOnlyList<(int IngredientId, decimal Quantity)> lines,
        CancellationToken cancellationToken = default);
}
