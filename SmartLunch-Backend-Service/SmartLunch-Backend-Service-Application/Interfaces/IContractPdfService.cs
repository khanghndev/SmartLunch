using SmartLunch.Backend.Service.Application.OrganizationMealOrders;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

/// <summary>
/// Tạo file PDF biên nhận hợp đồng, upload lưu trữ và trả URL công khai (Appwrite storage).
/// </summary>
public interface IContractPdfService
{
    /// <summary>Ghi PDF lên bucket, cập nhật <paramref name="contract"/>.ContractFileUrl và persisted qua repo ngoài không gọi ở đây.</summary>
    Task<string> GenerateUploadAndResolveUrlAsync(
        Contract contract,
        Partner supplier,
        Organization? buyer,
        Order? order = null,
        OrganizationMealDeliveryPdfContext? delivery = null,
        string? buyerSignatureDataUrl = null,
        CancellationToken cancellationToken = default);

    /// <summary>Tạo PDF hợp đồng trong bộ nhớ (không upload).</summary>
    byte[] GenerateContractPdfBytes(
        Contract contract,
        Partner supplier,
        Organization? buyer,
        Order? order = null,
        OrganizationMealDeliveryPdfContext? delivery = null,
        string? buyerSignatureDataUrl = null);
}
