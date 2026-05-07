using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Khoa_Luan_KS_Web.Areas.WarehouseStaff.Controllers
{
    [Area("WarehouseStaff")]
    [Authorize(Policy = "ManagerArea")]
    public class WarehouseController : Controller
    {
        public IActionResult Receive()
        {
            ViewData["Title"] = "Nhập Kho Thực Tế";
            return View();
        }

        [HttpPost]
        public IActionResult ConfirmReceive(string requisitionId)
        {
            TempData["Success"] = $"Đã xác nhận nhập kho phiếu #{requisitionId}. Tồn kho đã được cập nhật.";
            return RedirectToAction("Stock");
        }

        public IActionResult Stock()
        {
            ViewData["Title"] = "Tồn Kho Hiện Tại";
            return View();
        }
    }
}
