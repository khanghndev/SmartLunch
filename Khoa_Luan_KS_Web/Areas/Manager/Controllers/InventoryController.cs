using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khoa_Luan_KS_Web.Services;

namespace Khoa_Luan_KS_Web.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Policy = "ManagerArea")]
    public class InventoryController : Controller
    {
        private const int NearExpiryDays = 30;

        private readonly BackendWarehouseClient _warehouseClient;

        public InventoryController(BackendWarehouseClient warehouseClient)
        {
            _warehouseClient = warehouseClient;
        }

        public async Task<IActionResult> Index(
            int page = 1,
            int pageSize = 20,
            string? searchTerm = null,
            string? statusFilter = null,
            CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth", new { area = "" });

            statusFilter = string.IsNullOrWhiteSpace(statusFilter) ? "all" : statusFilter;
            var vm = new ManagerInventoryIndexViewModel
            {
                Page = page,
                PageSize = pageSize,
                SearchTerm = searchTerm ?? "",
                StatusFilter = statusFilter
            };

            try
            {
                var ingredients = await _warehouseClient.GetIngredientsAsync(token, 1, 500, isActive: true, ct: ct);
                var ingredientMap = ingredients.Items.ToDictionary(i => i.Id);

                var lowStockSet = new HashSet<int>();
                try
                {
                    var alerts = await _warehouseClient.GetLowStockAlertsAsync(token, ct);
                    vm.LowStockCount = alerts.TotalCount;
                    foreach (var a in alerts.Alerts)
                        lowStockSet.Add(a.IngredientId);
                }
                catch
                {
                    vm.LowStockCount = 0;
                }

                var fetchPage = statusFilter == "all" ? page : 1;
                var fetchSize = statusFilter == "all" ? pageSize : 500;
                var inv = await _warehouseClient.GetInventoriesAsync(token, fetchPage, fetchSize, searchTerm, ct);

                var idsForExpiry = inv.Items.Select(i => i.IngredientId).ToList();
                var expiryMap = await BuildNearestExpiryMapAsync(idsForExpiry, token, ct);

                var allRows = inv.Items.Select(i =>
                {
                    ingredientMap.TryGetValue(i.IngredientId, out var ing);
                    expiryMap.TryGetValue(i.IngredientId, out var nearestExpiry);
                    return ManagerInventoryRowViewModel.From(i, ing, nearestExpiry, lowStockSet.Contains(i.IngredientId));
                }).ToList();

                var filtered = ApplyStatusFilter(allRows, statusFilter);
                vm.TotalCount = statusFilter == "all" ? inv.TotalCount : filtered.Count;
                vm.Rows = statusFilter == "all"
                    ? filtered
                    : filtered.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                try
                {
                    var allInv = await _warehouseClient.GetInventoriesAsync(token, 1, 500, null, ct);
                    vm.TotalSkuCount = allInv.TotalCount;
                    vm.EstimatedStockValue = allInv.Items.Sum(i =>
                    {
                        ingredientMap.TryGetValue(i.IngredientId, out var ing);
                        var cost = ing?.CostPerUnit ?? 0m;
                        return i.QuantityAvailable * cost;
                    });

                    var allIds = allInv.Items.Where(i => i.QuantityAvailable > 0).Select(i => i.IngredientId).Take(80).ToList();
                    var allExpiry = await BuildNearestExpiryMapAsync(allIds, token, ct);
                    var cutoff = DateTime.Today.AddDays(NearExpiryDays);
                    vm.NearExpiryCount = allExpiry.Count(kv =>
                        kv.Value.HasValue && kv.Value.Value.Date <= cutoff);

                    vm.OutOfStockCount = allInv.Items.Count(i => i.QuantityAvailable <= 0);
                }
                catch
                {
                    vm.TotalSkuCount = inv.TotalCount;
                    vm.OutOfStockCount = allRows.Count(r => r.UiStatus == "HetHang");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return View(vm);
        }

        public async Task<IActionResult> Detail(int id, CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                var detail = await _warehouseClient.GetIngredientInventoryDetailAsync(id, token, 30, ct);
                return View(detail);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Delivery() => View();

        private async Task<Dictionary<int, DateTime?>> BuildNearestExpiryMapAsync(
            IReadOnlyList<int> ingredientIds,
            string accessToken,
            CancellationToken ct)
        {
            var map = new ConcurrentDictionary<int, DateTime?>();
            if (ingredientIds.Count == 0)
                return new Dictionary<int, DateTime?>();

            await Parallel.ForEachAsync(
                ingredientIds.Distinct(),
                new ParallelOptions { MaxDegreeOfParallelism = 6, CancellationToken = ct },
                async (ingredientId, token) =>
                {
                    try
                    {
                        var detail = await _warehouseClient.GetIngredientInventoryDetailAsync(
                            ingredientId, accessToken, 10, token);
                        var nearest = detail.RecentBatches
                            .Where(b => b.ExpirationDate.HasValue)
                            .Select(b => b.ExpirationDate!.Value)
                            .OrderBy(d => d)
                            .FirstOrDefault();
                        if (nearest != default)
                            map[ingredientId] = nearest;
                    }
                    catch
                    {
                        // Bỏ qua mặt hàng không load được chi tiết
                    }
                });

            return map.ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        private static List<ManagerInventoryRowViewModel> ApplyStatusFilter(
            List<ManagerInventoryRowViewModel> rows,
            string statusFilter)
        {
            return statusFilter switch
            {
                "low" => rows.Where(r => r.UiStatus is "SapHet" or "HetHang").ToList(),
                "out" => rows.Where(r => r.UiStatus == "HetHang").ToList(),
                "near_expiry" => rows.Where(r => r.UiStatus == "CanDate").ToList(),
                _ => rows
            };
        }
    }

    public sealed class ManagerInventoryIndexViewModel
    {
        public List<ManagerInventoryRowViewModel> Rows { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string SearchTerm { get; set; } = "";
        public string StatusFilter { get; set; } = "all";
        public int TotalSkuCount { get; set; }
        public decimal EstimatedStockValue { get; set; }
        public int LowStockCount { get; set; }
        public int NearExpiryCount { get; set; }
        public int OutOfStockCount { get; set; }
    }

    public sealed class ManagerInventoryRowViewModel
    {
        public int IngredientId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public decimal QuantityAvailable { get; set; }
        public decimal? ReorderLevel { get; set; }
        public DateTime LastUpdated { get; set; }
        public decimal? CostPerUnit { get; set; }
        public decimal LineValue { get; set; }
        public DateTime? NearestExpiry { get; set; }
        public string UiStatus { get; set; } = "BinhThuong";
        public string UiStatusLabel { get; set; } = "Bình thường";

        public static ManagerInventoryRowViewModel From(
            InventoryClientDto inv,
            IngredientClientDto? ingredient,
            DateTime? nearestExpiry,
            bool isLowStockAlert)
        {
            var qty = inv.QuantityAvailable;
            var reorder = inv.ReorderLevel;
            var isOut = qty <= 0;
            var isLow = isLowStockAlert || (reorder.HasValue && qty > 0 && qty <= reorder.Value);
            var cutoff = DateTime.Today.AddDays(30);
            var isNearExpiry = nearestExpiry.HasValue && nearestExpiry.Value.Date <= cutoff && qty > 0;

            string uiStatus;
            string uiLabel;
            if (isOut)
            {
                uiStatus = "HetHang";
                uiLabel = "Hết hàng";
            }
            else if (isNearExpiry)
            {
                uiStatus = "CanDate";
                uiLabel = "Cận HSD";
            }
            else if (isLow)
            {
                uiStatus = "SapHet";
                uiLabel = "Sắp hết hàng";
            }
            else
            {
                uiStatus = "BinhThuong";
                uiLabel = "Bình thường";
            }

            var cost = ingredient?.CostPerUnit ?? 0m;
            return new ManagerInventoryRowViewModel
            {
                IngredientId = inv.IngredientId,
                Name = ingredient?.Name ?? $"Nguyên liệu #{inv.IngredientId}",
                Unit = ingredient?.Unit ?? "",
                QuantityAvailable = qty,
                ReorderLevel = reorder,
                LastUpdated = inv.LastUpdated,
                CostPerUnit = ingredient?.CostPerUnit,
                LineValue = qty * cost,
                NearestExpiry = nearestExpiry,
                UiStatus = uiStatus,
                UiStatusLabel = uiLabel
            };
        }
    }
}
