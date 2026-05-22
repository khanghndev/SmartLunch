using System.Net;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Domain.Time;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace SmartLunch.Backend.Service.Infrastructure.Pdf;

public sealed class QuestPdfOrderAnnexPdfService : IOrderAnnexPdfService
{
    private readonly IStorageService _storage;
    private readonly IOrganizationMealDocumentPdfService _combinedPdf;

    static QuestPdfOrderAnnexPdfService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public QuestPdfOrderAnnexPdfService(IStorageService storage, IOrganizationMealDocumentPdfService combinedPdf)
    {
        _storage = storage;
        _combinedPdf = combinedPdf;
    }

    public async Task<string> GenerateUploadAndResolveUrlAsync(
        Order order,
        string buyerDisplayName,
        string signatureDataUrl,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(order);
        if (order.Id <= 0)
            throw new ArgumentException("Order must be persisted.", nameof(order));

        if (order.ContractId is int contractId && contractId > 0 &&
            order.Contract?.Partner != null &&
            order.Contract.Organization != null)
        {
            return await _combinedPdf.GenerateCombinedUploadAndResolveUrlAsync(
                order.Contract,
                order.Contract.Partner,
                order.Contract.Organization,
                order,
                buyerDisplayName,
                signatureDataUrl,
                cancellationToken);
        }

        var pdf = GenerateAnnexOnlyPdfBytes(order, buyerDisplayName, signatureDataUrl);
        var objectName = $"orders/o{order.Id}/phu-luc-don-{order.Id}-{VietnamTime.Now:yyyyMMddHHmmss}.pdf";

        await using var ms = new MemoryStream(pdf);
        await _storage.UploadObjectAsync(objectName, ms, "application/pdf", cancellationToken);

        var signed = await _storage.CreateSignedUrlAsync(
            objectName,
            HttpMethod.Get,
            "application/pdf",
            TimeSpan.FromDays(365));
        return signed.Url;
    }

    public byte[] GeneratePdfBytes(Order order, string buyerDisplayName, string? signatureDataUrl = null)
    {
        if (order.ContractId is int contractId && contractId > 0 &&
            order.Contract?.Partner != null &&
            order.Contract.Organization != null)
        {
            return _combinedPdf.GenerateCombinedPdfBytes(
                order.Contract,
                order.Contract.Partner,
                order.Contract.Organization,
                order,
                buyerDisplayName,
                signatureDataUrl);
        }

        return GenerateAnnexOnlyPdfBytes(order, buyerDisplayName, signatureDataUrl);
    }

    private static byte[] GenerateAnnexOnlyPdfBytes(Order order, string buyerDisplayName, string? signatureDataUrl) =>
        Document.Create(container =>
        {
            container.Page(page => MealContractPdfSections.ComposeAnnexPage(page, order, buyerDisplayName, signatureDataUrl));
        }).GeneratePdf();
}
