using Microsoft.AspNetCore.Mvc;

namespace Khoa_Luan_KS_Web.Controllers
{
    public class CustomerController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult About() => View();
        public IActionResult Achievements() => View();
        public IActionResult ChooseHuitMeal() => View();
        public IActionResult Reviews() => View();
        public IActionResult Partners() => View();
        public IActionResult HuitMeal() => View();
        public IActionResult Recruitment() => View();
        public IActionResult Contact() => View();
        
        // Services
        public IActionResult Factory() => View();
        public IActionResult Office() => View();
        public IActionResult School() => View();
        public IActionResult Safety() => View();
        public IActionResult SafetyS() => View();
        
        // Menu
        public IActionResult Menu() => View();
        public IActionResult MealDetail() => View();
        
        // Private Profile Dashboard Functions
        public IActionResult Profile() => View();
        public IActionResult Orders() => View();
        public IActionResult Contracts() => View();
        public IActionResult SchoolMenu(string level)
        {
            ViewData["Level"] = string.IsNullOrEmpty(level) ? "mamnon" : level.ToLower();
            return View();
        }
    }
}
