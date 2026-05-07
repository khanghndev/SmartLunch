using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.MenuSuggestions;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.Models.AiMenuPlanner;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Commands.MenuSuggestions.GenerateMenuSuggestionFromAi;

public class GenerateMenuSuggestionFromAiCommandHandler
    : IRequestHandler<GenerateMenuSuggestionFromAiCommand, CreateMenuSuggestionResponse>
{
    private readonly IAiMenuPlannerClient _aiClient;
    private readonly IDishRepository _dishRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IMenuSuggestionRepository _menuSuggestionRepository;
    private readonly IIngredientRepository _ingredientRepository;
    private readonly ILogger<GenerateMenuSuggestionFromAiCommandHandler> _logger;

    public GenerateMenuSuggestionFromAiCommandHandler(
        IAiMenuPlannerClient aiClient,
        IDishRepository dishRepository,
        IInventoryRepository inventoryRepository,
        IMenuSuggestionRepository menuSuggestionRepository,
        IIngredientRepository ingredientRepository,
        ILogger<GenerateMenuSuggestionFromAiCommandHandler> logger)
    {
        _aiClient = aiClient;
        _dishRepository = dishRepository;
        _inventoryRepository = inventoryRepository;
        _menuSuggestionRepository = menuSuggestionRepository;
        _ingredientRepository = ingredientRepository;
        _logger = logger;
    }

    public async Task<CreateMenuSuggestionResponse> Handle(
        GenerateMenuSuggestionFromAiCommand request,
        CancellationToken cancellationToken)
    {
        var req = request.Request;
        if (req == null)
            throw new ArgumentException("Request is required.");
        if (req.WeekStartUtc == default)
            throw new ArgumentException("WeekStartUtc is required.");
        if (req.DishIds == null || req.DishIds.Count == 0)
            throw new ArgumentException("DishIds is required and must not be empty.");

        var createdBy = request.CreatedByUserId
            ?? throw new ArgumentException("CreatedByUserId is required.");

        var weekStartUtc = DateTime.SpecifyKind(req.WeekStartUtc.Date, DateTimeKind.Utc);
        var nextVersion = await _menuSuggestionRepository.GetNextVersionAsync(weekStartUtc, createdBy);

        // ── 1. Fetch dishes with ingredients from DB ──────────────────────
        var dishes = await _dishRepository.GetByIdsWithIngredientsAsync(req.DishIds, cancellationToken);
        if (dishes.Count == 0)
            throw new ArgumentException("No active dishes found for the provided DishIds.");

        var missingSlots = dishes
            .Where(d => DishAiEnglishCatalog.BuildSlotKeysFromJunction(d).Count == 0)
            .Select(d => d.Name)
            .ToList();
        if (missingSlots.Count > 0)
            throw new InvalidOperationException(
                $"The following dishes have no meal slots (dish_dish_categories): {string.Join(", ", missingSlots)}. " +
                "Assign at least one dish_categories.SlotKey per dish (e.g. main, soup, noodle_soup).");

        var missingMethod = dishes
            .Where(d => d.CookingMethod == null || string.IsNullOrWhiteSpace(d.CookingMethod.MethodKey))
            .Select(d => d.Name)
            .ToList();
        if (missingMethod.Count > 0)
            throw new InvalidOperationException(
                $"The following dishes have no cooking method (cooking_methods / CookingMethodId): {string.Join(", ", missingMethod)}.");

        // ── 2. Compute popularity scores (4-week rolling window) ──────────
        var popularityMap = await _dishRepository.GetPopularityScoresAsync(
            req.DishIds, lookbackDays: 28, cancellationToken);

        // ── 3. Fetch inventory for all involved ingredients ────────────────
        var ingredientIds = dishes
            .SelectMany(d => d.DishIngredients.Select(di => di.IngredientId))
            .Distinct()
            .ToList();

        var inventories = await _inventoryRepository.GetByIngredientIdsAsync(ingredientIds, cancellationToken);

        // Map ingredient: NameEnglish (or Name as fallback) → quantity_kg
        // NameEnglish is the identifier AI uses to match available_ingredients
        var inventoryMap = inventories.ToDictionary(
            inv => string.IsNullOrWhiteSpace(inv.Ingredient.NameEnglish)
                ? inv.Ingredient.Name
                : inv.Ingredient.NameEnglish,
            inv => (double)inv.QuantityAvailable);

        // ── 4. Map dishes → AiIndustrialDish ─────────────────────────────
        // No runtime text mapping — all enum values come directly from DB columns.
        var aiDishes = dishes.Select(d =>
        {
            // Sort ingredients by quantity desc: largest = main ingredient
            var sortedIngredients = d.DishIngredients
                .OrderByDescending(di => di.Quantity)
                .ToList();

            // main_ingredient = English name of the highest-quantity ingredient
            // This is what CP-SAT uses for protein grouping (_PROTEIN_INGREDIENTS set)
            var mainIngredientEn = sortedIngredients
                .Select(di => di.Ingredient.NameEnglish ?? di.Ingredient.Name)
                .FirstOrDefault() ?? string.Empty;

            // sub_ingredients = English names of remaining ingredients
            var subIngredients = sortedIngredients
                .Skip(1)
                .Select(di => di.Ingredient.NameEnglish ?? di.Ingredient.Name)
                .ToList();

            var tags = string.IsNullOrWhiteSpace(d.DietaryLabel)
                ? new List<string>()
                : d.DietaryLabel
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .ToList();

            var covers = DishAiEnglishCatalog.BuildSlotKeysFromJunction(d);
            var primaryCategory = DishAiEnglishCatalog.PickPrimaryCategoryForAi(covers);

            return new AiIndustrialDish
            {
                Name = d.Name,                                              // Vietnamese — displayed in output
                NameEnglish = d.NameEnglish ?? d.Name,                     // English — used by AI for grouping
                Category = primaryCategory,                                // Primary shelf for Python dishes_by_cat
                CoversCategories = covers,
                MainIngredient = mainIngredientEn,                         // English — CP-SAT protein grouping
                SubIngredients = subIngredients,                           // English — shopping list / reuse logic
                CookingMethod = d.CookingMethod!.MethodKey,
                CostPerServing = d.Price,
                Popularity = popularityMap.GetValueOrDefault(d.Id, 3),
                MaxPerWeek = 2, // Default — chưa có dữ liệu chính xác
                Tags = tags,
            };
        }).ToList();

        // ── 5. Build available_ingredients từ inventory ───────────────────
        // AI matches ingredient names against main_ingredient/sub_ingredients
        // which are now English, so inventory keys must also be English.
        var availableIngredients = inventoryMap
            .Where(kv => kv.Value > 0)
            .Select(kv => new AiAvailableIngredient
            {
                Name = kv.Key,
                QuantityKg = (decimal)kv.Value,
            })
            .ToList();

        // ── 5.5 Fetch & Group all ingredients for AI rule engine ─────────
        var allIngredients = await _ingredientRepository.GetAllWithCategoryAsync(cancellationToken);
        var ingredientGroups = allIngredients
            .Where(i => i.Category != null)
            .GroupBy(i => i.Category!.NameEnglish)
            .ToDictionary(
                g => g.Key,
                g => g.Select(i => i.NameEnglish ?? i.Name).Distinct().ToList()
            );

        // ── 6. Build AI request ───────────────────────────────────────────
        // Map Vietnamese day names / slot labels sent by FE to AI enum values
        var dayMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Thứ 2"] = "Thu_2",  ["thu 2"] = "Thu_2",  ["Thu_2"] = "Thu_2",  ["Monday"] = "Thu_2",
            ["Thứ 3"] = "Thu_3",  ["thu 3"] = "Thu_3",  ["Thu_3"] = "Thu_3",  ["Tuesday"] = "Thu_3",
            ["Thứ 4"] = "Thu_4",  ["thu 4"] = "Thu_4",  ["Thu_4"] = "Thu_4",  ["Wednesday"] = "Thu_4",
            ["Thứ 5"] = "Thu_5",  ["thu 5"] = "Thu_5",  ["Thu_5"] = "Thu_5",  ["Thursday"] = "Thu_5",
            ["Thứ 6"] = "Thu_6",  ["thu 6"] = "Thu_6",  ["Thu_6"] = "Thu_6",  ["Friday"] = "Thu_6",
            ["Thứ 7"] = "Thu_7",  ["thu 7"] = "Thu_7",  ["Thu_7"] = "Thu_7",  ["Saturday"] = "Thu_7",
            ["Chủ nhật"] = "CN",  ["chu nhat"] = "CN",  ["CN"] = "CN",        ["Sunday"] = "CN",
        };
        var mealMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Món chính"] = "main",      ["mon chinh"] = "main",     ["main"] = "main",
            ["Món mặn"] = "main",        ["mon man"] = "main",
            ["Món phụ"] = "side",        ["mon phu"] = "side",       ["side"] = "side",
            ["Món xào"] = "side",        ["mon xao"] = "side",
            ["Canh"] = "soup",           ["Món canh"] = "soup",      ["mon canh"] = "soup",      ["soup"] = "soup",
            ["Món rau"] = "vegetable",   ["mon rau"] = "vegetable",  ["vegetable"] = "vegetable",
            ["Rau xanh"] = "vegetable",
            ["Món nước"] = "noodle_soup", ["noodle_soup"] = "noodle_soup",
            ["Tráng miệng"] = "dessert", ["trang mieng"] = "dessert", ["dessert"] = "dessert",
        };
        var mappedDays = req.Days.Select(d => dayMap.TryGetValue(d.Trim(), out var mapped) ? mapped : d).Distinct().ToList();
        var mappedMeal = req.MealStructure.Select(m => mealMap.TryGetValue(m.Trim(), out var mapped) ? mapped : m).Distinct().ToList();

        // ── Ensure ingredient_groups has fallback protein/seafood keywords ──────────────
        // AI rules.json uses these group names for constraints; if DB has no categories,
        // we provide a broad keyword set to avoid INFEASIBLE solver.
        var mergedGroups = new Dictionary<string, List<string>>(ingredientGroups, StringComparer.OrdinalIgnoreCase);

        if (!mergedGroups.ContainsKey("protein"))
        {
            mergedGroups["protein"] = new List<string>
            {
                "pork", "chicken", "beef", "fish", "shrimp", "egg", "tofu",
                "Thịt heo", "Thịt gà", "Thịt bò", "Cá", "Tôm", "Trứng", "Đậu hũ",
                "Thịt heo nạc vai", "Thịt gà ta"
            };
        }
        if (!mergedGroups.ContainsKey("seafood"))
        {
            mergedGroups["seafood"] = new List<string>
            {
                "shrimp", "fish", "crab", "squid", "clam",
                "Tôm", "Cá", "Cua", "Mực", "Nghêu", "Tôm sú", "Cá basa", "Cá lóc"
            };
        }

        // ── Detect if there are enough seafood-covered dishes to satisfy min_fish constraint ──
        var seafoodKeywords = new HashSet<string>(mergedGroups["seafood"], StringComparer.OrdinalIgnoreCase);
        int seafoodDishCount = aiDishes.Count(d =>
            seafoodKeywords.Contains(d.MainIngredient) ||
            d.SubIngredients.Any(s => seafoodKeywords.Contains(s))
        );

        // If fewer than 2 seafood dishes, pass explicit constraints that skip the seafood min requirement
        AiConstraintsOverride? constraintsOverride = null;
        if (seafoodDishCount < 2)
        {
            _logger.LogWarning(
                "Only {SeafoodCount} seafood dish(es) found — relaxing min_fish constraint to avoid INFEASIBLE.",
                seafoodDishCount);
            constraintsOverride = new AiConstraintsOverride
            {
                GroupFrequencies = new List<AiGroupFrequencyConstraint>
                {
                    new() { GroupName = "protein", MaxCount = mappedDays.Count }
                },
                NoRepeatMainIngredientConsecutiveDays = true,
                AlternateCookingMethods = false,  // relax to improve feasibility
                PreferIngredientReuse = false,
                MaxConsecutiveSameMainDish = 3,
            };
        }

        var aiReq = new AiIndustrialMenuPlansRequest
        {
            BudgetPerServing = req.BudgetPerServing,
            Days = mappedDays,
            MealStructure = mappedMeal,   // mapped from Vietnamese to AI enum values
            TopK = req.TopK,
            TimeLimitSeconds = req.TimeLimitSeconds,
            RulesKey = req.RulesKey,
            Dishes = aiDishes,
            AvailableIngredients = availableIngredients,
            IngredientGroups = mergedGroups,
            Constraints = constraintsOverride,
        };

        _logger.LogInformation(
            "Calling AI with {DishCount} dishes, {IngCount} available ingredients, {DayCount} days, slots: {Slots}",
            aiDishes.Count, availableIngredients.Count, mappedDays.Count, string.Join(",", mappedMeal));

        var aiRes = await _aiClient.RecommendIndustrialMenusAsync(aiReq, cancellationToken);

        // ── 7. Persist ────────────────────────────────────────────────────
        var entity = new MenuSuggestion
        {
            WeekStart = weekStartUtc,
            GeneratedAt = DateTime.UtcNow,
            Version = nextVersion,
            RulesKey = req.RulesKey,
            BudgetPerServing = req.BudgetPerServing,
            TopK = req.TopK,
            TimeLimitSeconds = req.TimeLimitSeconds,
            PlanCount = aiRes.Plans.Count,
            AlgorithmVersion = "ortools-cpsat",
            CreatedBy = createdBy,
            SuggestionText = $"Generated {aiRes.Plans.Count} plan(s).",
        };

        foreach (var plan in aiRes.Plans)
        {
            var planEntity = new MenuSuggestionPlan
            {
                Rank = plan.Rank,
                PlanScore = plan.PlanScore,
                ObjectiveValue = plan.ObjectiveValue,
            };

            for (var d = 0; d < plan.WeekMenu.Count; d++)
            {
                var day = plan.WeekMenu[d];
                var dayEntity = new MenuSuggestionPlanDay
                {
                    DayIndex = (byte)d,
                    DayName = day.Day,
                };

                foreach (var item in day.Dishes)
                {
                    dayEntity.Items.Add(new MenuSuggestionPlanItem
                    {
                        SlotCategory = item.Category,
                        DishName = item.Name,
                        DishSourceCategory = null,
                        Score = item.Score,
                        CostPerServing = item.CostPerServing,
                        ReasonsJson = item.Reasons.Count == 0
                            ? null
                            : JsonSerializer.Serialize(item.Reasons),
                    });
                }

                planEntity.Days.Add(dayEntity);
            }

            entity.Plans.Add(planEntity);
        }

        await _menuSuggestionRepository.AddAsync(entity, cancellationToken);
        await _menuSuggestionRepository.CommitAsync();

        _logger.LogInformation(
            "Created menu suggestion week {WeekStart} v{Version} with {Plans} plan(s)",
            weekStartUtc, nextVersion, aiRes.Plans.Count);

        return new CreateMenuSuggestionResponse
        {
            MenuSuggestion = new MenuSuggestionDto
            {
                Id = entity.Id,
                WeekStart = entity.WeekStart,
                GeneratedAt = entity.GeneratedAt,
                Version = entity.Version,
                RulesKey = entity.RulesKey,
                BudgetPerServing = entity.BudgetPerServing,
                TopK = entity.TopK,
                TimeLimitSeconds = entity.TimeLimitSeconds,
                PlanCount = entity.PlanCount,
                SuggestionText = entity.SuggestionText,
                AlgorithmVersion = entity.AlgorithmVersion,
                CreatedBy = entity.CreatedBy,
                Plans = entity.Plans
                    .OrderBy(p => p.Rank)
                    .Select(p => new MenuSuggestionPlanDto
                    {
                        Id = p.Id,
                        Rank = p.Rank,
                        PlanScore = p.PlanScore,
                        ObjectiveValue = p.ObjectiveValue,
                        Days = p.Days
                            .OrderBy(d => d.DayIndex)
                            .Select(d => new MenuSuggestionPlanDayDto
                            {
                                Id = d.Id,
                                DayIndex = d.DayIndex,
                                DayName = d.DayName,
                                Items = d.Items
                                    .Select(i =>
                                    {
                                        var dto = new MenuSuggestionPlanItemDto
                                        {
                                            Id = i.Id,
                                            SlotCategory = i.SlotCategory,
                                            DishName = i.DishName,
                                            DishSourceCategory = i.DishSourceCategory,
                                            Score = i.Score,
                                            CostPerServing = i.CostPerServing,
                                        };
                                        dto.SetReasonsFromJson(i.ReasonsJson);
                                        return dto;
                                    })
                                    .ToList(),
                            })
                            .ToList(),
                    })
                    .ToList(),
            }
        };
    }

}
