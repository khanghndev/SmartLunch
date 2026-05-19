using System.Text.Json;
using System.Text.Json.Serialization;
using Khoa_Luan_KS_Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Khoa_Luan_KS_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminArea")]
    public class RoleController : Controller
    {
        private readonly Services.BackendMasterDataClient _adminClient;

        private static readonly JsonSerializerOptions JsonCamelCase = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        private static readonly Dictionary<string, string> ResourceModuleLabels = new(StringComparer.OrdinalIgnoreCase)
        {
            ["users"] = "Hệ thống & Tài khoản",
            ["roles"] = "Hệ thống & Tài khoản",
            ["permissions"] = "Hệ thống & Tài khoản",
            ["user_roles"] = "Hệ thống & Tài khoản",
            ["user_permissions"] = "Hệ thống & Tài khoản",
            ["role_permissions"] = "Hệ thống & Tài khoản",
            ["user_tokens"] = "Hệ thống & Tài khoản",
            ["media_files"] = "Hệ thống & Tài khoản",
            ["organizations"] = "Tổ chức & Đối tác",
            ["user_organizations"] = "Tổ chức & Đối tác",
            ["partners"] = "Tổ chức & Đối tác",
            ["contracts"] = "Tổ chức & Đối tác",
            ["partner_payments"] = "Tổ chức & Đối tác",
            ["ingredients"] = "Kho & Nguyên liệu",
            ["ingredient_sources"] = "Kho & Nguyên liệu",
            ["inventory"] = "Kho & Nguyên liệu",
            ["internal_stock_issues"] = "Kho & Nguyên liệu",
            ["internal_stock_issue_lines"] = "Kho & Nguyên liệu",
            ["ingredient_intake_proposals"] = "Kho & Nguyên liệu",
            ["ingredient_intake_proposal_lines"] = "Kho & Nguyên liệu",
            ["ingredient_actual_intakes"] = "Kho & Nguyên liệu",
            ["ingredient_actual_intake_lines"] = "Kho & Nguyên liệu",
            ["dishes"] = "Thực đơn & Món ăn",
            ["dish_ingredients"] = "Thực đơn & Món ăn",
            ["weekly_menus"] = "Thực đơn & Món ăn",
            ["menu_schedule"] = "Thực đơn & Món ăn",
            ["menu_suggestions"] = "Thực đơn & Món ăn",
            ["orders"] = "Kinh doanh & Đơn hàng",
            ["order_items"] = "Kinh doanh & Đơn hàng",
            ["deliveries"] = "Kinh doanh & Đơn hàng",
            ["payments"] = "Kinh doanh & Đơn hàng",
            ["transactions"] = "Kinh doanh & Đơn hàng",
            ["reviews"] = "Chăm sóc Khách hàng",
            ["sentiments"] = "Chăm sóc Khách hàng",
            ["complaints"] = "Chăm sóc Khách hàng",
            ["chatbot_logs"] = "Chăm sóc Khách hàng",
        };

        public RoleController(Services.BackendMasterDataClient adminClient)
        {
            _adminClient = adminClient;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            var model = new RolePermissionIndexViewModel();

            try
            {
                var rolesTask = _adminClient.GetRolesAsync(token, ct: ct);
                var permissionsTask = _adminClient.GetPermissionsAsync(token, isActive: true, ct: ct);
                await Task.WhenAll(rolesTask, permissionsTask);

                var roles = rolesTask.Result.Items.OrderBy(r => r.Name).ToList();
                var permissions = permissionsTask.Result.Items
                    .Where(p => p.IsActive)
                    .OrderBy(p => p.Resource)
                    .ThenBy(p => ActionSortKey(p.Action))
                    .ToList();

                model.Roles = roles.Select(r => new RoleListItemVm
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    IsSystemRole = r.IsSystemRole,
                    IsActive = r.IsActive,
                }).ToList();

                model.Actions = permissions
                    .Select(p => p.Action)
                    .Distinct()
                    .OrderBy(ActionSortKey)
                    .ToList();

                model.PermissionMap = permissions
                    .GroupBy(p => p.Resource)
                    .ToDictionary(
                        g => g.Key,
                        g => g.ToDictionary(p => p.Action, p => p.Id, StringComparer.OrdinalIgnoreCase),
                        StringComparer.OrdinalIgnoreCase);

                model.Modules = permissions
                    .Select(p => p.Resource)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .GroupBy(r => ResourceModuleLabels.TryGetValue(r, out var label) ? label : "Khác")
                    .OrderBy(g => ModuleSortKey(g.Key))
                    .Select(g => new PermissionModuleVm
                    {
                        Name = g.Key,
                        Resources = g.OrderBy(r => r, StringComparer.OrdinalIgnoreCase).ToList(),
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return View(model);
        }

        private static int ActionSortKey(string action) => action switch
        {
            "list" => 1,
            "read" => 2,
            "create" => 3,
            "update" => 4,
            "delete" => 5,
            _ => 99,
        };

        private static int ModuleSortKey(string name) => name switch
        {
            "Hệ thống & Tài khoản" => 1,
            "Tổ chức & Đối tác" => 2,
            "Kho & Nguyên liệu" => 3,
            "Thực đơn & Món ăn" => 4,
            "Kinh doanh & Đơn hàng" => 5,
            "Chăm sóc Khách hàng" => 6,
            _ => 99,
        };

        [HttpGet]
        public async Task<IActionResult> GetRolePermissions(int roleId, CancellationToken ct)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return Unauthorized();

            try
            {
                var response = await _adminClient.GetRolePermissionsAsync(roleId, token, ct);
                var permissionIds = response.Items
                    .Where(p => p.IsActive)
                    .Select(p => p.PermissionId)
                    .Distinct()
                    .ToList();
                return Json(new { success = true, permissionIds }, JsonCamelCase);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonCamelCase);
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
                return Json(new { success = true, message = request.Grant ? "Đã cấp quyền." : "Đã thu hồi quyền." }, JsonCamelCase);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonCamelCase);
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
