using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace SmartLunch.Backend.Service.Infrastructure.Pdf;

/// <summary>Con dấu / xác nhận sẵn của Bên A (HuitMeal) trên hợp đồng &amp; phụ lục.</summary>
internal static class ProviderPartyStamp
{
    internal static byte[]? TryLoadStampImageBytes()
    {
        var baseDir = AppContext.BaseDirectory;
        foreach (var rel in new[]
                 {
                     Path.Combine("Pdf", "Assets", "provider-stamp.png"),
                     Path.Combine("Pdf", "Assets", "provider-stamp.jpg"),
                 })
        {
            var full = Path.GetFullPath(Path.Combine(baseDir, rel));
            if (!File.Exists(full))
                continue;
            try { return File.ReadAllBytes(full); }
            catch { /* ignore */ }
        }

        return null;
    }

    internal static void Compose(IContainer container)
    {
        var stampImage = TryLoadStampImageBytes();
        container.AlignCenter().Column(col =>
        {
            col.Item().AlignCenter().Element(box =>
            {
                if (stampImage is { Length: > 0 })
                {
                    box.Width(96).Height(96).Image(stampImage).FitArea();
                    return;
                }

                box.Width(96).Height(96)
                    .Border(2.5f).BorderColor(Colors.Red.Medium)
                    .Background(Colors.Red.Lighten5)
                    .Padding(6)
                    .AlignMiddle()
                    .AlignCenter()
                    .Column(inner =>
                    {
                        inner.Item().AlignCenter().Text("CÔNG TY CP").Bold().FontSize(7).FontColor(Colors.Red.Darken2);
                        inner.Item().AlignCenter().Text("HUITMEAL").Bold().FontSize(8).FontColor(Colors.Red.Darken2);
                        inner.Item().AlignCenter().PaddingTop(2).Text("ĐÃ KÝ").Bold().FontSize(7).FontColor(Colors.Red.Medium);
                    });
            });
            col.Item().PaddingTop(4).AlignCenter().Text("CÔNG TY CỔ PHẦN HUITMEAL").SemiBold().FontSize(10);
            col.Item().AlignCenter().Text("Đã xác thực hệ thống").Italic().FontSize(9);
        });
    }
}
