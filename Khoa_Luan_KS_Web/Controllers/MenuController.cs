using System.Linq;
using Khoa_Luan_KS_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Khoa_Luan_KS_Web.Controllers
{
    public class MenuController : Controller
    {
        private readonly Services.BackendMasterDataClient _masterDataClient;

        public MenuController(Services.BackendMasterDataClient masterDataClient)
        {
            _masterDataClient = masterDataClient;
        }

        public override async Task OnActionExecutionAsync(
            Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context,
            Microsoft.AspNetCore.Mvc.Filters.ActionExecutionDelegate next)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                bool isCustomer = User.IsInRole("Customer") ||
                                  User.IsInRole("Organization") ||
                                  User.IsInRole("Khách hàng doanh nghiệp") ||
                                  User.IsInRole("Khách hàng cá nhân");

                if (!isCustomer)
                {
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    HttpContext.Session.Clear();
                    context.Result = Redirect(Request.Path + Request.QueryString);
                    return;
                }
            }
            await next();
        }

        /// <summary>
        /// Trang thực đơn chính: lấy danh sách thực đơn tuần + chi tiết thực đơn hiện tại.
        /// </summary>
        public async Task<IActionResult> Index(int? menuId = null, string? profile = null, CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            var profileKey = string.IsNullOrWhiteSpace(profile) ? null : profile.Trim();

            Services.GetWeeklyMenusClientResponse? menuList = null;
            Services.GetWeeklyMenuDetailClientResponse? menuDetail = null;

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    // Lấy danh sách thực đơn tuần (tối đa 20 mục gần nhất), có thể lọc theo phân khúc (profile key)
                    menuList = await _masterDataClient.GetWeeklyMenusAsync(
                        token,
                        page: 1,
                        pageSize: 20,
                        customerProfileKey: profileKey,
                        ct: ct);

                    // Xác định thực đơn sẽ hiển thị chi tiết
                    int targetId;
                    if (menuId.HasValue)
                    {
                        targetId = menuId.Value;
                    }
                    else
                    {
                        // Ưu tiên thực đơn đang áp dụng, nếu không thì lấy thực đơn mới nhất
                        var today = DateTime.Today;
                        var current = menuList?.Items.FirstOrDefault(m =>
                            m.StartDate.Date <= today && m.EndDate.Date >= today);
                        var target = current ?? menuList?.Items.OrderByDescending(m => m.StartDate).FirstOrDefault();
                        targetId = target?.Id ?? 0;
                    }

                    if (targetId > 0)
                    {
                        menuDetail = await _masterDataClient.GetWeeklyMenuDetailAsync(targetId, token, ct);
                    }
                }
                catch (Exception ex)
                {
                    ViewData["ApiError"] = ex.Message;
                }
            }

            ViewData["MenuList"] = menuList;
            ViewData["MenuDetail"] = menuDetail;
            ViewData["SelectedMenuId"] = menuId;
            ViewData["CustomerProfileKey"] = profileKey;
            return View();
        }

        /// <summary>
        /// Chi tiết món: nếu có menuId + scheduleId thì lấy từ thực đơn tuần + API món; không thì hiển thị mẫu tĩnh (legacy).
        /// </summary>
        /// <summary>Legacy URL — chuyển hướng sang <see cref="DishDetail"/> (một trang chi tiết chung).</summary>
        public async Task<IActionResult> MealDetail(int? menuId = null, int? scheduleId = null, CancellationToken ct = default)
        {
            if (menuId is > 0 && scheduleId is > 0)
            {
                var token = HttpContext.Session.GetString("access_token");
                if (string.IsNullOrEmpty(token))
                {
                    var returnUrl = $"/Menu/DishDetail?menuId={menuId}&scheduleId={scheduleId}";
                    return RedirectToAction("Login", "Auth", new { area = "", returnUrl });
                }

                try
                {
                    var detail = await _masterDataClient.GetWeeklyMenuDetailAsync(menuId!.Value, token, ct);
                    var schedule = detail.Schedules.FirstOrDefault(s => s.Id == scheduleId!.Value);
                    if (schedule == null || schedule.DishId <= 0)
                        return NotFound();

                    return RedirectToAction(nameof(DishDetail), new
                    {
                        id = schedule.DishId,
                        menuId,
                        scheduleId
                    });
                }
                catch (Exception ex)
                {
                    TempData["Error"] = ex.Message;
                    return RedirectToAction(nameof(Index));
                }
            }

            return View();
        }

        public IActionResult SchoolMenu(string level)
        {
            ViewData["Level"] = string.IsNullOrEmpty(level) ? "mamnon" : level.ToLower();
            return View();
        }

        /// <summary>Thư viện toàn bộ món ăn (công khai).</summary>
        public async Task<IActionResult> Gallery(
            int page = 1,
            int? categoryId = null,
            string? q = null,
            CancellationToken ct = default)
        {
            var vm = new DishGalleryViewModel
            {
                Page = Math.Max(1, page),
                SelectedCategoryId = categoryId,
                Search = string.IsNullOrWhiteSpace(q) ? null : q.Trim(),
            };

            try
            {
                vm.Categories = await _masterDataClient.GetOrganizationDishCategoriesPublicAsync(ct);
                vm.Browse = await _masterDataClient.GetPublicDishesBrowseAsync(
                    vm.Page,
                    pageSize: 24,
                    categoryId: categoryId,
                    search: vm.Search,
                    ct: ct);
            }
            catch (Exception ex)
            {
                vm.LoadError = ex.Message;
            }

            return View(vm);
        }

        /// <summary>Chi tiết món ăn — trang chung (thư viện, thực đơn tuần, trang chủ).</summary>
        public async Task<IActionResult> DishDetail(
            int id,
            int? menuId = null,
            int? scheduleId = null,
            CancellationToken ct = default)
        {
            if (id <= 0 && menuId is > 0 && scheduleId is > 0)
            {
                var tokenForResolve = HttpContext.Session.GetString("access_token");
                if (!string.IsNullOrEmpty(tokenForResolve))
                {
                    try
                    {
                        var menuDetail = await _masterDataClient.GetWeeklyMenuDetailAsync(menuId.Value, tokenForResolve, ct);
                        var schedule = menuDetail.Schedules.FirstOrDefault(s => s.Id == scheduleId.Value);
                        if (schedule?.DishId is > 0)
                            return RedirectToAction(nameof(DishDetail), new { id = schedule.DishId, menuId, scheduleId });
                    }
                    catch
                    {
                        // fall through
                    }
                }
            }

            if (id <= 0)
                return NotFound();

            var vm = new DishDetailPageViewModel();
            var token = HttpContext.Session.GetString("access_token");

            try
            {
                vm.Detail = await _masterDataClient.GetPublicDishDetailAsync(id, ct);
                if (vm.Detail?.Dish == null || vm.Detail.Dish.Id <= 0)
                    return NotFound();
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("404", StringComparison.Ordinal) || ex.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrEmpty(token))
                {
                    try
                    {
                        vm.Detail = await _masterDataClient.GetDishAsync(id, token, ct);
                    }
                    catch
                    {
                        return NotFound();
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(token))
                {
                    try
                    {
                        vm.Detail = await _masterDataClient.GetDishAsync(id, token, ct);
                    }
                    catch (Exception authEx)
                    {
                        vm.LoadError = authEx.Message;
                    }
                }
                else
                {
                    vm.LoadError = ex.Message;
                }
            }

            if (menuId is > 0 && scheduleId is > 0 && !string.IsNullOrEmpty(token))
            {
                try
                {
                    var menuDetail = await _masterDataClient.GetWeeklyMenuDetailAsync(menuId.Value, token, ct);
                    var schedule = menuDetail.Schedules.FirstOrDefault(s => s.Id == scheduleId.Value);
                    if (schedule != null && schedule.DishId == id)
                    {
                        vm.MenuContext = new DishDetailMenuContext
                        {
                            MenuId = menuId.Value,
                            ScheduleId = scheduleId.Value,
                            MenuStartDate = menuDetail.WeeklyMenu.StartDate,
                            MenuEndDate = menuDetail.WeeklyMenu.EndDate,
                            ScheduleDate = schedule.Date,
                            MealSlot = schedule.MealSlot,
                        };
                    }

                    var quotasEmpty = vm.Detail?.PriceTiers == null || vm.Detail.PriceTiers.Count == 0;
                    if (quotasEmpty)
                        quotasEmpty = vm.Detail?.IngredientQuotas == null || vm.Detail.IngredientQuotas.Count == 0;
                    if (quotasEmpty)
                    {
                        try
                        {
                            var authDetail = await _masterDataClient.GetDishAsync(id, token, ct);
                            if (authDetail?.Dish != null)
                                vm.Detail = authDetail;
                        }
                        catch
                        {
                            // giữ dữ liệu public
                        }
                    }
                }
                catch
                {
                    // không chặn xem món nếu lỗi ngữ cảnh thực đơn
                }
            }

            if (vm.Detail?.Dish == null || vm.Detail.Dish.Id <= 0)
            {
                if (string.IsNullOrEmpty(vm.LoadError))
                    return NotFound();
            }
            else
            {
                try
                {
                    var suggestions = await _masterDataClient.GetPublicDishSuggestionsAsync(id, maxItems: 4, ct);
                    vm.SuggestedDishes = suggestions?.Items ?? new List<Services.SuggestedDishItemClientDto>();
                    if (vm.SuggestedDishes.Count == 0)
                        vm.SuggestedDishes = await BuildFallbackSuggestionsAsync(id, vm.Detail, ct);
                }
                catch (Exception ex)
                {
                    vm.SuggestedDishes = await BuildFallbackSuggestionsAsync(id, vm.Detail, ct);
                    if (vm.SuggestedDishes.Count == 0)
                        ViewData["SuggestionsError"] = ex.Message;
                }
            }

            return View(vm);
        }

        /// <summary>
        /// Fallback khi API gợi ý lỗi — chọn theo cân bằng dinh dưỡng + đa dạng NVL (không sort A→Z).
        /// </summary>
        private async Task<List<Services.SuggestedDishItemClientDto>> BuildFallbackSuggestionsAsync(
            int anchorDishId,
            Services.DishDetailResponse? anchorDetail,
            CancellationToken ct)
        {
            var result = new List<Services.SuggestedDishItemClientDto>();
            var anchor = anchorDetail?.Dish;
            if (anchor == null)
                return result;

            var anchorIngredients = MenuDishDisplayHelper.ResolveIngredientQuotas(anchorDetail)?
                .Select(q => q.IngredientName)
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .ToList() ?? [];

            var anchorVector = BuildTfIdfVectorFromNames(anchor.Name, anchorIngredients);
            var mealMacros = new MealMacroState(
                (double)(anchor.Calories ?? 0),
                (double)(anchor.Protein ?? 0),
                (double)(anchor.Fat ?? 0),
                (double)(anchor.Carbs ?? 0));

            var primary = (anchor.PrimarySlotKey ?? "main").Trim().ToLowerInvariant();
            var targetSlots = primary switch
            {
                "vegetable" => new[] { "main", "soup", "side", "dessert" },
                "soup" => new[] { "main", "vegetable", "side" },
                "side" => new[] { "main", "soup", "vegetable" },
                "dessert" => new[] { "main", "soup", "side" },
                "noodle_soup" => new[] { "side", "vegetable", "dessert" },
                _ => new[] { "soup", "vegetable", "side", "dessert" },
            };

            var mealVector = new Dictionary<string, double>(anchorVector, StringComparer.OrdinalIgnoreCase);
            var ideal = BuildIdealMealProfile(mealMacros);

            try
            {
                var categories = await _masterDataClient.GetOrganizationDishCategoriesPublicAsync(ct);

                foreach (var slot in targetSlots)
                {
                    if (result.Count >= 4)
                        break;

                    var cat = categories.Categories.FirstOrDefault(c =>
                        string.Equals(c.SlotKey, slot, StringComparison.OrdinalIgnoreCase));
                    if (cat == null)
                        continue;

                    var browse = await _masterDataClient.GetPublicDishesBrowseAsync(1, 100, cat.Id, null, ct);

                    var best = browse.Items
                        .Where(i => i.Id != anchorDishId)
                        .Select(i =>
                        {
                            var vec = BuildTfIdfVectorFromNames(i.Name, []);
                            var mealSim = CosineSimilarity(mealVector, vec);
                            var anchorSim = CosineSimilarity(anchorVector, vec);
                            var diversity = (1.0 - mealSim) * 0.4 + (1.0 - anchorSim) * 0.3;

                            var candMacros = new MealMacroState(
                                ResolveMacro(i.Calories, EstimateCalories(i.Name, slot)),
                                ResolveMacro(i.Protein, EstimateProtein(i.Name, slot)),
                                ResolveMacro(i.Fat, EstimateFat(i.Name, slot)),
                                ResolveMacro(i.Carbs, EstimateCarbs(i.Name, slot)));

                            var nutrition = ScoreNutritionImprovement(mealMacros, candMacros, ideal, anchor, slot);
                            var total = nutrition * 0.65 + diversity * 0.35;
                            return new { Item = i, Score = total, Vector = vec, Nutrition = nutrition };
                        })
                        .OrderByDescending(x => x.Score)
                        .ThenByDescending(x => x.Nutrition)
                        .ThenBy(x => StableFallbackRank(anchorDishId, slot, x.Item.Id))
                        .FirstOrDefault();

                    if (best == null)
                        continue;

                    foreach (var kvp in best.Vector)
                    {
                        if (!mealVector.ContainsKey(kvp.Key) || mealVector[kvp.Key] < kvp.Value)
                            mealVector[kvp.Key] = kvp.Value;
                    }

                    mealMacros = mealMacros.Add(new MealMacroState(
                        ResolveMacro(best.Item.Calories, EstimateCalories(best.Item.Name, slot)),
                        ResolveMacro(best.Item.Protein, EstimateProtein(best.Item.Name, slot)),
                        ResolveMacro(best.Item.Fat, EstimateFat(best.Item.Name, slot)),
                        ResolveMacro(best.Item.Carbs, EstimateCarbs(best.Item.Name, slot))));

                    result.Add(new Services.SuggestedDishItemClientDto
                    {
                        Id = best.Item.Id,
                        Name = best.Item.Name,
                        Description = best.Item.Description,
                        ImageUrl = best.Item.ImageUrl,
                        PrimarySlotKey = best.Item.PrimarySlotKey ?? slot,
                        CategoryLabel = best.Item.CategoryLabel,
                        DietaryLabel = best.Item.DietaryLabel,
                        MatchReason = BuildFallbackMatchReason(anchor, slot, best.Item),
                    });
                }
            }
            catch
            {
                // ignore fallback errors
            }

            return result;
        }

        private readonly record struct MealMacroState(double Calories, double Protein, double Fat, double Carbs)
        {
            public MealMacroState Add(MealMacroState other) =>
                new(Calories + other.Calories, Protein + other.Protein, Fat + other.Fat, Carbs + other.Carbs);
        }

        private static MealMacroState BuildIdealMealProfile(MealMacroState anchor)
        {
            if (anchor.Calories <= 0 && anchor.Protein <= 0)
                return new MealMacroState(700, 40, 18, 65);

            return new MealMacroState(
                Math.Clamp(anchor.Calories * 1.32, 620, 980),
                Math.Clamp(anchor.Protein * 1.10, 30, 65),
                Math.Clamp(anchor.Fat * 0.92, 10, 32),
                Math.Clamp(anchor.Carbs * 1.15, 45, 110));
        }

        private static double MacroDistance(MealMacroState actual, MealMacroState target)
        {
            static double N(double v, double t) => Math.Abs(v - t) / Math.Max(t, 1);
            return 0.25 * N(actual.Calories, target.Calories)
                 + 0.30 * N(actual.Protein, target.Protein)
                 + 0.20 * N(actual.Fat, target.Fat)
                 + 0.25 * N(actual.Carbs, target.Carbs);
        }

        private static double ScoreNutritionImprovement(
            MealMacroState meal,
            MealMacroState candidate,
            MealMacroState ideal,
            Services.DishDto anchor,
            string slot)
        {
            var before = MacroDistance(meal, ideal);
            var after = MacroDistance(meal.Add(candidate), ideal);
            var improvement = before > 1e-9 ? Math.Clamp((before - after) / before, 0, 1) : 0.5;

            var anchorPro = (double)(anchor.Protein ?? 0);
            var anchorFat = (double)(anchor.Fat ?? 0);
            var anchorCal = (double)(anchor.Calories ?? 0);

            if (anchorPro > 24 && candidate.Protein > 14 && slot is "side" or "soup" or "vegetable")
                improvement -= 0.35;
            if (anchorFat > 14 && candidate.Fat is > 0 and < 10 && slot == "vegetable")
                improvement += 0.15;
            if (anchorCal > 480 && candidate.Calories is > 0 and < 180 && slot == "soup")
                improvement += 0.12;
            if (anchorCal > 420 && candidate.Calories is > 0 and < 200 && slot == "dessert")
                improvement += 0.10;

            return Math.Clamp(improvement, 0, 1);
        }

        private static string BuildFallbackMatchReason(Services.DishDto anchor, string slot, Services.PublicDishBrowseItemClientDto pick)
        {
            var anchorPro = (double)(anchor.Protein ?? 0);
            var anchorFat = (double)(anchor.Fat ?? 0);
            var anchorCal = (double)(anchor.Calories ?? 0);
            var candPro = ResolveMacro(pick.Protein, EstimateProtein(pick.Name, slot));

            return slot switch
            {
                "vegetable" when anchorFat > 14 => "Bổ sung rau xanh · giảm chất béo bữa ăn",
                "vegetable" when anchorPro > 22 => "Thêm món xanh · cân bằng đạm",
                "soup" when anchorCal > 480 => "Canh thanh · cân bằng năng lượng",
                "soup" when anchorPro > 22 && candPro < 12 => "Canh đi kèm · ít đạm",
                "side" when anchorPro > 24 && candPro < 10 => "Món phụ nhẹ · không tăng đạm",
                "dessert" when anchorCal > 420 => "Tráng miệng nhẹ · hợp bữa nhiều năng lượng",
                "main" => "Bổ sung món mặn / protein",
                "soup" => "Canh đi kèm cân bằng",
                "vegetable" => "Thêm món xanh · bổ sung vi chất",
                "side" => "Món phụ hợp khẩu vị",
                "dessert" => "Tráng miệng nhẹ",
                _ => "Phù hợp bữa ăn trọn vẹn",
            };
        }

        private static double ResolveMacro(decimal? value, double estimate) =>
            value.HasValue ? (double)value.Value : estimate;

        private static double EstimateCalories(string name, string slot) =>
            slot switch
            {
                "soup" => 120,
                "vegetable" => 90,
                "side" => 150,
                "dessert" => 180,
                "main" => 450,
                _ => 200,
            };

        private static double EstimateProtein(string name, string slot)
        {
            var n = name.ToLowerInvariant();
            if (n.Contains("trứng")) return 12;
            if (n.Contains("tôm") || n.Contains("cá") || n.Contains("gà") || n.Contains("thịt")) return 18;
            return slot switch
            {
                "main" => 22,
                "soup" => 8,
                "vegetable" => 4,
                "side" => 6,
                "dessert" => 2,
                _ => 8,
            };
        }

        private static double EstimateFat(string name, string slot)
        {
            var n = name.ToLowerInvariant();
            if (n.Contains("chiên") || n.Contains("rán")) return 14;
            return slot switch
            {
                "vegetable" => 5,
                "soup" => 4,
                "dessert" => 3,
                "side" => 8,
                _ => 10,
            };
        }

        private static double EstimateCarbs(string name, string slot) =>
            slot switch
            {
                "dessert" => 35,
                "main" => 55,
                "side" => 20,
                _ => 12,
            };

        // ─── TF-IDF helpers (dùng trong fallback) ───────────────────────────────

        private static int StableFallbackRank(int anchorId, string slot, int dishId)
        {
            unchecked
            {
                var h = anchorId;
                h = h * 31 + slot.GetHashCode(StringComparison.OrdinalIgnoreCase);
                h = h * 31 + dishId;
                return h & int.MaxValue;
            }
        }

        private static readonly string[] _familyRoots =
        [
            "bí đỏ", "cà rốt", "khoai tây", "khoai lang", "cải ngọt", "rau muống", "mồng tơi",
            "cà chua", "dưa leo", "dưa hấu", "bắp cải", "súp lơ", "đậu cove", "đậu hũ", "nấm",
            "tôm", "cua", "mực", "cá basa", "cá lóc", "cá", "thịt heo", "thịt bò", "gà", "thịt bằm",
            "trứng", "gạo", "bún", "phở", "mì",
        ];

        private static readonly HashSet<string> _seasoningFamilies = new(StringComparer.OrdinalIgnoreCase)
        {
            "tỏi", "hành", "hành tím", "hành tây", "gừng", "ớt", "tiêu", "muối", "đường", "nước mắm",
        };

        /// <summary>
        /// Xây dựng TF-IDF vector từ tên món + danh sách tên nguyên liệu.
        /// TF đơn giản: mỗi family root xuất hiện = 1 (binary presence).
        /// </summary>
        private static Dictionary<string, double> BuildTfIdfVectorFromNames(
            string dishName,
            IReadOnlyList<string> ingredientNames)
        {
            var vector = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

            // Nguyên liệu: weight 1.0 mỗi family
            foreach (var ingName in ingredientNames)
            {
                if (string.IsNullOrWhiteSpace(ingName))
                    continue;
                foreach (var family in ExtractFamilyRoots(ingName))
                {
                    if (!_seasoningFamilies.Contains(family))
                        vector[family] = Math.Max(vector.GetValueOrDefault(family), 1.0);
                }
            }

            // Tên món: weight theo prominence (0.48–1.0), nhân 0.7 để nhẹ hơn
            foreach (var family in ExtractFamilyRoots(dishName))
            {
                if (_seasoningFamilies.Contains(family))
                    continue;
                var prominence = GetDishNameProminence(dishName, family) * 0.7;
                vector[family] = Math.Max(vector.GetValueOrDefault(family), prominence);
            }

            return vector;
        }

        private static double CosineSimilarity(
            Dictionary<string, double> vecA,
            Dictionary<string, double> vecB)
        {
            if (vecA.Count == 0 || vecB.Count == 0)
                return 0.0;

            double dot = 0, normA = 0, normB = 0;
            foreach (var (key, valA) in vecA)
            {
                normA += valA * valA;
                if (vecB.TryGetValue(key, out var valB))
                    dot += valA * valB;
            }
            foreach (var valB in vecB.Values)
                normB += valB * valB;

            return (normA <= 0 || normB <= 0) ? 0.0 : dot / (Math.Sqrt(normA) * Math.Sqrt(normB));
        }

        private static IEnumerable<string> ExtractFamilyRoots(string text)
        {
            var lower = text.Trim().ToLowerInvariant();
            if (lower.Length == 0) yield break;
            foreach (var root in _familyRoots.OrderByDescending(r => r.Length))
                if (lower.Contains(root, StringComparison.OrdinalIgnoreCase))
                    yield return root;
        }

        private static double GetDishNameProminence(string dishName, string family)
        {
            var name = dishName.Trim().ToLowerInvariant();
            if (!name.Contains(family, StringComparison.OrdinalIgnoreCase))
                return 0;
            if (name.StartsWith(family, StringComparison.OrdinalIgnoreCase))
                return 1.0;
            if (name.Contains($"canh {family}",    StringComparison.OrdinalIgnoreCase) ||
                name.Contains($"chè {family}",     StringComparison.OrdinalIgnoreCase) ||
                name.Contains($"{family} xào",     StringComparison.OrdinalIgnoreCase) ||
                name.Contains($"{family} luộc",    StringComparison.OrdinalIgnoreCase) ||
                name.Contains($"{family} kho",     StringComparison.OrdinalIgnoreCase) ||
                name.Contains($"{family} chiên",   StringComparison.OrdinalIgnoreCase))
                return 0.92;
            return 0.48;
        }
    }
}
