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
        public IActionResult Dish() => View();
        public IActionResult History() => View();
        public IActionResult Ingredients() => View();
        public IActionResult DishForm(string id) => View();

        /// <summary>
        /// Danh sách thực đơn tuần có phân trang và tìm kiếm.
        /// </summary>
        public async Task<IActionResult> Weekly(int page = 1, int pageSize = 10, string? searchTerm = null, CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                var response = await _masterDataClient.GetWeeklyMenusAsync(token, page, pageSize, searchTerm, ct: ct);
                ViewData["SearchTerm"] = searchTerm;
                ViewData["CurrentPage"] = page;
                ViewData["PageSize"] = pageSize;
                return View(response);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new Services.GetWeeklyMenusClientResponse());
            }
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
                var response = await _masterDataClient.GetWeeklyMenuDetailAsync(id, token, ct);
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
