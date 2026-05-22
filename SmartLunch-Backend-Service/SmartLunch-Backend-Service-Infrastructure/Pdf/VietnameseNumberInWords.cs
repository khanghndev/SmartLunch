namespace SmartLunch.Backend.Service.Infrastructure.Pdf;

/// <summary>Đọc số tiền bằng chữ (VNĐ) — phục vụ mẫu hợp đồng.</summary>
internal static class VietnameseNumberInWords
{
    private static readonly string[] Units = ["không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín"];
    private static readonly string[] Scales = ["", "nghìn", "triệu", "tỷ", "nghìn tỷ", "triệu tỷ"];

    public static string ReadMoney(decimal amount)
    {
        if (amount < 0) return "âm " + ReadMoney(-amount);
        var rounded = (long)Math.Round(amount, 0, MidpointRounding.AwayFromZero);
        if (rounded == 0) return "không đồng";
        return ReadNumber(rounded) + " đồng";
    }

    private static string ReadNumber(long n)
    {
        if (n == 0) return Units[0];
        var parts = new List<string>();
        var scale = 0;
        while (n > 0)
        {
            var chunk = (int)(n % 1000);
            if (chunk > 0)
            {
                var chunkText = ReadThreeDigits(chunk, scale > 0 && n >= 1000);
                var scaleName = Scales[scale];
                parts.Insert(0, string.IsNullOrEmpty(scaleName) ? chunkText : $"{chunkText} {scaleName}");
            }
            n /= 1000;
            scale++;
        }
        return string.Join(" ", parts).Trim();
    }

    private static string ReadThreeDigits(int n, bool hasHigher)
    {
        var hundred = n / 100;
        var ten = (n % 100) / 10;
        var unit = n % 10;
        var sb = new List<string>();
        if (hundred > 0)
            sb.Add($"{Units[hundred]} trăm");
        else if (hasHigher && (ten > 0 || unit > 0))
            sb.Add("không trăm");

        if (ten > 1)
            sb.Add($"{Units[ten]} mươi");
        else if (ten == 1)
            sb.Add("mười");
        else if (ten == 0 && unit > 0 && (hundred > 0 || hasHigher))
            sb.Add("lẻ");

        if (unit > 0)
        {
            if (ten >= 2 && unit == 1) sb.Add("mốt");
            else if (ten >= 1 && unit == 5) sb.Add("lăm");
            else sb.Add(Units[unit]);
        }

        return string.Join(" ", sb);
    }
}
