using System.Net;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.OrganizationMealOrders;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Domain.Time;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace SmartLunch.Backend.Service.Infrastructure.Pdf;

public sealed class QuestPdfContractFileService : IContractPdfService
{
    private readonly IStorageService _storage;

    static QuestPdfContractFileService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public QuestPdfContractFileService(IStorageService storage)
    {
        _storage = storage;
    }

    public async Task<string> GenerateUploadAndResolveUrlAsync(
        Contract contract,
        Partner supplier,
        Organization? buyer,
        Order? order = null,
        OrganizationMealDeliveryPdfContext? delivery = null,
        string? buyerSignatureDataUrl = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(contract);
        ArgumentNullException.ThrowIfNull(supplier);
        if (contract.Id <= 0)
            throw new ArgumentException("Contract must be persisted before generating PDF.", nameof(contract));

        var pdf = GenerateContractPdfBytes(contract, supplier, buyer, order, delivery, buyerSignatureDataUrl);

        var objectName = $"contracts/c{contract.Id}/hop-dong-{contract.Id}-{VietnamTime.Now:yyyyMMddHHmmss}.pdf";

        await using var ms = new MemoryStream(pdf);
        await _storage.UploadObjectAsync(objectName, ms, "application/pdf", cancellationToken);

        var signed = await _storage.CreateSignedUrlAsync(
            objectName,
            HttpMethod.Get,
            "application/pdf",
            TimeSpan.FromDays(365));
        return signed.Url;
    }

    public byte[] GenerateContractPdfBytes(
        Contract contract,
        Partner supplier,
        Organization? buyer,
        Order? order = null,
        OrganizationMealDeliveryPdfContext? delivery = null,
        string? buyerSignatureDataUrl = null)
    {
        ArgumentNullException.ThrowIfNull(contract);
        ArgumentNullException.ThrowIfNull(supplier);
        return Document.Create(container =>
        {
            container.Page(page => MealContractPdfSections.ComposeContractPage(
                page, contract, supplier, buyer, order, buyerSignatureDataUrl, delivery));
        }).GeneratePdf();
    }
}
