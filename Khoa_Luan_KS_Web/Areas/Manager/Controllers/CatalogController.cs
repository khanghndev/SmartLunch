using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Khoa_Luan_KS_Web.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Policy = "ManagerArea")]
    public class CatalogController : Controller
    {
        private readonly Services.BackendMasterDataClient _masterDataClient;

        public CatalogController(Services.BackendMasterDataClient masterDataClient)
        {
            _masterDataClient = masterDataClient;
        }

        public async Task<IActionResult> Employees(
            int page = 1,
            int pageSize = 10,
            string? searchTerm = null,
            string? roleName = null,
            CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            page = page < 1 ? 1 : page;
            pageSize = pageSize switch
            {
                15 => 15,
                20 => 20,
                30 => 30,
                50 => 50,
                _ => 10
            };

            try
            {
                var rolesResponse = await _masterDataClient.GetRolesAsync(token, isActive: true, ct: ct);
                var staffRoleNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "Admin", "Manager", "WarehouseStaff", "ChefStaff", "SalesStaff", "Shipper"
                };
                ViewBag.StaffRoles = rolesResponse.Items
                    .Where(r => staffRoleNames.Contains(r.Name))
                    .OrderBy(r => r.Name)
                    .ToList();

                var response = await _masterDataClient.GetUsersAsync(
                    token, page, pageSize, searchTerm, roleName, staffOnly: true, ct: ct);

                var effectivePageSize = response.PageSize > 0 ? response.PageSize : pageSize;
                var totalPages = effectivePageSize > 0
                    ? Math.Max(1, (int)Math.Ceiling(response.TotalCount / (double)effectivePageSize))
                    : 1;
                var effectivePage = response.Page > 0 ? response.Page : page;

                if (response.TotalCount > 0 && effectivePage > totalPages)
                {
                    return RedirectToAction(nameof(Employees), new { page = totalPages, pageSize = effectivePageSize, searchTerm, roleName });
                }

                ViewBag.CurrentPage = effectivePage;
                ViewBag.PageSize = effectivePageSize;
                ViewBag.TotalPages = totalPages;
                ViewBag.SearchTerm = searchTerm ?? "";
                ViewBag.RoleName = roleName ?? "";
                ViewBag.Pagination = new Models.ManagerPaginationVm
                {
                    Controller = "Catalog",
                    Action = "Employees",
                    Page = effectivePage,
                    PageSize = effectivePageSize,
                    TotalCount = response.TotalCount,
                    ItemCount = response.Items.Count,
                    TotalPages = totalPages,
                    SearchTerm = searchTerm,
                    RoleName = roleName
                };

                return View(response);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                ViewBag.StaffRoles = new List<Services.RoleDto>();
                return View(new Services.AdminGetUsersResponse());
            }
        }

        private static readonly HashSet<string> StaffRoleNames = new(StringComparer.OrdinalIgnoreCase)
        {
            "Admin", "Manager", "WarehouseStaff", "ChefStaff", "SalesStaff", "Shipper"
        };

        private IActionResult RedirectAfterEmployeeAction(string? returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToEmployeesList();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEmployee(
            [FromForm] Services.CreateUserRequest request,
            string? roleName,
            CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                if (string.IsNullOrWhiteSpace(request.Username))
                    request.Username = request.Email.Trim();
                if (string.IsNullOrWhiteSpace(request.Password))
                    request.Password = "HuitMeal@2026";

                if (!string.IsNullOrWhiteSpace(roleName))
                {
                    var roles = await _masterDataClient.GetRolesAsync(token, isActive: true, ct: ct);
                    var role = roles.Items.FirstOrDefault(r =>
                        string.Equals(r.Name, roleName, StringComparison.OrdinalIgnoreCase));
                    if (role != null)
                        request.InitialRoleId = role.Id;
                }

                var created = await _masterDataClient.CreateUserAsync(request, token, ct);
                TempData["Success"] = "Đã tạo hồ sơ nhân viên mới thành công.";
                if (created.User.Id > 0)
                    return RedirectToAction(nameof(EmployeeDetail), new { id = created.User.Id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToEmployeesList();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateEmployee(
            int id,
            [FromForm] Services.UpdateUserRequest request,
            string? returnUrl,
            CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                await _masterDataClient.UpdateUserAsync(id, request, token, ct);
                TempData["Success"] = "Đã cập nhật thông tin nhân viên thành công.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectAfterEmployeeAction(returnUrl);
        }

        private IActionResult RedirectToEmployeesList()
        {
            var page = 1;
            var pageSize = 10;
            if (int.TryParse(Request.Form["page"].FirstOrDefault() ?? Request.Query["page"], out var p)) page = p;
            if (int.TryParse(Request.Form["pageSize"].FirstOrDefault() ?? Request.Query["pageSize"], out var ps)) pageSize = ps;
            var searchTerm = Request.Form["searchTerm"].FirstOrDefault() ?? Request.Query["searchTerm"].FirstOrDefault();
            var roleName = Request.Form["roleName"].FirstOrDefault() ?? Request.Query["roleName"].FirstOrDefault();
            return RedirectToAction(nameof(Employees), new { page, pageSize, searchTerm, roleName });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LockEmployee(int id, string? returnUrl, CancellationToken ct)
        {
            var token = HttpContext.Session.GetString("access_token");
            try
            {
                await _masterDataClient.LockUserAsync(id, token!, ct);
                TempData["Success"] = "Đã khóa hồ sơ nhân viên thành công.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectAfterEmployeeAction(returnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnlockEmployee(int id, string? returnUrl, CancellationToken ct)
        {
            var token = HttpContext.Session.GetString("access_token");
            try
            {
                await _masterDataClient.UnlockUserAsync(id, token!, ct);
                TempData["Success"] = "Đã khôi phục hồ sơ nhân viên thành công.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectAfterEmployeeAction(returnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetEmployeePassword(int id, string newPassword, string? returnUrl, CancellationToken ct)
        {
            var token = HttpContext.Session.GetString("access_token");
            try
            {
                await _masterDataClient.ResetPasswordAsync(id, newPassword, token!, ct);
                TempData["Success"] = "Đã đặt lại mật khẩu nhân viên thành công.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectAfterEmployeeAction(returnUrl);
        }

        public async Task<IActionResult> EmployeeDetail(int id, CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                var response = await _masterDataClient.GetUserAsync(id, token, ct);
                if (response?.User == null || response.User.Id == 0)
                {
                    TempData["Error"] = "Không tìm thấy nhân viên.";
                    return RedirectToAction(nameof(Employees));
                }

                var user = response.User;
                var primaryRole = user.RoleNames.FirstOrDefault(r => StaffRoleNames.Contains(r));
                if (primaryRole == null && user.RoleNames.Count > 0)
                {
                    TempData["Error"] = "Tài khoản này không thuộc danh sách nhân viên nội bộ.";
                    return RedirectToAction(nameof(Employees));
                }

                ViewBag.ReturnListUrl = Url.Action(nameof(Employees), "Catalog", new { area = "Manager" });
                return View(user);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Employees));
            }
        }

        public async Task<IActionResult> Suppliers(int page = 1, int pageSize = 10, string? searchTerm = null, CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                var response = await _masterDataClient.GetPartnersAsync(token, page, pageSize, searchTerm, ct);
                return View(response);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new Services.GetPartnersResponse());
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateSupplier(Services.CreatePartnerRequest request, CancellationToken ct)
        {
            var token = HttpContext.Session.GetString("access_token");
            try
            {
                await _masterDataClient.CreatePartnerAsync(request, token!, ct);
                TempData["Success"] = "Đã thêm nhà cung cấp mới thành công.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Suppliers));
        }

        public async Task<IActionResult> SupplierDetail(int id, CancellationToken ct)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                var response = await _masterDataClient.GetPartnerAsync(id, token, ct);
                return View(response.Partner);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Suppliers));
            }
        }
        public async Task<IActionResult> Customers(int page = 1, int pageSize = 10, string? searchTerm = null, bool? isActive = null, CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                var response = await _masterDataClient.GetUnitsAsync(token, page, pageSize, searchTerm, isActive, ct);
                ViewBag.CurrentPage = page;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalPages = response.TotalCount == 0 ? 1 : (int)Math.Ceiling((double)response.TotalCount / pageSize);
                ViewBag.IsActiveFilter = isActive;
                ViewBag.SearchTerm = searchTerm ?? string.Empty;
                return View(response);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new Services.GetUnitsResponse());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCustomerStatus(int id, bool isActive, string? returnUrl, CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                await _masterDataClient.UpdateOrganizationStatusAsync(id, isActive, token, ct);
                TempData["Success"] = isActive ? "Đã kích hoạt khách hàng B2B." : "Đã tạm ngưng phục vụ khách hàng B2B.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction(nameof(Customers));
        }

        public async Task<IActionResult> CustomerDetail(int id, CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                var response = await _masterDataClient.GetUnitAsync(id, token, ct);
                if (response?.Unit == null || response.Unit.Id == 0)
                {
                    TempData["Error"] = "Không tìm thấy khách hàng.";
                    return RedirectToAction(nameof(Customers));
                }

                return View(response.Unit);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Customers));
            }
        }

        public IActionResult MealCreate()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("access_token")))
                return RedirectToAction("Login", "Auth", new { area = "" });
            return View();
        }

        public async Task<IActionResult> Meals(int page = 1, int pageSize = 20, string? searchTerm = null, string? category = null, CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            page = page < 1 ? 1 : page;
            pageSize = pageSize switch
            {
                10 => 10,
                15 => 15,
                20 => 20,
                30 => 30,
                50 => 50,
                _ => 20
            };

            try
            {
                var response = await _masterDataClient.GetDishesAsync(token, page, pageSize, searchTerm, category, ct: ct);

                var effectivePageSize = response.PageSize > 0 ? response.PageSize : pageSize;
                var totalPages = effectivePageSize > 0
                    ? Math.Max(1, (int)Math.Ceiling(response.TotalCount / (double)effectivePageSize))
                    : 1;
                var effectivePage = response.Page > 0 ? response.Page : page;

                if (response.TotalCount > 0 && effectivePage > totalPages)
                {
                    return RedirectToAction(nameof(Meals), new { page = totalPages, pageSize = effectivePageSize, searchTerm, category });
                }

                ViewBag.Pagination = new Models.ManagerPaginationVm
                {
                    Controller = "Catalog",
                    Action = "Meals",
                    Page = effectivePage,
                    PageSize = effectivePageSize,
                    TotalCount = response.TotalCount,
                    ItemCount = response.Items.Count,
                    TotalPages = totalPages,
                    SearchTerm = searchTerm,
                    Category = category
                };

                return View(response);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new Services.GetDishesResponse());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMeal([FromBody] Services.CreateDishRequest request, CancellationToken ct)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return Unauthorized(new { message = "Not authenticated" });

            try
            {
                var dish = await _masterDataClient.CreateDishAsync(request, token!, ct);
                TempData["Success"] = $"Đã thêm món \"{dish.Name}\" vào thực đơn thành công.";
                return Json(new { success = true, redirectUrl = Url.Action(nameof(MealDetail), new { id = dish.Id }) });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        public async Task<IActionResult> MealDetail(int id, CancellationToken ct)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                var response = await _masterDataClient.GetDishAsync(id, token, ct);
                return View(response);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Meals));
            }
        }

        public async Task<IActionResult> MealEdit(int id, CancellationToken ct)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                var response = await _masterDataClient.GetDishAsync(id, token, ct);
                return View(response.Dish);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Meals));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadDishImage(IFormFile file, CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return Unauthorized(new { message = "Not authenticated" });

            try
            {
                var uploaded = await _masterDataClient.UploadMediaAsync(file, token!, mediaType: "image", purpose: "dish-image", isPublic: true, ct: ct);
                // Return mediaFileId only. The linking will happen during UpdateMeal.
                return Json(new { mediaFileId = uploaded.MediaFileId, objectName = uploaded.ObjectName, previewUrl = uploaded.Url });
            }
            catch (Exception ex)
            {
                Response.StatusCode = 400;
                return Json(new { message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateMeal(int id, [FromBody] Services.UpdateDishRequest request, CancellationToken ct)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return Unauthorized(new { message = "Not authenticated" });

            try
            {
                await _masterDataClient.UpdateDishAsync(id, request, token!, ct);
                TempData["Success"] = $"Đã cập nhật món \"{request.Name}\" thành công.";
                return Json(new { success = true, redirectUrl = Url.Action(nameof(MealEdit), new { id }) });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
