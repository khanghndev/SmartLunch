using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Khoa_Luan_KS_Web.Controllers
{
    [Authorize(Policy = "CustomerArea")]
    public class ProfileController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult Orders() => View();
        public IActionResult Contracts() => View();
    }
}
