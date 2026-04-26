using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Khoa_Luan_KS_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminArea")]
    public class SystemController : Controller
    {
        public IActionResult BackupRestore() => View();
        public IActionResult ActivityLog() => View();
        public IActionResult AiConfig() => View();
    }
}
