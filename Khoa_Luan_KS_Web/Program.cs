using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient();

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
                var path = context.Request.Path.Value ?? string.Empty;
                var role = path.StartsWith("/Admin", StringComparison.OrdinalIgnoreCase) || path.StartsWith("/admin", StringComparison.OrdinalIgnoreCase)
                    ? "Admin"
                    : path.StartsWith("/Manager", StringComparison.OrdinalIgnoreCase) || path.StartsWith("/manager", StringComparison.OrdinalIgnoreCase)
                        ? "Manager"
                        : "Customer";

                var returnUrl = context.Request.PathBase + context.Request.Path + context.Request.QueryString;
                var redirectUri = $"/Auth/Login?role={Uri.EscapeDataString(role)}&returnUrl={Uri.EscapeDataString(returnUrl)}";

                context.Response.Redirect(redirectUri);
                return Task.CompletedTask;
            },
            OnRedirectToAccessDenied = async context =>
            {
                await context.HttpContext.SignOutAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
                context.HttpContext.Session.Clear();

                var path = context.Request.Path.Value ?? string.Empty;
                var role = path.StartsWith("/Admin", StringComparison.OrdinalIgnoreCase) || path.StartsWith("/admin", StringComparison.OrdinalIgnoreCase)
                    ? "Admin"
                    : path.StartsWith("/Manager", StringComparison.OrdinalIgnoreCase) || path.StartsWith("/manager", StringComparison.OrdinalIgnoreCase)
                        ? "Manager"
                        : "Customer";

                var returnUrl = context.Request.PathBase + context.Request.Path + context.Request.QueryString;
                var redirectUri = $"/Auth/Login?role={Uri.EscapeDataString(role)}&returnUrl={Uri.EscapeDataString(returnUrl)}";

                context.Response.Redirect(redirectUri);
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminArea", policy =>
        policy.RequireAssertion(ctx =>
            ctx.User.IsInRole("Admin") || ctx.User.IsInRole("Super Admin")));

    options.AddPolicy("ManagerArea", policy =>
        policy.RequireRole("Quản lý công ty"));

    options.AddPolicy("CustomerArea", policy =>
        policy.RequireAssertion(ctx =>
            ctx.User.IsInRole("Khách hàng doanh nghiệp") || ctx.User.IsInRole("Khách hàng cá nhân")));
});

builder.Services.AddScoped<Khoa_Luan_KS_Web.Services.BackendAuthClient>();

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

app.UseAuthentication();
app.UseAuthorization();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Customer}/{action=Index}/{id?}");

app.Run();
