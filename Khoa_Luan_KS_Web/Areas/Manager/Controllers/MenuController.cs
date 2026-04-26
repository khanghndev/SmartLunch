using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Khoa_Luan_KS_Web.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Policy = "ManagerArea")]
    public class MenuController : Controller
    {
        public IActionResult Dashboard() => View();
        public IActionResult Weekly() => View();
        public IActionResult Dish() => View();
        public IActionResult History() => View();
        public IActionResult Ingredients() => View();
        public IActionResult DishForm(string id) => View();
        public IActionResult Detail(string id) => View();
    }
}
