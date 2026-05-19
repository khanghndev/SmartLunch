using System.Linq;
using Khoa_Luan_KS_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Khoa_Luan_KS_Web.Controllers
{
    public class MenuController : Controller
    {
        private readonly Services.BackendMasterDataClient _masterDataClient;

        public MenuController(Services.BackendMasterDataClient masterDataClient)
        {
            _masterDataClient = masterDataClient;
        }

        public override async Task OnActionExecutionAsync(
            Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context,
            Microsoft.AspNetCore.Mvc.Filters.ActionExecutionDelegate next)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                bool isCustomer = User.IsInRole("Customer") ||
                                  User.IsInRole("Organization") ||
                                  User.IsInRole("Khách hàng doanh nghiệp") ||
                                  User.IsInRole("Khách hàng cá nhân");

                if (!isCustomer)
                {
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    HttpContext.Session.Clear();
                    context.Result = Redirect(Request.Path + Request.QueryString);
                    return;
                }
            }
            await next();
        }

        /// <summary>
        /// Trang thực đơn chính: lấy danh sách thực đơn tuần + chi tiết thực đơn hiện tại.
        /// </summary>
        public async Task<IActionResult> Index(int? menuId = null, string? profile = null, CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            var profileKey = string.IsNullOrWhiteSpace(profile) ? null : profile.Trim();

            Services.GetWeeklyMenusClientResponse? menuList = null;
            Services.GetWeeklyMenuDetailClientResponse? menuDetail = null;

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    // Lấy danh sách thực đơn tuần (tối đa 20 mục gần nhất), có thể lọc theo phân khúc (profile key)
                    menuList = await _masterDataClient.GetWeeklyMenusAsync(
                        token,
                        page: 1,
                        pageSize: 20,
                        customerProfileKey: profileKey,
                        ct: ct);

                    // Xác định thực đơn sẽ hiển thị chi tiết
                    int targetId;
                    if (menuId.HasValue)
                    {
                        targetId = menuId.Value;
                    }
                    else
                    {
                        // Ưu tiên thực đơn đang áp dụng, nếu không thì lấy thực đơn mới nhất
                        var today = DateTime.Today;
                        var current = menuList?.Items.FirstOrDefault(m =>
                            m.StartDate.Date <= today && m.EndDate.Date >= today);
                        var target = current ?? menuList?.Items.OrderByDescending(m => m.StartDate).FirstOrDefault();
                        targetId = target?.Id ?? 0;
                    }

                    if (targetId > 0)
                    {
                        menuDetail = await _masterDataClient.GetWeeklyMenuDetailAsync(targetId, token, ct);
                    }
                }
                catch (Exception ex)
                {
                    ViewData["ApiError"] = ex.Message;
                }
            }

            ViewData["MenuList"] = menuList;
            ViewData["MenuDetail"] = menuDetail;
            ViewData["SelectedMenuId"] = menuId;
            ViewData["CustomerProfileKey"] = profileKey;
            return View();
        }

        /// <summary>
        /// Chi tiết món: nếu có menuId + scheduleId thì lấy từ thực đơn tuần + API món; không thì hiển thị mẫu tĩnh (legacy).
        /// </summary>
        public async Task<IActionResult> MealDetail(int? menuId = null, int? scheduleId = null, CancellationToken ct = default)
        {
            if (menuId is > 0 && scheduleId is > 0)
            {
                var token = HttpContext.Session.GetString("access_token");
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Auth", new
                    {
                        area = "",
                        returnUrl = Url.Action(nameof(MealDetail), new { menuId, scheduleId })
                    });
                }

                try
                {
                    var detail = await _masterDataClient.GetWeeklyMenuDetailAsync(menuId!.Value, token, ct);
                    var schedule = detail.Schedules.FirstOrDefault(s => s.Id == scheduleId!.Value);
                    if (schedule == null)
                        return NotFound();

                    Services.DishDetailResponse? dishDetail = null;
                    string? dishLoadWarning = null;
                    try
                    {
                        dishDetail = await _masterDataClient.GetDishAsync(schedule.DishId, token, ct);
                        if (dishDetail?.Dish == null || string.IsNullOrWhiteSpace(dishDetail.Dish.Name))
                            dishLoadWarning = "Không tải đủ dữ liệu món từ hệ thống.";
                    }
                    catch (Exception dishEx)
                    {
                        dishLoadWarning = $"Không tải chi tiết món: {dishEx.Message}";
                    }

                    var vm = new MenuMealDetailViewModel
                    {
                        WeeklyMenu = detail.WeeklyMenu,
                        Schedule = schedule,
                        DishDetail = dishDetail,
                        DishLoadWarning = dishLoadWarning,
                    };
                    return View("MealDetailFromMenu", vm);
                }
                catch (Exception ex)
                {
                    TempData["Error"] = ex.Message;
                    return RedirectToAction(nameof(Index));
                }
            }

            return View();
        }

        public IActionResult SchoolMenu(string level)
        {
            ViewData["Level"] = string.IsNullOrEmpty(level) ? "mamnon" : level.ToLower();
            return View();
        }
    }
}
