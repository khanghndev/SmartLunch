using System.Globalization;
using System.Net;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using SmartLunch.Backend.Service.Domain.Time;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
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
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(contract);
        ArgumentNullException.ThrowIfNull(supplier);
        if (contract.Id <= 0)
            throw new ArgumentException("Contract must be persisted before generating PDF.", nameof(contract));

        var pdf = BuildPdfBytes(contract, supplier, buyer);
        var objectName =
            $"contracts/c{contract.Id}/hop-dong-{contract.Id}-{VietnamTime.Now:yyyyMMddHHmmss}.pdf";

        await using var ms = new MemoryStream(pdf);
        await _storage.UploadObjectAsync(objectName, ms, "application/pdf", cancellationToken);

        var signed = await _storage.CreateSignedUrlAsync(
            objectName,
            HttpMethod.Get,
            "application/pdf",
            TimeSpan.FromDays(365));
        return signed.Url;
    }

    private static byte[] BuildPdfBytes(Contract contract, Partner supplier, Organization? buyer)
    {
        var dateFmt = CultureInfo.GetCultureInfo("vi-VN");
        string Dt(DateTime d) => d.ToString("dd/MM/yyyy", dateFmt);
        string Money(decimal? v) =>
            v.HasValue ? v.Value.ToString("N0", dateFmt) + " đ" : "(chưa ghi nhận)";

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Element(h => VietnameseFormalPdfHeader.ComposeHeader(h,
                    "HỢP ĐỒNG / BIÊN BẢN HỢP ĐỒNG CUNG CẤP SUẤT ĂN",
                    $"Văn bản điện tử — lập lúc {Dt(VietnamTime.Now)} (GMT+7) — Mã hồ sơ hợp đồng: {contract.Id}"));

                page.Footer().AlignCenter().PaddingTop(6).DefaultTextStyle(x => x.FontSize(8).FontColor(Colors.Grey.Medium))
                    .Text(t =>
                    {
                        t.Span("Trang ");
                        t.CurrentPageNumber();
                        t.Span(" / ");
                        t.TotalPages();
                    });

                page.Content().PaddingVertical(12).Column(col =>
                {
                    col.Spacing(10);

                    col.Item().Text(txt =>
                    {
                        txt.Span("Mã / ID hợp đồng trong hệ thống: ").SemiBold();
                        txt.Span($"{contract.Id}");
                    });
                    col.Item().Text(txt =>
                    {
                        txt.Span("Loại hợp đồng: ").SemiBold();
                        txt.Span(contract.ContractType);
                    });
                    col.Item().Text(txt =>
                    {
                        txt.Span("Số hợp đồng (nếu có): ").SemiBold();
                        txt.Span(contract.ContractNumber ?? "(chưa cấp số)");
                    });
                    col.Item().Text(txt =>
                    {
                        txt.Span("Trạng thái: ").SemiBold();
                        txt.Span(contract.Status);
                    });
                    col.Item().LineHorizontal(1);

                    col.Item().Text("Bên cung cấp ( nhà cung cấp dịch vụ )").SemiBold().FontSize(12);
                    col.Item().PaddingLeft(8).Column(p =>
                    {
                        p.Spacing(4);
                        p.Item().Text($"Tên pháp lý: {supplier.LegalName}");
                        if (!string.IsNullOrWhiteSpace(supplier.Address))
                            p.Item().Text($"Địa chỉ: {supplier.Address}");
                        if (!string.IsNullOrWhiteSpace(supplier.TaxId))
                            p.Item().Text($"Mã số thuế: {supplier.TaxId}");
                        if (!string.IsNullOrWhiteSpace(supplier.ContactPerson))
                            p.Item().Text($"Người liên hệ: {supplier.ContactPerson}");
                        if (!string.IsNullOrWhiteSpace(supplier.Phone))
                            p.Item().Text($"Điện thoại: {supplier.Phone}");
                        if (!string.IsNullOrWhiteSpace(supplier.Email))
                            p.Item().Text($"Email: {supplier.Email}");
                    });

                    col.Item().Text("Bên đặt hàng / khách hàng").SemiBold().FontSize(12);
                    col.Item().PaddingLeft(8).Column(p =>
                    {
                        p.Spacing(4);
                        if (buyer != null)
                        {
                            p.Item().Text($"Tên đơn vị: {buyer.Name}");
                            if (!string.IsNullOrWhiteSpace(buyer.Address))
                                p.Item().Text($"Địa chỉ: {buyer.Address}");
                            if (!string.IsNullOrWhiteSpace(buyer.TaxCode))
                                p.Item().Text($"Mã số thuế: {buyer.TaxCode}");
                            if (!string.IsNullOrWhiteSpace(buyer.ContactPerson))
                                p.Item().Text($"Người liên hệ: {buyer.ContactPerson}");
                            if (!string.IsNullOrWhiteSpace(buyer.Phone))
                                p.Item().Text($"Điện thoại: {buyer.Phone}");
                            if (!string.IsNullOrWhiteSpace(buyer.ContactEmail))
                                p.Item().Text($"Email: {buyer.ContactEmail}");
                        }
                        else
                            p.Item().Text("(Không có đơn vị khách hàng gán trên hợp đồng ở thời điểm tạo)");
                    });

                    col.Item().LineHorizontal(1);

                    col.Item().Text("Nội dung và điều khoản khung").SemiBold().FontSize(12);
                    col.Item().PaddingLeft(8).Column(p =>
                    {
                        p.Spacing(4);
                        if (!string.IsNullOrWhiteSpace(contract.Description))
                            p.Item().Text($"Mô tả: {contract.Description}");
                        if (!string.IsNullOrWhiteSpace(contract.SupplySchedule))
                            p.Item().Text($"Lịch cung cấp / khung thời gian: {contract.SupplySchedule}");
                        p.Item().Text(txt =>
                        {
                            txt.Span("Ngày bắt đầu: ").SemiBold();
                            txt.Span(Dt(contract.StartDate));
                        });
                        if (contract.EndDate.HasValue)
                            p.Item().Text($"Ngày kết thúc: {Dt(contract.EndDate!.Value.Date)}");

                        p.Item().Text(txt =>
                        {
                            txt.Span("Giá trị / gói định mức (tham chiếu): ").SemiBold();
                            txt.Span(Money(contract.TotalValue));
                        });
                        p.Item().Text(txt =>
                        {
                            txt.Span("Đặt cọc (nếu có): ").SemiBold();
                            txt.Span(Money(contract.DepositAmount));
                        });
                    });

                    col.Item().PaddingTop(24).AlignCenter().Text(
                        "Đây là bản được hệ thống xuất tự động. Văn bản pháp lý cuối cùng có thể được chỉnh sửa / ký tay / ký số điện tử theo quy trình của hai bên.")
                        .Italic().FontSize(9).AlignCenter();

                    col.Item().PaddingTop(28).LineHorizontal(1);
                    col.Item().PaddingTop(10).Text("Chữ ký xác nhận").SemiBold().FontSize(12);
                    col.Item().PaddingTop(8).Row(row =>
                    {
                        row.RelativeItem().Padding(10).Border(1).BorderColor(Colors.Grey.Lighten2).Column(left =>
                        {
                            left.Item().Text("Đại diện Bên A (HuitMeal)").SemiBold().FontSize(10);
                            left.Item().PaddingTop(10).AlignCenter().Column(st =>
                            {
                                st.Item().AlignCenter().Width(100).Border(2).BorderColor(Colors.Red.Medium)
                                    .Padding(10).Column(inner =>
                                    {
                                        inner.Item().AlignCenter().Text("APPROVED").Bold()
                                            .FontColor(Colors.Red.Medium).FontSize(8);
                                        inner.Item().AlignCenter().Text("HUITMEAL").Bold()
                                            .FontColor(Colors.Red.Medium).FontSize(7);
                                    });
                                st.Item().PaddingTop(4).AlignCenter().Text("Đã xác thực hệ thống")
                                    .FontSize(8).FontColor(Colors.Grey.Darken1);
                            });
                        });

                        row.RelativeItem().Padding(10).Border(1).BorderColor(Colors.Grey.Lighten2).Column(right =>
                        {
                            right.Item().Text("Đại diện Bên B (Khách hàng)").SemiBold().FontSize(10);
                            var sigPng = TryDecodeSignaturePng(contract);
                            if (sigPng is { Length: > 0 })
                            {
                                right.Item().PaddingTop(6).AlignCenter().Height(90)
                                    .Image(sigPng).FitArea();
                            }
                            else if (contract.IsDigitallySigned)
                            {
                                right.Item().PaddingTop(24).AlignCenter().Text("Đã ký số điện tử")
                                    .Italic().FontSize(10).FontColor(Colors.Green.Medium);
                            }
                            else
                            {
                                right.Item().PaddingTop(28).AlignCenter().Text("CHỜ BẠN KÝ")
                                    .FontSize(11).Italic().FontColor(Colors.Grey.Medium);
                            }

                            if (contract.IsDigitallySigned && contract.DigitallySignedAt.HasValue)
                            {
                                right.Item().PaddingTop(6).AlignCenter()
                                    .Text($"Thời điểm ký: {Dt(contract.DigitallySignedAt.Value)} (GMT+7)")
                                    .FontSize(8).FontColor(Colors.Grey.Darken1);
                            }

                            if (buyer != null)
                                right.Item().PaddingTop(4).AlignCenter().Text(buyer.Name).FontSize(9).SemiBold();
                        });
                    });
                });
            });
        }).GeneratePdf();
    }

    private static byte[]? TryDecodeSignaturePng(Contract contract)
    {
        var dataUrl = contract.DigitalSignature;
        if (string.IsNullOrWhiteSpace(dataUrl))
            dataUrl = contract.SignatureImage;
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
