using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Khoa_Luan_KS_Web.Areas.Admin.Models;

namespace Khoa_Luan_KS_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminArea")]
    public class SystemController : Controller
    {
        private readonly Services.BackendMasterDataClient _adminClient;
        private readonly IConfiguration _configuration;

        public SystemController(Services.BackendMasterDataClient adminClient, IConfiguration configuration)
        {
            _adminClient = adminClient;
            _configuration = configuration;
        }

        public async Task<IActionResult> BackupRestore(
            int page = 1,
            DateTime? from = null,
            DateTime? to = null,
            CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                if (from.HasValue) from = from.Value.Date;
                if (to.HasValue) to = to.Value.Date.AddDays(1).AddTicks(-1);

                var backups = await _adminClient.GetSystemBackupsAsync(token, page, 15, false, from, to, ct);
                var schedule = await _adminClient.GetBackupScheduleAsync(token, ct);
                ViewBag.BackendApiBaseUrl = (_configuration["BackendApi:BaseUrl"] ?? "").TrimEnd('/');

                return View(new BackupRestorePageVm
                {
                    Backups = backups,
                    Schedule = schedule,
                    Page = page,
                    FilterFrom = from,
                    FilterTo = to
                });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new BackupRestorePageVm());
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveBackupSchedule(
            bool isEnabled,
            string scheduleMode,
            string? timeOfDay,
            int? dayOfWeek,
            DateTime? onceScheduledAt,
            CancellationToken ct)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                await _adminClient.UpdateBackupScheduleAsync(new Services.UpdateBackupScheduleRequest
                {
                    IsEnabled = isEnabled,
                    ScheduleMode = "Daily",
                    TimeOfDay = string.IsNullOrWhiteSpace(timeOfDay) ? "21:00" : timeOfDay,
                    DayOfWeek = null,
                    OnceScheduledAt = null
                }, token, ct);
                TempData["Success"] = "Đã lưu lịch sao lưu tự động.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(BackupRestore));
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
                TempData["Success"] = "Đã tạo bản sao lưu và tải lên Appwrite thành công.";
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
                await _adminClient.RestoreSystemAsync(new Services.RestoreSystemRequest { BackupId = id }, token!, ct);
                TempData["Success"] = "Đã khôi phục cơ sở dữ liệu từ bản sao lưu đã chọn.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(BackupRestore));
        }

        public async Task<IActionResult> DownloadBackup(int id, CancellationToken ct)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                var url = await _adminClient.GetSystemBackupDownloadUrlAsync(id, token, ct);
                return Redirect(url);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(BackupRestore));
            }
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
