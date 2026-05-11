using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace SmartLunch.Backend.Service.Infrastructure.Pdf;

/// <summary>Đầu trang kiểu văn bản hành chính VN: quốc hiệu, tiêu ngữ, logo đơn vị (file Pdf/Assets/brand-logo.jpg nếu có).</summary>
internal static class VietnameseFormalPdfHeader
{
    internal static byte[]? TryLoadBrandLogoBytes()
    {
        var baseDir = AppContext.BaseDirectory;
        foreach (var rel in new[]
                 {
                     Path.Combine("Pdf", "Assets", "brand-logo.jpg"),
                     Path.Combine("Pdf", "Assets", "brand-logo.png"),
                 })
        {
            var full = Path.GetFullPath(Path.Combine(baseDir, rel));
            if (!File.Exists(full))
                continue;
            try
            {
                return File.ReadAllBytes(full);
            }
            catch
            {
                /* ignore */
            }
        }

        return null;
    }

    internal static void ComposeHeader(IContainer header, string documentTitle, string? subtitle)
    {
        var logo = TryLoadBrandLogoBytes();
        header.BorderBottom(1.2f).BorderColor(Colors.Blue.Darken3).PaddingBottom(10).Row(row =>
        {
            row.ConstantItem(88).Column(left =>
            {
                left.Item().Height(64).Background(Colors.White)
                    .Border(1).BorderColor(Colors.Grey.Lighten1)
                    .Padding(4)
                    .AlignMiddle()
                    .AlignCenter()
                    .Element(logoBox =>
                    {
                        if (logo != null)
                        {
                            try
                            {
                                logoBox.Image(logo).FitArea();
                                return;
                            }
                            catch
                            {
                                /* invalid image */
                            }
                        }

                        logoBox.Text("HuitMeal").Bold().FontSize(11).FontColor(Colors.Blue.Darken3);
                    });
                left.Item().AlignCenter().PaddingTop(4)
                    .Text("Công ty CP HuitMeal").FontSize(7).FontColor(Colors.Grey.Darken2);
            });

            row.RelativeItem().PaddingLeft(10).Column(right =>
            {
                right.Item().AlignCenter()
                    .Text("CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM")
                    .FontSize(11).Bold();
                right.Item().AlignCenter().PaddingTop(2)
                    .Text("Độc lập - Tự do - Hạnh phúc")
                    .FontSize(10).Italic();
                right.Item().AlignCenter().PaddingTop(6).Width(240)
                    .LineHorizontal(0.75f).LineColor(Colors.Black);
                right.Item().AlignCenter().PaddingTop(10)
                    .Text(documentTitle)
                    .FontSize(12).Bold().FontColor(Colors.Blue.Darken4);
                if (!string.IsNullOrWhiteSpace(subtitle))
                    right.Item().AlignCenter().PaddingTop(5).Text(subtitle).FontSize(8.5f).FontColor(Colors.Grey.Darken2);
            });
        });
    }
}
