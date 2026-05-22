using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

/// <summary>PDF gộp: Phần I Hợp đồng + Phần II Phụ lục đặt hàng (đặt suất đơn vị).</summary>
public interface IOrganizationMealDocumentPdfService
{
    Task<string> GenerateCombinedUploadAndResolveUrlAsync(
        Contract contract,
        Partner supplier,
        Organization buyer,
        Order order,
        string buyerDisplayName,
        string? signatureDataUrl = null,
        CancellationToken cancellationToken = default);

    byte[] GenerateCombinedPdfBytes(
        Contract contract,
        Partner supplier,
        Organization buyer,
        Order order,
        string buyerDisplayName,
        string? signatureDataUrl = null);
}
