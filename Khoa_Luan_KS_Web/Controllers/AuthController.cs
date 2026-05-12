using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Khoa_Luan_KS_Web.Services;

namespace Khoa_Luan_KS_Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly BackendAuthClient _backendAuthClient;

        public AuthController(BackendAuthClient backendAuthClient)
        {
            _backendAuthClient = backendAuthClient;
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

                // Persist access/refresh tokens — store in long-lived cookies AND session
                var tokenCookieOpts = new CookieOptions
                {
                    HttpOnly = true,
                    Secure   = true,
                    SameSite = SameSiteMode.Lax,
                    Expires  = DateTimeOffset.UtcNow.AddDays(7)
                };
                Response.Cookies.Append("hm_access_token",  login.AccessToken,  tokenCookieOpts);
                Response.Cookies.Append("hm_refresh_token", login.RefreshToken, tokenCookieOpts);
                Response.Cookies.Append("hm_user_email",    login.Email,
                    new CookieOptions { HttpOnly = true, Secure = true, SameSite = SameSiteMode.Lax, Expires = DateTimeOffset.UtcNow.AddDays(7) });
                Response.Cookies.Append("hm_user_name",     login.FullName,
                    new CookieOptions { HttpOnly = true, Secure = true, SameSite = SameSiteMode.Lax, Expires = DateTimeOffset.UtcNow.AddDays(7) });

                HttpContext.Session.SetString("access_token",  login.AccessToken);
                HttpContext.Session.SetString("refresh_token", login.RefreshToken);
                HttpContext.Session.SetString("user_email",    login.Email);
                HttpContext.Session.SetString("user_name",     login.FullName);

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
            // Remove persistent token cookies
            foreach (var key in new[] { "hm_access_token", "hm_refresh_token", "hm_user_email", "hm_user_name" })
                Response.Cookies.Delete(key);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!string.IsNullOrEmpty(roleHint))
                return RedirectToAction("Login", new { role = roleHint });

            return RedirectToAction("Index", "Customer");
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(string FullName, string Email, string Phone, string Password, string ConfirmPassword, CancellationToken cancellationToken)
        {
            if (Password != ConfirmPassword)
            {
                TempData["Error"] = "Mật khẩu xác nhận không khớp.";
                return RedirectToAction("Register");
            }

            try
            {
                await _backendAuthClient.RegisterAsync(Email, Password, ConfirmPassword, null, cancellationToken);
                TempData["Success"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login", new { role = "CUSTOMER" });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Register");
            }
        }

        public IActionResult ForgotPassword() => View();

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

            // Ensure Name is set correctly for UI usage if not already in standard claim
            if (!identity.HasClaim(c => c.Type == ClaimTypes.Name))
            {
                var name = token.Claims.FirstOrDefault(c => c.Type == "FullName")?.Value
                           ?? token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value
                           ?? token.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value
                           ?? token.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
                if (!string.IsNullOrWhiteSpace(name))
                    identity.AddClaim(new Claim(ClaimTypes.Name, name));
            }

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
