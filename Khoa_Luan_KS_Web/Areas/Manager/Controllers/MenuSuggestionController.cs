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

            // Ensure UTC
            request.WeekStartUtc = DateTime.SpecifyKind(request.WeekStartUtc, DateTimeKind.Utc);

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
}
