using System.Globalization;
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
    private const string DeliverySessionKey = "huit_cart_checkout_delivery_v1";
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

    /// <summary>Tài khoản đơn vị: sau tạo đơn có thể gọi API ký phụ lục/PDF (không còn ép redirect sang trang Contracts).</summary>
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
            if (IsAjaxRequest(Request))
                return Json(new CartBundleSyncResponse { Ok = false, Error = CartViewText.ErrorNoSlotSelected });

            TempData["CartError"] = CartViewText.ErrorNoSlotSelected;
            return RedirectToAction(nameof(Index));
        }

        var q = quantity < 1 ? 1 : quantity > 999 ? 999 : quantity;
        bundle.BundleQuantity = q;
        foreach (var slot in bundle.Slots)
            slot.Selected = selected.Contains(slot.ScheduleId);

        SaveCartState(state);

        if (IsAjaxRequest(Request))
            return Json(BuildBundleSyncResponse(state, bundleId));

        return RedirectToAction(nameof(Index));
    }

    private static bool IsAjaxRequest(HttpRequest request) =>
        string.Equals(request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);

    private static CartBundleSyncResponse BuildBundleSyncResponse(CartState state, string bundleId)
    {
        var vm = CartIndexViewModel.Create(state);
        var bundle = state.MenuBundles.FirstOrDefault(b => b.BundleId == bundleId);
        decimal bundleSub = 0;
        if (bundle != null)
        {
            var unit = bundle.Slots.Where(s => s.Selected).Sum(s => s.Price);
            bundleSub = unit * bundle.BundleQuantity;
        }

        return new CartBundleSyncResponse
        {
            Ok = true,
            CartTotal = vm.TotalAmount,
            BundleSubtotal = bundleSub,
            TotalPortions = vm.TotalPortions,
            MeetsMinimum = vm.MeetsMinimumPortions,
        };
    }

    [HttpGet]
    public IActionResult Delivery()
    {
        var state = GetCartState();
        var flat = CartIndexViewModel.FlattenForOrder(state);
        if (flat.Count == 0)
            return RedirectToAction(nameof(Index));

        var vmCart = CartIndexViewModel.Create(state);
        if (!vmCart.MeetsMinimumPortions)
        {
            TempData["CartError"] = CartViewText.ErrorMinimumPortions;
            return RedirectToAction(nameof(Index));
        }

        var vm = new CartDeliveryPageVm
        {
            Cart = vmCart,
            Days = BuildDeliveryDayRows(state),
            PrefillCommonAddress = TempData["DeliveryCommonAddress"] as string,
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delivery(string? commonAddress, [FromForm(Name = "rows")] List<CartDeliveryTimeRowVm>? times)
    {
        var state = GetCartState();
        var flat = CartIndexViewModel.FlattenForOrder(state);
        if (flat.Count == 0)
            return RedirectToAction(nameof(Index));

        var vmCart = CartIndexViewModel.Create(state);
        if (!vmCart.MeetsMinimumPortions)
        {
            TempData["CartError"] = CartViewText.ErrorMinimumPortions;
            return RedirectToAction(nameof(Index));
        }

        var addr = (commonAddress ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(addr))
        {
            TempData["CheckoutError"] = "Vui lòng nhập địa chỉ giao hàng.";
            TempData["DeliveryCommonAddress"] = commonAddress ?? string.Empty;
            return RedirectToAction(nameof(Delivery));
        }

        var expected = BuildDeliveryDayRows(state);
        if (times == null || times.Count == 0)
        {
            TempData["CheckoutError"] = "Thiếu dữ liệu giờ giao. Vui lòng thử lại.";
            TempData["DeliveryCommonAddress"] = addr;
            return RedirectToAction(nameof(Delivery));
        }

        var merged = new List<CartDeliveryDayRowVm>(expected.Count);
        foreach (var exp in expected)
        {
            var row = times!.FirstOrDefault(t => string.Equals(t.DateKey, exp.DateKey, StringComparison.Ordinal));
            if (row == null)
            {
                TempData["CheckoutError"] = "Thiếu dữ liệu giờ giao cho một hoặc nhiều ngày.";
                TempData["DeliveryCommonAddress"] = addr;
                return RedirectToAction(nameof(Delivery));
            }

            var time = string.IsNullOrWhiteSpace(row.DeliveryTime) ? exp.DeliveryTime : row.DeliveryTime.Trim();
            merged.Add(new CartDeliveryDayRowVm
            {
                DateKey = exp.DateKey,
                DisplayLabel = exp.DisplayLabel,
                Address = addr,
                DeliveryTime = time,
            });
        }

        HttpContext.Session.SetString(DeliverySessionKey, JsonSerializer.Serialize(merged, JsonOpts));
        return RedirectToAction(nameof(ContractCheckout));
    }

    [HttpGet]
    public IActionResult ContractCheckout()
    {
        var state = GetCartState();
        var flat = CartIndexViewModel.FlattenForOrder(state);
        if (flat.Count == 0)
            return RedirectToAction(nameof(Index));

        var vmCart = CartIndexViewModel.Create(state);
        if (!vmCart.MeetsMinimumPortions)
        {
            TempData["CartError"] = CartViewText.ErrorMinimumPortions;
            return RedirectToAction(nameof(Index));
        }

        var raw = HttpContext.Session.GetString(DeliverySessionKey);
        if (string.IsNullOrEmpty(raw))
            return RedirectToAction(nameof(Delivery));

        List<CartDeliveryDayRowVm>? delivery;
        try
        {
            delivery = JsonSerializer.Deserialize<List<CartDeliveryDayRowVm>>(raw, JsonOpts);
        }
        catch
        {
            return RedirectToAction(nameof(Delivery));
        }

        if (delivery == null || delivery.Count == 0)
            return RedirectToAction(nameof(Delivery));

        var vm = new CartContractPageVm
        {
            Cart = vmCart,
            Delivery = delivery,
            ScheduledDateForApi = BuildScheduledDateTimeFromDelivery(delivery),
        };
        return View(vm);
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

        if (IsAjaxRequest(Request))
        {
            var vm = CartIndexViewModel.Create(state);
            return Json(new CartBundleSyncResponse
            {
                Ok = true,
                CartTotal = vm.TotalAmount,
                BundleSubtotal = 0,
                TotalPortions = vm.TotalPortions,
                MeetsMinimum = vm.MeetsMinimumPortions,
            });
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Clear()
    {
        HttpContext.Session.Remove(CartSessionKey);
        HttpContext.Session.Remove(DeliverySessionKey);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Checkout() => RedirectToAction(nameof(Delivery));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(DateTime scheduledDate, string? signatureDataUrl, CancellationToken ct)
    {
        if (!CanSubmitOnlineMealOrder(User))
        {
            TempData["CheckoutError"] =
                "Tài khoản của bạn chưa được cấp quyền đặt suất trực tuyến. Vui lòng đăng nhập bằng tài khoản khách (cá nhân hoặc đại diện đơn vị) hoặc liên hệ quản trị.";
            return RedirectToAction(nameof(ContractCheckout));
        }

        var state = GetCartState();
        var vmPre = CartIndexViewModel.Create(state);
        if (!vmPre.MeetsMinimumPortions)
        {
            TempData["CartError"] = CartViewText.ErrorMinimumPortions;
            return RedirectToAction(nameof(Index));
        }

        var cart = CartIndexViewModel.FlattenForOrder(state);
        if (cart.Count == 0)
            return RedirectToAction(nameof(Index));

        var deliveryRaw = HttpContext.Session.GetString(DeliverySessionKey);
        if (string.IsNullOrEmpty(deliveryRaw))
        {
            TempData["CheckoutError"] = "Thiếu thông tin giao hàng. Vui lòng nhập địa chỉ theo từng ngày.";
            return RedirectToAction(nameof(Delivery));
        }

        List<CartDeliveryDayRowVm>? deliveryRows;
        try
        {
            deliveryRows = JsonSerializer.Deserialize<List<CartDeliveryDayRowVm>>(deliveryRaw, JsonOpts);
        }
        catch
        {
            return RedirectToAction(nameof(Delivery));
        }

        if (deliveryRows == null || deliveryRows.Count == 0)
            return RedirectToAction(nameof(Delivery));

        var token = HttpContext.Session.GetString("access_token");
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action(nameof(ContractCheckout)) });

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
            HttpContext.Session.Remove(DeliverySessionKey);
            var orderId = res.Order.Id;

            var notes = deliveryRows
                .Select(d => new OrderDeliveryNoteLine
                {
                    DateKey = d.DateKey,
                    DisplayLabel = d.DisplayLabel,
                    Address = d.Address?.Trim() ?? string.Empty,
                    DeliveryTime = string.IsNullOrWhiteSpace(d.DeliveryTime) ? null : d.DeliveryTime.Trim(),
                })
                .ToList();
            HttpContext.Session.SetString($"order_delivery_{orderId}", JsonSerializer.Serialize(notes, JsonOpts));

            var sigTrim = (signatureDataUrl ?? string.Empty).Trim();
            const int maxSig = 400_000;
            if (sigTrim.Length > maxSig)
                sigTrim = sigTrim.Substring(0, maxSig);

            if (!string.IsNullOrEmpty(sigTrim))
                HttpContext.Session.SetString($"order_sig_{orderId}", sigTrim);

            var code = res.Order.InvoiceCode ?? res.Order.Id.ToString();
            TempData["OrderSuccess"] = $"Đặt hàng thành công. Mã đơn: {code}";
            TempData["OrderPlacedId"] = orderId.ToString();

            // Đơn vị: chữ ký trên ContractCheckout → lưu PDF phụ lục qua API (giống SignOrderAnnex), không ép sang cổng Contracts.
            if (PreferContractsAfterOrder(User) && !string.IsNullOrWhiteSpace(sigTrim))
            {
                try
                {
                    await _masterDataClient.SignOrderAnnexAsync(
                        orderId,
                        new SignOrderAnnexApiRequest { DigitalSignature = sigTrim },
                        token,
                        ct);
                    TempData["OrderSuccess"] =
                        $"Đặt hàng thành công. Mã đơn: {code} — Phụ lục đã ký và PDF đã lưu trên hệ thống.";
                }
                catch (Exception annexEx)
                {
                    TempData["AnnexSignWarning"] =
                        "Đơn đã được tạo nhưng chưa lưu được PDF phụ lục lên cloud: " + annexEx.Message +
                        " Bạn có thể thử lại bằng nút \"Ký phụ lục đặt hàng (PDF)\" trên trang chi tiết đơn.";
                }
            }

            return RedirectToAction("OrderDetail", "Profile", new { id = orderId });
        }
        catch (Exception ex)
        {
            TempData["CheckoutError"] = ex.Message;
            return RedirectToAction(nameof(ContractCheckout));
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

    private static List<CartDeliveryDayRowVm> BuildDeliveryDayRows(CartState state)
    {
        var dates = CartOrderRules.DistinctSelectedDates(state);
        if (dates.Count == 0)
        {
            var fb = DateTime.Today.AddDays(1).Date;
            return new List<CartDeliveryDayRowVm>
            {
                new()
                {
                    DateKey = fb.ToString("yyyy-MM-dd"),
                    DisplayLabel = "Ngày giao dự kiến",
                    Address = string.Empty,
                    DeliveryTime = "11:30",
                },
            };
        }

        return dates.Select(d => new CartDeliveryDayRowVm
        {
            DateKey = d.ToString("yyyy-MM-dd"),
            DisplayLabel = $"{CartViewText.DayShortVi(d)} {d:dd/MM/yyyy}",
            Address = string.Empty,
            DeliveryTime = "11:30",
        }).ToList();
    }

    private static DateTime BuildScheduledDateTimeFromDelivery(List<CartDeliveryDayRowVm> delivery)
    {
        var row = delivery.OrderBy(x => x.DateKey, StringComparer.Ordinal).First();
        if (!DateTime.TryParse(row.DateKey, CultureInfo.InvariantCulture, DateTimeStyles.None, out var day))
            day = DateTime.Today.AddDays(1);

        var tod = ParseDeliveryTimeSpan(row.DeliveryTime);
        return day.Date + tod;
    }

    private static TimeSpan ParseDeliveryTimeSpan(string? s)
    {
        if (string.IsNullOrWhiteSpace(s))
            return TimeSpan.FromHours(11);

        var t = s.Trim();
        var p = t.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (p.Length >= 2 && int.TryParse(p[0], NumberStyles.None, CultureInfo.InvariantCulture, out var h) &&
            int.TryParse(p[1], NumberStyles.None, CultureInfo.InvariantCulture, out var m))
            return new TimeSpan(h, m, 0);

        return TimeSpan.FromHours(11);
    }
}
