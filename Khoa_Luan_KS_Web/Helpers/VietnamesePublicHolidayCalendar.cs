namespace Khoa_Luan_KS_Web.Helpers;

/// <summary>Ngày lễ / sự kiện thường gặp tại Việt Nam (tham khảo khi lập kế hoạch suất ăn).</summary>
public static class VietnamesePublicHolidayCalendar
{
    public sealed record HolidayInfo(string Date, string Name, string Kind, bool IsWeekend);

    private static readonly (int Month, int Day, string Name)[] FixedSolar =
    {
        (1, 1, "Tết Dương lịch"),
        (3, 8, "Quốc tế Phụ nữ"),
        (4, 30, "Giải phóng miền Nam"),
        (5, 1, "Quốc tế Lao động"),
        (9, 2, "Quốc khánh"),
    };

    private static readonly Dictionary<int, (string DateIso, string Name)[]> MoveableByYear = new()
    {
        [2025] =
        [
            ("2025-01-28", "Tết Nguyên Đán"),
            ("2025-01-29", "Tết Nguyên Đán"),
            ("2025-01-30", "Tết Nguyên Đán"),
            ("2025-01-31", "Tết Nguyên Đán"),
            ("2025-02-01", "Tết Nguyên Đán"),
            ("2025-02-02", "Tết Nguyên Đán"),
            ("2025-02-03", "Tết Nguyên Đán"),
            ("2025-04-07", "Giỗ Tổ Hùng Vương"),
            ("2025-05-31", "Tết Đoan Ngọ"),
            ("2025-09-06", "Tết Trung thu"),
        ],
        [2026] =
        [
            ("2026-02-16", "Tết Nguyên Đán"),
            ("2026-02-17", "Tết Nguyên Đán"),
            ("2026-02-18", "Tết Nguyên Đán"),
            ("2026-02-19", "Tết Nguyên Đán"),
            ("2026-02-20", "Tết Nguyên Đán"),
            ("2026-04-26", "Giỗ Tổ Hùng Vương"),
            ("2026-06-19", "Tết Đoan Ngọ"),
            ("2026-09-25", "Tết Trung thu"),
        ],
        [2027] =
        [
            ("2027-02-05", "Tết Nguyên Đán"),
            ("2027-02-06", "Tết Nguyên Đán"),
            ("2027-02-07", "Tết Nguyên Đán"),
            ("2027-02-08", "Tết Nguyên Đán"),
            ("2027-02-09", "Tết Nguyên Đán"),
            ("2027-04-16", "Giỗ Tổ Hùng Vương"),
            ("2027-06-09", "Tết Đoan Ngọ"),
            ("2027-09-15", "Tết Trung thu"),
        ],
    };

    public static IReadOnlyDictionary<string, string> GetHolidayMap(DateOnly from, DateOnly to)
    {
        var map = new Dictionary<string, string>();
        foreach (var h in GetHolidays(from, to))
            map[h.Date] = h.Name;
        return map;
    }

    public static IReadOnlyList<HolidayInfo> GetHolidays(DateOnly from, DateOnly to)
    {
        if (to < from)
            return Array.Empty<HolidayInfo>();

        var map = new Dictionary<DateOnly, string>();

        for (var y = from.Year; y <= to.Year; y++)
        {
            foreach (var (month, day, name) in FixedSolar)
                TryAdd(map, new DateOnly(y, month, day), name);

            if (MoveableByYear.TryGetValue(y, out var moveable))
            {
                foreach (var (iso, name) in moveable)
                {
                    if (DateOnly.TryParse(iso, out var d))
                        TryAdd(map, d, name);
                }
            }
        }

        return map
            .Where(kv => kv.Key >= from && kv.Key <= to)
            .OrderBy(kv => kv.Key)
            .Select(kv => new HolidayInfo(
                kv.Key.ToString("yyyy-MM-dd"),
                kv.Value,
                "holiday",
                kv.Key.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday))
            .ToList();
    }

    public static string? GetCellNote(DateOnly date, IReadOnlyDictionary<string, string>? holidayMap)
    {
        var iso = date.ToString("yyyy-MM-dd");
        if (holidayMap != null && holidayMap.TryGetValue(iso, out var holiday))
            return holiday;

        return date.DayOfWeek switch
        {
            DayOfWeek.Saturday => "Thứ Bảy",
            DayOfWeek.Sunday => "Chủ nhật",
            _ => null,
        };
    }

    private static void TryAdd(Dictionary<DateOnly, string> map, DateOnly date, string name)
    {
        if (!map.ContainsKey(date))
            map[date] = name;
        else if (!map[date].Contains(name, StringComparison.Ordinal))
            map[date] = map[date] + " · " + name;
    }
}
