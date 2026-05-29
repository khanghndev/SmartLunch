using System.Globalization;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Khoa_Luan_KS_Web.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Policy = "ManagerArea")]
    public class MenuController : Controller
    {
        private readonly Services.BackendMasterDataClient _masterDataClient;

        public MenuController(Services.BackendMasterDataClient masterDataClient)
        {
            _masterDataClient = masterDataClient;
        }

        public IActionResult Dashboard() => View();
        /// <summary>
        /// Kho món ăn — danh sách có phân trang, lọc theo danh mục và tìm kiếm.
        /// </summary>
        public async Task<IActionResult> Dish(
            int page = 1,
            int pageSize = 12,
            string? searchTerm = null,
            string? category = null,
            CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                var response = await _masterDataClient.GetDishesAsync(token, page, pageSize, searchTerm, category, ct: ct);
                ViewData["SearchTerm"] = searchTerm;
                ViewData["Category"] = category ?? "";
                ViewData["CurrentPage"] = page;
                ViewData["PageSize"] = pageSize;
                return View(response);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                ViewData["SearchTerm"] = searchTerm;
                ViewData["Category"] = category ?? "";
                ViewData["CurrentPage"] = page;
                ViewData["PageSize"] = pageSize;
                return View(new Services.GetDishesResponse());
            }
        }
        public IActionResult History() => View();
        public IActionResult Ingredients() => View();
        public IActionResult DishForm(string id) => View();

        /// <summary>
        /// Danh sách thực đơn tuần: lọc theo trạng thái (còn hiệu lực, đang áp dụng, sắp tới, hết hạn).
        /// </summary>
        public async Task<IActionResult> Weekly(
            int page = 1,
            int pageSize = 12,
            string? searchTerm = null,
            string? tab = null,
            CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth", new { area = "" });

            var activeTab = Services.WeeklyMenuManagerCatalog.NormalizeTab(tab);
            var today = DateTime.Today;

            try
            {
                const int fetchSize = 500;
                var response = await _masterDataClient.GetWeeklyMenusAsync(
                    token, 1, fetchSize, searchTerm, ct: ct);
                var allMenus = response.Items ?? new List<Services.WeeklyMenuClientDto>();

                var counts = Services.WeeklyMenuManagerCatalog.CountByTab(allMenus, today);
                var (pageItems, total) = Services.WeeklyMenuManagerCatalog.BuildPage(
                    allMenus, activeTab, today, page, pageSize);

                var model = new Services.WeeklyMenuListPageViewModel
                {
                    Items = pageItems,
                    TotalCount = total,
                    Page = Math.Max(1, page),
                    PageSize = pageSize,
                    ActiveTab = activeTab,
                    SearchTerm = searchTerm,
                    CountValid = counts.Valid,
                    CountCurrent = counts.Current,
                    CountUpcoming = counts.Upcoming,
                    CountExpired = counts.Expired,
                    CountAll = counts.All,
                    Today = today
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new Services.WeeklyMenuListPageViewModel
                {
                    ActiveTab = activeTab,
                    SearchTerm = searchTerm,
                    Page = page,
                    PageSize = pageSize,
                    Today = today
                });
            }
        }

        /// <summary>
        /// Form tạo thực đơn tuần thủ công (không qua AI).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                await PrepareCreateViewAsync(token, ct);
                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Weekly));
            }
        }

        /// <summary>
        /// Lưu thực đơn tuần thủ công.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            DateTime startDate,
            DateTime endDate,
            int? customerTypeId,
            string? description,
            string menuType = "General",
            string? schedulesJson = null,
            string? daySelectionsJson = null,
            CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth", new { area = "" });

            schedulesJson = GetFormJsonField(schedulesJson, "schedulesJson");
            daySelectionsJson = GetFormJsonField(daySelectionsJson, "daySelectionsJson");

            List<Services.CreateMenuScheduleItemClientRequest> schedules;
            WeeklyMenuCreateDraft draftForError;
            try
            {
                schedules = ResolveSchedulesFromPost(schedulesJson, daySelectionsJson);
                draftForError = BuildCreateDraftFromPost(
                    startDate, endDate, customerTypeId, description, menuType,
                    schedules, schedulesJson, daySelectionsJson);
            }
            catch
            {
                var partialDraft = BuildCreateDraftFromPost(
                    startDate, endDate, customerTypeId, description, menuType,
                    new List<Services.CreateMenuScheduleItemClientRequest>(),
                    schedulesJson, daySelectionsJson);
                return await ReturnCreateWithErrorAsync(token, partialDraft, "Dữ liệu lịch món không hợp lệ.", ct);
            }

            var draft = draftForError;

            if (schedules.Count == 0)
            {
                return await ReturnCreateWithErrorAsync(
                    token, draft,
                    "Không nhận được danh sách món từ form. Vui lòng chọn lại món cho từng ngày rồi bấm Lưu.",
                    ct);
            }

            var missingDays = GetDaysWithoutDishes(startDate.Date, endDate.Date, schedules);
            if (missingDays.Count > 0)
            {
                var dayList = string.Join(", ", missingDays.Select(d => d.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)));
                return await ReturnCreateWithErrorAsync(
                    token, draft,
                    $"Mỗi ngày trong khoảng thực đơn cần ít nhất một món. Còn thiếu: {dayList}.",
                    ct);
            }

            try
            {
                var request = new Services.CreateWeeklyMenuClientRequest
                {
                    StartDate = startDate.Date,
                    EndDate = endDate.Date,
                    MenuType = string.IsNullOrWhiteSpace(menuType) ? "General" : menuType.Trim(),
                    CustomerTypeId = customerTypeId,
                    Description = description,
                    Schedules = schedules.Select(s => new Services.CreateMenuScheduleItemClientRequest
                    {
                        Date = s.Date.Date,
                        MealSlot = string.IsNullOrWhiteSpace(s.MealSlot) ? "lunch" : s.MealSlot,
                        DishId = s.DishId
                    }).ToList()
                };

                var result = await _masterDataClient.CreateWeeklyMenuAsync(request, token, ct);
                var menuId = result?.WeeklyMenu?.Id ?? 0;
                if (menuId <= 0)
                {
                    return await ReturnCreateWithErrorAsync(token, draft, "Tạo thực đơn thất bại.", ct);
                }

                TempData["Success"] = "Đã tạo thực đơn tuần thành công.";
                return RedirectToAction(nameof(Detail), new { id = menuId });
            }
            catch (Exception ex)
            {
                return await ReturnCreateWithErrorAsync(token, draft, ex.Message, ct);
            }
        }

        private static readonly JsonSerializerOptions JsonDraftOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        private sealed class WeeklyMenuCreateDraft
        {
            public DateTime StartDate { get; init; }
            public DateTime EndDate { get; init; }
            public int? CustomerTypeId { get; init; }
            public string? Description { get; init; }
            public string MenuType { get; init; } = "General";
            public Dictionary<string, List<int>> DaySelections { get; init; } = new();
        }

        private sealed class SchedulePostItem
        {
            public string? Date { get; set; }
            public string? MealSlot { get; set; }
            public int DishId { get; set; }
        }

        /// <summary>
        /// Đọc trường JSON từ form — ưu tiên Request.Form (tránh model binder lấy hidden rỗng); nếu trùng key lấy bản cuối có dữ liệu.
        /// </summary>
        private string? GetFormJsonField(string? bound, string formKey)
        {
            static bool HasPayload(string? value) =>
                !string.IsNullOrWhiteSpace(value)
                && value != "[]"
                && value != "{}";

            static string? PickPayload(Microsoft.Extensions.Primitives.StringValues values)
            {
                if (values.Count == 0)
                    return null;

                for (var i = values.Count - 1; i >= 0; i--)
                {
                    if (HasPayload(values[i]))
                        return values[i];
                }

                return null;
            }

            if (Request.Form.TryGetValue(formKey, out var directValues))
            {
                var picked = PickPayload(directValues);
                if (picked != null)
                    return picked;
            }

            foreach (var key in Request.Form.Keys)
            {
                if (!string.Equals(key, formKey, StringComparison.OrdinalIgnoreCase))
                    continue;

                var picked = PickPayload(Request.Form[key]);
                if (picked != null)
                    return picked;
            }

            return HasPayload(bound) ? bound : null;
        }

        private static List<DateTime> GetDaysWithoutDishes(
            DateTime start,
            DateTime end,
            List<Services.CreateMenuScheduleItemClientRequest> schedules)
        {
            var covered = schedules
                .Where(s => s.DishId > 0)
                .Select(s => s.Date.Date)
                .ToHashSet();

            var missing = new List<DateTime>();
            for (var d = start; d <= end; d = d.AddDays(1))
            {
                if (!covered.Contains(d))
                    missing.Add(d);
            }

            return missing;
        }

        private static WeeklyMenuCreateDraft BuildCreateDraftFromPost(
            DateTime startDate,
            DateTime endDate,
            int? customerTypeId,
            string? description,
            string menuType,
            List<Services.CreateMenuScheduleItemClientRequest> schedules,
            string? schedulesJson,
            string? daySelectionsJson)
        {
            var daySelections = MergeDaySelectionsFromPost(schedulesJson, daySelectionsJson);
            if (daySelections.Count == 0 && schedules.Count > 0)
            {
                daySelections = BuildDaySelectionsDictionary(schedules);
            }

            return new WeeklyMenuCreateDraft
            {
                StartDate = startDate.Date,
                EndDate = endDate.Date,
                CustomerTypeId = customerTypeId,
                Description = description,
                MenuType = string.IsNullOrWhiteSpace(menuType) ? "General" : menuType.Trim(),
                DaySelections = daySelections
            };
        }

        private static Dictionary<string, List<int>> BuildDaySelectionsDictionary(
            List<Services.CreateMenuScheduleItemClientRequest> schedules)
        {
            var dict = new Dictionary<string, List<int>>(StringComparer.Ordinal);
            foreach (var s in schedules.Where(x => x.DishId > 0))
            {
                var key = s.Date.Date.ToString("yyyy-MM-dd");
                if (!dict.TryGetValue(key, out var list))
                {
                    list = new List<int>();
                    dict[key] = list;
                }
                if (!list.Contains(s.DishId))
                    list.Add(s.DishId);
            }
            return dict;
        }

        private static List<Services.CreateMenuScheduleItemClientRequest> ResolveSchedulesFromPost(
            string? schedulesJson,
            string? daySelectionsJson)
        {
            // 1) Mảng schedulesJson từ client: [{ date, mealSlot, dishId }, ...]
            var fromSchedulesArray = ParseSchedulesJson(schedulesJson);
            if (fromSchedulesArray.Count > 0)
                return fromSchedulesArray;

            // 2) Object daySelectionsJson: { "yyyy-MM-dd": [dishId, ...] }
            var daySelections = ParseDaySelectionsJson(daySelectionsJson);
            if (daySelections.Count == 0)
                daySelections = TryParseDaySelectionsFromRaw(schedulesJson);

            return BuildSchedulesFromDaySelections(daySelections);
        }

        private static List<Services.CreateMenuScheduleItemClientRequest> ParseSchedulesJson(string? schedulesJson)
        {
            if (string.IsNullOrWhiteSpace(schedulesJson))
                return new List<Services.CreateMenuScheduleItemClientRequest>();

            try
            {
                var deserialized = JsonSerializer.Deserialize<List<SchedulePostItem>>(schedulesJson, JsonDraftOptions);
                if (deserialized is { Count: > 0 })
                {
                    var fromDeserialize = new List<Services.CreateMenuScheduleItemClientRequest>();
                    foreach (var item in deserialized)
                    {
                        if (item.DishId <= 0)
                            continue;

                        if (!TryParseScheduleDate(item.Date, out var date))
                            continue;

                        fromDeserialize.Add(new Services.CreateMenuScheduleItemClientRequest
                        {
                            Date = date,
                            MealSlot = string.IsNullOrWhiteSpace(item.MealSlot) ? "lunch" : item.MealSlot.Trim(),
                            DishId = item.DishId
                        });
                    }

                    if (fromDeserialize.Count > 0)
                        return fromDeserialize;
                }

                using var doc = JsonDocument.Parse(schedulesJson);
                if (doc.RootElement.ValueKind != JsonValueKind.Array)
                    return new List<Services.CreateMenuScheduleItemClientRequest>();

                var list = new List<Services.CreateMenuScheduleItemClientRequest>();
                foreach (var el in doc.RootElement.EnumerateArray())
                {
                    if (!TryReadScheduleElement(el, out var date, out var dishId))
                        continue;

                    list.Add(new Services.CreateMenuScheduleItemClientRequest
                    {
                        Date = date,
                        MealSlot = "lunch",
                        DishId = dishId
                    });
                }

                return list;
            }
            catch
            {
                return new List<Services.CreateMenuScheduleItemClientRequest>();
            }
        }

        private static bool TryParseScheduleDate(string? dateStr, out DateTime date)
        {
            date = default;
            if (string.IsNullOrWhiteSpace(dateStr))
                return false;

            return DateTime.TryParse(
                       dateStr,
                       CultureInfo.InvariantCulture,
                       DateTimeStyles.None,
                       out date)
                   || DateTime.TryParse(dateStr, CultureInfo.CurrentCulture, DateTimeStyles.None, out date);
        }

        private static bool TryReadScheduleElement(JsonElement el, out DateTime date, out int dishId)
        {
            date = default;
            dishId = 0;

            if (!el.TryGetProperty("date", out var dateEl) && !el.TryGetProperty("Date", out dateEl))
                return false;
            if (!el.TryGetProperty("dishId", out var dishEl) && !el.TryGetProperty("DishId", out dishEl))
                return false;

            var dateStr = dateEl.ValueKind == JsonValueKind.String
                ? dateEl.GetString()
                : dateEl.GetRawText();
            if (!TryParseScheduleDate(dateStr, out date))
                return false;

            date = date.Date;

            if (dishEl.ValueKind == JsonValueKind.Number)
            {
                if (dishEl.TryGetInt32(out dishId))
                    return dishId > 0;
                if (dishEl.TryGetInt64(out var lid) && lid > 0 && lid <= int.MaxValue)
                {
                    dishId = (int)lid;
                    return true;
                }

                return false;
            }

            if (!int.TryParse(dishEl.GetString(), out dishId))
                return false;

            return dishId > 0;
        }

        private static Dictionary<string, List<int>> ParseDaySelectionsJson(string? daySelectionsJson)
        {
            if (string.IsNullOrWhiteSpace(daySelectionsJson))
                return new Dictionary<string, List<int>>(StringComparer.Ordinal);

            try
            {
                var deserialized = JsonSerializer.Deserialize<Dictionary<string, List<int>>>(
                    daySelectionsJson, JsonDraftOptions);
                if (deserialized is { Count: > 0 })
                {
                    var fromDeserialize = new Dictionary<string, List<int>>(StringComparer.Ordinal);
                    foreach (var (key, ids) in deserialized)
                    {
                        if (!DateTime.TryParse(key, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                            continue;

                        var validIds = (ids ?? new List<int>()).Where(id => id > 0).Distinct().ToList();
                        if (validIds.Count > 0)
                        {
                            fromDeserialize[date.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)] = validIds;
                        }
                    }

                    if (fromDeserialize.Count > 0)
                        return fromDeserialize;
                }

                using var doc = JsonDocument.Parse(daySelectionsJson);
                if (doc.RootElement.ValueKind != JsonValueKind.Object)
                    return new Dictionary<string, List<int>>(StringComparer.Ordinal);

                var normalized = new Dictionary<string, List<int>>(StringComparer.Ordinal);
                foreach (var prop in doc.RootElement.EnumerateObject())
                {
                    if (!DateTime.TryParse(prop.Name, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                        continue;

                    var ids = new List<int>();
                    if (prop.Value.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var item in prop.Value.EnumerateArray())
                        {
                            if (item.ValueKind == JsonValueKind.Number && item.TryGetInt32(out var id) && id > 0)
                                ids.Add(id);
                            else if (item.ValueKind == JsonValueKind.String
                                     && int.TryParse(item.GetString(), out var sid) && sid > 0)
                                ids.Add(sid);
                        }
                    }

                    if (ids.Count > 0)
                        normalized[date.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)] = ids.Distinct().ToList();
                }

                return normalized;
            }
            catch
            {
                return new Dictionary<string, List<int>>(StringComparer.Ordinal);
            }
        }

        private static Dictionary<string, List<int>> MergeDaySelectionsFromPost(
            string? schedulesJson,
            string? daySelectionsJson)
        {
            var merged = ParseDaySelectionsJson(daySelectionsJson);
            if (merged.Count > 0)
                return merged;
            return TryParseDaySelectionsFromRaw(schedulesJson);
        }

        private static List<Services.CreateMenuScheduleItemClientRequest> BuildSchedulesFromDaySelections(
            Dictionary<string, List<int>> daySelections)
        {
            var items = new List<Services.CreateMenuScheduleItemClientRequest>();
            foreach (var (dateIso, dishIds) in daySelections)
            {
                if (!DateTime.TryParse(dateIso, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                    continue;

                foreach (var dishId in dishIds.Where(id => id > 0).Distinct())
                {
                    items.Add(new Services.CreateMenuScheduleItemClientRequest
                    {
                        Date = date.Date,
                        MealSlot = "lunch",
                        DishId = dishId
                    });
                }
            }

            return items;
        }

        private static Dictionary<string, List<int>> TryParseDaySelectionsFromRaw(string? schedulesJson)
        {
            var daySelections = new Dictionary<string, List<int>>(StringComparer.Ordinal);
            if (string.IsNullOrWhiteSpace(schedulesJson))
                return daySelections;

            try
            {
                using var doc = JsonDocument.Parse(schedulesJson);
                if (doc.RootElement.ValueKind != JsonValueKind.Array)
                    return daySelections;

                foreach (var el in doc.RootElement.EnumerateArray())
                {
                    if (!TryReadScheduleElement(el, out var date, out var dishId))
                        continue;

                    var key = date.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                    if (!daySelections.TryGetValue(key, out var list))
                    {
                        list = new List<int>();
                        daySelections[key] = list;
                    }
                    if (!list.Contains(dishId))
                        list.Add(dishId);
                }
            }
            catch
            {
                // ignore — trả về phần đã parse được (có thể rỗng)
            }

            return daySelections;
        }

        private static WeeklyMenuCreateDraft BuildCreateDraft(
            DateTime startDate,
            DateTime endDate,
            int? customerTypeId,
            string? description,
            string menuType,
            List<Services.CreateMenuScheduleItemClientRequest> schedules)
        {
            var daySelections = new Dictionary<string, List<int>>(StringComparer.Ordinal);
            foreach (var s in schedules)
            {
                if (s.DishId <= 0) continue;
                var key = s.Date.Date.ToString("yyyy-MM-dd");
                if (!daySelections.TryGetValue(key, out var list))
                {
                    list = new List<int>();
                    daySelections[key] = list;
                }
                if (!list.Contains(s.DishId))
                    list.Add(s.DishId);
            }

            return new WeeklyMenuCreateDraft
            {
                StartDate = startDate.Date,
                EndDate = endDate.Date,
                CustomerTypeId = customerTypeId,
                Description = description,
                MenuType = string.IsNullOrWhiteSpace(menuType) ? "General" : menuType.Trim(),
                DaySelections = daySelections
            };
        }

        private async Task PrepareCreateViewAsync(
            string token,
            CancellationToken ct,
            WeeklyMenuCreateDraft? draft = null)
        {
            var dishesResp = await _masterDataClient.GetDishesAsync(token, 1, 500, isActive: true, ct: ct);
            var customerTypes = await _masterDataClient.GetCustomerTypesAsync(token, ct);

            ViewBag.Dishes = dishesResp.Items ?? new List<Services.DishDto>();
            ViewBag.CustomerTypes = customerTypes.Data ?? new List<Services.CustomerTypeClientDto>();

            if (draft != null)
            {
                ViewBag.DefaultStart = draft.StartDate.ToString("yyyy-MM-dd");
                ViewBag.DefaultEnd = draft.EndDate.ToString("yyyy-MM-dd");
                ViewBag.DraftCustomerTypeId = draft.CustomerTypeId;
                ViewBag.DraftDescription = draft.Description ?? "";
                ViewBag.DraftMenuType = draft.MenuType;
                ViewBag.DraftDaySelectionsJson = JsonSerializer.Serialize(draft.DaySelections, JsonDraftOptions);
            }
            else
            {
                var today = DateTime.Today;
                int daysUntilMonday = ((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7;
                if (daysUntilMonday == 0)
                    daysUntilMonday = 7;
                var nextMonday = today.AddDays(daysUntilMonday);
                var nextFriday = nextMonday.AddDays(4);

                ViewBag.DefaultStart = nextMonday.ToString("yyyy-MM-dd");
                ViewBag.DefaultEnd = nextFriday.ToString("yyyy-MM-dd");
                ViewBag.DraftCustomerTypeId = null;
                ViewBag.DraftDescription = "";
                ViewBag.DraftMenuType = "General";
                ViewBag.DraftDaySelectionsJson = "{}";
            }
        }

        private async Task<IActionResult> ReturnCreateWithErrorAsync(
            string token,
            WeeklyMenuCreateDraft draft,
            string errorMessage,
            CancellationToken ct)
        {
            ViewData["Error"] = errorMessage;
            ViewBag.DraftHasSelections = draft.DaySelections.Values.Any(list => list.Count > 0);
            await PrepareCreateViewAsync(token, ct, draft);
            return View("Create");
        }

        /// <summary>
        /// Chi tiết một thực đơn tuần: thông tin header + toàn bộ lịch món theo ngày/ca.
        /// </summary>
        public async Task<IActionResult> Detail(int id, CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                var response = await _masterDataClient.GetWeeklyMenuDetailAsync(id, token, ct: ct);
                if (response?.WeeklyMenu == null || response.WeeklyMenu.Id == 0)
                {
                    TempData["Error"] = "Không tìm thấy thực đơn tuần.";
                    return RedirectToAction(nameof(Weekly));
                }

                return View(response);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Weekly));
            }
        }
    }
}
