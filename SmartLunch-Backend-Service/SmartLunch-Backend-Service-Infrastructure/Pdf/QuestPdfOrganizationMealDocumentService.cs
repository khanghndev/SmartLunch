using System.Net;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Domain.Time;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace SmartLunch.Backend.Service.Infrastructure.Pdf;

/// <summary>PDF gộp Phần I (hợp đồng) + Phần II (phụ lục) khi đặt suất đơn vị.</summary>
public sealed class QuestPdfOrganizationMealDocumentService : IOrganizationMealDocumentPdfService
{
    private readonly IStorageService _storage;

    static QuestPdfOrganizationMealDocumentService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public QuestPdfOrganizationMealDocumentService(IStorageService storage)
    {
        _storage = storage;
    }

    public Task<string> GenerateCombinedUploadAndResolveUrlAsync(
        Contract contract,
        Partner supplier,
        Organization buyer,
        Order order,
        string buyerDisplayName,
        string? signatureDataUrl = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(contract);
        ArgumentNullException.ThrowIfNull(supplier);
        ArgumentNullException.ThrowIfNull(buyer);
        ArgumentNullException.ThrowIfNull(order);
        if (contract.Id <= 0 || order.Id <= 0)
            throw new ArgumentException("Contract and order must be persisted before generating combined PDF.");

        return UploadAsync(contract.Id, order.Id, GenerateCombinedPdfBytes(contract, supplier, buyer, order, buyerDisplayName, signatureDataUrl), cancellationToken);
    }

    public byte[] GenerateCombinedPdfBytes(
        Contract contract,
        Partner supplier,
        Organization buyer,
        Order order,
        string buyerDisplayName,
        string? signatureDataUrl = null)
    {
        return Document.Create(container =>
        {
            container.Page(page => MealContractPdfSections.ComposeContractPage(
                page, contract, supplier, buyer, order, signatureDataUrl));
            container.Page(page => MealContractPdfSections.ComposeAnnexPage(page, order, buyerDisplayName, signatureDataUrl));
        }).GeneratePdf();
    }

    private async Task<string> UploadAsync(int contractId, int orderId, byte[] pdf, CancellationToken cancellationToken)
    {
        var objectName =
            $"contracts/c{contractId}/hop-dong-phu-luc-o{orderId}-{VietnamTime.Now:yyyyMMddHHmmss}.pdf";

        await using var ms = new MemoryStream(pdf);
        await _storage.UploadObjectAsync(objectName, ms, "application/pdf", cancellationToken);

        var signed = await _storage.CreateSignedUrlAsync(
            objectName,
            HttpMethod.Get,
            "application/pdf",
            TimeSpan.FromDays(365));
        return signed.Url;
    }
}
