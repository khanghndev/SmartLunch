using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IOrderAnnexPdfService
{
    /// <summary>Tạo PDF phụ lục đơn hàng, upload object storage, trả về URL (signed GET).</summary>
    Task<string> GenerateUploadAndResolveUrlAsync(
        Order order,
        string buyerDisplayName,
        string signatureDataUrl,
        CancellationToken cancellationToken = default);
}
