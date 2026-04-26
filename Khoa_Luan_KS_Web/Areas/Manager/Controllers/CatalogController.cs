using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Khoa_Luan_KS_Web.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Policy = "ManagerArea")]
    public class CatalogController : Controller
    {
        public IActionResult Employees() => View();
        public IActionResult EmployeeDetail(string id) => View();
        public IActionResult Suppliers() => View();
        public IActionResult SupplierDetail(string id) => View();
        public IActionResult Customers() => View();
        public IActionResult CustomerDetail(string id) => View();
        public IActionResult Meals() => View();
        public IActionResult MealDetail(string id) => View();
        public IActionResult MealEdit(string id) => View();
    }
}
