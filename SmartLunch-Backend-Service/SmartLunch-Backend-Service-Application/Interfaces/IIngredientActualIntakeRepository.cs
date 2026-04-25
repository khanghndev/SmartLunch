using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IIngredientActualIntakeRepository
{
    /// <summary>
    /// Phiếu đề xuất phải approved, chưa có phiếu nhập thực tế; cộng tồn kho; đổi trạng thái phiếu thành fulfilled.
    /// </summary>
    Task<IngredientActualIntake> CreateFromApprovedProposalAsync(
        int proposalId,
        int actorUserId,
        bool actorIsElevated,
        DateTime receivedAtUtc,
        string? note,
        CancellationToken cancellationToken = default);
}
