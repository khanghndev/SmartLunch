using System.Globalization;
using System.Security.Claims;
using System.Text.Json;
using Khoa_Luan_KS_Web.Helpers;
using Khoa_Luan_KS_Web.Models;
using Khoa_Luan_KS_Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Khoa_Luan_KS_Web.Controllers;

[Authorize(Policy = "CustomerArea")]
public class OrganizationMealContractOrderController : Controller
{
    private const string DraftSessionKey = "org_meal_period_draft_v1";
    private const string ExcludedSessionPrefix = "org_meal_period_excluded_";
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    private readonly BackendMasterDataClient _masterDataClient;
    private readonly BackendAuthClient _authClient;
    private readonly BackendCompanyProfileClient _companyProfileClient;

    public OrganizationMealContractOrderController(
        BackendMasterDataClient masterDataClient,
        BackendAuthClient authClient,
        BackendCompanyProfileClient companyProfileClient)
    {
        _masterDataClient = masterDataClient;
        _authClient = authClient;
        _companyProfileClient = companyProfileClient;
    }

    private static bool IsOrgUser(ClaimsPrincipal user) =>
        user.IsInRole("Organization") ||
        user.IsInRole("Company") ||
        user.IsInRole("Khách hàng doanh nghiệp");

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct = default)
    {
        if (!IsOrgUser(User))
        {
            TempData["CartError"] = "Đặt suất theo hợp đồng chỉ dành cho tài khoản doanh nghiệp.";
            return RedirectToAction("Index", "Menu");
        }

        var accessToken = HttpContext.Session.GetString("access_token");
        if (string.IsNullOrEmpty(accessToken))
            return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action(nameof(Index)) });

        var vm = new OrganizationMealContractOrderIndexVm();
        try
        {
            var profile = await _authClient.GetProfileAsync(accessToken, ct);
            if (profile.Unit == null || profile.Unit.Id <= 0)
            {
                vm.ApiError = "Tài khoản chưa được gán đơn vị. Vui lòng liên hệ quản trị.";
                return View(vm);
            }

            vm.OrganizationId = profile.Unit.Id;
            vm.OrganizationName = profile.Unit.Name;
            var today = OrganizationMealContractDateRules.TodayVietnam();
            vm.PeriodStartDefault = OrganizationMealContractDateRules.GetDefaultPeriodStart(today);
            vm.PeriodEndDefault = OrganizationMealContractDateRules.GetDefaultPeriodEnd(vm.PeriodStartDefault);
            vm.PeriodStartMin = OrganizationMealContractDateRules.GetMinPeriodStart(today);
            vm.PeriodEndMax = OrganizationMealContractDateRules.GetMaxPeriodEnd(vm.PeriodStartDefault);
            vm.DateRuleHint = OrganizationMealContractDateRules.GetRuleHint(today);

            try
            {
                var orgProfile = await _companyProfileClient.GetProfileAsync(accessToken, ct);
                vm.DeliveryDefaults = OrganizationMealDeliveryDefaultsBuilder.Build(profile, orgProfile);
            }
            catch
            {
                vm.DeliveryDefaults = OrganizationMealDeliveryDefaultsBuilder.Build(profile, null);
            }
        }
        catch (Exception ex)
        {
            vm.ApiError = ex.Message;
        }

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> MainDishes(int page = 1, int pageSize = 50, string? search = null, CancellationToken ct = default)
    {
        if (!IsOrgUser(User)) return Forbid();
        var accessToken = HttpContext.Session.GetString("access_token");
        if (string.IsNullOrEmpty(accessToken)) return Unauthorized();
        try
        {
            var res = await _masterDataClient.GetOrganizationMealContractMainDishesAsync(accessToken, page, pageSize, search, ct);
            return Json(res);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EligiblePromotions(
        [FromBody] PrepareOrganizationMealPeriodContractClientRequest request,
        CancellationToken ct)
    {
        if (!IsOrgUser(User)) return Forbid();
        var accessToken = HttpContext.Session.GetString("access_token");
        if (string.IsNullOrEmpty(accessToken)) return Unauthorized();

        try
        {
            var profile = await _authClient.GetProfileAsync(accessToken, ct);
            if (profile.Unit == null || profile.Unit.Id <= 0)
                return BadRequest(new { message = "Tài khoản chưa được gán đơn vị." });

            request.OrganizationId = profile.Unit.Id;
            var previewReq = OrganizationMealPeriodPromotionPreviewBuilder.ToPreviewRequest(request, profile.Unit.Id);
            if (previewReq.Subtotal <= 0)
                return BadRequest(new { message = "Hợp đồng chưa có giá trị để áp dụng khuyến mãi." });

            var result = await _masterDataClient.ListEligiblePromotionsAsync(previewReq, accessToken, ct);
            return Json(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PreviewPromotion(
        [FromBody] OrganizationMealPeriodPreviewPromotionRequest request,
        CancellationToken ct)
    {
        if (!IsOrgUser(User)) return Forbid();
        var accessToken = HttpContext.Session.GetString("access_token");
        if (string.IsNullOrEmpty(accessToken)) return Unauthorized();

        try
        {
            var profile = await _authClient.GetProfileAsync(accessToken, ct);
            if (profile.Unit == null || profile.Unit.Id <= 0)
                return BadRequest(new { message = "Tài khoản chưa được gán đơn vị." });

            request.Order.OrganizationId = profile.Unit.Id;
            var previewReq = OrganizationMealPeriodPromotionPreviewBuilder.ToPreviewRequest(
                request.Order,
                profile.Unit.Id,
                request.PromotionCode);
            previewReq.PromotionId = request.PromotionId;

            if (previewReq.Subtotal <= 0)
                return BadRequest(new { message = "Hợp đồng chưa có giá trị để áp dụng khuyến mãi." });

            var result = await _masterDataClient.PreviewPromotionAsync(previewReq, accessToken, ct);
            return Json(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PrepareContract([FromBody] PrepareOrganizationMealPeriodContractClientRequest request, CancellationToken ct)
    {
        if (!IsOrgUser(User)) return Forbid();
        var accessToken = HttpContext.Session.GetString("access_token");
        if (string.IsNullOrEmpty(accessToken)) return Unauthorized();

        try
        {
            var profile = await _authClient.GetProfileAsync(accessToken, ct);
            if (profile.Unit == null || profile.Unit.Id <= 0)
                return BadRequest(new { message = "Tài khoản chưa được gán đơn vị." });

            request.OrganizationId = profile.Unit.Id;
            var draft = await _masterDataClient.PrepareOrganizationMealPeriodContractAsync(request, accessToken, ct);
            HttpContext.Session.SetString(DraftSessionKey, JsonSerializer.Serialize(draft, JsonOpts));
            HttpContext.Session.SetString(DraftSessionKey + "_org", profile.Unit.Name);

            if (draft.ContractId > 0 && draft.ExcludedDates?.Count > 0)
            {
                HttpContext.Session.SetString(
                    ExcludedSessionPrefix + draft.ContractId,
                    JsonSerializer.Serialize(draft.ExcludedDates, JsonOpts));
            }

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
        if (!IsOrgUser(User)) return RedirectToAction(nameof(Index));
        var raw = HttpContext.Session.GetString(DraftSessionKey);
        if (string.IsNullOrEmpty(raw))
        {
            TempData["OrgPeriodError"] = "Không tìm thấy bản nháp. Vui lòng thiết lập lại.";
            return RedirectToAction(nameof(Index));
        }

        var draft = JsonSerializer.Deserialize<PrepareOrganizationMealPeriodContractClientResponse>(raw, JsonOpts);
        if (draft == null || string.IsNullOrEmpty(draft.DraftId))
        {
            TempData["OrgPeriodError"] = "Bản nháp không hợp lệ.";
            return RedirectToAction(nameof(Index));
        }

        return View(new OrganizationMealContractOrderReviewVm
        {
            Draft = draft,
            OrganizationName = HttpContext.Session.GetString(DraftSessionKey + "_org") ?? "Đơn vị",
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(int depositPercent, CancellationToken ct)
    {
        if (!IsOrgUser(User)) return Forbid();
        var accessToken = HttpContext.Session.GetString("access_token");
        if (string.IsNullOrEmpty(accessToken))
            return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action(nameof(Review)) });

        var raw = HttpContext.Session.GetString(DraftSessionKey);
        if (string.IsNullOrEmpty(raw))
        {
            TempData["OrgPeriodError"] = "Bản nháp đã hết hạn.";
            return RedirectToAction(nameof(Index));
        }

        var draft = JsonSerializer.Deserialize<PrepareOrganizationMealPeriodContractClientResponse>(raw, JsonOpts);
        if (draft == null || string.IsNullOrEmpty(draft.DraftId))
        {
            TempData["OrgPeriodError"] = "Bản nháp không hợp lệ.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var result = await _masterDataClient.CheckoutOrganizationMealPeriodContractAsync(
                new CheckoutOrganizationMealPeriodContractClientRequest
                {
                    DraftId = draft.DraftId,
                    DepositPercent = depositPercent,
                },
                accessToken,
                ct);

            HttpContext.Session.Remove(DraftSessionKey);
            HttpContext.Session.Remove(DraftSessionKey + "_org");

            var order = result.Order?.Order;
            if (order == null || order.Id <= 0)
            {
                TempData["OrgPeriodError"] = "Không tạo được đơn hàng đặt cọc.";
                return RedirectToAction(nameof(Review));
            }

            TempData["OrderSuccess"] =
                $"Đã tạo hợp đồng #{result.ContractId} và đơn đặt cọc #{order.Id}. Ký hợp đồng, sau đó thanh toán cọc.";
            TempData["OrderPlacedId"] = order.Id.ToString();
            TempData["PeriodContractId"] = result.ContractId.ToString();

            return RedirectToAction("Contracts", "Profile", new { orderId = order.Id, contractId = result.ContractId });
        }
        catch (Exception ex)
        {
            TempData["OrgPeriodError"] = ex.Message;
            return RedirectToAction(nameof(Review));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PayDeposit(int orderId, CancellationToken ct)
    {
        if (!IsOrgUser(User)) return Forbid();
        var accessToken = HttpContext.Session.GetString("access_token");
        if (string.IsNullOrEmpty(accessToken))
            return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action(nameof(Contracts)) });

        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var returnUrl = $"{baseUrl}{Url.Action(nameof(PaymentResult), new { orderId })}";
        var cancelUrl = $"{baseUrl}{Url.Action(nameof(Contracts))}";

        try
        {
            var result = await _masterDataClient.InitiateOrganizationMealPeriodPaymentAsync(
                new InitiateOrganizationMealPeriodPaymentClientRequest
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

            TempData["OrgPeriodError"] = result.PayOsMessage ?? "Không tạo được liên kết thanh toán.";
            return RedirectToAction("OrderDetail", "Profile", new { id = orderId });
        }
        catch (Exception ex)
        {
            TempData["OrgPeriodError"] = ex.Message;
            return RedirectToAction("OrderDetail", "Profile", new { id = orderId });
        }
    }

    [HttpGet]
    public IActionResult PaymentResult(int? orderId, int? contractId, int? depositPercent, int? depositAmount, string? message)
    {
        var vm = new OrganizationMealContractOrderPaymentResultVm
        {
            OrderId = orderId ?? 0,
            ContractId = contractId ?? 0,
            DepositPercent = depositPercent ?? 0,
            DepositAmountVnd = depositAmount ?? 0,
            PayOsMessage = message,
        };
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Contracts(CancellationToken ct = default)
    {
        if (!IsOrgUser(User)) return RedirectToAction(nameof(Index));
        var accessToken = HttpContext.Session.GetString("access_token");
        if (string.IsNullOrEmpty(accessToken))
            return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action(nameof(Contracts)) });

        var vm = new OrganizationMealContractListVm();
        try
        {
            var profile = await _authClient.GetProfileAsync(accessToken, ct);
            vm.OrganizationName = profile.Unit?.Name ?? "Đơn vị";
            var res = await _masterDataClient.GetMyOrganizationContractsAsync(accessToken, ct);
            var today = OrganizationMealContractDateRules.TodayVietnam();

            vm.Contracts = res.Contracts
                .Where(c => string.Equals(c.ContractType, "Period-Based", StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(c => c.StartDate)
                .Select(c =>
                {
                    var start = DateOnly.FromDateTime(c.StartDate);
                    var end = c.EndDate.HasValue ? DateOnly.FromDateTime(c.EndDate.Value) : start;
                    var signed = c.IsDigitallySigned;
                    var hasOrder = c.SourceOrderId is > 0;
                    return new PeriodContractListItemVm
                    {
                        Id = c.Id,
                        ContractNumber = c.ContractNumber,
                        Description = c.Description,
                        StartDate = start,
                        EndDate = end,
                        TotalValue = c.TotalValue,
                        DepositAmount = c.DepositAmount,
                        Status = c.Status,
                        IsDigitallySigned = signed,
                        SourceOrderId = c.SourceOrderId,
                        MealsPerDay = c.MealsPerDay,
                        ContractFileUrl = c.ContractFileUrl,
                        CanPayDeposit = signed && hasOrder && IsAwaitingDeposit(c.Status),
                        CanSelectWeeklyMeals = signed && hasOrder && !IsFullyCompleted(end, today),
                    };
                })
                .ToList();
        }
        catch (Exception ex)
        {
            vm.ApiError = ex.Message;
        }

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Weekly(int contractId, CancellationToken ct = default)
    {
        if (!IsOrgUser(User)) return RedirectToAction(nameof(Index));
        var accessToken = HttpContext.Session.GetString("access_token");
        if (string.IsNullOrEmpty(accessToken))
            return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action(nameof(Weekly), new { contractId }) });

        var vm = new OrganizationMealContractWeeklyVm { ContractId = contractId };
        try
        {
            var profile = await _authClient.GetProfileAsync(accessToken, ct);
            vm.OrganizationName = profile.Unit?.Name ?? "Đơn vị";

            var detail = await _masterDataClient.GetCompanyContractAsync(contractId, accessToken, ct);
            var c = detail.Contract;
            if (!string.Equals(c.ContractType, "Period-Based", StringComparison.OrdinalIgnoreCase))
            {
                vm.ApiError = "Hợp đồng này không phải loại đặt suất theo kỳ.";
                return View(vm);
            }

            vm.ContractNumber = c.ContractNumber;
            vm.ContractStart = DateOnly.FromDateTime(c.StartDate);
            vm.ContractEnd = c.EndDate.HasValue
                ? DateOnly.FromDateTime(c.EndDate.Value)
                : vm.ContractStart;
            vm.MealsPerDay = c.MealsPerDay is > 0 ? c.MealsPerDay.Value : 1;

            var excluded = LoadExcludedDates(contractId);
            var today = OrganizationMealContractDateRules.TodayVietnam();
            var currentMonday = OrganizationMealContractDateRules.GetWeekMonday(today);

            foreach (var (monday, weekEnd) in OrganizationMealContractDateRules.EnumerateWeeks(vm.ContractStart, vm.ContractEnd))
            {
                var serviceDays = OrganizationMealContractDateRules
                    .GetWeekServiceDates(monday, vm.ContractStart, vm.ContractEnd, excluded)
                    .ToList();
                if (serviceDays.Count == 0 && monday.AddDays(6) < vm.ContractStart)
                    continue;

                var isPast = weekEnd < today;
                var isCurrent = monday <= currentMonday && weekEnd >= currentMonday;
                vm.Weeks.Add(new ContractWeekVm
                {
                    WeekMonday = monday,
                    WeekEnd = weekEnd,
                    Label = $"Tuần {monday:dd/MM} – {weekEnd:dd/MM/yyyy}",
                    IsPast = isPast,
                    IsCurrent = isCurrent,
                    IsFuture = monday > currentMonday,
                    HasServiceDays = serviceDays.Count > 0,
                });
            }

            ViewBag.ExcludedDatesJson = JsonSerializer.Serialize(
                excluded.Select(d => d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)).ToList(),
                JsonOpts);
        }
        catch (Exception ex)
        {
            vm.ApiError = ex.Message;
            ViewBag.ExcludedDatesJson = "[]";
        }

        return View(vm);
    }

    private List<DateOnly> LoadExcludedDates(int contractId)
    {
        var raw = HttpContext.Session.GetString(ExcludedSessionPrefix + contractId);
        if (string.IsNullOrEmpty(raw)) return new List<DateOnly>();
        try
        {
            var strings = JsonSerializer.Deserialize<List<string>>(raw, JsonOpts) ?? new List<string>();
            return strings
                .Select(s => DateOnly.TryParse(s, CultureInfo.InvariantCulture, out var d) ? d : (DateOnly?)null)
                .Where(d => d.HasValue)
                .Select(d => d!.Value)
                .ToList();
        }
        catch
        {
            return new List<DateOnly>();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitWeekly(int contractId, [FromBody] SubmitOrganizationMealWeeklySelectionClientRequest request, CancellationToken ct)
    {
        if (!IsOrgUser(User)) return Forbid();
        var accessToken = HttpContext.Session.GetString("access_token");
        if (string.IsNullOrEmpty(accessToken)) return Unauthorized();

        try
        {
            var res = await _masterDataClient.SubmitOrganizationMealWeeklySelectionAsync(contractId, request, accessToken, ct);
            return Json(new { success = true, itemCount = res.ItemCount, redirectUrl = Url.Action(nameof(Contracts)) });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private static bool IsAwaitingDeposit(string status) =>
        status.Contains("deposit", StringComparison.OrdinalIgnoreCase) ||
        status.Contains("cọc", StringComparison.OrdinalIgnoreCase) ||
        status.Contains("pending", StringComparison.OrdinalIgnoreCase) ||
        status.Contains("chờ", StringComparison.OrdinalIgnoreCase);

    private static bool IsFullyCompleted(DateOnly end, DateOnly today) => end < today;
}
