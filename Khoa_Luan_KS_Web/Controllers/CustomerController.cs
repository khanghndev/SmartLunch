using Microsoft.AspNetCore.Mvc;

namespace Khoa_Luan_KS_Web.Controllers
{
    public class CustomerController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult About() => View();
        public IActionResult Achievements() => View();
        public IActionResult ChooseHaseca() => View();
        public IActionResult Reviews() => View();
        public IActionResult Partners() => View();
        public IActionResult Haseca() => View();
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
    }
}
