using System.Globalization;
using System.Net;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Infrastructure.Email;

internal static class OrganizationEmailHtmlTemplates
{
    private static readonly CultureInfo Vi = CultureInfo.GetCultureInfo("vi-VN");

    public static string OrderConfirmation(
        Order order,
        string organizationName,
        string? contractNumber,
        int depositPercent,
        decimal depositAmount,
        string fullAddress)
    {
        var invoice = WebUtility.HtmlEncode(order.InvoiceCode ?? $"#{order.Id}");
        var org = WebUtility.HtmlEncode(organizationName);
        var recipient = WebUtility.HtmlEncode(order.RecipientName ?? "—");
        var phone = WebUtility.HtmlEncode(order.RecipientPhone ?? "—");
        var email = WebUtility.HtmlEncode(order.RecipientEmail ?? "—");
        var addr = WebUtility.HtmlEncode(fullAddress);
        var time = WebUtility.HtmlEncode(order.PreferredDeliveryTime ?? "Theo thỏa thuận");
        var notes = string.IsNullOrWhiteSpace(order.DeliveryNotes)
            ? "—"
            : WebUtility.HtmlEncode(order.DeliveryNotes);
        var total = FormatMoney(order.TotalAmount);
        var deposit = FormatMoney(depositAmount);
        var contractLabel = WebUtility.HtmlEncode(contractNumber ?? (order.ContractId?.ToString() ?? "—"));
        var scheduled = order.ScheduledDate.ToString("dd/MM/yyyy", Vi);

        return Wrap(
            title: "Xác nhận đặt suất ăn",
            accent: "#ea580c",
            heroTitle: "Đơn đặt suất đã được tạo thành công",
            heroSubtitle: $"Mã đơn <strong>{invoice}</strong> · {org}",
            body: $@"
<p style=""margin:0 0 16px;font-size:15px;line-height:1.6;color:#334155"">
  Cảm ơn bạn đã đặt suất ăn qua <strong>HuitMeal</strong>. Đơn hàng đã được ghi nhận. Vui lòng hoàn tất <strong>ký phụ lục đặt hàng</strong> trên cổng thông tin, sau đó thanh toán đặt cọc <strong>{depositPercent}%</strong> ({deposit} đ).
</p>
{InfoCard("Thông tin đơn", new (string, string)[]
{
    ("Mã hóa đơn", invoice),
    ("Hợp đồng", contractLabel),
    ("Ngày phục vụ đầu tiên", scheduled),
    ("Tổng thanh toán", $"{total} đ"),
    ("Đặt cọc ({depositPercent}%)", $"{deposit} đ"),
})}
{InfoCard("Giao hàng & người nhận", new (string, string)[]
{
    ("Người nhận", recipient),
    ("Số điện thoại", phone),
    ("Email", email),
    ("Địa chỉ", addr),
    ("Giờ giao mong muốn", time),
    ("Ghi chú", notes),
})}
<p style=""margin:20px 0 0;font-size:13px;line-height:1.55;color:#64748b"">
  File PDF đính kèm gồm <strong>Phần I — Hợp đồng</strong> và <strong>Phần II — Phụ lục đặt hàng</strong> (có địa chỉ giao nhận và chi tiết món).
  Bên A đã ký sẵn; <strong>Bên B vui lòng đăng nhập cổng HuitMeal để ký phụ lục</strong>, sau đó thanh toán đặt cọc {depositPercent}%.
</p>");
    }

    public static string PaymentReminder(
        Order order,
        string organizationName,
        decimal depositAmount,
        int depositPercent)
    {
        var invoice = WebUtility.HtmlEncode(order.InvoiceCode ?? $"#{order.Id}");
        var org = WebUtility.HtmlEncode(organizationName);
        var deposit = FormatMoney(depositAmount);
        var total = FormatMoney(order.TotalAmount);

        return Wrap(
            title: "Nhắc thanh toán đặt cọc",
            accent: "#7c3aed",
            heroTitle: "Nhắc thanh toán đặt cọc đơn suất ăn",
            heroSubtitle: $"Mã đơn <strong>{invoice}</strong> · {org}",
            body: $@"
<p style=""margin:0 0 16px;font-size:15px;line-height:1.6;color:#334155"">
  Phụ lục đặt hàng của bạn đã được ký. Đơn đang chờ thanh toán đặt cọc <strong>{depositPercent}%</strong> tương đương <strong style=""color:#7c3aed"">{deposit} đ</strong> (tổng đơn {total} đ).
</p>
<p style=""margin:0 0 16px;font-size:15px;line-height:1.6;color:#334155"">
  Vui lòng đăng nhập cổng <strong>Đặt suất doanh nghiệp</strong> → mở đơn hàng → chọn <strong>Thanh toán đặt cọc</strong> để quét mã QR PayOS.
</p>
{InfoCard("Tóm tắt", new (string, string)[]
{
    ("Mã hóa đơn", invoice),
    ("Tổng đơn", $"{total} đ"),
    ("Cần thanh toán ngay", $"{deposit} đ ({depositPercent}%)"),
})}");
    }

    public static string WeeklyMealSelectionReminder(
        string organizationName,
        string? contractNumber,
        DateOnly weekStart,
        DateOnly weekEnd)
    {
        var org = WebUtility.HtmlEncode(organizationName);
        var contractLabel = WebUtility.HtmlEncode(contractNumber ?? "—");
        var from = weekStart.ToString("dd/MM/yyyy", Vi);
        var to = weekEnd.ToString("dd/MM/yyyy", Vi);

        return Wrap(
            title: "Nhắc đặt món tuần tới",
            accent: "#0d9488",
            heroTitle: "Nhắc chọn suất ăn cho tuần tới",
            heroSubtitle: $"{org} · HĐ <strong>{contractLabel}</strong>",
            body: $@"
<p style=""margin:0 0 16px;font-size:15px;line-height:1.6;color:#334155"">
  Theo hợp đồng đặt suất theo kỳ, vui lòng đăng nhập cổng <strong>HuitMeal</strong> và chọn món cho tuần
  <strong>{from} – {to}</strong> trước <strong>18:00 thứ Sáu</strong>.
</p>
<p style=""margin:0;font-size:13px;line-height:1.55;color:#64748b"">
  Nếu không chọn món, hệ thống sẽ tự động gán khoảng 5–10 món chính ngẫu nhiên phân bổ theo các ngày trong tuần (cuối ngày thứ Sáu).
</p>");
    }

    private static string InfoCard(string title, (string label, string value)[] rows)
    {
        var trs = string.Join("", rows.Select(r => $@"
<tr>
  <td style=""padding:10px 12px;font-size:12px;font-weight:700;color:#64748b;width:38%;vertical-align:top;border-bottom:1px solid #f1f5f9"">{WebUtility.HtmlEncode(r.label)}</td>
  <td style=""padding:10px 12px;font-size:14px;font-weight:600;color:#0f172a;border-bottom:1px solid #f1f5f9"">{r.value}</td>
</tr>"));

        return $@"
<table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""margin:0 0 20px;border:1px solid #e2e8f0;border-radius:16px;overflow:hidden;background:#ffffff"">
  <tr><td style=""padding:14px 16px;background:#f8fafc;font-size:11px;font-weight:800;letter-spacing:0.08em;text-transform:uppercase;color:#475569"">{WebUtility.HtmlEncode(title)}</td></tr>
  <tr><td style=""padding:0""><table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"">{trs}</table></td></tr>
</table>";
    }

    private static string Wrap(string title, string accent, string heroTitle, string heroSubtitle, string body) =>
        $@"<!DOCTYPE html>
<html lang=""vi"">
<head><meta charset=""utf-8""/><meta name=""viewport"" content=""width=device-width,initial-scale=1""/><title>{WebUtility.HtmlEncode(title)}</title></head>
<body style=""margin:0;padding:0;background:#f1f5f9;font-family:'Segoe UI',Tahoma,Geneva,Verdana,sans-serif"">
<table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background:#f1f5f9;padding:32px 16px"">
<tr><td align=""center"">
<table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""max-width:600px;background:#ffffff;border-radius:24px;overflow:hidden;box-shadow:0 20px 50px -20px rgba(15,23,42,0.25)"">
  <tr><td style=""padding:28px 32px;background:linear-gradient(135deg,{accent} 0%,#0f172a 100%);color:#ffffff"">
    <p style=""margin:0 0 8px;font-size:11px;font-weight:800;letter-spacing:0.15em;text-transform:uppercase;opacity:0.85"">HuitMeal · SmartLunch</p>
    <h1 style=""margin:0 0 10px;font-size:24px;font-weight:800;line-height:1.25"">{heroTitle}</h1>
    <p style=""margin:0;font-size:14px;opacity:0.92;line-height:1.5"">{heroSubtitle}</p>
  </td></tr>
  <tr><td style=""padding:28px 32px"">{body}</td></tr>
  <tr><td style=""padding:20px 32px;background:#f8fafc;border-top:1px solid #e2e8f0;text-align:center"">
    <p style=""margin:0;font-size:12px;color:#94a3b8;line-height:1.5"">Email tự động từ hệ thống HuitMeal. Vui lòng không trả lời trực tiếp email này.<br/>© {DateTime.Now.Year} HuitMeal</p>
  </td></tr>
</table>
</td></tr>
</table>
</body>
</html>";

    private static string FormatMoney(decimal amount) =>
        amount.ToString("N0", Vi);
}
