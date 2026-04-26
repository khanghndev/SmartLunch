using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Khoa_Luan_KS_Web.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Policy = "ManagerArea")]
    public class OperationController : Controller
    {
        public IActionResult FoodSafety() => View();
        public IActionResult Feedback() => View();
    }
}
