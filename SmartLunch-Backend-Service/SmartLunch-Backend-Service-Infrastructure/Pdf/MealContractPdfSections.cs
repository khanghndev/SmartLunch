using System.Globalization;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Domain.Time;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace SmartLunch.Backend.Service.Infrastructure.Pdf;

/// <summary>Phần I (hợp đồng) và Phần II (phụ lục) — dùng chung cho PDF đơn hoặc PDF gộp.</summary>
internal static class MealContractPdfSections
{
    private const string SupplierLegalName = "Công ty Cổ phần HuitMeal";
    private static readonly CultureInfo Vi = CultureInfo.GetCultureInfo("vi-VN");

    internal static void ComposeContractPage(
        PageDescriptor page,
        Contract contract,
        Partner supplier,
        Organization? buyer,
        Order? order = null,
        string? buyerSignatureDataUrl = null) =>
        MealSupplyContractDocumentComposer.Compose(page, contract, supplier, buyer, order, buyerSignatureDataUrl);

    internal static void ComposeAnnexPage(
        PageDescriptor page,
        Order order,
        string buyerDisplayName,
        string? signatureDataUrl)
    {
        var invoiceRef = string.IsNullOrEmpty(order.InvoiceCode) ? $"ĐH-{order.Id}" : order.InvoiceCode!;
        var contractNo = order.Contract?.ContractNumber ?? (order.ContractId.HasValue ? $"HĐ-{order.ContractId}" : "—");
        page.Size(PageSizes.A4);
        page.MarginHorizontal(50);
        page.MarginVertical(42);
        page.DefaultTextStyle(x => x.FontFamily("Times New Roman").FontSize(12).LineHeight(1.25f));

        page.Footer().AlignCenter().DefaultTextStyle(x => x.FontSize(9).FontColor(Colors.Grey.Darken2))
            .Text(t =>
            {
                t.Span("HuitMeal — Phụ lục — Trang ");
                t.CurrentPageNumber();
                t.Span(" / ");
                t.TotalPages();
            });

        page.Content().Column(col =>
        {
            col.Item().AlignCenter().Text("PHỤ LỤC ĐÍNH KÈM").Bold().FontSize(14);
            col.Item().AlignCenter().PaddingTop(2).Text("HỢP ĐỒNG BÁN THỨC ĂN SUẤT").Bold().FontSize(13);
            col.Item().AlignCenter().PaddingTop(6).Text($"Số hợp đồng: {contractNo} · Mã đơn: {invoiceRef}").FontSize(11);
            col.Item().PaddingTop(8).Text(
                    $"Kính gửi: {buyerDisplayName}. Phụ lục này là bộ phận không tách rời của Hợp đồng bán thức ăn suất nêu trên, " +
                    $"lập ngày {FmtDateTime(VietnamTime.Now)}.")
                .Italic().FontSize(11);
            col.Item().PaddingTop(8).Element(c => ComposeAnnexBody(c, order, buyerDisplayName, signatureDataUrl));
        });
    }

    internal static void ComposeAnnexBody(
        IContainer parent,
        Order order,
        string buyerDisplayName,
        string? signatureDataUrl)
    {
        var tableLines = BuildAnnexTableLines(order);
        var dailyPortions = BuildDailyPortions(order);
        var (subtotal, discount, total, promos, pricePerPortion, totalPortions) = BuildAnnexTotals(order, dailyPortions);

        parent.Column(col =>
        {
            col.Spacing(8);

            col.Item().Text("I. CÁC BÊN").Bold().FontSize(12);
            col.Item().PaddingTop(2).Text($"Bên A (Bên bán): {SupplierLegalName}");
            col.Item().Text($"Bên B (Bên mua): {buyerDisplayName}").SemiBold();

            col.Item().PaddingTop(8).Text("II. BẢNG KÊ MÓN ĂN THEO ĐƠN ĐẶT HÀNG").Bold().FontSize(12);
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.ConstantColumn(28);
                    c.RelativeColumn(5f);
                    c.RelativeColumn(1.2f);
                });
                table.Header(h =>
                {
                    h.Cell().Element(Th).Text("STT");
                    h.Cell().Element(Th).Text("Tên món");
                    h.Cell().Element(Th).AlignRight().Text("SL");
                });
                var stt = 1;
                foreach (var ln in tableLines)
                {
                    table.Cell().Element(Td).AlignCenter().Text(stt.ToString());
                    table.Cell().Element(Td).Text(ln.Name);
                    table.Cell().Element(Td).AlignRight().Text(ln.Quantity.ToString(Vi));
                    stt++;
                }
            });

            col.Item().PaddingTop(8).Text("III. TỔNG HỢP GIÁ TRỊ ĐƠN HÀNG").Bold().FontSize(12);
            col.Item().PaddingTop(4).Table(sum =>
            {
                sum.ColumnsDefinition(c => { c.RelativeColumn(2f); c.RelativeColumn(1f); });
                SummaryRow(sum, "Tổng suất chính (món có đơn giá)", $"{totalPortions:N0}");
                SummaryRow(sum, "Đơn giá mỗi suất", Money(pricePerPortion));
                SummaryRow(sum, "Thành tiền trước khuyến mãi", Money(subtotal));
                if (discount > 0 || promos.Count > 0)
                {
                    foreach (var p in promos)
                        SummaryRow(sum, $"Khuyến mãi: {p.PromotionName}", "−" + Money(p.DiscountAmount));
                    SummaryRow(sum, "Tổng giảm giá", "−" + Money(discount));
                }
                SummaryRow(sum, "TỔNG PHẢI TRẢ", Money(total), bold: true);
            });

            col.Item().PaddingTop(12).Text("IV. XÁC NHẬN CỦA CÁC BÊN").Bold().FontSize(12);
            col.Item().Element(c => ComposeAnnexSignatures(c, buyerDisplayName, signatureDataUrl));
        });
    }

    private static void SummaryRow(TableDescriptor t, string label, string value, bool bold = false)
    {
        var labelCell = t.Cell().Element(Td).Text(label);
        var valueCell = t.Cell().Element(Td).AlignRight().Text(value);
        if (bold)
        {
            labelCell.Bold();
            valueCell.Bold();
        }
    }

    private static void ComposeAnnexSignatures(IContainer container, string buyerDisplayName, string? signatureDataUrl)
    {
        var sig = TryDecodeSignatureUrl(signatureDataUrl);
        container.Row(row =>
        {
            row.RelativeItem().Column(left =>
            {
                left.Item().AlignCenter().Text("BÊN B").Bold();
                left.Item().AlignCenter().Text("(Ký, ghi rõ họ tên)").Italic().FontSize(10);
                if (sig is { Length: > 0 })
                    left.Item().PaddingTop(6).AlignCenter().Height(70).Image(sig).FitArea();
                else
                    left.Item().PaddingTop(36).AlignCenter().Text("…………………………");
                left.Item().PaddingTop(6).AlignCenter().Text(buyerDisplayName).SemiBold();
                left.Item().AlignCenter().Text($"Ngày: {FmtDateTime(VietnamTime.Now)}").FontSize(10);
            });
            row.RelativeItem().Column(right =>
            {
                right.Item().AlignCenter().Text("BÊN A").Bold();
                right.Item().AlignCenter().Text("(Ký, ghi rõ họ tên)").Italic().FontSize(10);
                right.Item().PaddingTop(8).Element(ProviderPartyStamp.Compose);
            });
        });
    }

    private static IContainer Th(IContainer x) =>
        x.Border(0.5f).BorderColor(Colors.Black).Background(Colors.Grey.Lighten4).Padding(5);

    private static IContainer Td(IContainer x) =>
        x.Border(0.5f).BorderColor(Colors.Black).Padding(5);

    private static string FmtDate(DateTime d) => d.ToString("dd/MM/yyyy", Vi);
    private static string FmtDateTime(DateTime d) => d.ToString("dd/MM/yyyy HH:mm", Vi);
    private static string Money(decimal? v) => v.HasValue ? v.Value.ToString("N0", Vi) + " VNĐ" : "—";
    private static string Money(decimal v) => v.ToString("N0", Vi) + " VNĐ";

    private static byte[]? TryDecodeSignatureUrl(string? dataUrl)
    {
        if (string.IsNullOrWhiteSpace(dataUrl) || !dataUrl.Trim().StartsWith("data:image", StringComparison.OrdinalIgnoreCase))
            return null;
        var comma = dataUrl.IndexOf(',', StringComparison.Ordinal);
        if (comma <= 0) return null;
        try { return Convert.FromBase64String(dataUrl[(comma + 1)..]); }
        catch { return null; }
    }

    private static List<(string Name, int Quantity)> BuildAnnexTableLines(Order order) =>
        order.OrderItems
            .OrderBy(i => i.ServiceDate ?? DateOnly.FromDateTime(order.ScheduledDate))
            .ThenBy(i => i.Dish?.Name)
            .Select(i => (i.Dish?.Name ?? "Món", i.Quantity))
            .ToList();

    private static List<(DateOnly Date, int Portions)> BuildDailyPortions(Order order) =>
        order.OrderItems
            .Where(i => i.UnitPrice > 0)
            .GroupBy(i => i.ServiceDate ?? DateOnly.FromDateTime(order.ScheduledDate))
            .OrderBy(g => g.Key)
            .Select(g => (g.Key, g.Sum(x => x.Quantity)))
            .ToList();

    private static (decimal Subtotal, decimal Discount, decimal Total, List<OrderPromotionApplication> Promos, decimal PricePerPortion, int TotalPortions)
        BuildAnnexTotals(Order order, List<(DateOnly Date, int Portions)> dailyPortions)
    {
        var promos = order.PromotionApplications?.OrderBy(p => p.Id).ToList() ?? new List<OrderPromotionApplication>();
        var totalPortions = dailyPortions.Sum(d => d.Portions);
        var pricePerPortion = order.Contract?.MealUnitPrice
            ?? order.OrderItems.Where(i => i.UnitPrice > 0).Select(i => i.UnitPrice).FirstOrDefault();
        if (pricePerPortion <= 0 && totalPortions > 0 && order.TotalAmount > 0)
            pricePerPortion = decimal.Round(order.TotalAmount / totalPortions, 2, MidpointRounding.AwayFromZero);

        var discount = order.DiscountAmount;
        if (discount <= 0 && promos.Count > 0) discount = promos.Sum(p => p.DiscountAmount);

        var subtotal = order.SubtotalAmount ?? decimal.Round(pricePerPortion * totalPortions, 2, MidpointRounding.AwayFromZero);
        var total = order.TotalAmount > 0 ? order.TotalAmount : Math.Max(0, subtotal - discount);
        return (subtotal, discount, total, promos, pricePerPortion, totalPortions);
    }
}
