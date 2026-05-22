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
        Organization? buyer)
    {
        page.Size(PageSizes.A4);
        page.Margin(36);
        page.DefaultTextStyle(x => x.FontSize(10.5f).FontColor(Colors.Grey.Darken4));

        page.Header().Element(h => VietnameseFormalPdfHeader.ComposeHeader(h,
            "PHẦN I — HỢP ĐỒNG CUNG CẤP SUẤT ĂN",
            $"Số hồ sơ: {contract.ContractNumber ?? $"CTR-{contract.Id}"} · Lập {FmtDate(VietnamTime.Now)} (GMT+7)"));

        page.Footer().Element(ComposeFooter);

        page.Content().PaddingVertical(8).Column(col =>
        {
            col.Spacing(8);
            ComposeMetaTable(col, contract);
            ComposePartyBox(col, "BÊN A — BÊN CUNG CẤP", supplier.LegalName, supplier.Address, supplier.TaxId,
                supplier.ContactPerson, supplier.Phone, supplier.Email, Colors.Blue.Lighten5, Colors.Blue.Darken3);
            ComposePartyBox(col, "BÊN B — BÊN ĐẶT HÀNG", buyer?.Name, buyer?.Address, buyer?.TaxCode,
                buyer?.ContactPerson, buyer?.Phone, buyer?.ContactEmail, Colors.Green.Lighten5, Colors.Green.Darken3,
                buyer == null);

            col.Item().PaddingTop(4).Text("ĐIỀU KHOẢN THỎA THUẬN").Bold().FontSize(11).FontColor(Colors.Blue.Darken4);
            col.Item().Background(Colors.Grey.Lighten5).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(12).Column(terms =>
            {
                terms.Spacing(6);
                terms.Item().Text(Clause("1", "Phạm vi", contract.Description ?? "Cung cấp suất ăn theo nhu cầu đặt hàng của Bên B."));
                if (!string.IsNullOrWhiteSpace(contract.SupplySchedule))
                    terms.Item().Text(Clause("2", "Lịch cung cấp", contract.SupplySchedule));
                terms.Item().Text(Clause("3", "Thời hạn",
                    $"Từ {FmtDate(contract.StartDate)}" +
                    (contract.EndDate.HasValue ? $" đến {FmtDate(contract.EndDate.Value)}" : " (không thời hạn cố định)")));
                if (contract.MealUnitPrice.HasValue && contract.MealUnitPrice > 0)
                    terms.Item().Text(Clause("4", "Đơn giá / suất", Money(contract.MealUnitPrice) + " (giá thỏa thuận, không theo giá catalog)."));
                terms.Item().Text(Clause("5", "Giá trị tham chiếu", Money(contract.TotalValue)));
                if (contract.DepositAmount.HasValue && contract.DepositAmount > 0)
                    terms.Item().Text(Clause("6", "Đặt cọc", Money(contract.DepositAmount)));
                terms.Item().Text(Clause("7", "Phụ lục",
                    "Chi tiết món ăn, khối lượng, khuyến mãi và thanh toán được quy định tại Phần II — Phụ lục đặt hàng kèm theo."));
            });

            col.Item().PaddingTop(8).Text(
                    "Văn bản được hệ thống HuitMeal phát hành điện tử. Bản ký số / ký tay có giá trị pháp lý theo thỏa thuận giữa các bên.")
                .Italic().FontSize(8.5f).FontColor(Colors.Grey.Darken1);

            col.Item().PaddingTop(12).Element(c =>
                ComposeSignatureRow(c, contract, buyer?.Name, includeBuyerSignature: contract.IsDigitallySigned));
        });
    }

    internal static void ComposeAnnexPage(
        PageDescriptor page,
        Order order,
        string buyerDisplayName,
        string? signatureDataUrl)
    {
        var invoiceRef = string.IsNullOrEmpty(order.InvoiceCode) ? $"ĐH-{order.Id}" : order.InvoiceCode!;
        page.Size(PageSizes.A4);
        page.Margin(32);
        page.DefaultTextStyle(x => x.FontSize(10f));

        page.Header().Element(h => VietnameseFormalPdfHeader.ComposeHeader(h,
            "PHẦN II — PHỤ LỤC ĐẶT HÀNG",
            $"PL-{order.Id}/{VietnamTime.Now:yyyy} · Tham chiếu {invoiceRef} · {FmtDateTime(VietnamTime.Now)}"));

        page.Footer().Element(ComposeFooter);

        page.Content().Column(col =>
        {
            col.Spacing(8);

            col.Item().Background(Colors.Orange.Lighten5).Border(1).BorderColor(Colors.Orange.Lighten2).Padding(10)
                .Text($"Kính gửi: {buyerDisplayName} — Phụ lục là bộ phận không tách rời của hợp đồng tại Phần I.")
                .FontSize(9.5f);

            col.Item().Element(c => ComposeAnnexBody(c, order, buyerDisplayName, signatureDataUrl));
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

            col.Item().Text("I. CÁC BÊN").Bold().FontSize(10.5f).FontColor(Colors.Blue.Darken3);
            col.Item().PaddingLeft(6).Text($"Bên A: {SupplierLegalName}").FontSize(9.5f);
            col.Item().PaddingLeft(6).Text($"Bên B: {buyerDisplayName}").SemiBold().FontSize(9.5f);

            col.Item().PaddingTop(4).Text("II. BẢNG KÊ MÓN ĂN").Bold().FontSize(10.5f).FontColor(Colors.Blue.Darken3);
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

            col.Item().PaddingTop(6).Background(Colors.Blue.Lighten5).Border(1).BorderColor(Colors.Blue.Lighten2).Padding(10)
                .Column(summary =>
                {
                    summary.Item().AlignCenter().Text("TỔNG HỢP GIÁ TRỊ").Bold().FontSize(10.5f).FontColor(Colors.Blue.Darken4);
                    summary.Item().PaddingTop(6).Row(row =>
                    {
                        row.RelativeItem().Column(left =>
                        {
                            left.Item().Text($"Tổng suất chính: {totalPortions:N0}").FontSize(9.5f);
                            left.Item().Text($"Đơn giá/suất: {Money(pricePerPortion)}").FontSize(9.5f);
                            left.Item().Text($"Thành tiền: {Money(subtotal)}").SemiBold().FontSize(10);
                        });
                        row.RelativeItem().Column(right =>
                        {
                            if (discount > 0 || promos.Count > 0)
                            {
                                foreach (var p in promos)
                                {
                                    right.Item().Text($"KM: {p.PromotionName} (−{Money(p.DiscountAmount)})").FontSize(9f)
                                        .FontColor(Colors.Green.Darken3);
                                }
                                right.Item().Text($"Tổng giảm: −{Money(discount)}").SemiBold().FontSize(9.5f);
                            }
                            else
                                right.Item().Text("Không áp dụng KM").Italic().FontSize(9f);
                        });
                    });
                    summary.Item().PaddingTop(8).Background(Colors.Blue.Medium).Padding(8).Row(tr =>
                    {
                        tr.RelativeItem().Text("TỔNG PHẢI TRẢ").Bold().FontColor(Colors.White);
                        tr.ConstantItem(140).AlignRight().Text(Money(total)).Bold().FontSize(12).FontColor(Colors.White);
                    });
                });

            col.Item().PaddingTop(10).Text("III. XÁC NHẬN").Bold().FontSize(10.5f).FontColor(Colors.Blue.Darken3);
            col.Item().Element(c => ComposeAnnexSignatures(c, buyerDisplayName, signatureDataUrl));
        });
    }

    private static void ComposeMetaTable(ColumnDescriptor col, Contract contract)
    {
        col.Item().Table(t =>
        {
            t.ColumnsDefinition(c => { c.RelativeColumn(); c.RelativeColumn(); });
            MetaRow(t, "Mã hồ sơ (ID)", contract.Id.ToString());
            MetaRow(t, "Loại hợp đồng", contract.ContractType);
            MetaRow(t, "Số hợp đồng", contract.ContractNumber ?? "—");
            MetaRow(t, "Trạng thái", contract.Status);
            if (contract.SourceOrderId.HasValue)
                MetaRow(t, "Đơn hàng liên kết", $"#{contract.SourceOrderId}");
        });
    }

    private static void MetaRow(TableDescriptor t, string label, string value)
    {
        t.Cell().Element(MetaCell).Text(label).SemiBold().FontSize(9f);
        t.Cell().Element(MetaCell).Text(value).FontSize(9.5f);
    }

    private static IContainer MetaCell(IContainer x) =>
        x.Border(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.White).Padding(6);

    private static void ComposePartyBox(
        ColumnDescriptor col,
        string title,
        string? name,
        string? address,
        string? tax,
        string? contact,
        string? phone,
        string? email,
        string bg,
        string accent,
        bool empty = false)
    {
        col.Item().Background(bg).Border(1).BorderColor(accent).Padding(10).Column(p =>
        {
            p.Item().Text(title).Bold().FontSize(10).FontColor(accent);
            if (empty)
            {
                p.Item().PaddingTop(4).Text("(Chưa gán đơn vị khách hàng)").Italic();
                return;
            }
            p.Item().PaddingTop(4).Text(name ?? "—").SemiBold();
            if (!string.IsNullOrWhiteSpace(address)) p.Item().Text($"Địa chỉ: {address}").FontSize(9.5f);
            if (!string.IsNullOrWhiteSpace(tax)) p.Item().Text($"MST: {tax}").FontSize(9.5f);
            if (!string.IsNullOrWhiteSpace(contact)) p.Item().Text($"Liên hệ: {contact}").FontSize(9.5f);
            if (!string.IsNullOrWhiteSpace(phone)) p.Item().Text($"ĐT: {phone}").FontSize(9.5f);
            if (!string.IsNullOrWhiteSpace(email)) p.Item().Text($"Email: {email}").FontSize(9.5f);
        });
    }

    private static string Clause(string no, string title, string body) => $"Điều {no}. {title}: {body}";

    private static void ComposeSignatureRow(IContainer container, Contract contract, string? buyerName, bool includeBuyerSignature)
    {
        container.Row(row =>
        {
            row.RelativeItem().Padding(8).Border(1).BorderColor(Colors.Grey.Lighten2).Column(left =>
            {
                left.Item().Text("Đại diện Bên A").SemiBold().FontSize(9);
                left.Item().PaddingTop(8).AlignCenter().Width(90).Border(2).BorderColor(Colors.Red.Medium).Padding(8)
                    .Column(inner =>
                    {
                        inner.Item().AlignCenter().Text("APPROVED").Bold().FontSize(7).FontColor(Colors.Red.Medium);
                        inner.Item().AlignCenter().Text("HUITMEAL").Bold().FontSize(6).FontColor(Colors.Red.Medium);
                    });
                left.Item().PaddingTop(4).AlignCenter().Text("Hệ thống xác thực").FontSize(7.5f);
            });

            row.RelativeItem().Padding(8).Border(1).BorderColor(Colors.Grey.Lighten2).Column(right =>
            {
                right.Item().Text("Đại diện Bên B").SemiBold().FontSize(9);
                var sig = includeBuyerSignature ? TryDecodeSignature(contract) : null;
                if (sig is { Length: > 0 })
                    right.Item().PaddingTop(4).AlignCenter().Height(72).Image(sig).FitArea();
                else if (contract.IsDigitallySigned)
                    right.Item().PaddingTop(20).AlignCenter().Text("Đã ký số").FontColor(Colors.Green.Medium).Italic();
                else
                    right.Item().PaddingTop(24).AlignCenter().Text("CHỜ KÝ").Italic().FontColor(Colors.Grey.Medium);

                if (contract.DigitallySignedAt.HasValue)
                    right.Item().AlignCenter().Text($"Ký: {FmtDateTime(contract.DigitallySignedAt.Value)}").FontSize(7.5f);
                if (!string.IsNullOrWhiteSpace(buyerName))
                    right.Item().AlignCenter().Text(buyerName).SemiBold().FontSize(8.5f);
            });
        });
    }

    private static void ComposeAnnexSignatures(IContainer container, string buyerDisplayName, string? signatureDataUrl)
    {
        container.Row(row =>
        {
            row.RelativeItem().Padding(8).Border(1).BorderColor(Colors.Grey.Lighten2).Column(left =>
            {
                left.Item().Text("Bên A").SemiBold().FontSize(9);
                left.Item().PaddingTop(6).AlignCenter().Width(80).Border(2).BorderColor(Colors.Red.Medium).Padding(6)
                    .Text("APPROVED").Bold().FontSize(7).FontColor(Colors.Red.Medium);
            });
            row.RelativeItem().Padding(8).Border(1).BorderColor(Colors.Grey.Lighten2).Column(right =>
            {
                right.Item().Text("Bên B").SemiBold().FontSize(9);
                var sig = TryDecodeSignatureUrl(signatureDataUrl);
                if (sig is { Length: > 0 })
                    right.Item().PaddingTop(4).AlignCenter().Height(76).Image(sig).FitArea();
                else
                    right.Item().PaddingTop(20).AlignCenter().Text("(Chữ ký)").Italic();
                right.Item().AlignCenter().Text(buyerDisplayName).SemiBold().FontSize(8.5f);
                right.Item().AlignCenter().Text($"Ngày: {FmtDateTime(VietnamTime.Now)}").FontSize(7.5f);
            });
        });
    }

    private static void ComposeFooter(IContainer footer) =>
        footer.AlignCenter().DefaultTextStyle(x => x.FontSize(8).FontColor(Colors.Grey.Medium)).Text(t =>
        {
            t.Span("HuitMeal · Trang ");
            t.CurrentPageNumber();
            t.Span(" / ");
            t.TotalPages();
        });

    private static IContainer Th(IContainer x) =>
        x.Border(1).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten3).Padding(5);

    private static IContainer Td(IContainer x) =>
        x.Border(1).BorderColor(Colors.Grey.Lighten1).Padding(5);

    private static string FmtDate(DateTime d) => d.ToString("dd/MM/yyyy", Vi);
    private static string FmtDateTime(DateTime d) => d.ToString("dd/MM/yyyy HH:mm", Vi);
    private static string Money(decimal? v) => v.HasValue ? v.Value.ToString("N0", Vi) + " đ" : "—";

    private static byte[]? TryDecodeSignature(Contract contract)
    {
        var url = contract.DigitalSignature ?? contract.SignatureImage;
        return TryDecodeSignatureUrl(url);
    }

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
