using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Khoa_Luan_KS_Web.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Policy = "ManagerArea")]
    public class ImportController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult Create() => View();
        public IActionResult List() => View();
        public IActionResult Approve() => View();
        public IActionResult History() => View();
        public IActionResult Receipt(string id) => View();
    }
}
