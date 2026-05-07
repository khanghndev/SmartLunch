using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Khoa_Luan_KS_Web.Areas.WarehouseStaff.Controllers
{
    [Area("WarehouseStaff")]
    [Authorize(Policy = "ManagerArea")]
    public class RequisitionController : Controller
    {
        public IActionResult Create()
        {
            ViewData["Title"] = "Tạo Phiếu Đề Xuất";
            return View();
        }

        [HttpPost]
        public IActionResult Create(string requisitionType, string note)
        {
            TempData["Success"] = "Phiếu đề xuất đã được tạo thành công và gửi cho Quản lý duyệt!";
            return RedirectToAction("History");
        }

        public IActionResult History()
        {
            ViewData["Title"] = "Lịch Sử Phiếu Duyệt";
            return View();
        }
    }
}
