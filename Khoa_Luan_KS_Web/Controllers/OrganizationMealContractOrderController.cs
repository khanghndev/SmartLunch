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
    private const string DailyMealsSessionPrefix = "org_meal_period_daily_";
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
    public IActionResult Holidays(string? from, string? to)
    {
        if (!DateOnly.TryParse(from, out var start) || !DateOnly.TryParse(to, out var end))
            return BadRequest(new { message = "Tham số from và to phải là ngày yyyy-MM-dd." });

        var items = VietnamesePublicHolidayCalendar.GetHolidays(start, end)
            .Select(h => new { date = h.Date, name = h.Name, kind = h.Kind, isWeekend = h.IsWeekend })
            .ToList();
        var map = VietnamesePublicHolidayCalendar.GetHolidayMap(start, end);
        return Json(new { items, map });
    }

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

            try
            {
                var prices = await _masterDataClient.GetMealPortionPricesAsync(accessToken, ct);
                vm.MealPortionPrices = (prices.Items ?? new List<MealPortionPriceOptionClientDto>())
                    .Select(p => new MealPortionPriceOptionVm
                    {
                        Id = p.Id,
                        Amount = p.Amount,
                        Label = !string.IsNullOrWhiteSpace(p.Label)
                            ? p.Label!
                            : $"{p.Amount:N0} đ/suất",
                    })
                    .ToList();
            }
            catch
            {
                vm.MealPortionPrices = new List<MealPortionPriceOptionVm>();
            }
        }
        catch (Exception ex)
        {
            vm.ApiError = ex.Message;
        }

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Dish(int id, CancellationToken ct = default)
    {
        if (!IsOrgUser(User)) return Forbid();
        try
        {
            var detail = await _masterDataClient.GetPublicDishDetailAsync(id, ct);
            return Json(new
            {
                dish = detail.Dish,
                ingredientQuotas = detail.IngredientQuotas,
                priceTiers = detail.PriceTiers,
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
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

            if (draft.ContractId > 0)
            {
                if (draft.ExcludedDates?.Count > 0)
                {
                    HttpContext.Session.SetString(
                        ExcludedSessionPrefix + draft.ContractId,
                        JsonSerializer.Serialize(draft.ExcludedDates, JsonOpts));
                }

                if (request.DailyMealPortions?.Count > 0)
                {
                    HttpContext.Session.SetString(
                        DailyMealsSessionPrefix + draft.ContractId,
                        JsonSerializer.Serialize(request.DailyMealPortions, JsonOpts));
                }
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
                    var excluded = ResolveExcludedDates(c.Id, c);
                    var openWeek = OrganizationMealContractDateRules.ResolveOpenWeekMonday(
                        today, start, end, excluded);
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
                        CanSelectWeeklyMeals = signed && hasOrder && !IsFullyCompleted(end, today) && openWeek.HasValue,
                        OpenWeekMonday = openWeek,
                        TotalServiceWeeks = c.TotalServiceWeeks ?? 0,
                        FilledServiceWeeks = c.FilledServiceWeeks ?? 0,
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
    public async Task<IActionResult> Detail(int id, CancellationToken ct = default)
    {
        if (!IsOrgUser(User)) return RedirectToAction(nameof(Index));
        if (id <= 0) return RedirectToAction(nameof(Contracts));

        var accessToken = HttpContext.Session.GetString("access_token");
        if (string.IsNullOrEmpty(accessToken))
            return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action(nameof(Detail), new { id }) });

        var vm = new OrganizationMealContractDetailVm { ContractId = id };
        try
        {
            var profile = await _authClient.GetProfileAsync(accessToken, ct);
            vm.OrganizationName = profile.Unit?.Name ?? "Đơn vị";

            var detail = await _masterDataClient.GetCompanyContractAsync(id, accessToken, ct);
            vm.Contract = detail.Contract;
            vm.SourceOrderId = detail.Contract.SourceOrderId;
            vm.Weekly = await _masterDataClient.GetContractWeeklySelectionsAsync(id, accessToken, ct);
        }
        catch (Exception ex)
        {
            vm.ApiError = ex.Message;
        }

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> WeeklyTrack(
        int id,
        string? weekStart = null,
        CancellationToken ct = default)
    {
        if (!IsOrgUser(User)) return RedirectToAction(nameof(Index));
        if (id <= 0) return RedirectToAction(nameof(Contracts));

        var accessToken = HttpContext.Session.GetString("access_token");
        if (string.IsNullOrEmpty(accessToken))
            return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action(nameof(WeeklyTrack), new { id, weekStart }) });

        var vm = new OrganizationMealContractWeeklyTrackVm { ContractId = id };
        try
        {
            var profile = await _authClient.GetProfileAsync(accessToken, ct);
            vm.OrganizationName = profile.Unit?.Name ?? "Đơn vị";

            var detail = await _masterDataClient.GetCompanyContractAsync(id, accessToken, ct);
            vm.ContractNumber = detail.Contract.ContractNumber;

            var weekly = await _masterDataClient.GetContractWeeklySelectionsAsync(id, accessToken, ct);
            ContractWeeklySelectionClientDto? week = null;
            if (!string.IsNullOrWhiteSpace(weekStart)
                && DateOnly.TryParse(weekStart, CultureInfo.InvariantCulture, out var parsed))
            {
                var monday = OrganizationMealContractDateRules.GetWeekMonday(parsed);
                week = weekly.Weeks.FirstOrDefault(w => w.WeekMonday == monday);
            }

            week ??= weekly.Weeks.FirstOrDefault(w => w.IsOpenWeek && w.IsFilled)
                     ?? weekly.Weeks.LastOrDefault(w => w.IsFilled);

            if (week == null || !week.IsFilled)
            {
                return RedirectToAction(nameof(Detail), new { id });
            }

            vm.Week = week;
        }
        catch (Exception ex)
        {
            vm.ApiError = ex.Message;
        }

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Weekly(
        int id,
        [FromQuery(Name = "contractId")] int contractIdQuery = 0,
        string? weekStart = null,
        CancellationToken ct = default)
    {
        if (!IsOrgUser(User)) return RedirectToAction(nameof(Index));
        var contractId = id > 0 ? id : contractIdQuery;
        if (contractId <= 0)
        {
            return RedirectToAction(nameof(Contracts));
        }

        var accessToken = HttpContext.Session.GetString("access_token");
        if (string.IsNullOrEmpty(accessToken))
            return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action(nameof(Weekly), new { id = contractId }) });

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

            var weeklySelections = await _masterDataClient.GetContractWeeklySelectionsAsync(contractId, accessToken, ct);
            var openSel = weeklySelections.Weeks.FirstOrDefault(w => w.IsOpenWeek);
            if (openSel != null)
            {
                if (openSel.IsFilled)
                {
                    return RedirectToAction(nameof(WeeklyTrack), new
                    {
                        id = contractId,
                        weekStart = openSel.WeekMonday.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    });
                }

                if (!openSel.CanSelect)
                {
                    TempData["OrgPeriodInfo"] =
                        "Tuần đang mở đã quá hạn chọn món thủ công (trước 3 ngày). Hệ thống sẽ tự chọn món hoặc đã gửi email nhắc.";
                    return RedirectToAction(nameof(Detail), new { id = contractId });
                }
            }

            var excluded = ResolveExcludedDates(contractId, c);
            var today = OrganizationMealContractDateRules.TodayVietnam();
            var currentMonday = OrganizationMealContractDateRules.GetWeekMonday(today);
            var openWeekMonday = OrganizationMealContractDateRules.ResolveOpenWeekMonday(
                today, vm.ContractStart, vm.ContractEnd, excluded);

            foreach (var (monday, weekEnd) in OrganizationMealContractDateRules.EnumerateWeeks(vm.ContractStart, vm.ContractEnd))
            {
                var serviceDays = OrganizationMealContractDateRules
                    .GetWeekServiceDates(monday, vm.ContractStart, vm.ContractEnd, excluded)
                    .ToList();
                if (serviceDays.Count == 0 && monday.AddDays(6) < vm.ContractStart)
                    continue;

                var isPast = weekEnd < today;
                var isCurrent = monday <= currentMonday && weekEnd >= currentMonday;
                var isOpenWeek = openWeekMonday.HasValue && monday == openWeekMonday.Value;
                vm.Weeks.Add(new ContractWeekVm
                {
                    WeekMonday = monday,
                    WeekEnd = weekEnd,
                    Label = $"Tuần {monday:dd/MM} – {weekEnd:dd/MM/yyyy}",
                    IsPast = isPast,
                    IsCurrent = isCurrent,
                    IsFuture = monday > currentMonday,
                    HasServiceDays = serviceDays.Count > 0,
                    CanSelect = isOpenWeek,
                    IsOpenWeek = isOpenWeek,
                });
            }

            ViewBag.ExcludedDatesJson = JsonSerializer.Serialize(
                excluded.Select(d => d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)).ToList(),
                JsonOpts);
            var dailyPortions = LoadDailyMealPortions(contractId, detail.Contract.DailyMealPortions);
            ViewBag.DailyMealPortionsJson = JsonSerializer.Serialize(dailyPortions, JsonOpts);
            ViewBag.OpenWeekMonday = openWeekMonday?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            ViewBag.InitialWeekStart = ResolveInitialWeekStart(
                weekStart, vm.ContractStart, vm.ContractEnd, today, excluded, openWeekMonday);

            if (!openWeekMonday.HasValue && string.IsNullOrEmpty(vm.ApiError))
            {
                vm.ApiError = "Hiện không có tuần nào cần chọn món. Tuần kế tiếp sẽ mở khi đến kỳ (thường từ thứ 5). " +
                              "Nếu không chọn, thứ 6 hệ thống tự phân món ngẫu nhiên theo số suất từng ngày.";
            }
            else if (openWeekMonday.HasValue
                     && !string.IsNullOrWhiteSpace(weekStart)
                     && DateOnly.TryParse(weekStart, CultureInfo.InvariantCulture, out var requestedWeek)
                     && OrganizationMealContractDateRules.GetWeekMonday(requestedWeek) != openWeekMonday.Value)
            {
                return RedirectToAction(nameof(Weekly), new { id = contractId });
            }
        }
        catch (Exception ex)
        {
            vm.ApiError = ex.Message;
            ViewBag.ExcludedDatesJson = "[]";
            ViewBag.DailyMealPortionsJson = "{}";
            ViewBag.InitialWeekStart = weekStart;
            ViewBag.OpenWeekMonday = null;
        }

        return View(vm);
    }

    private static string? ResolveInitialWeekStart(
        string? weekStart,
        DateOnly contractStart,
        DateOnly contractEnd,
        DateOnly today,
        IReadOnlyCollection<DateOnly> excluded,
        DateOnly? openWeekMonday)
    {
        if (openWeekMonday.HasValue
            && !string.IsNullOrWhiteSpace(weekStart)
            && DateOnly.TryParse(weekStart, CultureInfo.InvariantCulture, out var parsed))
        {
            var monday = OrganizationMealContractDateRules.GetWeekMonday(parsed);
            if (monday == openWeekMonday.Value)
                return monday.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        return openWeekMonday?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    }

    private Dictionary<string, int> LoadDailyMealPortions(
        int contractId,
        IReadOnlyList<ContractDailyMealPortionClientDto>? fromApi)
    {
        if (fromApi is { Count: > 0 })
        {
            return fromApi.ToDictionary(
                p => p.ServiceDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                p => p.MealCount);
        }

        var raw = HttpContext.Session.GetString(DailyMealsSessionPrefix + contractId);
        if (string.IsNullOrEmpty(raw)) return new Dictionary<string, int>();
        try
        {
            var items = JsonSerializer.Deserialize<List<ContractDailyMealPortionClientRequest>>(raw, JsonOpts)
                ?? new List<ContractDailyMealPortionClientRequest>();
            return items
                .Where(p => !string.IsNullOrWhiteSpace(p.ServiceDate) && p.MealCount > 0)
                .ToDictionary(p => p.ServiceDate, p => p.MealCount);
        }
        catch
        {
            return new Dictionary<string, int>();
        }
    }

    private List<DateOnly> ResolveExcludedDates(int contractId, CustomerContractDto? contract = null)
    {
        if (contract?.ExcludedDates is { Count: > 0 } fromApi)
        {
            var list = fromApi.OrderBy(d => d).ToList();
            HttpContext.Session.SetString(
                ExcludedSessionPrefix + contractId,
                JsonSerializer.Serialize(
                    list.Select(d => d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)).ToList(),
                    JsonOpts));
            return list;
        }

        return LoadExcludedDates(contractId);
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
    public async Task<IActionResult> SubmitWeekly(
        int id,
        [FromQuery(Name = "contractId")] int contractIdQuery = 0,
        [FromBody] SubmitOrganizationMealWeeklySelectionClientRequest? request = null,
        CancellationToken ct = default)
    {
        if (!IsOrgUser(User)) return Forbid();
        var contractId = id > 0 ? id : contractIdQuery;
        if (contractId <= 0)
            return BadRequest(new { message = "Thiếu mã hợp đồng. Vui lòng mở lại từ danh sách hợp đồng." });
        if (request == null)
            return BadRequest(new { message = "Thiếu dữ liệu thực đơn tuần." });

        var accessToken = HttpContext.Session.GetString("access_token");
        if (string.IsNullOrEmpty(accessToken)) return Unauthorized();

        try
        {
            var detail = await _masterDataClient.GetCompanyContractAsync(contractId, accessToken, ct);
            if (!string.Equals(detail.Contract.ContractType, "Period-Based", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new
                {
                    message = "Hợp đồng này không phải loại đặt suất theo kỳ. Vui lòng mở đúng hợp đồng từ mục Hợp đồng.",
                });
            }

            var c = detail.Contract;
            var contractStart = DateOnly.FromDateTime(c.StartDate);
            var contractEnd = c.EndDate.HasValue
                ? DateOnly.FromDateTime(c.EndDate.Value)
                : contractStart;
            var excluded = ResolveExcludedDates(contractId, c);
            var today = OrganizationMealContractDateRules.TodayVietnam();
            var openWeekMonday = OrganizationMealContractDateRules.ResolveOpenWeekMonday(
                today, contractStart, contractEnd, excluded);
            if (!openWeekMonday.HasValue)
            {
                return BadRequest(new
                {
                    message = "Hiện không có tuần nào cần chọn món. Vui lòng quay lại sau hoặc chờ hệ thống tự chọn món.",
                });
            }

            var weekStartIso = openWeekMonday.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            request.WeekStart = weekStartIso;

            var allowedDates = OrganizationMealContractDateRules
                .GetWeekServiceDates(openWeekMonday.Value, contractStart, contractEnd, excluded)
                .ToHashSet();
            request.MealDays = (request.MealDays ?? new List<OrganizationMealDayClientRequest>())
                .Where(d =>
                    !string.IsNullOrWhiteSpace(d.ServiceDate)
                    && DateOnly.TryParse(d.ServiceDate, CultureInfo.InvariantCulture, out var sd)
                    && allowedDates.Contains(sd))
                .ToList();
            if (request.MealDays.Count == 0)
            {
                var openEnd = openWeekMonday.Value.AddDays(6);
                return BadRequest(new
                {
                    message = $"Không có ngày ăn hợp lệ trong tuần đang mở ({openWeekMonday.Value:dd/MM/yyyy} – {openEnd:dd/MM/yyyy}). " +
                              "Vui lòng tải lại trang và chọn món cho đúng các ngày trong tuần.",
                });
            }

            var res = await _masterDataClient.SubmitOrganizationMealWeeklySelectionAsync(contractId, request, accessToken, ct);
            TempData["OrgPeriodSuccess"] = "Đã lưu thực đơn tuần thành công.";
            return Json(new
            {
                success = true,
                itemCount = res.ItemCount,
                redirectUrl = Url.Action(nameof(WeeklyTrack), new
                {
                    id = contractId,
                    weekStart = openWeekMonday.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                }),
            });
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
