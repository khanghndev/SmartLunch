namespace Khoa_Luan_KS_Web.Services;

/// <summary>
/// Phân loại và sắp xếp thực đơn tuần cho màn Manager.
/// </summary>
public static class WeeklyMenuManagerCatalog
{
    public const string TabValid = "valid";
    public const string TabCurrent = "current";
    public const string TabUpcoming = "upcoming";
    public const string TabExpired = "expired";
    public const string TabAll = "all";

    public static readonly IReadOnlySet<string> AllTabs = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        TabValid, TabCurrent, TabUpcoming, TabExpired, TabAll
    };

    public enum MenuLifecycle
    {
        Current,
        Upcoming,
        Expired
    }

    public static string NormalizeTab(string? tab)
        => !string.IsNullOrWhiteSpace(tab) && AllTabs.Contains(tab.Trim())
            ? tab.Trim().ToLowerInvariant()
            : TabValid;

    public static MenuLifecycle GetLifecycle(WeeklyMenuClientDto menu, DateTime today)
    {
        if (menu.EndDate.Date < today)
            return MenuLifecycle.Expired;
        if (menu.StartDate.Date > today)
            return MenuLifecycle.Upcoming;
        return MenuLifecycle.Current;
    }

    public static bool MatchesTab(WeeklyMenuClientDto menu, string tab, DateTime today)
    {
        var life = GetLifecycle(menu, today);
        return tab switch
        {
            TabValid => life != MenuLifecycle.Expired,
            TabCurrent => life == MenuLifecycle.Current,
            TabUpcoming => life == MenuLifecycle.Upcoming,
            TabExpired => life == MenuLifecycle.Expired,
            _ => true
        };
    }

    public static IEnumerable<WeeklyMenuClientDto> SortForTab(
        IEnumerable<WeeklyMenuClientDto> menus,
        string tab,
        DateTime today)
    {
        var list = menus.ToList();
        return tab switch
        {
            TabExpired => list
                .OrderByDescending(m => m.EndDate)
                .ThenByDescending(m => m.Id),
            TabUpcoming => list
                .OrderBy(m => m.StartDate)
                .ThenByDescending(m => m.Id),
            TabCurrent => list
                .OrderByDescending(m => m.StartDate)
                .ThenByDescending(m => m.Id),
            TabValid => list
                .OrderBy(m => GetLifecycle(m, today) switch
                {
                    MenuLifecycle.Current => 0,
                    MenuLifecycle.Upcoming => 1,
                    _ => 2
                })
                .ThenBy(m => GetLifecycle(m, today) == MenuLifecycle.Upcoming ? m.StartDate : DateTime.MaxValue)
                .ThenByDescending(m => GetLifecycle(m, today) == MenuLifecycle.Current ? m.StartDate : DateTime.MinValue)
                .ThenByDescending(m => m.Id),
            _ => list
                .OrderByDescending(m => m.StartDate)
                .ThenByDescending(m => m.Id)
        };
    }

    public static (List<WeeklyMenuClientDto> PageItems, int TotalCount) BuildPage(
        IEnumerable<WeeklyMenuClientDto> source,
        string tab,
        DateTime today,
        int page,
        int pageSize)
    {
        var filtered = source.Where(m => MatchesTab(m, tab, today));
        var sorted = SortForTab(filtered, tab, today).ToList();
        var total = sorted.Count;
        var safePage = Math.Max(1, page);
        var safeSize = Math.Clamp(pageSize, 1, 100);
        var items = sorted
            .Skip((safePage - 1) * safeSize)
            .Take(safeSize)
            .ToList();
        return (items, total);
    }

    public static (int Valid, int Current, int Upcoming, int Expired, int All) CountByTab(
        IEnumerable<WeeklyMenuClientDto> menus,
        DateTime today)
    {
        var list = menus.ToList();
        return (
            Valid: list.Count(m => MatchesTab(m, TabValid, today)),
            Current: list.Count(m => MatchesTab(m, TabCurrent, today)),
            Upcoming: list.Count(m => MatchesTab(m, TabUpcoming, today)),
            Expired: list.Count(m => MatchesTab(m, TabExpired, today)),
            All: list.Count
        );
    }
}

public class WeeklyMenuListPageViewModel
{
    public List<WeeklyMenuClientDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
    public string ActiveTab { get; set; } = WeeklyMenuManagerCatalog.TabValid;
    public string? SearchTerm { get; set; }
    public int CountValid { get; set; }
    public int CountCurrent { get; set; }
    public int CountUpcoming { get; set; }
    public int CountExpired { get; set; }
    public int CountAll { get; set; }
    public DateTime Today { get; set; } = DateTime.Today;

    public int TotalPages => PageSize > 0
        ? Math.Max(1, (int)Math.Ceiling((double)TotalCount / PageSize))
        : 1;
}
