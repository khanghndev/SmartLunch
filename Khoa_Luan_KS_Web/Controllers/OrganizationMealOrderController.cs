using System.Security.Claims;
using System.Text.Json;
using Khoa_Luan_KS_Web.Helpers;
using Khoa_Luan_KS_Web.Models;
using Khoa_Luan_KS_Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Khoa_Luan_KS_Web.Controllers;

[Authorize(Policy = "CustomerArea")]
public class OrganizationMealOrderController : Controller
{
    private const string DraftSessionKey = "org_meal_order_draft_v1";
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    private readonly BackendMasterDataClient _masterDataClient;
    private readonly BackendAuthClient _authClient;

    public OrganizationMealOrderController(BackendMasterDataClient masterDataClient, BackendAuthClient authClient)
    {
        _masterDataClient = masterDataClient;
        _authClient = authClient;
    }

    private static bool IsOrganizationMealOrderUser(ClaimsPrincipal user) =>
        user.IsInRole("Organization") ||
        user.IsInRole("Company") ||
        user.IsInRole("Khách hàng doanh nghiệp");

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        if (!IsOrganizationMealOrderUser(User))
        {
            TempData["CartError"] = "Đặt suất theo đơn vị chỉ dành cho tài khoản doanh nghiệp.";
            return RedirectToAction("Index", "Menu");
        }

        var accessToken = HttpContext.Session.GetString("access_token");
        if (string.IsNullOrEmpty(accessToken))
            return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action(nameof(Index)) });

        var vm = new OrganizationMealOrderIndexVm();
        try
        {
            var profile = await _authClient.GetProfileAsync(accessToken, ct);
            if (profile.Unit == null || profile.Unit.Id <= 0)
            {
                vm.ApiError = "Tài khoản chưa được gán đơn vị (Organization). Vui lòng liên hệ quản trị hệ thống.";
                return View(vm);
            }

            vm.OrganizationId = profile.Unit.Id;
            vm.OrganizationName = profile.Unit.Name;

            var categories = await _masterDataClient.GetOrganizationDishCategoriesAsync(accessToken, ct);
            vm.Categories = categories;

            var today = OrganizationMealOrderDateRules.TodayVietnam();
            vm.DateMin = categories.AllowedFirstServiceDate != default
                ? categories.AllowedFirstServiceDate
                : OrganizationMealOrderDateRules.GetMinimumServiceDate(today);
            vm.DateMax = categories.AllowedLastServiceDate != default
                ? categories.AllowedLastServiceDate
                : OrganizationMealOrderDateRules.GetMaximumServiceDate(today);
            vm.DateDefault = OrganizationMealOrderDateRules.GetDefaultServiceDate(today);
            vm.DateRuleHint = OrganizationMealOrderDateRules.GetRuleHint(today);
        }
        catch (Exception ex)
        {
            vm.ApiError = ex.Message;
        }

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Dishes(int categoryId, int page = 1, int pageSize = 24, CancellationToken ct = default)
    {
        if (!IsOrganizationMealOrderUser(User))
            return Forbid();

        var accessToken = HttpContext.Session.GetString("access_token");
        if (string.IsNullOrEmpty(accessToken))
            return Unauthorized();

        try
        {
            var res = await _masterDataClient.GetOrganizationDishesByCategoryAsync(categoryId, accessToken, page, pageSize, ct);
            return Json(res);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PrepareContract([FromBody] PrepareOrganizationMealContractClientRequest request, CancellationToken ct)
    {
        if (!IsOrganizationMealOrderUser(User))
            return Forbid();

        var accessToken = HttpContext.Session.GetString("access_token");
        if (string.IsNullOrEmpty(accessToken))
            return Unauthorized();

        try
        {
            var profile = await _authClient.GetProfileAsync(accessToken, ct);
            if (profile.Unit == null || profile.Unit.Id <= 0)
                return BadRequest(new { message = "Tài khoản chưa được gán đơn vị." });

            request.OrganizationId = profile.Unit.Id;

            var draft = await _masterDataClient.PrepareOrganizationMealContractAsync(request, accessToken, ct);
            HttpContext.Session.SetString(DraftSessionKey, JsonSerializer.Serialize(draft, JsonOpts));
            HttpContext.Session.SetString(DraftSessionKey + "_org_name", profile.Unit.Name);

            return Json(new { success = true, redirectUrl = Url.Action(nameof(Review)) });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public IActionResult Review()
    {
        if (!IsOrganizationMealOrderUser(User))
            return RedirectToAction(nameof(Index));

        var raw = HttpContext.Session.GetString(DraftSessionKey);
        if (string.IsNullOrEmpty(raw))
        {
            TempData["OrgMealError"] = "Không tìm thấy bản nháp. Vui lòng lập thực đơn lại.";
            return RedirectToAction(nameof(Index));
        }

        var draft = JsonSerializer.Deserialize<PrepareOrganizationMealContractClientResponse>(raw, JsonOpts);
        if (draft == null || string.IsNullOrEmpty(draft.DraftId))
        {
            TempData["OrgMealError"] = "Bản nháp không hợp lệ. Vui lòng lập thực đơn lại.";
            return RedirectToAction(nameof(Index));
        }

        var vm = new OrganizationMealOrderReviewVm
        {
            Draft = draft,
            OrganizationName = HttpContext.Session.GetString(DraftSessionKey + "_org_name") ?? "Đơn vị khách hàng",
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(int depositPercent, CancellationToken ct)
    {
        if (!IsOrganizationMealOrderUser(User))
            return Forbid();

        var accessToken = HttpContext.Session.GetString("access_token");
        if (string.IsNullOrEmpty(accessToken))
            return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action(nameof(Review)) });

        var raw = HttpContext.Session.GetString(DraftSessionKey);
        if (string.IsNullOrEmpty(raw))
        {
            TempData["OrgMealError"] = "Bản nháp đã hết hạn. Vui lòng lập thực đơn lại.";
            return RedirectToAction(nameof(Index));
        }

        var draft = JsonSerializer.Deserialize<PrepareOrganizationMealContractClientResponse>(raw, JsonOpts);
        if (draft == null || string.IsNullOrEmpty(draft.DraftId))
        {
            TempData["OrgMealError"] = "Bản nháp không hợp lệ.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var result = await _masterDataClient.CheckoutOrganizationMealAsync(
                new CheckoutOrganizationMealClientRequest
                {
                    DraftId = draft.DraftId,
                    DepositPercent = depositPercent,
                },
                accessToken,
                ct);

            HttpContext.Session.Remove(DraftSessionKey);
            HttpContext.Session.Remove(DraftSessionKey + "_org_name");

            var order = result.Order?.Order;
            if (order == null || order.Id <= 0)
            {
                TempData["OrgMealError"] = "Không tạo được đơn hàng.";
                return RedirectToAction(nameof(Review));
            }

            TempData["OrderSuccess"] =
                $"Đã tạo đơn hàng #{order.Id}. Vui lòng ký phụ lục đặt hàng, sau đó thanh toán đặt cọc từ lịch sử đơn.";
            TempData["OrderPlacedId"] = order.Id.ToString();

            return RedirectToAction("Contracts", "Profile", new { orderId = order.Id });
        }
        catch (Exception ex)
        {
            TempData["OrgMealError"] = ex.Message;
            return RedirectToAction(nameof(Review));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PayDeposit(int orderId, CancellationToken ct)
    {
        if (!IsOrganizationMealOrderUser(User))
            return Forbid();

        var accessToken = HttpContext.Session.GetString("access_token");
        if (string.IsNullOrEmpty(accessToken))
            return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action("Orders", "Profile") });

        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var returnUrl = $"{baseUrl}{Url.Action(nameof(PaymentResult), new { orderId })}";
        var cancelUrl = $"{baseUrl}{Url.Action("Orders", "Profile")}";

        try
        {
            var result = await _masterDataClient.InitiateOrganizationMealPaymentAsync(
                new InitiateOrganizationMealPaymentClientRequest
                {
                    OrderId = orderId,
                    ReturnUrl = returnUrl,
                    CancelUrl = cancelUrl,
                },
                accessToken,
                ct);

            if (!string.IsNullOrEmpty(result.CheckoutUrl))
            {
                TempData["OrderPlacedId"] = orderId.ToString();
                return Redirect(result.CheckoutUrl);
            }

            TempData["OrgMealError"] = result.PayOsMessage ?? "Không tạo được liên kết thanh toán PayOS.";
            return RedirectToAction("OrderDetail", "Profile", new { id = orderId });
        }
        catch (Exception ex)
        {
            TempData["OrgMealError"] = ex.Message;
            return RedirectToAction("Orders", "Profile");
        }
    }

    [HttpGet]
    public IActionResult PaymentResult(int? orderId, int? depositPercent, int? depositAmount, string? message)
    {
        var payUrl = TempData["OrgMealPayOsUrl"] as string;
        var vm = new OrganizationMealOrderPaymentResultVm
        {
            OrderId = orderId ?? 0,
            DepositPercent = depositPercent ?? 0,
            DepositAmountVnd = depositAmount ?? 0,
            CheckoutUrl = payUrl,
            PayOsMessage = message,
            PayOsReady = !string.IsNullOrEmpty(payUrl),
        };

        if (orderId is > 0)
        {
            vm.OrderId = orderId.Value;
            if (TempData["OrderSuccess"] is string ok)
                ViewBag.SuccessMessage = ok;
            else
                ViewBag.SuccessMessage = "Thanh toán đặt cọc thành công. Hợp đồng đã chuyển sang trạng thái đã đặt cọc.";
        }

        return View(vm);
    }
}
