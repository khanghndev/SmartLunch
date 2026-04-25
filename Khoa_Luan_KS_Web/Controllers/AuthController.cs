using Microsoft.AspNetCore.Mvc;

namespace Khoa_Luan_KS_Web.Controllers
{
    public class AuthController : Controller
    {
        [HttpGet]
        public IActionResult Login() => View();
        
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            if (password != "123456")
            {
                TempData["Error"] = "Mật khẩu không đúng! Hãy nhập 123456";
                return RedirectToAction("Login");
            }

            if (email == "admin@gmail.com") 
            {
                Response.Cookies.Append("user_role", "admin");
                Response.Cookies.Append("user_name", "Quản Trị Viên");
                return Redirect("/Admin/Index");
            }
            if (email == "manager@gmail.com") 
            {
                Response.Cookies.Append("user_role", "manager");
                Response.Cookies.Append("user_name", "Quản Lý Vùng");
                return Redirect("/Manager/Dashboard");
            }
            if (email == "customer@gmail.com") 
            {
                Response.Cookies.Append("user_role", "customer");
                Response.Cookies.Append("user_name", "TRƯỜNG THPT HOÀNG VĂN THỤ");
                return Redirect("/Customer");
            }

            TempData["Error"] = "Email không tồn tại! (Mock: admin@gmail.com, manager@gmail.com, customer@gmail.com)";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("user_role");
            Response.Cookies.Delete("user_name");
            return Redirect("/Customer");
        }

        public IActionResult Register() => View();
        public IActionResult ForgotPassword() => View();
    }
}
