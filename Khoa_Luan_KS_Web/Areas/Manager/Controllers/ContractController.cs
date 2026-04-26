using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Khoa_Luan_KS_Web.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Policy = "ManagerArea")]
    public class ContractController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult Detail(string id) => View();
        public IActionResult Edit(string id) => View();
    }
}
