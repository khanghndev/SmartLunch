using Microsoft.AspNetCore.Mvc;

namespace Khoa_Luan_KS_Web.Controllers
{
    public class ManagerController : Controller
    {
        // 1. Dashboard
        public IActionResult Index() => View();

        // 2. Catalog
        public IActionResult CatalogEmployees() => View();
        public IActionResult CatalogSuppliers() => View();
        public IActionResult CatalogCustomers() => View();
        public IActionResult CatalogMeals() => View();

        // 3. Menu
        public IActionResult MenuDashboard() => View();
        public IActionResult MenuWeekly() => View();
        public IActionResult MenuDish() => View();
        public IActionResult MenuHistory() => View();
        public IActionResult MenuIngredients() => View();

        // 4. Các module lớn
        public IActionResult FoodSafety() => View();
        public IActionResult Orders() => View();
        public IActionResult Finance() => View();
        public IActionResult Feedback() => View();
        public IActionResult Debts() => View();
        public IActionResult Inventory() => View();
        public IActionResult Import() => View();
        public IActionResult Delivery() => View();
        public IActionResult Contracts() => View();
        // 5. Thêm các tính năng phụ & Detail
        public IActionResult UserProfile() => View(); // Using UserProfile instead of Profile to avoid conflict
        public IActionResult ContractDetail(string id) => View();
        public IActionResult ContractEdit(string id) => View();
        public IActionResult MenuDishForm(string id) => View();
        public IActionResult MenuDetail(string id) => View();
        
        // 6. Nhập hàng theo flow
        public IActionResult ImportCreate() => View();
        public IActionResult ImportList() => View();
        public IActionResult ImportApprove() => View();
        public IActionResult ImportHistory() => View();
        public IActionResult ImportReceipt(string id) => View();
    }
}
