using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

/// <summary>Tạo PDF biên bản bàn giao suất ăn sau khi người nhận ký xác nhận.</summary>
public interface IDeliveryHandoverPdfService
{
    Task<string> GenerateUploadAndResolveUrlAsync(
        Delivery delivery,
        int mealCount,
        string recipientConfirmedName,
        byte[] recipientSignaturePng,
        byte[] shipperSignaturePng,
        byte[]? proofPhotoBytes,
        string? proofPhotoContentType,
        string? shipperDisplayName,
        string? notes,
        DateTime confirmedAt,
        CancellationToken cancellationToken = default);
}
