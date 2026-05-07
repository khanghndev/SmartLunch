using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Khoa_Luan_KS_Web.Areas.WarehouseStaff.Controllers
{
    [Area("WarehouseStaff")]
    [Authorize(Policy = "ManagerArea")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Tổng quan Kho";
            return View();
        }
    }
}
