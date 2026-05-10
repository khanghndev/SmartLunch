using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Khoa_Luan_KS_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminArea")]
    public class SystemController : Controller
    {
        private readonly Services.BackendMasterDataClient _adminClient;

        public SystemController(Services.BackendMasterDataClient adminClient)
        {
            _adminClient = adminClient;
        }

        public async Task<IActionResult> BackupRestore(int page = 1, CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                var response = await _adminClient.GetSystemBackupsAsync(token, page, 10, false, ct);
                return View(response);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new Services.GetSystemBackupsResponse());
            }
        }

        public async Task<IActionResult> ActivityLog(int page = 1, CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                var response = await _adminClient.GetSystemLogsAsync(token, page, 50, ct);
                return View(response);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new Services.GetSystemLogsResponse());
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateBackup(CancellationToken ct)
        {
            var token = HttpContext.Session.GetString("access_token");
            try
            {
                await _adminClient.BackupSystemAsync(token!, ct);
                TempData["Success"] = "Đã tạo bản sao lưu thành công.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(BackupRestore));
        }

        [HttpPost]
        public async Task<IActionResult> RestoreBackup(int id, CancellationToken ct)
        {
            var token = HttpContext.Session.GetString("access_token");
            try
            {
                var request = new Services.RestoreSystemRequest { BackupId = id };
                await _adminClient.RestoreSystemAsync(request, token!, ct);
                TempData["Success"] = "Đã phục hồi hệ thống thành công.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(BackupRestore));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBackup(int id, bool deleteFile, CancellationToken ct)
        {
            var token = HttpContext.Session.GetString("access_token");
            try
            {
                await _adminClient.DeleteSystemBackupAsync(id, deleteFile, token!, ct);
                TempData["Success"] = "Đã xóa bản sao lưu thành công.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(BackupRestore));
        }

        public IActionResult AiConfig() => View();
    }
}
