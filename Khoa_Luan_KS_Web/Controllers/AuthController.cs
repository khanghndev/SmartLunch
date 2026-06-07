using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Khoa_Luan_KS_Web.Services;
using Khoa_Luan_KS_Web.Helpers;

namespace Khoa_Luan_KS_Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly BackendAuthClient _backendAuthClient;
        private readonly IApiTokenService _apiTokenService;

        public AuthController(BackendAuthClient backendAuthClient, IApiTokenService apiTokenService)
        {
            _backendAuthClient = backendAuthClient;
            _apiTokenService = apiTokenService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // If you open the wrong app host (ex: admin.localhost) without role query, default role by APP_ROLE for nicer demos.
            if (string.IsNullOrWhiteSpace(Request.Query["role"]))
            {
                var appRole = Environment.GetEnvironmentVariable("APP_ROLE");
                if (!string.IsNullOrWhiteSpace(appRole))
                    return RedirectToAction("Login", new { role = appRole, returnUrl = Request.Query["returnUrl"].ToString() });
            }
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, string? role, string? returnUrl, CancellationToken cancellationToken)
        {
            // role comes from query (?role=Admin/Manager/Customer) and is posted as a hidden input
            role = string.IsNullOrWhiteSpace(role) ? Request.Query["role"].ToString() : role;
            role = string.IsNullOrWhiteSpace(role) ? "Customer" : role;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                TempData["Error"] = "Vui lòng nhập đầy đủ thông tin đăng nhập.";
                return RedirectToAction("Login");
            }

            try
            {
                LoginResponse login;

                if (string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
                {
                    // backend admin login uses Username (seed: admin / superadmin, password: 123)
                    var username = NormalizeAdminUsername(email);
                    login = await _backendAuthClient.LoginAdminAsync(username, password, cancellationToken);
                }
                else
                {
                    // backend user login uses Email (seed: quanly@smartlunch.com / khach1@gmail.com, password: 123)
                    login = await _backendAuthClient.LoginUserAsync(email, password, cancellationToken);
                }

                var principal = BuildPrincipalFromJwt(login.AccessToken);

                _apiTokenService.PersistTokens(
                    login.AccessToken,
                    login.RefreshToken,
                    login.Email,
                    UserDisplayNameHelper.Resolve(login.FullName, login.Email, login.Username));

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                    });

                if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                // Redirect by effective area role derived from JWT claims
                if (IsAdmin(principal)) return Redirect("/admin");
                if (IsWarehouseStaff(principal)) return Redirect("/WarehouseStaff");
                if (IsStaff(principal)) return Redirect("/manager");
                return RedirectToAction("Index", "Customer");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Login", new { role });
            }

        }

        [HttpGet]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            // Identify role before clearing session/auth cookie
            string? roleHint = null;
            if (IsAdmin(User)) roleHint = "Admin";
            else if (IsWarehouseStaff(User)) roleHint = "WarehouseStaff";
            else if (IsStaff(User)) roleHint = "Manager";

            var accessToken = HttpContext.Session.GetString("access_token");

            try
            {
                if (!string.IsNullOrWhiteSpace(accessToken))
                    await _backendAuthClient.LogoutAsync(accessToken, cancellationToken);
            }
            catch
            {
                // best-effort logout; still clear local session
            }

            HttpContext.Session.Clear();
            _apiTokenService.ClearTokens();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!string.IsNullOrEmpty(roleHint))
                return RedirectToAction("Login", new { role = roleHint });

            return RedirectToAction("Index", "Customer");
        }

        [HttpGet]
        public IActionResult Register(string? role)
        {
            ViewBag.LoginRole = string.IsNullOrWhiteSpace(role) ? "Customer" : role;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            string FullName,
            string Email,
            string Phone,
            string Password,
            string ConfirmPassword,
            string OrganizationName,
            string OrganizationType,
            string OrganizationAddress,
            string? TaxCode,
            string? role,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(OrganizationName))
            {
                TempData["Error"] = "Vui lòng nhập tên doanh nghiệp / đơn vị.";
                return RedirectToAction(nameof(Register), new { role });
            }

            if (string.IsNullOrWhiteSpace(OrganizationAddress))
            {
                TempData["Error"] = "Vui lòng nhập địa chỉ doanh nghiệp.";
                return RedirectToAction(nameof(Register), new { role });
            }

            if (string.IsNullOrWhiteSpace(FullName))
            {
                TempData["Error"] = "Vui lòng nhập họ tên người liên hệ.";
                return RedirectToAction(nameof(Register), new { role });
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                TempData["Error"] = "Vui lòng nhập email.";
                return RedirectToAction(nameof(Register), new { role });
            }

            if (Password != ConfirmPassword)
            {
                TempData["Error"] = "Mật khẩu xác nhận không khớp.";
                return RedirectToAction(nameof(Register), new { role });
            }

            if (Password.Length < 6)
            {
                TempData["Error"] = "Mật khẩu phải có ít nhất 6 ký tự.";
                return RedirectToAction(nameof(Register), new { role });
            }

            try
            {
                await _backendAuthClient.RegisterAsync(new RegisterClientRequest
                {
                    Email = Email.Trim(),
                    Password = Password,
                    ConfirmPassword = ConfirmPassword,
                    FullName = FullName.Trim(),
                    PhoneNumber = string.IsNullOrWhiteSpace(Phone) ? null : Phone.Trim(),
                    AccountType = "Organization",
                    OrganizationName = OrganizationName.Trim(),
                    OrganizationType = string.IsNullOrWhiteSpace(OrganizationType) ? "Office" : OrganizationType.Trim(),
                    OrganizationAddress = OrganizationAddress.Trim(),
                    TaxCode = string.IsNullOrWhiteSpace(TaxCode) ? null : TaxCode.Trim()
                }, cancellationToken);

                TempData["Success"] = "Đăng ký tài khoản doanh nghiệp thành công! Vui lòng đăng nhập để đặt suất ăn.";
                return RedirectToAction("Login", new { role = "CUSTOMER" });
            }
            catch (Exception ex)
            {
                TempData["Error"] = TranslateRegisterError(ex.Message);
                return RedirectToAction(nameof(Register), new { role });
            }
        }

        private static string TranslateRegisterError(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return "Không thể đăng ký. Vui lòng thử lại.";

            return message switch
            {
                "Email already exists" => "Email này đã được sử dụng. Vui lòng đăng nhập hoặc dùng email khác.",
                "Email and password are required" => "Vui lòng nhập email và mật khẩu.",
                "Password must be at least 6 characters" => "Mật khẩu phải có ít nhất 6 ký tự.",
                "Password and confirm password do not match" => "Mật khẩu xác nhận không khớp.",
                "Organization name is required" => "Vui lòng nhập tên doanh nghiệp / đơn vị.",
                "Organization address is required" => "Vui lòng nhập địa chỉ doanh nghiệp.",
                "Contact person name is required" => "Vui lòng nhập họ tên người liên hệ.",
                var m when m.Contains("Customer role", StringComparison.OrdinalIgnoreCase)
                    => "Hệ thống chưa cấu hình vai trò Khách hàng. Liên hệ quản trị viên.",
                var m when m.Contains("Organization", StringComparison.OrdinalIgnoreCase) && m.Contains("not configured", StringComparison.OrdinalIgnoreCase)
                    => "Hệ thống chưa cấu hình vai trò Doanh nghiệp. Liên hệ quản trị viên.",
                "An internal server error occurred" => "Lỗi máy chủ khi đăng ký. Vui lòng thử lại sau.",
                _ => message
            };
        }

        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string email, string? role, CancellationToken cancellationToken)
        {
            role = string.IsNullOrWhiteSpace(role) ? "Customer" : role;

            if (string.IsNullOrWhiteSpace(email))
            {
                TempData["Error"] = "Vui lòng nhập email đã đăng ký.";
                return RedirectToAction(nameof(ForgotPassword), new { role });
            }

            try
            {
                var resetPageUrl = $"{Request.Scheme}://{Request.Host}/Auth/ResetPassword";
                var message = await _backendAuthClient.RequestForgotPasswordAsync(email.Trim(), resetPageUrl, cancellationToken);
                TempData["Success"] = message;
                return RedirectToAction(nameof(Login), new { role });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(ForgotPassword), new { role });
            }
        }

        [HttpGet]
        public IActionResult ResetPassword(string? token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                TempData["Error"] = "Link đặt lại mật khẩu không hợp lệ.";
                return RedirectToAction(nameof(Login), new { role = "Customer" });
            }

            ViewBag.Token = token;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string token, string newPassword, string confirmPassword, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                TempData["Error"] = "Link đặt lại mật khẩu không hợp lệ.";
                return RedirectToAction(nameof(Login), new { role = "Customer" });
            }

            try
            {
                await _backendAuthClient.ConfirmForgotPasswordAsync(token, newPassword, confirmPassword, cancellationToken);
                TempData["Success"] = "Đặt lại mật khẩu thành công. Vui lòng đăng nhập bằng mật khẩu mới.";
                return RedirectToAction(nameof(Login), new { role = "Customer" });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                ViewBag.Token = token;
                return View();
            }
        }

        private static string NormalizeAdminUsername(string input)
        {
            input = input.Trim();
            if (input.Contains('@'))
            {
                if (input.Equals("admin@smartlunch.com", StringComparison.OrdinalIgnoreCase)) return "admin";
                if (input.Equals("superadmin@smartlunch.com", StringComparison.OrdinalIgnoreCase)) return "superadmin";
                // fallback: take local part
                return input.Split('@')[0];
            }
            return input;
        }

        private static ClaimsPrincipal BuildPrincipalFromJwt(string jwt)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);

            // Use the constructor that specifies name and role claim types
            var identity = new ClaimsIdentity(
                token.Claims, 
                CookieAuthenticationDefaults.AuthenticationScheme,
                ClaimTypes.Name,
                ClaimTypes.Role);

            ApplyDisplayNameClaim(identity, token.Claims);

            // Ensure Role is set correctly for IsInRole usage
            var roles = token.Claims.Where(c => c.Type == "role" || c.Type == "roles" || c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
            foreach (var roleName in roles)
            {
                if (!identity.HasClaim(ClaimTypes.Role, roleName))
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, roleName));
                }
            }

            return new ClaimsPrincipal(identity);
        }

        private static void ApplyDisplayNameClaim(ClaimsIdentity identity, IEnumerable<Claim> jwtClaims)
        {
            var claims = jwtClaims.ToList();
            var fullName = claims.FirstOrDefault(c => c.Type == "FullName")?.Value;
            var username = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value
                           ?? claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;
            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var displayName = UserDisplayNameHelper.Resolve(fullName, email, username);

            var existing = identity.FindFirst(ClaimTypes.Name);
            if (existing != null)
                identity.RemoveClaim(existing);

            identity.AddClaim(new Claim(ClaimTypes.Name, displayName));

            if (!string.IsNullOrWhiteSpace(username) && !identity.HasClaim(c => c.Type == "Username"))
                identity.AddClaim(new Claim("Username", username));
        }

        private static bool IsAdmin(ClaimsPrincipal principal) =>
            principal.IsInRole("Admin") || principal.IsInRole("Super Admin");

        private static bool IsWarehouseStaff(ClaimsPrincipal principal) =>
            principal.IsInRole("WarehouseStaff");

        private static bool IsStaff(ClaimsPrincipal principal) =>
            principal.IsInRole("Manager") || 
            principal.IsInRole("WarehouseStaff") || 
            principal.IsInRole("ChefStaff") || 
            principal.IsInRole("SalesStaff") || 
            principal.IsInRole("Shipper") ||
            principal.IsInRole("Quản lý công ty");
    }
}
