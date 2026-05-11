using System.Security.Claims;
using System.Text.Json;
using Khoa_Luan_KS_Web.Models;
using Khoa_Luan_KS_Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Khoa_Luan_KS_Web.Controllers;

[Authorize(Policy = "CustomerArea")]
public class CartController : Controller
{
    private const string CartSessionKey = "huit_cart_v1";
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    private readonly BackendMasterDataClient _masterDataClient;

    public CartController(BackendMasterDataClient masterDataClient)
    {
        _masterDataClient = masterDataClient;
    }

    /// <summary>Các role được đặt suất qua giỏ (cá nhân + đại diện đơn vị).</summary>
    private static bool CanSubmitOnlineMealOrder(ClaimsPrincipal user) =>
        user.IsInRole("Customer") ||
        user.IsInRole("Khách hàng cá nhân") ||
        user.IsInRole("Organization") ||
        user.IsInRole("Company") ||
        user.IsInRole("Khách hàng doanh nghiệp");

    /// <summary>Sau đặt hàng: đại diện đơn vị được dẫn sang cổng ký hợp đồng thay vì trang chi tiết đơn.</summary>
    private static bool PreferContractsAfterOrder(ClaimsPrincipal user) =>
        user.IsInRole("Organization") ||
        user.IsInRole("Company") ||
        user.IsInRole("Khách hàng doanh nghiệp");

    [HttpGet]
    public IActionResult Index()
    {
        return View(CartIndexViewModel.Create(GetCartState()));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Add(
        int dishId,
        string name,
        decimal price,
        string? imageUrl,
        int? menuScheduleId,
        DateTime? scheduleDateUtc,
        string? mealSlot,
        int quantity = 1)
    {
        if (dishId <= 0 || string.IsNullOrWhiteSpace(name) || quantity < 1)
        {
            TempData["CartError"] = "Thông tin món không hợp lệ.";
            return RedirectToReferer();
        }

        var state = GetCartState();
        var loose = state.LooseLines;
        var existing = loose.FirstOrDefault(c => c.DishId == dishId && c.MenuScheduleId == menuScheduleId);
        if (existing != null)
            existing.Quantity += quantity;
        else
        {
            loose.Add(new CartLine
            {
                DishId = dishId,
                Name = name.Trim(),
                Price = price,
                Quantity = quantity,
                ImageUrl = imageUrl,
                MenuScheduleId = menuScheduleId,
                ScheduleDateUtc = scheduleDateUtc,
                MealSlot = mealSlot,
            });
        }

        SaveCartState(state);
        TempData["CartSuccess"] = "Đã thêm vào giỏ hàng.";
        return RedirectToReferer();
    }

    /// <summary>
    /// Thêm thực đơn tuần dưới dạng một gói: số bộ menu chung + chọn từng suất (theo ngày).
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddWeekMenu(int weeklyMenuId, bool replaceExisting = true, CancellationToken ct = default)
    {
        if (weeklyMenuId <= 0)
        {
            TempData["CartError"] = MenuViewText.CartWholeWeekInvalid;
            return RedirectToReferer();
        }

        var token = HttpContext.Session.GetString("access_token");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action("Index", "Menu", new { menuId = weeklyMenuId }) });
        }

        GetWeeklyMenuDetailClientResponse detail;
        try
        {
            detail = await _masterDataClient.GetWeeklyMenuDetailAsync(weeklyMenuId, token, ct);
        }
        catch (Exception ex)
        {
            TempData["CartError"] = MenuViewText.CartWholeWeekLoadFailedPrefix + ex.Message;
            return RedirectToReferer();
        }

        if (detail.Schedules.Count == 0)
        {
            TempData["CartError"] = MenuViewText.CartWholeWeekEmpty;
            return RedirectToReferer();
        }

        var state = GetCartState();
        if (replaceExisting)
            state.MenuBundles.RemoveAll(b => b.WeeklyMenuId == weeklyMenuId);

        var wm = detail.WeeklyMenu;
        var bundle = new WeekMenuCartBundle
        {
            BundleId = Guid.NewGuid().ToString("N"),
            WeeklyMenuId = weeklyMenuId,
            Label = $"{wm.StartDate:dd/MM/yyyy} – {wm.EndDate:dd/MM/yyyy}",
            BundleQuantity = 1,
            Slots = detail.Schedules
                .Where(s => s.DishId > 0 && !string.IsNullOrWhiteSpace(s.Dish.Name))
                .Select(s => new WeekMenuSlotSelection
                {
                    ScheduleId = s.Id,
                    DishId = s.DishId,
                    Name = s.Dish.Name.Trim(),
                    Price = s.Dish.Price,
                    ImageUrl = s.Dish.ImageUrl,
                    ScheduleDate = s.Date,
                    MealSlot = s.MealSlot,
                    Selected = true,
                })
                .ToList(),
        };

        if (bundle.Slots.Count == 0)
        {
            TempData["CartError"] = MenuViewText.CartWholeWeekEmpty;
            return RedirectToReferer();
        }

        state.MenuBundles.Add(bundle);
        SaveCartState(state);
        TempData["CartSuccess"] = MenuViewText.CartWholeWeekSuccessBundle;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateBundle(string bundleId, int quantity, [FromForm] int[]? selectedScheduleIds)
    {
        if (string.IsNullOrWhiteSpace(bundleId))
            return RedirectToAction(nameof(Index));

        var state = GetCartState();
        var bundle = state.MenuBundles.FirstOrDefault(b => b.BundleId == bundleId);
        if (bundle == null)
            return RedirectToAction(nameof(Index));

        var selected = selectedScheduleIds != null && selectedScheduleIds.Length > 0
            ? selectedScheduleIds.ToHashSet()
            : new HashSet<int>();

        if (!bundle.Slots.Any(s => selected.Contains(s.ScheduleId)))
        {
            TempData["CartError"] = CartViewText.ErrorNoSlotSelected;
            return RedirectToAction(nameof(Index));
        }

        var q = quantity < 1 ? 1 : quantity > 999 ? 999 : quantity;
        bundle.BundleQuantity = q;
        foreach (var slot in bundle.Slots)
            slot.Selected = selected.Contains(slot.ScheduleId);

        SaveCartState(state);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RemoveBundle(string bundleId)
    {
        if (string.IsNullOrWhiteSpace(bundleId))
            return RedirectToAction(nameof(Index));

        var state = GetCartState();
        state.MenuBundles.RemoveAll(b => b.BundleId == bundleId);
        SaveCartState(state);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateLoose(int dishId, int? menuScheduleId, int quantity)
    {
        var state = GetCartState();
        var line = state.LooseLines.FirstOrDefault(c => c.DishId == dishId && c.MenuScheduleId == menuScheduleId);
        if (line == null)
            return RedirectToAction(nameof(Index));

        if (quantity <= 0)
            state.LooseLines.Remove(line);
        else
            line.Quantity = quantity;

        SaveCartState(state);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Clear()
    {
        HttpContext.Session.Remove(CartSessionKey);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Checkout()
    {
        var flat = CartIndexViewModel.FlattenForOrder(GetCartState());
        if (flat.Count == 0)
            return RedirectToAction(nameof(Index));

        ViewBag.SkipCheckoutSignature = PreferContractsAfterOrder(User);
        return View(CartIndexViewModel.Create(GetCartState()));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(DateTime scheduledDate, string? signatureDataUrl, CancellationToken ct)
    {
        if (!CanSubmitOnlineMealOrder(User))
        {
            TempData["CheckoutError"] =
                "Tài khoản của bạn chưa được cấp quyền đặt suất trực tuyến. Vui lòng đăng nhập bằng tài khoản khách (cá nhân hoặc đại diện đơn vị) hoặc liên hệ quản trị.";
            return RedirectToAction(nameof(Checkout));
        }

        var state = GetCartState();
        var cart = CartIndexViewModel.FlattenForOrder(state);
        if (cart.Count == 0)
            return RedirectToAction(nameof(Index));

        var token = HttpContext.Session.GetString("access_token");
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action(nameof(Checkout)) });

        var date = DateOnly.FromDateTime(scheduledDate.Date);
        var req = new CreateCustomerMealOrderApiRequest
        {
            ScheduledDate = date,
            Lines = cart
                .GroupBy(c => c.DishId)
                .Select(g => new CreateCustomerMealOrderLineApi { DishId = g.Key, Quantity = g.Sum(x => x.Quantity) })
                .ToList(),
        };

        try
        {
            var res = await _masterDataClient.CreateCustomerMealOrderAsync(req, token, ct);
            HttpContext.Session.Remove(CartSessionKey);
            var orderId = res.Order.Id;
            if (!string.IsNullOrEmpty(signatureDataUrl))
            {
                const int maxLen = 200_000;
                if (signatureDataUrl.Length > maxLen)
                    signatureDataUrl = signatureDataUrl.Substring(0, maxLen);
                HttpContext.Session.SetString($"order_sig_{orderId}", signatureDataUrl);
            }

            var code = res.Order.InvoiceCode ?? res.Order.Id.ToString();
            TempData["OrderSuccess"] = $"Đặt hàng thành công. Mã đơn: {code}";
            TempData["OrderPlacedId"] = orderId.ToString();

            if (PreferContractsAfterOrder(User))
                return RedirectToAction("Contracts", "Profile");

            return RedirectToAction("OrderDetail", "Profile", new { id = orderId });
        }
        catch (Exception ex)
        {
            TempData["CheckoutError"] = ex.Message;
            return RedirectToAction(nameof(Checkout));
        }
    }

    private IActionResult RedirectToReferer()
    {
        var referer = Request.Headers.Referer.ToString();
        if (!string.IsNullOrEmpty(referer) && Uri.TryCreate(referer, UriKind.Absolute, out var u) &&
            string.Equals(u.Host, Request.Host.Host, StringComparison.OrdinalIgnoreCase))
            return Redirect(referer);
        return RedirectToAction("Index", "Menu");
    }

    private CartState GetCartState()
    {
        var raw = HttpContext.Session.GetString(CartSessionKey);
        if (string.IsNullOrEmpty(raw))
            return new CartState();

        try
        {
            var st = JsonSerializer.Deserialize<CartState>(raw, JsonOpts);
            if (st != null)
                return st;
        }
        catch
        {
            // legacy JSON below
        }

        try
        {
            var legacy = JsonSerializer.Deserialize<List<CartLine>>(raw, JsonOpts);
            if (legacy is { Count: > 0 })
                return new CartState { LooseLines = legacy };
        }
        catch
        {
            // ignored
        }

        return new CartState();
    }

    private void SaveCartState(CartState state)
    {
        HttpContext.Session.SetString(CartSessionKey, JsonSerializer.Serialize(state));
    }
}
