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

        // Align with rules.json profile keys (lowercase), e.g. Org_elementary → org_elementary
        var rulesKeyNormalized = string.IsNullOrWhiteSpace(req.RulesKey)
            ? "industrial"
            : req.RulesKey.Trim().ToLowerInvariant();

        var weekStartUtc = DateTime.SpecifyKind(req.WeekStartUtc.Date, DateTimeKind.Utc);
        var nextVersion = await _menuSuggestionRepository.GetNextVersionAsync(weekStartUtc, createdBy);

        var requestedDishIds = req.DishIds.Distinct().ToList();

        // ── 1. Fetch dishes with ingredients from DB ──────────────────────
        var dishes = await _dishRepository.GetByIdsWithIngredientsAsync(requestedDishIds, cancellationToken);
        if (dishes.Count == 0)
            throw new ArgumentException("No active dishes found for the provided DishIds.");

        var foundIds = dishes.Select(d => d.Id).ToHashSet();
        var missingDishIds = requestedDishIds.Where(id => !foundIds.Contains(id)).ToList();
        if (missingDishIds.Count > 0)
            throw new ArgumentException(
                $"Không tìm thấy món hoạt động cho id: {string.Join(", ", missingDishIds)}. Kiểm tra DishIds hoặc trạng thái IsActive.");

        var mealStructureSlots = MealStructureEnglishNormalizer.NormalizeOrThrow(req.MealStructure);

        var missingSlots = dishes
            .Where(d => DishAiEnglishCatalog.BuildSlotKeysFromJunction(d).Count == 0)
            .Select(d => d.Name)
            .ToList();
        if (missingSlots.Count > 0)
            throw new ArgumentException(
                $"Các món sau chưa gán danh mục slot (dish_dish_categories / dish_categories.SlotKey): {string.Join(", ", missingSlots)}. " +
                "Gán ít nhất một slot (main, soup, vegetable, side, noodle_soup, dessert) cho mỗi món.");

        var missingMethod = dishes
            .Where(d => d.CookingMethod == null || string.IsNullOrWhiteSpace(d.CookingMethod.MethodKey))
            .Select(d => d.Name)
            .ToList();
        if (missingMethod.Count > 0)
            throw new ArgumentException(
                $"Các món sau chưa có cooking method (cooking_methods.MethodKey): {string.Join(", ", missingMethod)}.");

        var poolSlots = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var d in dishes)
        {
            foreach (var s in DishAiEnglishCatalog.BuildSlotKeysFromJunction(d))
                poolSlots.Add(s);
        }

        var uncoveredSlots = mealStructureSlots.Where(s => !poolSlots.Contains(s)).ToList();
        if (uncoveredSlots.Count > 0)
            throw new ArgumentException(
                "Mỗi thành phần trong cấu trúc bữa phải có ít nhất một món trong kho được gán category tương ứng (junction dish_categories). " +
                $"Thiếu món cho slot: {string.Join(", ", uncoveredSlots)}. Hãy thêm món hoặc bỏ slot khỏi cấu trúc bữa.");

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

        var aiReq = new AiIndustrialMenuPlansRequest
        {
            BudgetPerServing = req.BudgetPerServing,
            Days = req.Days,
            MealStructure = mealStructureSlots,
            TopK = req.TopK,
            TimeLimitSeconds = req.TimeLimitSeconds,
            RulesKey = rulesKeyNormalized,
            Dishes = aiDishes,
            AvailableIngredients = availableIngredients,
            IngredientGroups = ingredientGroups
        };

        _logger.LogInformation(
            "Calling AI with {DishCount} dishes, {IngCount} available ingredients, {DayCount} days, slots: {Slots}",
            aiDishes.Count, availableIngredients.Count, req.Days.Count, string.Join(",", mealStructureSlots));

        var aiRes = await _aiClient.RecommendIndustrialMenusAsync(aiReq, cancellationToken);

        // ── 7. Persist ────────────────────────────────────────────────────
        var entity = new MenuSuggestion
        {
            WeekStart = weekStartUtc,
            GeneratedAt = VietnamTime.Now,
            Version = nextVersion,
            RulesKey = rulesKeyNormalized,
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
