using SmartLunch.Backend.Service.Application.DTOs.Request.OrganizationMealOrders;
using SmartLunch.Backend.Service.Application.OrganizationMealOrders;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.OrganizationMealContractOrders;

/// <summary>Đặt món tuần HĐ Period-Based — chỉ món chính (main).</summary>
public static class OrganizationMealWeeklyMealPlanBuilder
{
    public const int MinRandomMainDishesPerWeek = 5;
    public const int MaxRandomMainDishesPerWeek = 10;

    public static Dictionary<DateOnly, OrganizationMealOrderDraftDay> MergeMainOnlyMealDays(
        IEnumerable<OrganizationMealDayRequest> mealDays,
        int? requiredMealsPerDay = null,
        Func<DateOnly, int>? requiredMealsForDate = null)
    {
        var merged = new Dictionary<DateOnly, OrganizationMealOrderDraftDay>();

        foreach (var day in mealDays)
        {
            if (day.MealPlan == null)
                throw new ArgumentException($"Meal day {day.ServiceDate} must include mealPlan.");

            var side = day.MealPlan.Side ?? new List<OrganizationMealLineRequest>();
            var soup = day.MealPlan.Soup ?? new List<OrganizationMealLineRequest>();
            if (side.Count > 0 || soup.Count > 0)
            {
                throw new ArgumentException(
                    $"Meal day {day.ServiceDate}: only main dishes are allowed for period contracts.");
            }

            var main = day.MealPlan.Main ?? new List<OrganizationMealLineRequest>();
            if (main.Count == 0)
                throw new ArgumentException($"Meal day {day.ServiceDate}: at least one main dish is required.");

            if (!merged.TryGetValue(day.ServiceDate, out var draftDay))
            {
                draftDay = new OrganizationMealOrderDraftDay { ServiceDate = day.ServiceDate };
                merged[day.ServiceDate] = draftDay;
            }

            foreach (var line in main)
            {
                if (line.DishId <= 0)
                    throw new ArgumentException("Each line must include a valid DishId.");
                if (line.Quantity < 1)
                    throw new ArgumentException("Quantity must be at least 1.");

                if (draftDay.Main.Any(l => l.DishId == line.DishId))
                {
                    throw new ArgumentException(
                        $"Meal day {day.ServiceDate}: duplicate main dish {line.DishId}.");
                }

                draftDay.Main.Add(new OrganizationMealOrderDraftLine
                {
                    DishId = line.DishId,
                    Quantity = line.Quantity,
                });
            }

            var required = requiredMealsForDate?.Invoke(day.ServiceDate) ?? requiredMealsPerDay;
            if (required is int mpd)
            {
                var mainQty = draftDay.Main.Sum(l => l.Quantity);
                if (mainQty != mpd)
                {
                    throw new ArgumentException(
                        $"Day {day.ServiceDate:yyyy-MM-dd}: total main quantity must be {mpd}.");
                }
            }
        }

        return merged;
    }

    /// <summary>
    /// Chọn 5–10 món chính ngẫu nhiên cho cả tuần, phân bổ quantity theo từng ngày phục vụ.
    /// </summary>
    public static List<OrganizationMealDayRequest> BuildRandomWeeklyMainMealDays(
        IReadOnlyList<DateOnly> serviceDates,
        int mealsPerDay,
        IReadOnlyList<Dish> mainCandidates,
        Random? rng = null,
        IReadOnlyDictionary<DateOnly, int>? dailyOverrides = null)
    {
        if (serviceDates.Count == 0)
            return new List<OrganizationMealDayRequest>();
        if (mealsPerDay < 1)
            throw new ArgumentException("MealsPerDay must be at least 1.");
        if (mainCandidates.Count == 0)
            throw new ArgumentException("No main dishes available for auto-fill.");

        rng ??= Random.Shared;

        var maxPool = Math.Min(MaxRandomMainDishesPerWeek, mainCandidates.Count);
        var minPool = Math.Min(MinRandomMainDishesPerWeek, maxPool);
        var poolSize = rng.Next(minPool, maxPool + 1);

        var weeklyPool = mainCandidates
            .OrderBy(_ => rng.Next())
            .Take(poolSize)
            .ToList();

        var result = new List<OrganizationMealDayRequest>();

        foreach (var date in serviceDates.OrderBy(d => d))
        {
            var mealsToday = OrganizationMealPeriodContractSchedule.ResolveMealsForDate(
                date, mealsPerDay, dailyOverrides);
            var maxLinesToday = Math.Min(3, Math.Min(weeklyPool.Count, mealsToday));
            var linesPerDay = maxLinesToday <= 1 ? 1 : rng.Next(1, maxLinesToday + 1);

            var dayDishes = weeklyPool
                .OrderBy(_ => rng.Next())
                .Take(linesPerDay)
                .ToList();

            var quantities = SplitQuantity(mealsToday, dayDishes.Count, rng);
            var mainLines = dayDishes
                .Select((dish, i) => new OrganizationMealLineRequest
                {
                    DishId = dish.Id,
                    Quantity = quantities[i],
                })
                .Where(l => l.Quantity > 0)
                .ToList();

            result.Add(new OrganizationMealDayRequest
            {
                ServiceDate = date,
                MealPlan = new OrganizationMealPlanSlotsRequest { Main = mainLines },
            });
        }

        return result;
    }

    private static int[] SplitQuantity(int total, int parts, Random rng)
    {
        if (parts <= 0)
            return Array.Empty<int>();
        if (parts == 1)
            return new[] { total };

        var cuts = new SortedSet<int> { 0, total };
        while (cuts.Count < parts + 1)
            cuts.Add(rng.Next(1, total));

        var sorted = cuts.ToList();
        var result = new int[parts];
        for (var i = 0; i < parts; i++)
            result[i] = sorted[i + 1] - sorted[i];

        return result;
    }

    public static bool DishIsMain(Dish d) => DishHasSlot(d, "main");

    public static bool DishHasSlot(Dish d, string slot)
    {
        var want = slot.Trim().ToLowerInvariant();
        return d.DishDishCategories.Any(ddc =>
            ddc.DishCategory != null &&
            string.Equals(ddc.DishCategory.SlotKey, want, StringComparison.OrdinalIgnoreCase));
    }
}
