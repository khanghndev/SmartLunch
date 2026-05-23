using System.Text.Json;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services
    .AddAuthentication(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/Login";
        options.Cookie.HttpOnly = true;
        // Cookie is not scoped by port, so keep it role-app specific to avoid demo logouts.
        // Different hostnames (admin.localhost/manager.localhost/customer.localhost) will also isolate cookies.
        var appRole = builder.Configuration["APP_ROLE"] ?? Environment.GetEnvironmentVariable("APP_ROLE") ?? "Customer";
        options.Cookie.Name = $".HuitMeal.Auth.{appRole}";
        options.SlidingExpiration = true;
        options.Events = new Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationEvents
        {
            OnRedirectToLogin = context =>
            {
                var role = ResolveAreaRole(context.Request.Path.Value);
                var returnUrl = context.Request.PathBase + context.Request.Path + context.Request.QueryString;
                var redirectUri = $"/Auth/Login?role={Uri.EscapeDataString(role)}&returnUrl={Uri.EscapeDataString(returnUrl)}";
                context.Response.Redirect(redirectUri);
                return Task.CompletedTask;
            },
            OnRedirectToAccessDenied = async context =>
            {
                // Đang đăng nhập role khác mà truy cập area không thuộc quyền:
                // tự đăng xuất cookie + session + xoá persistent cookie để buộc đăng nhập lại
                await context.HttpContext.SignOutAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
                context.HttpContext.Session.Clear();
                foreach (var cookieName in new[] { "hm_access_token", "hm_refresh_token", "hm_user_email", "hm_user_name" })
                {
                    if (context.Request.Cookies.ContainsKey(cookieName))
                    {
                        context.Response.Cookies.Delete(cookieName);
                    }
                }

                var role = ResolveAreaRole(context.Request.Path.Value);
                var returnUrl = context.Request.PathBase + context.Request.Path + context.Request.QueryString;
                var redirectUri = $"/Auth/Login?role={Uri.EscapeDataString(role)}&returnUrl={Uri.EscapeDataString(returnUrl)}";
                context.Response.Redirect(redirectUri);
            }
        };

        static string ResolveAreaRole(string? pathValue)
        {
            var path = pathValue ?? string.Empty;
            if (path.StartsWith("/Admin", StringComparison.OrdinalIgnoreCase)) return "Admin";
            if (path.StartsWith("/WarehouseStaff", StringComparison.OrdinalIgnoreCase)) return "WarehouseStaff";
            if (path.StartsWith("/Manager", StringComparison.OrdinalIgnoreCase)) return "Manager";
            return "Customer";
        }
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminArea", policy =>
        policy.RequireAssertion(ctx =>
            ctx.User.IsInRole("Admin") || ctx.User.IsInRole("Super Admin")));

    // Khu vực Quản lý công ty: KHÔNG bao gồm các nhân viên kho/bếp/bán hàng
    // (mỗi role có khu vực riêng để tránh nhầm tài khoản).
    options.AddPolicy("ManagerArea", policy =>
        policy.RequireAssertion(ctx =>
            ctx.User.IsInRole("Manager") ||
            ctx.User.IsInRole("Quản lý công ty") ||
            ctx.User.IsInRole("Admin") ||
            ctx.User.IsInRole("Super Admin")));

    // Khu vực Nhân viên kho riêng — chỉ WarehouseStaff (+ Admin để hỗ trợ vận hành)
    options.AddPolicy("WarehouseStaffArea", policy =>
        policy.RequireAssertion(ctx =>
            ctx.User.IsInRole("WarehouseStaff") ||
            ctx.User.IsInRole("Admin") ||
            ctx.User.IsInRole("Super Admin")));

    options.AddPolicy("CustomerArea", policy =>
        policy.RequireAssertion(ctx =>
            ctx.User.IsInRole("Customer") || 
            ctx.User.IsInRole("Organization") ||
            ctx.User.IsInRole("Khách hàng doanh nghiệp") || 
            ctx.User.IsInRole("Khách hàng cá nhân")));
});

builder.Services.AddScoped<Khoa_Luan_KS_Web.Services.IApiTokenService, Khoa_Luan_KS_Web.Services.ApiTokenService>();
builder.Services.AddScoped<Khoa_Luan_KS_Web.Services.BackendAuthClient>();
builder.Services.AddScoped<Khoa_Luan_KS_Web.Services.BackendCompanyProfileClient>();
builder.Services.AddScoped<Khoa_Luan_KS_Web.Services.BackendMasterDataClient>();
builder.Services.AddScoped<Khoa_Luan_KS_Web.Services.CustomerHomeFeaturedMenuService>();
builder.Services.AddScoped<Khoa_Luan_KS_Web.Services.BackendMenuSuggestionClient>();
builder.Services.AddScoped<Khoa_Luan_KS_Web.Services.BackendWarehouseClient>();
builder.Services.AddScoped<Khoa_Luan_KS_Web.Services.BackendDeliveryClient>();
builder.Services.AddScoped<Khoa_Luan_KS_Web.Services.ReportExcelExportService>();

builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "RequestVerificationToken";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

// Restore token from persistent cookie into Session when Session is empty
app.Use(async (ctx, next) =>
{
    if (string.IsNullOrEmpty(ctx.Session.GetString(Khoa_Luan_KS_Web.Services.ApiTokenService.AccessTokenSessionKey)))
    {
        var tokenFromCookie = ctx.Request.Cookies[Khoa_Luan_KS_Web.Services.ApiTokenService.AccessTokenCookieKey];
        if (!string.IsNullOrEmpty(tokenFromCookie))
        {
            ctx.Session.SetString(Khoa_Luan_KS_Web.Services.ApiTokenService.AccessTokenSessionKey, tokenFromCookie);
            ctx.Session.SetString(Khoa_Luan_KS_Web.Services.ApiTokenService.RefreshTokenSessionKey,
                ctx.Request.Cookies[Khoa_Luan_KS_Web.Services.ApiTokenService.RefreshTokenCookieKey] ?? "");
            ctx.Session.SetString("user_email",
                ctx.Request.Cookies[Khoa_Luan_KS_Web.Services.ApiTokenService.UserEmailCookieKey] ?? "");
            ctx.Session.SetString("user_name",
                ctx.Request.Cookies[Khoa_Luan_KS_Web.Services.ApiTokenService.UserNameCookieKey] ?? "");
        }
    }
    await next();
});

app.UseAuthentication();
app.UseAuthorization();

// Tự refresh JWT (Admin / Manager / WarehouseStaff / Customer / Organization)
app.UseMiddleware<Khoa_Luan_KS_Web.Middleware.JwtRefreshMiddleware>();

// Role-specific friendly prefixes for demo:
// - https://localhost:5101/admin
// - https://localhost:5102/manager
app.MapAreaControllerRoute(
    name: "AdminArea",
    areaName: "Admin",
    pattern: "admin/{controller=Home}/{action=Index}/{id?}");

app.MapAreaControllerRoute(
    name: "ManagerCatalogFriendly",
    areaName: "Manager",
    pattern: "Manager/{action}/{id?}",
    defaults: new { controller = "Catalog" },
    constraints: new { action = "(Suppliers|SupplierDetail|Employees|EmployeeDetail|Customers|CustomerDetail|Meals|MealDetail|MealEdit)" });

app.MapAreaControllerRoute(
    name: "ManagerArea",
    areaName: "Manager",
    pattern: "manager/{controller=Home}/{action=Index}/{id?}");

app.MapAreaControllerRoute(
    name: "WarehouseStaffArea",
    areaName: "WarehouseStaff",
    pattern: "WarehouseStaff/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Customer}/{action=Index}/{id?}");

app.Run();
