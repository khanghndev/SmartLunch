using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Khoa_Luan_KS_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminArea")]
    public class RoleController : Controller
    {
        private readonly Services.BackendMasterDataClient _adminClient;

        public RoleController(Services.BackendMasterDataClient adminClient)
        {
            _adminClient = adminClient;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                // Load all roles and all possible permissions
                var rolesTask = _adminClient.GetRolesAsync(token, ct: ct);
                var permissionsTask = _adminClient.GetPermissionsAsync(token, ct: ct);

                await Task.WhenAll(rolesTask, permissionsTask);

                ViewBag.Roles = rolesTask.Result.Items;
                ViewBag.Permissions = permissionsTask.Result.Items;
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                ViewBag.Roles = new List<Services.RoleDto>();
                ViewBag.Permissions = new List<Services.PermissionDto>();
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetRolePermissions(int roleId, CancellationToken ct)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return Unauthorized();

            try
            {
                var response = await _adminClient.GetRolePermissionsAsync(roleId, token, ct);
                var permissionIds = response.Items.Where(p => p.IsActive).Select(p => p.PermissionId).ToList();
                return Json(new { success = true, permissionIds });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> TogglePermission([FromBody] TogglePermissionRequest request, CancellationToken ct)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return Unauthorized();

            try
            {
                if (request.Grant)
                {
                    await _adminClient.GrantPermissionToRoleAsync(request.RoleId, request.PermissionId, token, ct);
                }
                else
                {
                    await _adminClient.RevokePermissionFromRoleAsync(request.RoleId, request.PermissionId, token, ct);
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }

    public class TogglePermissionRequest
    {
        public int RoleId { get; set; }
        public int PermissionId { get; set; }
        public bool Grant { get; set; }
    }
}
