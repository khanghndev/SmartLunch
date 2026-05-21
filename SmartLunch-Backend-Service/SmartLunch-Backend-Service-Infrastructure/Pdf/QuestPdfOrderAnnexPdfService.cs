using System.Globalization;
using System.Net;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using SmartLunch.Backend.Service.Domain.Time;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace SmartLunch.Backend.Service.Infrastructure.Pdf;

public sealed class QuestPdfOrderAnnexPdfService : IOrderAnnexPdfService
{
    private const string DefaultSupplierLegalName = "Công ty Cổ phần HuitMeal";
    private readonly IStorageService _storage;

    static QuestPdfOrderAnnexPdfService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public QuestPdfOrderAnnexPdfService(IStorageService storage)
    {
        _storage = storage;
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

        var pdf = GeneratePdfBytes(order, buyerDisplayName, signatureDataUrl);
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
        var vi = CultureInfo.GetCultureInfo("vi-VN");
        string Dt(DateTime d) => d.ToString("dd/MM/yyyy HH:mm", vi);
        string DateOnlyFmt(DateTime d) => d.Date.ToString("dd/MM/yyyy", vi);
        string Money(decimal v) => v.ToString("N0", vi) + " đ";

        static bool IsMainPortionLine(OrderItem i) => i.UnitPrice > 0m;

        static DateOnly ResolveServiceDate(OrderItem item, DateTime scheduledUtc)
        {
            if (item.ServiceDate.HasValue)
                return item.ServiceDate.Value;

            return DateOnly.FromDateTime(scheduledUtc);
        }

        var tableLines = order.OrderItems
            .OrderBy(i => ResolveServiceDate(i, order.ScheduledDate))
            .ThenBy(i => i.Dish?.Name)
            .Select(i => (
                Name: i.Dish?.Name ?? "Món",
                i.Quantity))
            .ToList();

        var dailyPortions = order.OrderItems
            .Where(IsMainPortionLine)
            .GroupBy(i => ResolveServiceDate(i, order.ScheduledDate))
            .OrderBy(g => g.Key)
            .Select(g => (Date: g.Key, Portions: g.Sum(i => i.Quantity)))
            .ToList();

        var totalMainPortions = dailyPortions.Sum(d => d.Portions);
        var pricePerPortion = order.Contract?.MealUnitPrice
            ?? order.OrderItems.Where(IsMainPortionLine).Select(i => i.UnitPrice).FirstOrDefault();
        if (pricePerPortion <= 0m && totalMainPortions > 0 && order.TotalAmount > 0m)
        {
            pricePerPortion = decimal.Round(
                order.TotalAmount / totalMainPortions,
                2,
                MidpointRounding.AwayFromZero);
        }

        var computedGross = decimal.Round(
            pricePerPortion * totalMainPortions,
            2,
            MidpointRounding.AwayFromZero);

        var promoApps = order.PromotionApplications?.OrderBy(p => p.Id).ToList() ?? new List<OrderPromotionApplication>();
        var discountTotal = order.DiscountAmount;
        if (discountTotal <= 0m && promoApps.Count > 0)
            discountTotal = promoApps.Sum(p => p.DiscountAmount);

        var subtotalBeforePromo = order.SubtotalAmount ?? computedGross;
        if (promoApps.Count > 0 && promoApps[0].SubtotalBefore > 0m)
            subtotalBeforePromo = promoApps[0].SubtotalBefore;
        else if (!order.SubtotalAmount.HasValue && discountTotal > 0m && order.TotalAmount > 0m)
            subtotalBeforePromo = order.TotalAmount + discountTotal;

        var totalPayable = order.TotalAmount > 0m ? order.TotalAmount : Math.Max(0m, subtotalBeforePromo - discountTotal);
        var hasPromotion = discountTotal > 0m || promoApps.Count > 0;

        static string FormatDiscountLabel(OrderPromotionApplication app)
        {
            if (string.Equals(app.DiscountType, PromotionConstants.DiscountPercent, StringComparison.OrdinalIgnoreCase))
                return $"{app.DiscountValue.ToString("0.##", CultureInfo.InvariantCulture)}%";
            if (string.Equals(app.DiscountType, PromotionConstants.DiscountFixedAmount, StringComparison.OrdinalIgnoreCase))
                return app.DiscountValue.ToString("N0", CultureInfo.GetCultureInfo("vi-VN")) + " đ / suất hoặc đơn";
            return app.DiscountValue.ToString("N0", CultureInfo.GetCultureInfo("vi-VN"));
        }

        var sigPng = TryDecodeSignaturePng(signatureDataUrl);
        var invoiceRef = string.IsNullOrEmpty(order.InvoiceCode) ? $"ĐH-{order.Id}" : order.InvoiceCode!;

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(28);
                page.DefaultTextStyle(x => x.FontSize(10.5f));

                page.Header().Element(h => VietnameseFormalPdfHeader.ComposeHeader(h,
                    "PHỤ LỤC ĐẶT HÀNG VÀ XÁC NHẬN BIÊN BẢN",
                    $"Kèm theo đơn đặt suất ăn — lập lúc {Dt(VietnamTime.Now)} (GMT+7) — Số phụ lục: PL-{order.Id}/{VietnamTime.Now:yyyy}"));

                page.Footer().AlignCenter().PaddingTop(6).DefaultTextStyle(x => x.FontSize(8).FontColor(Colors.Grey.Medium))
                    .Text(t =>
                    {
                        t.Span("Trang ");
                        t.CurrentPageNumber();
                        t.Span(" / ");
                        t.TotalPages();
                    });

                page.Content().Column(col =>
                {
                    col.Spacing(10);

                    col.Item().Background(Colors.Grey.Lighten4).Border(1).BorderColor(Colors.Grey.Medium).Padding(12)
                        .Column(meta =>
                        {
                            meta.Item().Text($"Kính gửi: {buyerDisplayName}").SemiBold();
                            meta.Item().PaddingTop(4).Text(txt =>
                            {
                                txt.Span("Căn cứ nhu cầu đặt suất ăn tập thể; số hóa đơn / tham chiếu: ").FontSize(10);
                                txt.Span(invoiceRef).SemiBold();
                            });
                            meta.Item().PaddingTop(2).Text(
                                    $"Thời điểm đặt: {Dt(order.OrderDate)}  ·  Ngày dự kiến giao suất: {DateOnlyFmt(order.ScheduledDate)}")
                                .FontSize(9.5f).FontColor(Colors.Grey.Darken2);
                        });

                    col.Item().Text("I. CÁC BÊN THAM GIA").Bold().FontSize(11).FontColor(Colors.Blue.Darken3);
                    col.Item().PaddingTop(4).BorderLeft(4).BorderColor(Colors.Blue.Medium).PaddingLeft(10).Column(p =>
                    {
                        p.Spacing(3);
                        p.Item().Text($"Bên A (Cung ứng dịch vụ suất ăn): {DefaultSupplierLegalName}").FontSize(10);
                        p.Item().Text($"Bên B (Đơn vị đặt hàng): {buyerDisplayName}").SemiBold().FontSize(10);
                    });

                    col.Item().PaddingTop(8).Text("II. NỘI DUNG THỐNG NHẤT").Bold().FontSize(11).FontColor(Colors.Blue.Darken3);
                    col.Item().PaddingTop(4).PaddingLeft(4).Column(p =>
                    {
                        p.Spacing(4);
                        p.Item().Text(
                            "Điều 1. Bên B xác nhận đã rà soát khối lượng món, đơn giá và tổng giá trị theo bảng kê chi tiết tại mục III; các điều chỉnh dự trữ suất (nếu có) thực hiện theo chính sách đã thống nhất với Bên A.");
                        p.Item().Text(
                            $"Điều 2. Thời điểm thực hiện: kỳ giao ngày {DateOnlyFmt(order.ScheduledDate)}; cam kết chất lượng, an toàn thực phẩm và vệ sinh theo quy định hiện hành.");
                        p.Item().Text(
                            "Điều 3. Thanh toán, đơn giá các kỳ tiếp theo và các quyền nghĩa vụ khác không trái với hợp đồng khung / phụ lục hợp đồng đã ký giữa các bên (nếu có).");
                    });

                    col.Item().PaddingTop(8).Text("III. BẢNG KÊ MÓN ĂN VÀ GIÁ TRỊ").Bold().FontSize(11).FontColor(Colors.Blue.Darken3);
                    col.Item().PaddingTop(6).Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn(5f);
                            c.RelativeColumn(1.2f);
                        });

                        static IContainer CellStyle(IContainer x, bool header) =>
                            x.Border(1).BorderColor(Colors.Grey.Lighten1)
                                .Background(header ? Colors.Grey.Lighten3 : Colors.White)
                                .PaddingVertical(6).PaddingHorizontal(6);

                        table.Header(h =>
                        {
                            h.Cell().Element(c => CellStyle(c, true)).Text("Tên món").SemiBold().FontSize(9.5f);
                            h.Cell().Element(c => CellStyle(c, true)).AlignRight().Text("Số lượng").SemiBold().FontSize(9.5f);
                        });

                        foreach (var ln in tableLines)
                        {
                            table.Cell().Element(c => CellStyle(c, false)).Text(ln.Name).FontSize(9.5f);
                            table.Cell().Element(c => CellStyle(c, false)).AlignRight().Text(ln.Quantity.ToString(vi)).FontSize(9.5f);
                        }
                    });

                    col.Item().PaddingTop(8).Background(Colors.Blue.Lighten5).Border(1).BorderColor(Colors.Blue.Lighten2)
                        .Padding(12).Column(summary =>
                        {
                            summary.Spacing(8);
                            summary.Item().AlignCenter().Text("TỔNG HỢP SUẤT ĂN, KHUYẾN MÃI VÀ THANH TOÁN")
                                .Bold().FontSize(11).FontColor(Colors.Blue.Darken4);

                            summary.Item().Row(row =>
                            {
                                row.Spacing(12);

                                row.RelativeItem().Background(Colors.White).Border(1).BorderColor(Colors.Blue.Lighten3)
                                    .Padding(10).Column(left =>
                                    {
                                        left.Spacing(4);
                                        left.Item().Text("Suất ăn & thành tiền").Bold().FontSize(9.5f)
                                            .FontColor(Colors.Blue.Darken3);
                                        left.Item().PaddingTop(2).LineHorizontal(0.5f).LineColor(Colors.Blue.Lighten3);

                                        if (dailyPortions.Count > 0)
                                        {
                                            foreach (var day in dailyPortions)
                                            {
                                                left.Item().Text(
                                                        $"Ngày {day.Date.ToString("dd/MM/yyyy", vi)}: {day.Portions.ToString("N0", vi)} suất")
                                                    .FontSize(9.5f);
                                            }
                                        }
                                        else
                                        {
                                            left.Item().Text(
                                                    $"Ngày {DateOnly.FromDateTime(order.ScheduledDate):dd/MM/yyyy}: {totalMainPortions.ToString("N0", vi)} suất")
                                                .FontSize(9.5f);
                                        }

                                        left.Item().PaddingTop(4).Text(
                                                $"Tổng suất: {totalMainPortions.ToString("N0", vi)} suất")
                                            .SemiBold().FontSize(10);
                                        left.Item().Text($"Đơn giá / suất: {Money(pricePerPortion)}").FontSize(9.5f);
                                        left.Item().PaddingTop(6).Background(Colors.Blue.Lighten5).Padding(6).Text(txt =>
                                        {
                                            txt.Span("Thành tiền: ").FontSize(9.5f);
                                            txt.Span(Money(subtotalBeforePromo)).Bold().FontSize(10.5f).FontColor(Colors.Blue.Darken4);
                                        });
                                        left.Item().Text(
                                                $"{totalMainPortions.ToString("N0", vi)} × {Money(pricePerPortion)}")
                                            .FontSize(8.5f).FontColor(Colors.Grey.Darken1);
                                    });

                                row.RelativeItem().Background(Colors.White).Border(1).BorderColor(Colors.Green.Lighten2)
                                    .Padding(10).Column(right =>
                                    {
                                        right.Spacing(4);
                                        right.Item().Text("Khuyến mãi").Bold().FontSize(9.5f)
                                            .FontColor(Colors.Green.Darken3);
                                        right.Item().PaddingTop(2).LineHorizontal(0.5f).LineColor(Colors.Green.Lighten2);

                                        if (hasPromotion)
                                        {
                                            if (promoApps.Count > 0)
                                            {
                                                foreach (var promo in promoApps)
                                                {
                                                    right.Item().PaddingTop(2).Text(txt =>
                                                    {
                                                        txt.Span("• ").FontSize(9.5f);
                                                        txt.Span(promo.PromotionName).SemiBold().FontSize(9.5f);
                                                        if (!string.IsNullOrWhiteSpace(promo.PromotionCode))
                                                        {
                                                            txt.Span(" (").FontSize(9f);
                                                            txt.Span(promo.PromotionCode.Trim()).SemiBold().FontSize(9f);
                                                            txt.Span(")").FontSize(9f);
                                                        }
                                                    });
                                                    right.Item().Text(
                                                            $"Mức giảm: {FormatDiscountLabel(promo)}")
                                                        .FontSize(9f).FontColor(Colors.Grey.Darken2);
                                                    right.Item().Text(
                                                            $"Tiền giảm: {Money(promo.DiscountAmount)}")
                                                        .FontSize(9.5f).SemiBold().FontColor(Colors.Green.Darken3);
                                                }
                                            }
                                            else
                                            {
                                                right.Item().Text("• Có áp dụng giảm giá theo chương trình KM")
                                                    .FontSize(9.5f);
                                            }

                                            right.Item().PaddingTop(6).Background(Colors.Green.Lighten5).Padding(6).Text(txt =>
                                            {
                                                txt.Span("Tổng KM: ").FontSize(9.5f);
                                                txt.Span("−" + Money(discountTotal)).Bold().FontSize(10.5f).FontColor(Colors.Green.Darken3);
                                            });
                                        }
                                        else
                                        {
                                            right.Item().PaddingTop(8).AlignCenter().Text("Không áp dụng khuyến mãi")
                                                .Italic().FontSize(10).FontColor(Colors.Grey.Medium);
                                            right.Item().AlignCenter().Text("Thành tiền = tổng phải trả")
                                                .FontSize(8.5f).FontColor(Colors.Grey.Darken1);
                                        }
                                    });
                            });

                            summary.Item().Background(Colors.Blue.Medium).Padding(10).Row(totalRow =>
                            {
                                totalRow.RelativeItem().AlignMiddle().Text("TỔNG TIỀN PHẢI TRẢ")
                                    .Bold().FontSize(11).FontColor(Colors.White);
                                totalRow.ConstantItem(200).AlignMiddle().AlignRight()
                                    .Text(Money(totalPayable))
                                    .Bold().FontSize(13).FontColor(Colors.White);
                            });
                        });

                    col.Item().PaddingTop(10).DefaultTextStyle(x => x.Italic().FontColor(Colors.Grey.Darken2).FontSize(8.5f))
                        .Text(
                            "Ghi chú: Văn bản được hệ thống HuitMeal xuất tự động; chữ ký điện tử của Bên B được ghép vào phụ lục và lưu trữ trên hệ thống lưu trữ đám mây (mô phỏng quy trình ký số).");

                    col.Item().PaddingTop(16).LineHorizontal(1).LineColor(Colors.Grey.Medium);
                    col.Item().PaddingTop(10).Text("IV. XÁC NHẬN CỦA CÁC BÊN").Bold().FontSize(11).FontColor(Colors.Blue.Darken3);
                    col.Item().PaddingTop(8).Row(row =>
                    {
                        row.RelativeItem().Padding(10).Border(1).BorderColor(Colors.Grey.Lighten2).Column(left =>
                        {
                            left.Item().Text("Đại diện Bên A (HuitMeal)").SemiBold().FontSize(9.5f);
                            left.Item().PaddingTop(8).AlignCenter().Column(st =>
                            {
                                st.Item().AlignCenter().Width(92).Border(2).BorderColor(Colors.Red.Medium)
                                    .Padding(8).Column(inner =>
                                    {
                                        inner.Item().AlignCenter().Text("APPROVED").Bold().FontColor(Colors.Red.Medium).FontSize(7);
                                        inner.Item().AlignCenter().Text("HUITMEAL").Bold().FontColor(Colors.Red.Medium).FontSize(6);
                                    });
                                st.Item().PaddingTop(4).AlignCenter().Text("Đã xác thực hệ thống").FontSize(7.5f).FontColor(Colors.Grey.Darken1);
                            });
                        });

                        row.RelativeItem().Padding(10).Border(1).BorderColor(Colors.Grey.Lighten2).Column(right =>
                        {
                            right.Item().Text("Đại diện Bên B (Khách hàng)").SemiBold().FontSize(9.5f);
                            if (sigPng is { Length: > 0 })
                                right.Item().PaddingTop(4).AlignCenter().Height(88).Image(sigPng).FitArea();
                            else
                                right.Item().PaddingTop(24).AlignCenter().Text("(Chữ ký)").Italic().FontColor(Colors.Grey.Medium);

                            right.Item().PaddingTop(6).AlignCenter().Text(buyerDisplayName).FontSize(8.5f).SemiBold();
                            right.Item().AlignCenter().PaddingTop(2).Text($"Ngày ký: {Dt(VietnamTime.Now)} (GMT+7)").FontSize(7.5f).FontColor(Colors.Grey.Darken1);
                        });
                    });
                });
            });
        }).GeneratePdf();
    }

    private static byte[]? TryDecodeSignaturePng(string dataUrl)
    {
        if (string.IsNullOrWhiteSpace(dataUrl))
            return null;
        dataUrl = dataUrl.Trim();
        if (!dataUrl.StartsWith("data:image", StringComparison.OrdinalIgnoreCase))
            return null;
        var comma = dataUrl.IndexOf(',', StringComparison.Ordinal);
        if (comma <= 0 || comma >= dataUrl.Length - 1)
            return null;
        try
        {
            return Convert.FromBase64String(dataUrl[(comma + 1)..]);
        }
        catch
        {
            return null;
        }
    }
}
