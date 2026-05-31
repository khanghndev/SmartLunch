using System.Globalization;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.OrganizationMealContractOrders;
using SmartLunch.Backend.Service.Application.OrganizationMealOrders;
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
        string? buyerSignatureDataUrl = null,
        OrganizationMealDeliveryPdfContext? delivery = null) =>
        MealSupplyContractDocumentComposer.Compose(page, contract, supplier, buyer, order, buyerSignatureDataUrl, delivery);

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
            col.Item().PaddingTop(8).Element(c => ComposeAnnexBody(
                c, order, buyerDisplayName, signatureDataUrl, OrganizationMealDeliveryPdfContext.FromOrder(order)));
        });
    }

    /// <summary>Phụ lục lịch suất/ngày khi tạo PDF hợp đồng Period-Based (chưa có đơn hoặc trước ký).</summary>
    internal static void ComposePeriodContractSchedulePage(
        PageDescriptor page,
        Contract contract,
        string buyerDisplayName,
        OrganizationMealDeliveryPdfContext? delivery,
        string? signatureDataUrl,
        Order? orderForTotals = null)
    {
        var contractNo = contract.ContractNumber ?? $"HĐ-{contract.Id}";
        page.Size(PageSizes.A4);
        page.MarginHorizontal(50);
        page.MarginVertical(42);
        page.DefaultTextStyle(x => x.FontFamily("Times New Roman").FontSize(12).LineHeight(1.25f));

        page.Footer().AlignCenter().DefaultTextStyle(x => x.FontSize(9).FontColor(Colors.Grey.Darken2))
            .Text(t =>
            {
                t.Span("HuitMeal — Phụ lục đính kèm — Trang ");
                t.CurrentPageNumber();
                t.Span(" / ");
                t.TotalPages();
            });

        page.Content().Column(col =>
        {
            col.Item().AlignCenter().Text("PHỤ LỤC ĐÍNH KÈM").Bold().FontSize(14);
            col.Item().AlignCenter().PaddingTop(2).Text("LỊCH CUNG CẤP SUẤT ĂN THEO KỲ").Bold().FontSize(13);
            col.Item().AlignCenter().PaddingTop(6).Text($"Số hợp đồng: {contractNo}").FontSize(11);
            col.Item().PaddingTop(8).Text(
                    $"Kính gửi: {buyerDisplayName}. Phụ lục này là bộ phận không tách rời của Hợp đồng bán thức ăn suất nêu trên, " +
                    $"lập ngày {FmtDateTime(VietnamTime.Now)}.")
                .Italic().FontSize(11);

            if (orderForTotals != null)
            {
                col.Item().PaddingTop(8).Element(c => ComposeAnnexBody(
                    c, orderForTotals, buyerDisplayName, signatureDataUrl, delivery));
            }
            else
            {
                col.Item().PaddingTop(8).Element(c =>
                    ComposePeriodScheduleAnnexBody(c, contract, buyerDisplayName, signatureDataUrl, delivery));
            }
        });
    }

    private static void ComposePeriodScheduleAnnexBody(
        IContainer parent,
        Contract contract,
        string buyerDisplayName,
        string? signatureDataUrl,
        OrganizationMealDeliveryPdfContext? delivery)
    {
        var periodRows = PeriodContractAnnexScheduleBuilder.BuildServiceDayRows(contract);
        var dailyPortions = periodRows.Select(r => (r.Date, r.MealCount)).ToList();
        var totalPortions = periodRows.Sum(r => r.MealCount);
        var pricePerPortion = contract.MealUnitPrice ?? 0m;
        var subtotal = decimal.Round(pricePerPortion * totalPortions, 2, MidpointRounding.AwayFromZero);
        var total = contract.TotalValue ?? subtotal;

        parent.Column(col =>
        {
            col.Spacing(8);
            col.Item().Text("I. CÁC BÊN").Bold().FontSize(12);
            col.Item().PaddingTop(2).Text($"Bên A (Bên bán): {SupplierLegalName}");
            col.Item().Text($"Bên B (Bên mua): {buyerDisplayName}").SemiBold();

            if (delivery is { HasData: true })
                col.Item().Element(c => MealSupplyContractDocumentComposer.ComposeDeliveryBlock(c, delivery));

            if (periodRows.Count > 0)
            {
                col.Item().PaddingTop(8).Text("II. LỊCH CUNG CẤP SUẤT ĂN THEO NGÀY").Bold().FontSize(12);
                var defaultMeals = contract.MealsPerDay is > 0 ? contract.MealsPerDay.Value : 1;
                var excludedCount = PeriodContractAnnexScheduleBuilder.CountExcludedDays(contract);
                col.Item().PaddingTop(4).Text(
                        $"Kỳ {FmtDateOnly(DateOnly.FromDateTime(contract.StartDate))} – {FmtDateOnly(DateOnly.FromDateTime(contract.EndDate!.Value))}; " +
                        $"mặc định {defaultMeals:N0} suất/ngày" +
                        (excludedCount > 0 ? $"; loại trừ {excludedCount:N0} ngày không cung cấp." : "."))
                    .FontSize(11);

                col.Item().PaddingTop(4).Table(schedule =>
                {
                    schedule.ColumnsDefinition(c =>
                    {
                        c.ConstantColumn(28);
                        c.RelativeColumn(1.4f);
                        c.RelativeColumn(1.2f);
                        c.ConstantColumn(52);
                        c.RelativeColumn(2.2f);
                    });
                    schedule.Header(h =>
                    {
                        h.Cell().Element(Th).Text("STT");
                        h.Cell().Element(Th).Text("Ngày");
                        h.Cell().Element(Th).Text("Thứ");
                        h.Cell().Element(Th).AlignRight().Text("Số suất");
                        h.Cell().Element(Th).Text("Ghi chú");
                    });
                    foreach (var row in periodRows)
                    {
                        schedule.Cell().Element(Td).AlignCenter().Text(row.Index.ToString());
                        schedule.Cell().Element(Td).Text(FmtDateOnly(row.Date));
                        schedule.Cell().Element(Td).Text(row.WeekdayLabel);
                        schedule.Cell().Element(Td).AlignRight().Text(row.MealCount.ToString("N0", Vi));
                        schedule.Cell().Element(Td).Text(string.IsNullOrWhiteSpace(row.Note) ? "—" : row.Note).FontSize(10);
                    }
                    schedule.Cell().ColumnSpan(3).Element(Td).AlignRight().Text("Tổng suất trong kỳ:").Bold();
                    schedule.Cell().Element(Td).AlignRight().Text(totalPortions.ToString("N0", Vi)).Bold();
                });

                col.Item().PaddingTop(4).Text(
                        "Món chính theo từng ngày sẽ được chọn và cập nhật theo từng tuần sau khi ký hợp đồng.")
                    .Italic().FontSize(10);
            }

            col.Item().PaddingTop(8).Text("III. TỔNG HỢP GIÁ TRỊ HỢP ĐỒNG").Bold().FontSize(12);
            col.Item().PaddingTop(4).Table(sum =>
            {
                sum.ColumnsDefinition(c => { c.RelativeColumn(2f); c.RelativeColumn(1f); });
                SummaryRow(sum, "Tổng suất trong kỳ", $"{totalPortions:N0}");
                SummaryRow(sum, "Đơn giá mỗi suất", Money(pricePerPortion));
                SummaryRow(sum, "Thành tiền (tham chiếu)", Money(subtotal));
                if (total != subtotal)
                    SummaryRow(sum, "TỔNG HỢP ĐỒNG (sau KM nếu có)", Money(total), bold: true);
                else
                    SummaryRow(sum, "TỔNG HỢP ĐỒNG", Money(total), bold: true);
            });

            col.Item().PaddingTop(12).Text("IV. XÁC NHẬN CỦA CÁC BÊN").Bold().FontSize(12);
            col.Item().Element(c => ComposeAnnexSignatures(c, buyerDisplayName, signatureDataUrl, delivery));
        });
    }

    internal static void ComposeAnnexBody(
        IContainer parent,
        Order order,
        string buyerDisplayName,
        string? signatureDataUrl,
        OrganizationMealDeliveryPdfContext? delivery = null)
    {
        delivery ??= OrganizationMealDeliveryPdfContext.FromOrder(order);
        var contract = order.Contract;
        var isPeriod = PeriodContractAnnexScheduleBuilder.IsPeriodContract(contract);
        var periodRows = isPeriod && contract != null
            ? PeriodContractAnnexScheduleBuilder.BuildServiceDayRows(contract)
            : new List<PeriodContractDayRow>();

        var tableLines = BuildAnnexTableLines(order);
        var bomByDish = BuildAnnexDishBom(order);
        var dailyPortions = BuildDailyPortions(order);
        if (dailyPortions.Count == 0 && periodRows.Count > 0)
            dailyPortions = periodRows.Select(r => (r.Date, r.MealCount)).ToList();

        var (subtotal, discount, total, promos, pricePerPortion, totalPortions) =
            BuildAnnexTotals(order, dailyPortions, contract, periodRows);

        parent.Column(col =>
        {
            col.Spacing(8);

            col.Item().Text("I. CÁC BÊN").Bold().FontSize(12);
            col.Item().PaddingTop(2).Text($"Bên A (Bên bán): {SupplierLegalName}");
            col.Item().Text($"Bên B (Bên mua): {buyerDisplayName}").SemiBold();

            if (delivery is { HasData: true })
                col.Item().Element(c => MealSupplyContractDocumentComposer.ComposeDeliveryBlock(c, delivery));

            if (periodRows.Count > 0 && contract != null)
            {
                col.Item().PaddingTop(8).Text("II. LỊCH CUNG CẤP SUẤT ĂN THEO NGÀY").Bold().FontSize(12);
                var defaultMeals = contract.MealsPerDay is > 0 ? contract.MealsPerDay.Value : 1;
                var excludedCount = PeriodContractAnnexScheduleBuilder.CountExcludedDays(contract);
                col.Item().PaddingTop(4).Text(
                        $"Kỳ {FmtDateOnly(DateOnly.FromDateTime(contract.StartDate))} – {FmtDateOnly(DateOnly.FromDateTime(contract.EndDate!.Value))}; " +
                        $"mặc định {defaultMeals:N0} suất/ngày" +
                        (excludedCount > 0 ? $"; loại trừ {excludedCount:N0} ngày không cung cấp." : "."))
                    .FontSize(11);

                col.Item().PaddingTop(4).Table(schedule =>
                {
                    schedule.ColumnsDefinition(c =>
                    {
                        c.ConstantColumn(28);
                        c.RelativeColumn(1.4f);
                        c.RelativeColumn(1.2f);
                        c.ConstantColumn(52);
                        c.RelativeColumn(2.2f);
                    });
                    schedule.Header(h =>
                    {
                        h.Cell().Element(Th).Text("STT");
                        h.Cell().Element(Th).Text("Ngày");
                        h.Cell().Element(Th).Text("Thứ");
                        h.Cell().Element(Th).AlignRight().Text("Số suất");
                        h.Cell().Element(Th).Text("Ghi chú");
                    });
                    foreach (var row in periodRows)
                    {
                        schedule.Cell().Element(Td).AlignCenter().Text(row.Index.ToString());
                        schedule.Cell().Element(Td).Text(FmtDateOnly(row.Date));
                        schedule.Cell().Element(Td).Text(row.WeekdayLabel);
                        schedule.Cell().Element(Td).AlignRight().Text(row.MealCount.ToString("N0", Vi));
                        schedule.Cell().Element(Td).Text(string.IsNullOrWhiteSpace(row.Note) ? "—" : row.Note).FontSize(10);
                    }
                    schedule.Cell().ColumnSpan(3).Element(Td).AlignRight().Text("Tổng suất trong kỳ:").Bold();
                    schedule.Cell().Element(Td).AlignRight().Text(totalPortions.ToString("N0", Vi)).Bold();
                });
            }

            if (tableLines.Count > 0)
            {
                col.Item().PaddingTop(8).Text(
                        periodRows.Count > 0
                            ? "III. BẢNG KÊ MÓN ĂN THEO ĐƠN ĐẶT HÀNG"
                            : "II. BẢNG KÊ MÓN ĂN THEO ĐƠN ĐẶT HÀNG")
                    .Bold().FontSize(12);
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
            }
            else if (periodRows.Count == 0)
            {
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
                    table.Cell().ColumnSpan(3).Element(Td)
                        .Text("Chi tiết món ăn theo từng ngày — xem phụ lục cập nhật khi có thực đơn.")
                        .Italic().FontSize(10);
                });
            }

            if (bomByDish.Count > 0)
            {
                col.Item().PaddingTop(10).Text("II.1. ĐỊNH LƯỢNG NGUYÊN LIỆU THEO MÓN (THEO SUẤT)").Bold().FontSize(12);
                foreach (var dish in bomByDish)
                {
                    col.Item().PaddingTop(4).Text($"- {dish.DishName} (SL: {dish.OrderQuantity:N0})").SemiBold();
                    col.Item().Table(t =>
                    {
                        t.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn(3.5f);
                            c.RelativeColumn(1.6f);
                            c.RelativeColumn(1.6f);
                        });
                        t.Header(h =>
                        {
                            h.Cell().Element(Th).Text("Nguyên liệu");
                            h.Cell().Element(Th).AlignRight().Text("ĐL / suất");
                            h.Cell().Element(Th).AlignRight().Text("Tổng");
                        });

                        foreach (var ln in dish.Ingredients)
                        {
                            t.Cell().Element(Td).Text(ln.IngredientName);
                            t.Cell().Element(Td).AlignRight().Text($"{ln.QuantityPerPortion} {ln.Unit}");
                            t.Cell().Element(Td).AlignRight().Text($"{ln.TotalQuantity} {ln.Unit}");
                        }
                    });
                }
            }

            col.Item().PaddingTop(8).Text(
                    periodRows.Count > 0 && tableLines.Count > 0
                        ? "IV. TỔNG HỢP GIÁ TRỊ ĐƠN HÀNG"
                        : "III. TỔNG HỢP GIÁ TRỊ ĐƠN HÀNG")
                .Bold().FontSize(12);
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

            col.Item().PaddingTop(12).Text(
                    periodRows.Count > 0 && tableLines.Count > 0
                        ? "V. XÁC NHẬN CỦA CÁC BÊN"
                        : "IV. XÁC NHẬN CỦA CÁC BÊN")
                .Bold().FontSize(12);
            col.Item().Element(c => ComposeAnnexSignatures(c, buyerDisplayName, signatureDataUrl, delivery));
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

    private static void ComposeAnnexSignatures(
        IContainer container,
        string buyerDisplayName,
        string? signatureDataUrl,
        OrganizationMealDeliveryPdfContext? delivery)
    {
        var sig = TryDecodeSignatureUrl(signatureDataUrl);
        var repName = !string.IsNullOrWhiteSpace(delivery?.RecipientName) ? delivery.RecipientName : buyerDisplayName;
        container.Row(row =>
        {
            row.RelativeItem().Column(partyA =>
            {
                partyA.Item().AlignCenter().Text("BÊN A").Bold();
                partyA.Item().AlignCenter().Text("(Ký, ghi rõ họ tên)").Italic().FontSize(10);
                partyA.Item().PaddingTop(8).Element(ProviderPartyStamp.Compose);
            });
            row.RelativeItem().Column(partyB =>
            {
                partyB.Item().AlignCenter().Text("BÊN B").Bold();
                partyB.Item().AlignCenter().Text("(Ký, ghi rõ họ tên)").Italic().FontSize(10);
                if (sig is { Length: > 0 })
                {
                    partyB.Item().PaddingTop(6).AlignCenter().Height(70).Image(sig).FitArea();
                    partyB.Item().PaddingTop(6).AlignCenter().Text(repName).SemiBold();
                    partyB.Item().AlignCenter().Text($"Ngày ký: {FmtDateTime(VietnamTime.Now)}").FontSize(10);
                }
                else
                {
                    partyB.Item().PaddingTop(8).AlignCenter().Text(repName).SemiBold().FontSize(11);
                    if (!string.IsNullOrWhiteSpace(delivery?.RecipientPhone))
                        partyB.Item().AlignCenter().Text($"SĐT: {delivery.RecipientPhone}").FontSize(10);
                    partyB.Item().PaddingTop(4).AlignCenter()
                        .Text("Chữ ký điện tử Bên B: xác nhận trên cổng HuitMeal").Italic().FontSize(9);
                }
            });
        });
    }

    private static IContainer Th(IContainer x) =>
        x.Border(0.5f).BorderColor(Colors.Black).Background(Colors.Grey.Lighten4).Padding(5);

    private static IContainer Td(IContainer x) =>
        x.Border(0.5f).BorderColor(Colors.Black).Padding(5);

    private static string FmtDate(DateTime d) => d.ToString("dd/MM/yyyy", Vi);
    private static string FmtDateOnly(DateOnly d) => d.ToString("dd/MM/yyyy", Vi);
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
        BuildAnnexTotals(
            Order order,
            List<(DateOnly Date, int Portions)> dailyPortions,
            Contract? contract = null,
            List<PeriodContractDayRow>? periodRows = null)
    {
        var promos = order.PromotionApplications?.OrderBy(p => p.Id).ToList() ?? new List<OrderPromotionApplication>();
        var totalPortions = dailyPortions.Sum(d => d.Portions);
        if (totalPortions <= 0 && periodRows is { Count: > 0 })
            totalPortions = periodRows.Sum(r => r.MealCount);

        var pricePerPortion = contract?.MealUnitPrice
            ?? order.Contract?.MealUnitPrice
            ?? order.OrderItems.Where(i => i.UnitPrice > 0).Select(i => i.UnitPrice).FirstOrDefault();
        if (pricePerPortion <= 0 && totalPortions > 0 && order.TotalAmount > 0)
            pricePerPortion = decimal.Round(order.TotalAmount / totalPortions, 2, MidpointRounding.AwayFromZero);

        var discount = order.DiscountAmount;
        if (discount <= 0 && promos.Count > 0) discount = promos.Sum(p => p.DiscountAmount);

        var subtotal = order.SubtotalAmount ?? 0m;
        if (subtotal <= 0 && pricePerPortion > 0 && totalPortions > 0)
            subtotal = decimal.Round(pricePerPortion * totalPortions, 2, MidpointRounding.AwayFromZero);

        var total = order.TotalAmount > 0 ? order.TotalAmount : Math.Max(0, subtotal - discount);
        return (subtotal, discount, total, promos, pricePerPortion, totalPortions);
    }

    private sealed record AnnexDishBom(
        string DishName,
        int OrderQuantity,
        List<(string IngredientName, string QuantityPerPortion, string TotalQuantity, string Unit)> Ingredients);

    private static List<AnnexDishBom> BuildAnnexDishBom(Order order)
    {
        var targetDishValueId = order.Contract?.DishValueId;

        // Aggregate order quantities by dish
        var dishQty = order.OrderItems
            .GroupBy(i => i.DishId)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Quantity));

        var outList = new List<AnnexDishBom>();
        foreach (var (dishId, qty) in dishQty.OrderBy(kv => kv.Key))
        {
            var dish = order.OrderItems.FirstOrDefault(i => i.DishId == dishId)?.Dish;
            if (dish == null)
                continue;

            var lines = dish.DishIngredients?.ToList() ?? new List<DishIngredient>();
            if (lines.Count == 0)
                continue;

            // Choose BOM lines by contract tier if available; otherwise take the first tier present.
            List<DishIngredient> picked;
            if (targetDishValueId.HasValue)
            {
                picked = lines.Where(di => di.DishValueId == targetDishValueId.Value).ToList();
            }
            else
            {
                var firstTier = lines.Select(di => di.DishValueId).OrderBy(x => x).FirstOrDefault();
                picked = lines.Where(di => di.DishValueId == firstTier).ToList();
            }

            if (picked.Count == 0)
                continue;

            var ingLines = picked
                .Where(di => di.Ingredient != null && di.Quantity > 0)
                .OrderBy(di => di.Ingredient.Name)
                .Select(di =>
                {
                    var unit = string.IsNullOrWhiteSpace(di.Unit) ? (di.Ingredient.Unit ?? "kg") : di.Unit!;
                    var per = di.Quantity.ToString("N2", Vi);
                    var total = (di.Quantity * qty).ToString("N2", Vi);
                    return (
                        IngredientName: di.Ingredient.Name,
                        QuantityPerPortion: per,
                        TotalQuantity: total,
                        Unit: unit
                    );
                })
                .ToList();

            if (ingLines.Count == 0)
                continue;

            outList.Add(new AnnexDishBom(dish.Name, qty, ingLines));
        }

        return outList;
    }
}
