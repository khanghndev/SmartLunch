using System.Globalization;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Domain.Time;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace SmartLunch.Backend.Service.Infrastructure.Pdf;

/// <summary>Bố cục hợp đồng bán thức ăn suất theo mẫu văn bản hành chính VN.</summary>
internal static class MealSupplyContractDocumentComposer
{
    private const string DefaultPlace = "TP. Hồ Chí Minh";
    private static readonly CultureInfo Vi = CultureInfo.GetCultureInfo("vi-VN");

    internal static void Compose(
        PageDescriptor page,
        Contract contract,
        Partner supplier,
        Organization? buyer,
        Order? order = null,
        string? buyerSignatureDataUrl = null)
    {
        var now = VietnamTime.Now;
        page.Size(PageSizes.A4);
        page.MarginHorizontal(50);
        page.MarginVertical(42);
        page.DefaultTextStyle(x => x
            .FontFamily("Times New Roman")
            .FontSize(12)
            .LineHeight(1.25f)
            .FontColor(Colors.Black));

        page.Footer().AlignCenter().DefaultTextStyle(x => x.FontSize(9).FontColor(Colors.Grey.Darken2))
            .Text(t =>
            {
                t.Span("HuitMeal — Trang ");
                t.CurrentPageNumber();
                t.Span(" / ");
                t.TotalPages();
            });

        page.Content().Column(col =>
        {
            col.Spacing(4);
            ComposeNationalTitle(col, now);
            ComposeContractNumber(col, contract);
            ComposeLegalBasis(col);
            ComposeOpening(col, now, buyer);
            ComposePartySeller(col, supplier);
            ComposePartyBuyer(col, buyer);
            col.Item().PaddingTop(6).Text("Cùng thỏa thuận với nhau Hợp đồng mua bán thức ăn suất với những nội dung sau:")
                .Italic();
            ComposeArticles(col, contract, supplier, buyer, order);
            col.Item().PaddingTop(16).Element(c =>
                ComposeSignatures(c, contract, buyer, buyerSignatureDataUrl));
        });
    }

    private static void ComposeNationalTitle(ColumnDescriptor col, DateTime now)
    {
        col.Item().AlignCenter().Text("CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM").Bold().FontSize(12);
        col.Item().AlignCenter().PaddingTop(2).Text("Độc lập – Tự do – Hạnh phúc").Italic().FontSize(12);
        col.Item().AlignCenter().PaddingTop(4).Text("---------------").FontSize(11);
        col.Item().AlignCenter().PaddingTop(8)
            .Text($"{DefaultPlace}, ngày {now.Day} tháng {now.Month} năm {now.Year}").Italic().FontSize(12);
        col.Item().AlignCenter().PaddingTop(12).Text("HỢP ĐỒNG BÁN").Bold().FontSize(14);
        col.Item().AlignCenter().Text("THỨC ĂN SUẤT").Bold().FontSize(14);
    }

    private static void ComposeContractNumber(ColumnDescriptor col, Contract contract)
    {
        var no = contract.ContractNumber ?? $"HĐ-{contract.Id}";
        col.Item().AlignCenter().PaddingTop(8).Text($"Số: {no}/HĐMB-{contract.StartDate.Year}").FontSize(12);
    }

    private static void ComposeLegalBasis(ColumnDescriptor col)
    {
        col.Item().PaddingTop(10);
        foreach (var line in new[]
                 {
                     "– Căn cứ Hiến pháp năm 2013;",
                     "– Căn cứ Bộ luật dân sự năm 2015;",
                     "– Căn cứ Luật thương mại năm 2005;",
                     "– Căn cứ khả năng và nhu cầu của hai bên."
                 })
            col.Item().PaddingTop(2).Text(line);
    }

    private static void ComposeOpening(ColumnDescriptor col, DateTime now, Organization? buyer)
    {
        var place = string.IsNullOrWhiteSpace(buyer?.Address) ? DefaultPlace : buyer!.Address!.Trim();
        col.Item().PaddingTop(8).Text(
            $"Hôm nay, ngày {now.Day} tháng {now.Month} năm {now.Year} tại địa chỉ: {place}, chúng tôi gồm:")
            .AlignLeft();
    }

    private static void ComposePartySeller(ColumnDescriptor col, Partner supplier)
    {
        col.Item().PaddingTop(8).Text("Bên bán (sau đây gọi là Bên A):").Bold();
        col.Item().PaddingTop(4).Text("Nếu là pháp nhân thì trình bày như sau:");
        PartyField(col, "Tên công ty", supplier.LegalName);
        PartyField(col, "Địa chỉ trụ sở chính", supplier.Address);
        PartyField(col, "Mã số thuế", supplier.TaxId);
        PartyField(col, "Số điện thoại liên hệ", supplier.Phone);
        PartyField(col, "Email", supplier.Email);
        PartyField(col, "Người đại diện theo pháp luật", supplier.ContactPerson);
        PartyField(col, "Chức vụ", "Người đại diện theo pháp luật");
    }

    private static void ComposePartyBuyer(ColumnDescriptor col, Organization? buyer)
    {
        col.Item().PaddingTop(10).Text("Bên mua (sau đây gọi là Bên B):").Bold();
        if (buyer == null)
        {
            col.Item().PaddingTop(4).Text("(Thông tin Bên B được bổ sung khi gắn đơn vị khách hàng.)").Italic();
            return;
        }

        col.Item().PaddingTop(4).Text("Nếu là pháp nhân thì trình bày như sau:");
        PartyField(col, "Tên đơn vị", buyer.Name);
        PartyField(col, "Địa chỉ trụ sở / địa điểm nhận suất", buyer.Address);
        PartyField(col, "Mã số thuế", buyer.TaxCode);
        PartyField(col, "Số điện thoại liên hệ", buyer.Phone);
        PartyField(col, "Email", buyer.ContactEmail);
        PartyField(col, "Người đại diện / liên hệ", buyer.ContactPerson);
    }

    private static void PartyField(ColumnDescriptor col, string label, string? value)
    {
        col.Item().PaddingTop(2).Text(text =>
        {
            text.Span($"{label}: ").SemiBold();
            text.Span(string.IsNullOrWhiteSpace(value) ? "………………………………………………" : value.Trim());
        });
    }

    private static void ComposeArticles(
        ColumnDescriptor col,
        Contract contract,
        Partner supplier,
        Organization? buyer,
        Order? order)
    {
        var total = contract.TotalValue ?? 0;
        var unitPrice = contract.MealUnitPrice ?? 0;
        var portions = unitPrice > 0 && total > 0
            ? (int)Math.Round(total / unitPrice, MidpointRounding.AwayFromZero)
            : EstimatePortions(order);
        var deposit = contract.DepositAmount ?? 0;
        var depositPct = total > 0 && deposit > 0
            ? Math.Round(deposit / total * 100, 1)
            : 30m;
        var schedule = string.IsNullOrWhiteSpace(contract.SupplySchedule)
            ? "từ 10 giờ 00 phút đến 11 giờ 30 phút, các ngày làm việc từ thứ hai đến thứ sáu hàng tuần"
            : contract.SupplySchedule.Trim();
        var endDate = contract.EndDate.HasValue
            ? FmtDate(contract.EndDate.Value)
            : "thời điểm thanh lý hợp đồng theo thỏa thuận";
        var deliveryPlace = string.IsNullOrWhiteSpace(buyer?.Address) ? "địa điểm do Bên B chỉ định" : buyer!.Address!.Trim();
        var desc = contract.Description ?? "cung cấp suất ăn theo nhu cầu đặt hàng của Bên B";

        ArticleTitle(col, "Điều 1. Nội dung hợp đồng");
        Body(col, $"Bên A sẽ cung cấp thức ăn suất cho Bên B trong thời hạn và phạm vi với chất lượng, số lượng được quy định tại Hợp đồng này và Phụ lục đính kèm ({desc}).");
        Body(col, "Bên B sẽ tiếp nhận thức ăn suất và thực hiện nghĩa vụ thanh toán theo quy định tại Hợp đồng này.");

        ArticleTitle(col, "Điều 2. Giá trị hợp đồng");
        Body(col,
            $"Hợp đồng này có tổng giá trị là: {Money(total)} (bằng chữ: {VietnameseNumberInWords.ReadMoney(total)}). " +
            $"Tức là {portions:N0} suất ăn (tham chiếu theo đơn đặt hàng hiện tại).");
        Body(col, "Giá trị của mỗi suất ăn được xác định theo thỏa thuận tại Điều 6 của Hợp đồng này.");
        Body(col,
            $"Các ngày từ thứ hai đến thứ sáu trong tuần trong thời hạn hợp đồng, Bên A có trách nhiệm giao suất ăn cho Bên B vào thời gian {schedule}.");

        ArticleTitle(col, "Điều 3. Phương thức thanh toán");
        Body(col, "Bên B sẽ thanh toán tiền cho Bên A theo từng đợt gắn với đơn đặt hàng và phụ lục đính kèm.");
        Body(col,
            $"Lần 1. Bên B thanh toán đặt cọc {Money(deposit)} (khoảng {depositPct:N0}% giá trị đơn hàng) trước hoặc khi xác nhận đặt hàng qua hệ thống HuitMeal (PayOS/chuyển khoản).");
        Body(col,
            $"Số tiền còn lại sẽ được thanh toán theo tiến độ giao suất ăn hoặc vào {endDate}, tức là thời điểm thanh lý hợp đồng.");
        Body(col, "Từ các lần tiếp theo, Bên B thanh toán cho Bên A số tiền tương ứng giá trị suất ăn đã nhận trong kỳ giao hàng.");
        Body(col, "Việc thanh toán được thực hiện bằng một trong các hình thức sau:");
        Bullet(col, "Chuyển khoản qua tài khoản ngân hàng do Bên A thông báo trên hệ thống HuitMeal;");
        Bullet(col, "Thanh toán trực tuyến qua cổng PayOS theo hóa đơn điện tử của đơn hàng.");

        ArticleTitle(col, "Điều 4. Quyền và nghĩa vụ của các bên");
        Body(col, "1. Quyền và nghĩa vụ của Bên A");
        Bullet(col, "Yêu cầu Bên B thực hiện nghĩa vụ thanh toán theo đúng nội dung thỏa thuận tại Hợp đồng này;");
        Bullet(col, "Đơn phương chấm dứt Hợp đồng khi Bên B không thực hiện đúng nghĩa vụ thanh toán, sau khi thông báo bằng văn bản trước 07 ngày;");
        Bullet(col, "Yêu cầu Bên B bồi thường thiệt hại khi vi phạm gây thiệt hại cho Bên A;");
        Bullet(col, "Phải giao suất ăn đúng chất lượng, đúng thời hạn, địa điểm theo Hợp đồng, trừ thỏa thuận khác bằng văn bản;");
        Body(col, "2. Quyền và nghĩa vụ của Bên B");
        Bullet(col, "Yêu cầu Bên A thực hiện đúng nghĩa vụ theo Hợp đồng và quy định pháp luật;");
        Bullet(col, "Đơn phương chấm dứt Hợp đồng khi Bên A vi phạm nghĩa vụ nghiêm trọng;");
        Bullet(col, "Yêu cầu Bên A bồi thường thiệt hại do vi phạm gây ra;");
        Bullet(col, "Phải tiếp nhận suất ăn theo thỏa thuận; trừ trường hợp Bên A giao không đúng chất lượng;");
        Bullet(col, "Phải thực hiện thanh toán theo Hợp đồng; phải bồi thường thiệt hại theo thỏa thuận.");

        ArticleTitle(col, "Điều 5. Thuế, phí, lệ phí");
        Body(col, "Toàn bộ thuế, phí, lệ phí và các chi phí khác phát sinh trong quá trình thực hiện hợp đồng do Bên A chịu trách nhiệm theo quy định pháp luật hiện hành.");

        ArticleTitle(col, "Điều 6. Tiêu chuẩn và thỏa thuận cụ thể");
        Body(col, "Những tiêu chuẩn, số lượng dưới đây được tính trên một lần giao hàng / theo phụ lục đơn hàng:");
        Body(col, $"Bên A cung cấp suất ăn với đơn giá thỏa thuận: {Money(unitPrice)}/suất (không theo giá catalog công khai).");
        col.Item().PaddingTop(4).Element(c => ComposeMealTable(c, contract, order, unitPrice));
        Body(col, "Mỗi suất ăn gồm một phần món chính kèm món phụ và canh với cùng số lượng theo từng ngày (chi tiết tại Phụ lục).");
        Body(col, "Trong đó, tiêu chuẩn thành phần suất ăn phải đáp ứng:");
        Bullet(col, "Yêu cầu chung: đáp ứng điều kiện an toàn thực phẩm theo quy định pháp luật;");
        Bullet(col, "Nguyên liệu, quy trình chế biến, bảo quản và vận chuyển đảm bảo VSATTP, có thể kiểm tra hồ sơ truy xuất khi cần;");
        Bullet(col, "Chi tiết thực đơn, món ăn, số lượng từng ngày ghi tại Phụ lục đính kèm (Phần II văn bản này).");

        ArticleTitle(col, "Điều 7. Một số thỏa thuận khác");
        Bullet(col, "Khi suất ăn không đạt chất lượng, Bên B có thể yêu cầu Bên A khắc phục, giao bù hoặc bồi thường;");
        Bullet(col, "Mọi tranh chấp được hai Bên thương lượng; không thỏa thuận được thì Tòa án có thẩm quyền tại Việt Nam giải quyết;");
        Bullet(col, $"Chất lượng suất ăn xác định tại thời điểm giao cho Bên B tại {deliveryPlace}.");

        ArticleTitle(col, "Điều 8. Trường hợp bất khả kháng");
        Body(col, "Bên A được miễn trách nhiệm bồi thường chậm giao do sự kiện bất khả kháng (thiên tai, dịch bệnh, chiến tranh, quyết định cấm của cơ quan nhà nước...) đã thông báo kịp thời.");
        Body(col, "Các trường hợp còn lại, bên vi phạm phải bồi thường thiệt hại theo quy định.");

        ArticleTitle(col, "Điều 9. Phạt vi phạm");
        Body(col,
            "Trường hợp một trong hai bên vi phạm nghĩa vụ thì bên vi phạm chịu phạt 8% giá trị phần nghĩa vụ bị vi phạm, " +
            "đồng thời bồi thường thiệt hại thực tế trong thời hạn 15 ngày kể từ ngày xác định thiệt hại.");

        ArticleTitle(col, "Điều 10. Chấm dứt hợp đồng");
        Body(col, "Các bên có quyền chấm dứt hợp đồng khi bên kia vi phạm nghĩa vụ hoặc khi hợp đồng đã hoàn thành.");

        ArticleTitle(col, "Điều 11. Hiệu lực hợp đồng");
        Body(col, "Hợp đồng được lập thành 02 bản bằng tiếng Việt (bản điện tử có giá trị tương đương), có giá trị như nhau. Sửa đổi, bổ sung phải bằng văn bản do hai bên thỏa thuận.");
        Body(col,
            $"Hợp đồng có hiệu lực từ ngày {FmtDate(contract.StartDate)}" +
            (contract.EndDate.HasValue ? $" đến ngày {FmtDate(contract.EndDate.Value)}" : " cho đến khi hai bên thanh lý.") +
            " Phụ lục đặt hàng là bộ phận không tách rời.");
    }

    private static void ComposeMealTable(IContainer container, Contract contract, Order? order, decimal unitPrice)
    {
        container.Table(t =>
        {
            t.ColumnsDefinition(c =>
            {
                c.RelativeColumn(2.4f);
                c.RelativeColumn(2.6f);
                c.RelativeColumn(1.2f);
                c.RelativeColumn(1f);
            });
            t.Header(h =>
            {
                foreach (var title in new[] { "Tên suất ăn", "Thành phần", "Giá (nghìn đ)", "Số lượng" })
                    h.Cell().Element(ContractTableHeader).Text(title).Bold().FontSize(10);
            });

            var rows = BuildMealRows(order, unitPrice);
            if (rows.Count == 0)
            {
                t.Cell().ColumnSpan(4).Element(ContractTableCell)
                    .Text("Chi tiết món ăn, số lượng theo từng ngày — xem Phụ lục đính kèm (Phần II).")
                    .Italic().FontSize(10);
            }
            else
            {
                foreach (var r in rows)
                {
                    t.Cell().Element(ContractTableCell).Text(r.Name).FontSize(10);
                    t.Cell().Element(ContractTableCell).Text(r.Components).FontSize(10);
                    t.Cell().Element(ContractTableCell).AlignRight().Text(r.PriceThousands).FontSize(10);
                    t.Cell().Element(ContractTableCell).AlignRight().Text(r.Qty).FontSize(10);
                }
            }

            var total = contract.TotalValue ?? order?.TotalAmount ?? 0;
            t.Cell().ColumnSpan(3).Element(ContractTableCell).AlignRight().Text("Tổng:").Bold().FontSize(10);
            t.Cell().Element(ContractTableCell).AlignRight().Text(Money(total)).Bold().FontSize(10);
        });
    }

    private static List<MealTableRow> BuildMealRows(Order? order, decimal unitPrice)
    {
        if (order?.OrderItems == null || order.OrderItems.Count == 0)
            return new List<MealTableRow>();

        var priceK = unitPrice > 0 ? (unitPrice / 1000m).ToString("N1", Vi) : "—";
        return order.OrderItems
            .Where(i => i.UnitPrice > 0)
            .GroupBy(i => i.Dish?.Name ?? "Suất ăn")
            .Select(g => new MealTableRow(
                g.Key,
                "Món chính (kèm món phụ, canh cùng số suất)",
                priceK,
                g.Sum(x => x.Quantity).ToString("N0", Vi)))
            .Take(20)
            .ToList();
    }

    private static void ComposeSignatures(
        IContainer container,
        Contract contract,
        Organization? buyer,
        string? buyerSignatureDataUrl)
    {
        var sigBytes = TryDecodeSignatureUrl(buyerSignatureDataUrl);
        var signedAt = sigBytes is { Length: > 0 }
            ? VietnamTime.Now
            : contract.DigitallySignedAt;

        container.Row(row =>
        {
            row.RelativeItem().Column(partyA =>
            {
                partyA.Item().AlignCenter().Text("BÊN A").Bold().FontSize(12);
                partyA.Item().AlignCenter().PaddingTop(2).Text("(Ký, ghi rõ họ tên, đóng dấu)").Italic().FontSize(10);
                partyA.Item().PaddingTop(8).Element(ProviderPartyStamp.Compose);
            });

            row.RelativeItem().Column(partyB =>
            {
                partyB.Item().AlignCenter().Text("BÊN B").Bold().FontSize(12);
                partyB.Item().AlignCenter().PaddingTop(2).Text("(Ký, ghi rõ họ tên, đóng dấu)").Italic().FontSize(10);
                if (sigBytes is { Length: > 0 })
                    partyB.Item().PaddingTop(8).AlignCenter().Height(70).Image(sigBytes).FitArea();
                else
                    partyB.Item().PaddingTop(40).AlignCenter().Text("………………………………").FontSize(11);
                partyB.Item().PaddingTop(8).AlignCenter().Text(buyer?.Name ?? "………………………………").SemiBold().FontSize(11);
                if (signedAt.HasValue)
                    partyB.Item().AlignCenter().Text($"Ngày ký: {FmtDateTime(signedAt.Value)}").FontSize(9);
            });
        });
    }

    private static void ArticleTitle(ColumnDescriptor col, string title) =>
        col.Item().PaddingTop(10).Text(title).Bold().FontSize(12);

    private static void Body(ColumnDescriptor col, string text) =>
        col.Item().PaddingTop(3).Text(text).FontSize(12).AlignLeft();

    private static void Bullet(ColumnDescriptor col, string text) =>
        col.Item().PaddingLeft(14).PaddingTop(2).Text("– " + text).FontSize(12);

    private static IContainer ContractTableHeader(IContainer x) =>
        x.Border(0.5f).BorderColor(Colors.Black).Background(Colors.Grey.Lighten4).Padding(4);

    private static IContainer ContractTableCell(IContainer x) =>
        x.Border(0.5f).BorderColor(Colors.Black).Padding(4);

    private static int EstimatePortions(Order? order)
    {
        if (order?.OrderItems == null) return 0;
        return order.OrderItems.Where(i => i.UnitPrice > 0).Sum(i => i.Quantity);
    }

    private static string FmtDate(DateTime d) => d.ToString("dd/MM/yyyy", Vi);
    private static string FmtDateTime(DateTime d) => d.ToString("dd/MM/yyyy HH:mm", Vi);
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

    private sealed record MealTableRow(string Name, string Components, string PriceThousands, string Qty);
}
