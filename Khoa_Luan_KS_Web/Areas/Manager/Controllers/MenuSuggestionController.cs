using Khoa_Luan_KS_Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Khoa_Luan_KS_Web.Areas.Manager.Controllers;

[Area("Manager")]
[Authorize(Policy = "ManagerArea")]
public class MenuSuggestionController : Controller
{
    private readonly BackendMenuSuggestionClient _menuSuggestionClient;
    private readonly BackendMasterDataClient _masterDataClient;

    public MenuSuggestionController(BackendMenuSuggestionClient menuSuggestionClient, BackendMasterDataClient masterDataClient)
    {
        _menuSuggestionClient = menuSuggestionClient;
        _masterDataClient = masterDataClient;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchTerm = null)
    {
        try
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            var result = await _menuSuggestionClient.GetMenuSuggestionsAsync(token, page, pageSize, searchTerm);
            ViewData["SearchTerm"] = searchTerm;
            return View(result);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("401")) return RedirectToAction("Logout", "Auth", new { area = "" });
            TempData["Error"] = "Lỗi khi lấy danh sách Gợi ý thực đơn: " + ex.Message;
            return View(new GetMenuSuggestionsResponse());
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            var result = await _menuSuggestionClient.GetMenuSuggestionAsync(id, token);
            return View(result);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("401")) return RedirectToAction("Logout", "Auth", new { area = "" });
            TempData["Error"] = "Lỗi khi lấy chi tiết Gợi ý thực đơn: " + ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    public async Task<IActionResult> Generate()
    {
        try
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            // Fetch dishes to let user select which dishes the AI can use
            var dishesResp = await _masterDataClient.GetDishesAsync(token, 1, 1000); // Fetch up to 1000 active dishes
            ViewBag.Dishes = dishesResp.Items ?? new List<DishDto>();

            // Calculate Next Monday
            var today = DateTime.UtcNow.Date;
            int daysUntilMonday = ((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7;
            if (daysUntilMonday == 0) daysUntilMonday = 7; // If today is Monday, get next Monday
            var nextMonday = today.AddDays(daysUntilMonday);

            var model = new GenerateMenuSuggestionFromAiRequest
            {
                WeekStartUtc = nextMonday,
                BudgetPerServing = 35000,
                TopK = 3,
                TimeLimitSeconds = 60,  // Give CP-SAT enough time
                Days = new List<string> { "Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6" },
                MealStructure = new List<string> { "Món chính", "Món canh", "Món rau", "Món phụ" },
                RulesKey = "industrial"
            };

            var validRules = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "industrial", "org_company", "org_elementary", "org_primary_school",
                "org_secondary_school", "org_high_school", "vegetarian",
            };
            if (!validRules.Contains(model.RulesKey ?? ""))
                model.RulesKey = "industrial";

            return View(model);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("401")) return RedirectToAction("Logout", "Auth", new { area = "" });
            TempData["Error"] = "Không thể tải danh sách món ăn: " + ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Generate(GenerateMenuSuggestionFromAiRequest request)
    {
        try
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            if (request.DishIds == null || !request.DishIds.Any())
            {
                TempData["Error"] = "Vui lòng chọn ít nhất một món ăn.";
                return RedirectToAction(nameof(Generate));
            }

            if (request.Days == null || !request.Days.Any() || request.MealStructure == null || !request.MealStructure.Any())
            {
                TempData["Error"] = "Vui lòng chọn ngày và cấu trúc bữa ăn.";
                return RedirectToAction(nameof(Generate));
            }

            if (string.Equals(request.RulesKey, "school", StringComparison.OrdinalIgnoreCase))
                request.RulesKey = "org_primary_school";

            // Week boundary in UTC (align with backend handler)
            request.WeekStartUtc = DateTime.SpecifyKind(request.WeekStartUtc.Date, DateTimeKind.Utc);

            var result = await _menuSuggestionClient.GenerateMenuSuggestionAsync(request, token);
            TempData["Success"] = "Đã tạo gợi ý thực đơn thành công!";
            return RedirectToAction(nameof(Details), new { id = result.Id });
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("401")) return RedirectToAction("Logout", "Auth", new { area = "" });
            TempData["Error"] = "Lỗi khi gọi AI sinh thực đơn: " + ex.Message;
            return RedirectToAction(nameof(Generate));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApplyAsWeeklyMenu(int suggestionId, int planId)
    {
        try
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            var suggestion = await _menuSuggestionClient.GetMenuSuggestionAsync(suggestionId, token);
            var plan = suggestion.Plans.FirstOrDefault(p => p.Id == planId);
            
            if (plan == null)
            {
                TempData["Error"] = "Không tìm thấy plan được chọn.";
                return RedirectToAction(nameof(Details), new { id = suggestionId });
            }

            var request = new CreateWeeklyMenuClientRequest
            {
                StartDate = suggestion.WeekStart,
                EndDate = suggestion.WeekStart.AddDays(6), // Assuming 7 days max, or we can calculate based on plan.Days
                MenuType = "General",
                Description = $"Áp dụng từ Gợi ý AI - Plan #{plan.Rank} (Version {suggestion.Version})"
            };

            // Only map items that have a DishId
            foreach (var day in plan.Days)
            {
                // DayIndex 0 = Monday, so StartDate should be Monday
                var currentDate = suggestion.WeekStart.AddDays(day.DayIndex);
                
                foreach (var item in day.Items)
                {
                    if (item.DishId.HasValue)
                    {
                        request.Schedules.Add(new CreateMenuScheduleItemClientRequest
                        {
                            Date = currentDate,
                            MealSlot = "lunch",
                            DishId = item.DishId.Value
                        });
                    }
                }
            }

            if (!request.Schedules.Any())
            {
                TempData["Error"] = "Plan không có món ăn nào chứa ID hợp lệ để tạo thực đơn.";
                return RedirectToAction(nameof(Details), new { id = suggestionId });
            }

            var result = await _masterDataClient.CreateWeeklyMenuAsync(request, token);
            TempData["Success"] = "Đã chuyển đổi thành Thực đơn Tuần cố định thành công!";
            return RedirectToAction("Detail", "Menu", new { id = result.WeeklyMenu.Id });
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("401")) return RedirectToAction("Logout", "Auth", new { area = "" });
            TempData["Error"] = "Lỗi khi tạo Thực đơn Tuần: " + ex.Message;
            return RedirectToAction(nameof(Details), new { id = suggestionId });
        }
    }
}
