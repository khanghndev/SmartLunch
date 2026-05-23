using Khoa_Luan_KS_Web.Models;

namespace Khoa_Luan_KS_Web.Services;

public sealed class CustomerHomeFeaturedMenuService
{
    private const string DefaultDishImage =
        "https://images.unsplash.com/photo-1546069901-ba9599a7e63c?auto=format&fit=crop&w=600&q=80";

    private static readonly (string Filter, string ProfileKey, string Label, string BadgeClass)[] Segments =
    {
        ("factory", "industrial", "Nhà máy", "bg-orange-500"),
        ("school", "org_primary_school", "Học đường", "bg-green-500"),
        ("office", "org_company", "Văn phòng", "bg-blue-500"),
    };

    private readonly BackendMasterDataClient _masterDataClient;

    public CustomerHomeFeaturedMenuService(BackendMasterDataClient masterDataClient)
    {
        _masterDataClient = masterDataClient;
    }

    public async Task<CustomerHomeIndexVm> LoadFeaturedDishesAsync(string? accessToken, CancellationToken ct = default)
    {
        var vm = new CustomerHomeIndexVm();

        if (string.IsNullOrWhiteSpace(accessToken))
        {
            vm.RequiresLogin = true;
            return vm;
        }

        try
        {
            foreach (var segment in Segments)
            {
                var dishes = await LoadSegmentDishesAsync(accessToken, segment, maxDishes: 3, ct);
                vm.FeaturedDishes.AddRange(dishes);
            }

            if (vm.FeaturedDishes.Count == 0)
            {
                vm.FeaturedDishes.AddRange(await LoadDishesFallbackAsync(accessToken, ct));
            }
        }
        catch (Exception ex)
        {
            vm.LoadError = ex.Message;
        }

        return vm;
    }

    private async Task<List<CustomerFeaturedDishVm>> LoadSegmentDishesAsync(
        string accessToken,
        (string Filter, string ProfileKey, string Label, string BadgeClass) segment,
        int maxDishes,
        CancellationToken ct)
    {
        var result = new List<CustomerFeaturedDishVm>();
        var menus = await _masterDataClient.GetWeeklyMenusAsync(
            accessToken, page: 1, pageSize: 10, customerProfileKey: segment.ProfileKey, ct: ct);

        var today = DateTime.Today;
        var menu = menus.Items.FirstOrDefault(m => m.StartDate.Date <= today && m.EndDate.Date >= today)
                   ?? menus.Items.OrderByDescending(m => m.StartDate).FirstOrDefault();

        if (menu == null)
            return result;

        var detail = await _masterDataClient.GetWeeklyMenuDetailAsync(menu.Id, accessToken, ct);
        var seen = new HashSet<int>();

        foreach (var schedule in detail.Schedules.OrderBy(s => s.Date).ThenBy(s => s.MealSlot))
        {
            if (!seen.Add(schedule.DishId))
                continue;

            var dish = schedule.Dish;
            if (string.IsNullOrWhiteSpace(dish.Name))
                continue;

            result.Add(MapScheduleToVm(schedule, menu.Id, segment));
            if (result.Count >= maxDishes)
                break;
        }

        return result;
    }

    private static CustomerFeaturedDishVm MapScheduleToVm(
        WeeklyMenuScheduleDetailClientDto schedule,
        int menuId,
        (string Filter, string ProfileKey, string Label, string BadgeClass) segment)
    {
        var dish = schedule.Dish;
        var tags = BuildTags(dish.Category, dish.Name);

        var detailUrl = schedule.Id > 0 && schedule.DishId > 0
            ? $"/Menu/DishDetail/{schedule.DishId}?menuId={menuId}&scheduleId={schedule.Id}"
            : $"/Menu/Index?menuId={menuId}&profile={Uri.EscapeDataString(segment.ProfileKey)}";

        return new CustomerFeaturedDishVm
        {
            DishId = schedule.DishId,
            MenuId = menuId,
            ScheduleId = schedule.Id,
            Name = dish.Name.Trim(),
            Description = BuildDescription(dish.Category, schedule.MealSlot),
            Price = dish.Price,
            ImageUrl = ResolveImage(dish.ImageUrl),
            SegmentFilter = segment.Filter,
            SegmentLabel = segment.Label,
            BadgeClass = segment.BadgeClass,
            Tags = tags,
            ProfileKey = segment.ProfileKey,
            DetailUrl = detailUrl,
        };
    }

    private async Task<List<CustomerFeaturedDishVm>> LoadDishesFallbackAsync(string accessToken, CancellationToken ct)
    {
        var response = await _masterDataClient.GetDishesAsync(accessToken, page: 1, pageSize: 12, isActive: true, ct: ct);
        var list = new List<CustomerFeaturedDishVm>();
        var segmentIndex = 0;

        foreach (var dish in response.Items.Where(d => d.IsActive && !string.IsNullOrWhiteSpace(d.Name)))
        {
            var segment = Segments[segmentIndex % Segments.Length];
            segmentIndex++;

            list.Add(new CustomerFeaturedDishVm
            {
                DishId = dish.Id,
                Name = dish.Name.Trim(),
                Description = dish.Description?.Trim() ?? BuildDescription(dish.Category, null),
                Price = dish.Price,
                ImageUrl = ResolveImage(dish.ImageUrl ?? dish.Images.FirstOrDefault()?.Url),
                SegmentFilter = segment.Filter,
                SegmentLabel = segment.Label,
                BadgeClass = segment.BadgeClass,
                Tags = BuildTags(dish.Category ?? dish.DietaryLabel, dish.Name),
                ProfileKey = segment.ProfileKey,
                DetailUrl = dish.Id > 0
                    ? $"/Menu/DishDetail/{dish.Id}"
                    : $"/Menu/Index?profile={Uri.EscapeDataString(segment.ProfileKey)}",
            });

            if (list.Count >= 12)
                break;
        }

        return list;
    }

    private static string ResolveImage(string? url) =>
        string.IsNullOrWhiteSpace(url) ? DefaultDishImage : url.Trim();

    private static string BuildDescription(string? category, string? mealSlot)
    {
        var parts = new List<string>();
        if (!string.IsNullOrWhiteSpace(category))
            parts.Add(category.Replace("·", ", ").Trim());
        if (!string.IsNullOrWhiteSpace(mealSlot))
            parts.Add(MenuViewText.SlotLabel(mealSlot));
        return parts.Count > 0 ? string.Join(" · ", parts) : "Suất ăn theo thực đơn tuần HuitMeal";
    }

    private static List<string> BuildTags(string? category, string dishName)
    {
        var tags = new List<string>();
        if (!string.IsNullOrWhiteSpace(category))
        {
            tags.AddRange(
                category.Split('·', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Take(2));
        }

        if (tags.Count == 0)
            tags.Add("HuitMeal");

        if (dishName.Contains("chay", StringComparison.OrdinalIgnoreCase))
            tags.Add("Chay");

        return tags.Distinct(StringComparer.OrdinalIgnoreCase).Take(3).ToList();
    }
}
