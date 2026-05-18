using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khoa_Luan_KS_Web.Services;

namespace Khoa_Luan_KS_Web.Areas.Manager.Controllers;

[Area("Manager")]
[Authorize(Policy = "ManagerArea")]
public class PromotionController : Controller
{
    private readonly BackendMasterDataClient _client;

    public PromotionController(BackendMasterDataClient client) => _client = client;

    public async Task<IActionResult> Index(int page = 1, int pageSize = 20, string? searchTerm = null, bool? isActive = null, string? scopeType = null, CancellationToken ct = default)
    {
        ViewData["Title"] = "Khuyến mãi";
        var token = HttpContext.Session.GetString("access_token");
        if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

        try
        {
            var res = await _client.GetPromotionsAsync(token, page, pageSize, searchTerm, isActive, scopeType, ct);
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.SearchTerm = searchTerm ?? "";
            ViewBag.IsActiveFilter = isActive;
            ViewBag.ScopeType = scopeType ?? "";
            return View(res);
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return View(new GetPromotionsClientResponse());
        }
    }

    public IActionResult Create() => View(new PromotionFormVm { IsActive = true, ValidFrom = DateOnly.FromDateTime(DateTime.Today), ValidTo = DateOnly.FromDateTime(DateTime.Today.AddMonths(3)) });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PromotionFormVm model, CancellationToken ct)
    {
        var token = HttpContext.Session.GetString("access_token");
        if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

        if (!ModelState.IsValid) return View(model);

        try
        {
            await _client.CreatePromotionAsync(model.ToRequest(), token, ct);
            TempData["Success"] = "Đã tạo khuyến mãi.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var token = HttpContext.Session.GetString("access_token");
        if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

        try
        {
            var res = await _client.GetPromotionAsync(id, token, ct);
            return View(PromotionFormVm.FromDto(res.Promotion));
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PromotionFormVm model, CancellationToken ct)
    {
        var token = HttpContext.Session.GetString("access_token");
        if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

        if (!ModelState.IsValid) return View(model);

        try
        {
            await _client.UpdatePromotionAsync(id, model.ToRequest(), token, ct);
            TempData["Success"] = "Đã cập nhật khuyến mãi.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    public async Task<IActionResult> Detail(int id, CancellationToken ct)
    {
        ViewData["Title"] = "Chi tiết khuyến mãi";
        var token = HttpContext.Session.GetString("access_token");
        if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

        try
        {
            var res = await _client.GetPromotionAsync(id, token, ct);
            return View(res.Promotion);
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var token = HttpContext.Session.GetString("access_token");
        if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth", new { area = "" });

        try
        {
            await _client.DeletePromotionAsync(id, token, ct);
            TempData["Success"] = "Đã vô hiệu hóa khuyến mãi.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}

public class PromotionFormVm
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ScopeType { get; set; } = "order_quantity";
    public string DiscountType { get; set; } = "percent";
    public decimal DiscountValue { get; set; } = 5;
    public int Priority { get; set; } = 10;
    public string SelectionMode { get; set; } = "best_discount";
    public string Channel { get; set; } = "all";
    public int? MinOrderQuantity { get; set; }
    public decimal? MinOrderAmount { get; set; }
    public DateOnly ValidFrom { get; set; }
    public DateOnly ValidTo { get; set; }
    public TimeOnly? BookingTimeStart { get; set; }
    public TimeOnly? BookingTimeEnd { get; set; }
    public int? MaxTotalUses { get; set; }
    public int? MaxUsesPerUser { get; set; }
    public bool IsActive { get; set; } = true;

    /// <summary>Mỗi dòng: dish:12 | organization:3 | contract_type:Order-Based</summary>
    public string? TargetsText { get; set; }

    public UpsertPromotionClientRequest ToRequest()
    {
        var targets = new List<PromotionTargetClientDto>();
        if (!string.IsNullOrWhiteSpace(TargetsText))
        {
            foreach (var raw in TargetsText.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                var parts = raw.Split(':', 2, StringSplitOptions.TrimEntries);
                if (parts.Length != 2) continue;
                var type = parts[0].Trim().ToLowerInvariant();
                var val = parts[1].Trim();
                if (type == "contract_type")
                    targets.Add(new PromotionTargetClientDto { TargetType = type, TargetKey = val });
                else if (int.TryParse(val, out var id))
                    targets.Add(new PromotionTargetClientDto { TargetType = type, TargetId = id });
            }
        }

        return new UpsertPromotionClientRequest
        {
            Code = Code,
            Name = Name,
            Description = Description,
            ScopeType = ScopeType,
            DiscountType = DiscountType,
            DiscountValue = DiscountValue,
            Priority = Priority,
            SelectionMode = SelectionMode,
            Channel = Channel,
            MinOrderQuantity = MinOrderQuantity,
            MinOrderAmount = MinOrderAmount,
            ValidFrom = ValidFrom,
            ValidTo = ValidTo,
            BookingTimeStart = BookingTimeStart,
            BookingTimeEnd = BookingTimeEnd,
            MaxTotalUses = MaxTotalUses,
            MaxUsesPerUser = MaxUsesPerUser,
            IsActive = IsActive,
            Targets = targets,
        };
    }

    public static PromotionFormVm FromDto(PromotionClientDto p)
    {
        var lines = p.Targets.Select(t =>
        {
            if (t.TargetType == "contract_type") return $"contract_type:{t.TargetKey}";
            return $"{t.TargetType}:{t.TargetId}";
        });
        return new PromotionFormVm
        {
            Id = p.Id,
            Code = p.Code,
            Name = p.Name,
            Description = p.Description,
            ScopeType = p.ScopeType,
            DiscountType = p.DiscountType,
            DiscountValue = p.DiscountValue,
            Priority = p.Priority,
            SelectionMode = p.SelectionMode,
            Channel = p.Channel,
            MinOrderQuantity = p.MinOrderQuantity,
            MinOrderAmount = p.MinOrderAmount,
            ValidFrom = p.ValidFrom,
            ValidTo = p.ValidTo,
            BookingTimeStart = p.BookingTimeStart,
            BookingTimeEnd = p.BookingTimeEnd,
            MaxTotalUses = p.MaxTotalUses,
            MaxUsesPerUser = p.MaxUsesPerUser,
            IsActive = p.IsActive,
            TargetsText = string.Join(Environment.NewLine, lines),
        };
    }
}
