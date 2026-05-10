using System.Globalization;
using System.Net;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
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
            $"contracts/c{contract.Id}/hop-dong-{contract.Id}-{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";

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

                page.Header().Column(h =>
                {
                    h.Spacing(4);
                    h.Item().AlignCenter().Text("HỢP ĐỒNG / BIÊN BẢN HỢP ĐỒNG CUNG CẤP SUẤT ĂN")
                        .SemiBold().FontSize(16);
                    h.Item().AlignCenter().Text($"Hệ thống Smart Lunch — được tự động tạo lúc {Dt(DateTime.UtcNow)} (UTC)")
                        .FontSize(9).Italic();
                });

                page.Content().PaddingVertical(16).Column(col =>
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
                });
            });
        }).GeneratePdf();
    }
}
