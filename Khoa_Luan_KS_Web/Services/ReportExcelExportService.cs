using ClosedXML.Excel;

namespace Khoa_Luan_KS_Web.Services;

public class ReportExcelExportService
{
    private static readonly XLColor BrandGreen = XLColor.FromHtml("#16a34a");
    private static readonly XLColor BrandDark = XLColor.FromHtml("#0f172a");
    private static readonly XLColor HeaderBg = XLColor.FromHtml("#ecfdf5");
    private static readonly XLColor AltRow = XLColor.FromHtml("#f8fafc");

    public byte[] ExportMealSummary(
        IReadOnlyList<MealStatisticItemClientDto> rows,
        DateOnly from,
        DateOnly to,
        string? organizationLabel = null)
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Tong_hop_suat_an");
        WriteReportHeader(ws, "BÁO CÁO TỔNG HỢP SUẤT ĂN", from, to, organizationLabel);

        var headers = new[] { "Ngày", "Buổi", "Khách hàng", "Tổng suất", "Doanh thu (VNĐ)" };
        WriteTableHeader(ws, 6, headers);

        var r = 7;
        var idx = 0;
        foreach (var row in rows.OrderBy(x => x.Date).ThenBy(x => x.MealSlot))
        {
            ws.Cell(r, 1).Value = row.Date.ToString("dd/MM/yyyy");
            ws.Cell(r, 2).Value = FormatMealSlot(row.MealSlot);
            ws.Cell(r, 3).Value = row.OrganizationName;
            ws.Cell(r, 4).Value = row.TotalMeals;
            ws.Cell(r, 5).Value = row.TotalAmount;
            ws.Cell(r, 5).Style.NumberFormat.Format = "#,##0";
            StyleDataRow(ws, r, 5, idx++);
            r++;
        }

        WriteSummaryFooter(ws, r + 1, rows.Sum(x => x.TotalMeals), rows.Sum(x => x.TotalAmount));
        ws.Columns().AdjustToContents();
        ws.Column(3).Width = Math.Max(ws.Column(3).Width, 28);
        return SaveWorkbook(wb);
    }

    public byte[] ExportMealDetailed(
        IReadOnlyList<DetailedMealItemClientDto> rows,
        DateOnly from,
        DateOnly to,
        string? organizationLabel = null)
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Chi_tiet_mon");
        WriteReportHeader(ws, "BÁO CÁO CHI TIẾT MÓN ĂN", from, to, organizationLabel);

        var headers = new[] { "Ngày", "Buổi", "Khách hàng", "Thực đơn", "Món", "SL", "Thành tiền (VNĐ)" };
        WriteTableHeader(ws, 6, headers);

        var r = 7;
        var idx = 0;
        foreach (var row in rows.OrderBy(x => x.Date).ThenBy(x => x.DishName))
        {
            ws.Cell(r, 1).Value = row.Date.ToString("dd/MM/yyyy");
            ws.Cell(r, 2).Value = FormatMealSlot(row.MealSlot);
            ws.Cell(r, 3).Value = row.OrganizationName;
            ws.Cell(r, 4).Value = row.MenuName;
            ws.Cell(r, 5).Value = row.DishName;
            ws.Cell(r, 6).Value = row.Quantity;
            ws.Cell(r, 7).Value = row.TotalAmount;
            ws.Cell(r, 7).Style.NumberFormat.Format = "#,##0";
            StyleDataRow(ws, r, 7, idx++);
            r++;
        }

        var footer = r + 1;
        ws.Cell(footer, 1).Value = "TỔNG CỘNG";
        ws.Cell(footer, 1).Style.Font.Bold = true;
        ws.Range(footer, 1, footer, 5).Merge();
        ws.Cell(footer, 6).Value = rows.Sum(x => x.Quantity);
        ws.Cell(footer, 7).Value = rows.Sum(x => x.TotalAmount);
        ws.Cell(footer, 7).Style.NumberFormat.Format = "#,##0";
        ws.Range(footer, 1, footer, 7).Style.Fill.BackgroundColor = HeaderBg;

        ws.Columns().AdjustToContents();
        return SaveWorkbook(wb);
    }

    public byte[] ExportCashflow(GetCashflowSummaryClientResponse data, string periodLabel)
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Tai_chinh");
        WriteReportHeader(ws, "BÁO CÁO TÀI CHÍNH — DÒNG TIỀN", data.From, data.To, periodLabel);

        ws.Cell(5, 1).Value = "Chỉ tiêu";
        ws.Cell(5, 2).Value = "Giá trị (VNĐ)";
        StyleHeaderRow(ws, 5, 2);

        var metrics = new (string Label, decimal Value)[]
        {
            ("Thu từ thanh toán", data.TotalCollectedPayments),
            ("Thu khác", data.TotalTransactionIncome),
            ("Chi", data.TotalTransactionExpense),
            ("Lũy kế ròng", data.TotalNet),
        };
        var r = 6;
        foreach (var (label, value) in metrics)
        {
            ws.Cell(r, 1).Value = label;
            ws.Cell(r, 2).Value = value;
            ws.Cell(r, 2).Style.NumberFormat.Format = "#,##0";
            StyleDataRow(ws, r, 2, r - 6);
            r++;
        }

        r += 1;
        ws.Cell(r, 1).Value = "Chi tiết theo kỳ";
        ws.Range(r, 1, r, 5).Merge().Style.Font.Bold = true;
        r++;

        var headers = new[] { "Kỳ", "Thu thanh toán", "Thu khác", "Chi", "Ròng" };
        WriteTableHeader(ws, r, headers);
        r++;

        var idx = 0;
        foreach (var b in data.Buckets)
        {
            ws.Cell(r, 1).Value = b.PeriodKey;
            ws.Cell(r, 2).Value = b.CollectedPayments;
            ws.Cell(r, 3).Value = b.TransactionIncome;
            ws.Cell(r, 4).Value = b.TransactionExpense;
            ws.Cell(r, 5).Value = b.NetFlow;
            for (var c = 2; c <= 5; c++)
                ws.Cell(r, c).Style.NumberFormat.Format = "#,##0";
            StyleDataRow(ws, r, 5, idx++);
            r++;
        }

        ws.Columns().AdjustToContents();
        return SaveWorkbook(wb);
    }

    public byte[] ExportPaymentHistory(
        IReadOnlyList<PaymentHistoryEntryClientDto> entries,
        DateOnly from,
        DateOnly to)
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Lich_su_thanh_toan");
        WriteReportHeader(ws, "LỊCH SỬ GIAO DỊCH THANH TOÁN", from, to, null);

        var headers = new[] { "Ngày", "Loại", "Đối tác", "Mô tả", "Số tiền (VNĐ)", "Trạng thái" };
        WriteTableHeader(ws, 6, headers);

        var r = 7;
        var idx = 0;
        foreach (var e in entries.OrderByDescending(x => x.PaymentDate))
        {
            ws.Cell(r, 1).Value = e.PaymentDate.ToString("dd/MM/yyyy HH:mm");
            ws.Cell(r, 2).Value = e.Source;
            ws.Cell(r, 3).Value = e.OrganizationName ?? e.PartnerLegalName ?? "—";
            ws.Cell(r, 4).Value = string.IsNullOrEmpty(e.ContractNumber) ? e.Method : $"{e.Method} · {e.ContractNumber}";
            ws.Cell(r, 5).Value = e.Amount;
            ws.Cell(r, 5).Style.NumberFormat.Format = "#,##0";
            ws.Cell(r, 6).Value = e.Status;
            StyleDataRow(ws, r, 6, idx++);
            r++;
        }

        ws.Columns().AdjustToContents();
        return SaveWorkbook(wb);
    }

    private static void WriteReportHeader(IXLWorksheet ws, string title, DateOnly from, DateOnly to, string? subtitle)
    {
        ws.Cell(1, 1).Value = "SmartLunch — HuitMeal";
        ws.Cell(1, 1).Style.Font.FontColor = BrandGreen;
        ws.Cell(1, 1).Style.Font.Bold = true;

        ws.Cell(2, 1).Value = title;
        ws.Range(2, 1, 2, 6).Merge();
        ws.Cell(2, 1).Style.Font.Bold = true;
        ws.Cell(2, 1).Style.Font.FontSize = 16;
        ws.Cell(2, 1).Style.Font.FontColor = BrandDark;

        ws.Cell(3, 1).Value = $"Kỳ báo cáo: {from:dd/MM/yyyy} — {to:dd/MM/yyyy}";
        ws.Cell(4, 1).Value = $"Xuất lúc: {DateTime.Now:dd/MM/yyyy HH:mm}";
        if (!string.IsNullOrWhiteSpace(subtitle))
            ws.Cell(4, 3).Value = subtitle;
    }

    private static void WriteTableHeader(IXLWorksheet ws, int row, string[] headers)
    {
        for (var i = 0; i < headers.Length; i++)
            ws.Cell(row, i + 1).Value = headers[i];
        StyleHeaderRow(ws, row, headers.Length);
    }

    private static void StyleHeaderRow(IXLWorksheet ws, int row, int colCount)
    {
        var range = ws.Range(row, 1, row, colCount);
        range.Style.Font.Bold = true;
        range.Style.Fill.BackgroundColor = HeaderBg;
        range.Style.Border.BottomBorder = XLBorderStyleValues.Medium;
        range.Style.Border.BottomBorderColor = BrandGreen;
    }

    private static void StyleDataRow(IXLWorksheet ws, int row, int colCount, int index)
    {
        var range = ws.Range(row, 1, row, colCount);
        if (index % 2 == 1)
            range.Style.Fill.BackgroundColor = AltRow;
        range.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
        range.Style.Border.BottomBorderColor = XLColor.FromHtml("#e2e8f0");
    }

    private static void WriteSummaryFooter(IXLWorksheet ws, int row, int totalQty, decimal totalAmount)
    {
        ws.Cell(row, 1).Value = "TỔNG CỘNG";
        ws.Cell(row, 1).Style.Font.Bold = true;
        ws.Range(row, 1, row, 3).Merge();
        ws.Cell(row, 4).Value = totalQty;
        ws.Cell(row, 4).Style.Font.Bold = true;
        ws.Cell(row, 5).Value = totalAmount;
        ws.Cell(row, 5).Style.NumberFormat.Format = "#,##0";
        ws.Cell(row, 5).Style.Font.Bold = true;
        ws.Range(row, 1, row, 5).Style.Fill.BackgroundColor = HeaderBg;
    }

    private static string FormatMealSlot(string slot) => slot?.ToLowerInvariant() switch
    {
        "breakfast" or "sang" => "Sáng",
        "lunch" or "trua" => "Trưa",
        "dinner" or "chieu" or "toi" => "Tối",
        _ => string.IsNullOrWhiteSpace(slot) ? "—" : slot
    };

    private static byte[] SaveWorkbook(XLWorkbook wb)
    {
        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }
}
