using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khoa_Luan_KS_Web.Services;

namespace Khoa_Luan_KS_Web.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Policy = "ManagerArea")]
    public class ContractController : Controller
    {
        private readonly BackendMasterDataClient _masterDataClient;
        private const int ExpiringWithinDays = 30;

        public ContractController(BackendMasterDataClient masterDataClient)
        {
            _masterDataClient = masterDataClient;
        }

        public async Task<IActionResult> Index(
            int page = 1,
            int pageSize = 12,
            string? searchTerm = null,
            string? statusFilter = null,
            CancellationToken ct = default)
        {
            var accessToken = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(accessToken))
                return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                var partners = await _masterDataClient.GetPartnersAsync(accessToken, 1, 200, ct: ct);
                ViewBag.Partners = partners.Items;

                var api = await _masterDataClient.GetContractsAsync(accessToken, 1, 500, searchTerm, ct: ct);
                var b2b = api.Items.Where(c => c.OrganizationId.HasValue).ToList();

                var filtered = ApplyStatusFilter(b2b, statusFilter);
                var totalCount = filtered.Count;
                var pageItems = filtered
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                ViewData["SearchTerm"] = searchTerm;
                ViewData["StatusFilter"] = statusFilter ?? "all";
                ViewData["CurrentPage"] = page;
                ViewData["PageSize"] = pageSize;

                return View(new GetContractsClientResponse
                {
                    Items = pageItems,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize
                });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                ViewData["SearchTerm"] = searchTerm;
                ViewData["StatusFilter"] = statusFilter ?? "all";
                ViewData["CurrentPage"] = page;
                ViewData["PageSize"] = pageSize;
                ViewBag.Partners = new List<PartnerDto>();
                return View(new GetContractsClientResponse());
            }
        }

        public async Task<IActionResult> Detail(int id, CancellationToken ct = default)
        {
            var accessToken = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(accessToken))
                return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                var response = await _masterDataClient.GetManagerContractAsync(id, accessToken, ct);
                if (response?.Contract == null || response.Contract.Id == 0)
                {
                    TempData["Error"] = "Không tìm thấy hợp đồng.";
                    return RedirectToAction(nameof(Index));
                }

                return View(response);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Edit(int id) => RedirectToAction(nameof(Detail), new { id });

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] CreateContractClientRequest request, CancellationToken ct = default)
        {
            try
            {
                var accessToken = HttpContext.Session.GetString("access_token");
                if (string.IsNullOrEmpty(accessToken))
                    return Unauthorized(new { message = "Not authenticated" });

                var created = await _masterDataClient.CreateContractAsync(request, accessToken, ct);
                return Json(new { success = true, contract = created.Contract });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private List<ContractListDto> ApplyStatusFilter(List<ContractListDto> items, string? statusFilter)
        {
            if (string.IsNullOrWhiteSpace(statusFilter) || statusFilter == "all")
                return items;

            return statusFilter switch
            {
                "active" => items.Where(c => string.Equals(c.Status, "active", StringComparison.OrdinalIgnoreCase) && !IsExpiringSoon(c)).ToList(),
                "expiring" => items.Where(IsExpiringSoon).ToList(),
                "expired" => items.Where(c => string.Equals(c.Status, "expired", StringComparison.OrdinalIgnoreCase)).ToList(),
                "cancelled" => items.Where(c => string.Equals(c.Status, "cancelled", StringComparison.OrdinalIgnoreCase)).ToList(),
                _ => items
            };
        }

        internal static bool IsExpiringSoon(ContractListDto c)
        {
            if (!string.Equals(c.Status, "active", StringComparison.OrdinalIgnoreCase) || !c.EndDate.HasValue)
                return false;
            var days = (c.EndDate.Value.Date - DateTime.Today).TotalDays;
            return days >= 0 && days <= ExpiringWithinDays;
        }

        internal static string UiStatus(ContractListDto c)
        {
            if (string.Equals(c.Status, "cancelled", StringComparison.OrdinalIgnoreCase))
                return "Đã thanh lý";
            if (string.Equals(c.Status, "expired", StringComparison.OrdinalIgnoreCase))
                return "Hết hạn";
            if (IsExpiringSoon(c))
                return "Sắp hết hạn";
            if (string.Equals(c.Status, "active", StringComparison.OrdinalIgnoreCase))
                return "Đang hiệu lực";
            return c.Status;
        }

    }
}
