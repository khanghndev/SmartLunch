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
        /// <summary>Legacy URL — chuyển hướng sang <see cref="DishDetail"/> (một trang chi tiết chung).</summary>
        public async Task<IActionResult> MealDetail(int? menuId = null, int? scheduleId = null, CancellationToken ct = default)
        {
            if (menuId is > 0 && scheduleId is > 0)
            {
                var token = HttpContext.Session.GetString("access_token");
                if (string.IsNullOrEmpty(token))
                {
                    var returnUrl = $"/Menu/DishDetail?menuId={menuId}&scheduleId={scheduleId}";
                    return RedirectToAction("Login", "Auth", new { area = "", returnUrl });
                }

                try
                {
                    var detail = await _masterDataClient.GetWeeklyMenuDetailAsync(menuId!.Value, token, ct);
                    var schedule = detail.Schedules.FirstOrDefault(s => s.Id == scheduleId!.Value);
                    if (schedule == null || schedule.DishId <= 0)
                        return NotFound();

                    return RedirectToAction(nameof(DishDetail), new
                    {
                        id = schedule.DishId,
                        menuId,
                        scheduleId
                    });
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

        /// <summary>Thư viện toàn bộ món ăn (công khai).</summary>
        public async Task<IActionResult> Gallery(
            int page = 1,
            int? categoryId = null,
            string? q = null,
            CancellationToken ct = default)
        {
            var vm = new DishGalleryViewModel
            {
                Page = Math.Max(1, page),
                SelectedCategoryId = categoryId,
                Search = string.IsNullOrWhiteSpace(q) ? null : q.Trim(),
            };

            try
            {
                vm.Categories = await _masterDataClient.GetOrganizationDishCategoriesPublicAsync(ct);
                vm.Browse = await _masterDataClient.GetPublicDishesBrowseAsync(
                    vm.Page,
                    pageSize: 24,
                    categoryId: categoryId,
                    search: vm.Search,
                    ct: ct);
            }
            catch (Exception ex)
            {
                vm.LoadError = ex.Message;
            }

            return View(vm);
        }

        /// <summary>Chi tiết món ăn — trang chung (thư viện, thực đơn tuần, trang chủ).</summary>
        public async Task<IActionResult> DishDetail(
            int id,
            int? menuId = null,
            int? scheduleId = null,
            CancellationToken ct = default)
        {
            if (id <= 0 && menuId is > 0 && scheduleId is > 0)
            {
                var tokenForResolve = HttpContext.Session.GetString("access_token");
                if (!string.IsNullOrEmpty(tokenForResolve))
                {
                    try
                    {
                        var menuDetail = await _masterDataClient.GetWeeklyMenuDetailAsync(menuId.Value, tokenForResolve, ct);
                        var schedule = menuDetail.Schedules.FirstOrDefault(s => s.Id == scheduleId.Value);
                        if (schedule?.DishId is > 0)
                            return RedirectToAction(nameof(DishDetail), new { id = schedule.DishId, menuId, scheduleId });
                    }
                    catch
                    {
                        // fall through
                    }
                }
            }

            if (id <= 0)
                return NotFound();

            var vm = new DishDetailPageViewModel();
            var token = HttpContext.Session.GetString("access_token");

            try
            {
                vm.Detail = await _masterDataClient.GetPublicDishDetailAsync(id, ct);
                if (vm.Detail?.Dish == null || vm.Detail.Dish.Id <= 0)
                    return NotFound();
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("404", StringComparison.Ordinal) || ex.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrEmpty(token))
                {
                    try
                    {
                        vm.Detail = await _masterDataClient.GetDishAsync(id, token, ct);
                    }
                    catch
                    {
                        return NotFound();
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(token))
                {
                    try
                    {
                        vm.Detail = await _masterDataClient.GetDishAsync(id, token, ct);
                    }
                    catch (Exception authEx)
                    {
                        vm.LoadError = authEx.Message;
                    }
                }
                else
                {
                    vm.LoadError = ex.Message;
                }
            }

            if (menuId is > 0 && scheduleId is > 0 && !string.IsNullOrEmpty(token))
            {
                try
                {
                    var menuDetail = await _masterDataClient.GetWeeklyMenuDetailAsync(menuId.Value, token, ct);
                    var schedule = menuDetail.Schedules.FirstOrDefault(s => s.Id == scheduleId.Value);
                    if (schedule != null && schedule.DishId == id)
                    {
                        vm.MenuContext = new DishDetailMenuContext
                        {
                            MenuId = menuId.Value,
                            ScheduleId = scheduleId.Value,
                            MenuStartDate = menuDetail.WeeklyMenu.StartDate,
                            MenuEndDate = menuDetail.WeeklyMenu.EndDate,
                            ScheduleDate = schedule.Date,
                            MealSlot = schedule.MealSlot,
                        };
                    }

                    var quotasEmpty = vm.Detail?.IngredientQuotas == null || vm.Detail.IngredientQuotas.Count == 0;
                    if (quotasEmpty)
                    {
                        try
                        {
                            var authDetail = await _masterDataClient.GetDishAsync(id, token, ct);
                            if (authDetail?.Dish != null)
                                vm.Detail = authDetail;
                        }
                        catch
                        {
                            // giữ dữ liệu public
                        }
                    }
                }
                catch
                {
                    // không chặn xem món nếu lỗi ngữ cảnh thực đơn
                }
            }

            if (vm.Detail?.Dish == null || vm.Detail.Dish.Id <= 0)
            {
                if (string.IsNullOrEmpty(vm.LoadError))
                    return NotFound();
            }

            return View(vm);
        }
    }
}
