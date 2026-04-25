using Microsoft.AspNetCore.Mvc;

namespace Khoa_Luan_KS_Web.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult Users() => View();
        public IActionResult BackupRestore() => View();
        public IActionResult ActivityLog() => View();
        public IActionResult AiConfig() => View();
    }
}
