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

                // Persist access/refresh tokens for server-side API calls
                HttpContext.Session.SetString("access_token", login.AccessToken);
                HttpContext.Session.SetString("refresh_token", login.RefreshToken);
                HttpContext.Session.SetString("user_email", login.Email);
                HttpContext.Session.SetString("user_name", login.Username);

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
                if (IsManager(principal)) return Redirect("/manager");
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
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Customer");
        }

        public IActionResult Register() => View();
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

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

            foreach (var claim in token.Claims)
            {
                identity.AddClaim(claim);
            }

            // Ensure Name is set for UI usage
            var name = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value
                       ?? token.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value
                       ?? token.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            if (!string.IsNullOrWhiteSpace(name))
                identity.AddClaim(new Claim(ClaimTypes.Name, name));

            return new ClaimsPrincipal(identity);
        }

        private static bool IsAdmin(ClaimsPrincipal principal) =>
            principal.IsInRole("Admin") || principal.IsInRole("Super Admin");

        private static bool IsManager(ClaimsPrincipal principal) =>
            principal.IsInRole("Quản lý công ty");
    }
}
