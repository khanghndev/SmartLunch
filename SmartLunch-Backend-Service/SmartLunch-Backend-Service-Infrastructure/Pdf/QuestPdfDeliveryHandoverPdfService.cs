using System.Globalization;
using System.Net;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Domain.Time;

namespace SmartLunch.Backend.Service.Infrastructure.Pdf;

public sealed class QuestPdfDeliveryHandoverPdfService : IDeliveryHandoverPdfService
{
    private static readonly CultureInfo Vi = CultureInfo.GetCultureInfo("vi-VN");
    private readonly IStorageService _storage;

    static QuestPdfDeliveryHandoverPdfService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public QuestPdfDeliveryHandoverPdfService(IStorageService storage)
    {
        _storage = storage;
    }

    public async Task<string> GenerateUploadAndResolveUrlAsync(
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
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(delivery);
        if (delivery.Id <= 0)
            throw new ArgumentException("Delivery must be persisted.", nameof(delivery));
        if (recipientSignaturePng is not { Length: > 0 })
            throw new ArgumentException("Recipient signature is required.", nameof(recipientSignaturePng));
        if (shipperSignaturePng is not { Length: > 0 })
            throw new ArgumentException("Shipper signature is required.", nameof(shipperSignaturePng));

        var pdf = GeneratePdfBytes(
            delivery,
            mealCount,
            recipientConfirmedName,
            recipientSignaturePng,
            shipperSignaturePng,
            proofPhotoBytes,
            shipperDisplayName,
            notes,
            confirmedAt);

        var objectName =
            $"deliveries/{delivery.Id:D}/handover/bien-ban-ban-giao-{delivery.Id}-{confirmedAt:yyyyMMddHHmmss}.pdf";

        await using var ms = new MemoryStream(pdf);
        await _storage.UploadObjectAsync(objectName, ms, "application/pdf", cancellationToken);

        var signed = await _storage.CreateSignedUrlAsync(
            objectName,
            HttpMethod.Get,
            "application/pdf",
            TimeSpan.FromDays(365));
        return signed.Url;
    }

    internal static byte[] GeneratePdfBytes(
        Delivery delivery,
        int mealCount,
        string recipientConfirmedName,
        byte[] recipientSignaturePng,
        byte[] shipperSignaturePng,
        byte[]? proofPhotoBytes,
        string? shipperDisplayName,
        string? notes,
        DateTime confirmedAt)
    {
        var order = delivery.Order;
        var orgName = order?.Contract?.Organization?.Name ?? "—";
        var scheduled = order?.ScheduledDate ?? confirmedAt;
        var shipper = string.IsNullOrWhiteSpace(shipperDisplayName) ? "Shipper HuitMeal" : shipperDisplayName.Trim();
        var recipient = recipientConfirmedName.Trim();

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(36);
                page.DefaultTextStyle(x => x.FontSize(10.5f));

                page.Header().Element(h =>
                    VietnameseFormalPdfHeader.ComposeHeader(
                        h,
                        "BIÊN BẢN BÀN GIAO SUẤT ĂN",
                        $"Mã giao hàng: {delivery.Code ?? delivery.Id.ToString()} · Đơn hàng #{delivery.OrderId}"));

                page.Content().PaddingTop(14).Column(col =>
                {
                    col.Spacing(8);
                    col.Item().Text(
                            "Hôm nay, các bên cùng lập biên bản bàn giao suất ăn với nội dung cụ thể như sau:")
                        .LineHeight(1.35f);

                    col.Item().PaddingTop(4).Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.ConstantColumn(130);
                            cols.RelativeColumn();
                        });

                        void Row(string label, string value)
                        {
                            table.Cell().Element(Td).Text(label).SemiBold();
                            table.Cell().Element(Td).Text(value);
                        }

                        Row("Đơn vị nhận", orgName);
                        Row("Địa chỉ giao", delivery.DeliveryAddress);
                        Row("Ngày phục vụ", FmtDateTime(scheduled));
                        Row("Số suất bàn giao", mealCount.ToString("N0", Vi));
                        Row("Bên giao (Shipper)", shipper);
                        Row("Người nhận xác nhận", recipient);
                        Row("Thời gian xác nhận", FmtDateTime(confirmedAt));
                        if (!string.IsNullOrWhiteSpace(notes))
                            Row("Ghi chú", notes.Trim());
                    });

                    if (proofPhotoBytes is { Length: > 0 })
                    {
                        col.Item().PaddingTop(8).Text("Ảnh minh chứng giao hàng (PoD)").SemiBold();
                        col.Item().PaddingTop(4).AlignCenter().MaxHeight(140).Image(proofPhotoBytes).FitArea();
                    }

                    col.Item().PaddingTop(12).Text("Xác nhận của các bên").SemiBold();
                    col.Item().PaddingTop(6).Row(row =>
                    {
                        row.RelativeItem().Column(partyA =>
                        {
                            partyA.Item().AlignCenter().Text("BÊN GIAO (SHIPPER)").Bold().FontSize(10);
                            partyA.Item().AlignCenter().Text("(Ký, ghi rõ họ tên)").Italic().FontSize(9);
                            partyA.Item().PaddingTop(6).AlignCenter().Height(70).Image(shipperSignaturePng).FitArea();
                            partyA.Item().PaddingTop(4).AlignCenter().Text(shipper).SemiBold();
                            partyA.Item().AlignCenter().Text($"Ngày ký: {FmtDateTime(confirmedAt)}").FontSize(9);
                        });
                        row.ConstantItem(16);
                        row.RelativeItem().Column(partyB =>
                        {
                            partyB.Item().AlignCenter().Text("BÊN NHẬN").Bold().FontSize(10);
                            partyB.Item().AlignCenter().Text("(Ký, ghi rõ họ tên)").Italic().FontSize(9);
                            partyB.Item().PaddingTop(6).AlignCenter().Height(70).Image(recipientSignaturePng).FitArea();
                            partyB.Item().PaddingTop(4).AlignCenter().Text(recipient).SemiBold();
                            partyB.Item().AlignCenter().Text($"Ngày ký: {FmtDateTime(confirmedAt)}").FontSize(9);
                        });
                    });

                    col.Item().PaddingTop(12).Text(
                            "Người nhận xác nhận đã nhận đủ số suất ăn theo biên bản, " +
                            "đúng địa điểm và thời gian ghi nhận. Biên bản được lập trên hệ thống HuitMeal " +
                            "và có giá trị đối chiếu khi phát sinh khiếu nại.")
                        .FontSize(9.5f)
                        .LineHeight(1.35f)
                        .FontColor(Colors.Grey.Darken2);
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("HuitMeal · Biên bản bàn giao điện tử · ");
                    text.Span(FmtDateTime(VietnamTime.Now)).FontSize(8);
                });
            });
        }).GeneratePdf();
    }

    private static IContainer Td(IContainer x) =>
        x.Border(0.5f).BorderColor(Colors.Black).Padding(6);

    private static string FmtDateTime(DateTime d) => d.ToString("dd/MM/yyyy HH:mm", Vi);
}
