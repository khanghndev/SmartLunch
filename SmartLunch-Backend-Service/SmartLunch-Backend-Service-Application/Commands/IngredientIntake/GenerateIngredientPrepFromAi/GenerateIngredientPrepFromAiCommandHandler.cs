using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Request.IngredientIntake;
using SmartLunch.Backend.Service.Application.DTOs.Response.IngredientIntake;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.Helpers;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.Mappings;
using SmartLunch.Backend.Service.Application.Models.AiMenuPlanner;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Domain.Time;
using MediatR;

namespace SmartLunch.Backend.Service.Application.Commands.IngredientIntake.GenerateIngredientPrepFromAi;

public sealed class GenerateIngredientPrepFromAiCommandHandler
    : IRequestHandler<GenerateIngredientPrepFromAiCommand, GenerateIngredientPrepFromAiResponse>
{
    private readonly IAiMenuPlannerClient _aiClient;
    private readonly IOrderRepository _orderRepository;
    private readonly IContractRepository _contractRepository;
    private readonly IDishRepository _dishRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IIngredientRepository _ingredientRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IIngredientIntakeProposalRepository _proposalRepository;
    private readonly ILogger<GenerateIngredientPrepFromAiCommandHandler> _logger;

    public GenerateIngredientPrepFromAiCommandHandler(
        IAiMenuPlannerClient aiClient,
        IOrderRepository orderRepository,
        IContractRepository contractRepository,
        IDishRepository dishRepository,
        IInventoryRepository inventoryRepository,
        IIngredientRepository ingredientRepository,
        IUserRoleRepository userRoleRepository,
        IIngredientIntakeProposalRepository proposalRepository,
        ILogger<GenerateIngredientPrepFromAiCommandHandler> logger)
    {
        _aiClient = aiClient;
        _orderRepository = orderRepository;
        _contractRepository = contractRepository;
        _dishRepository = dishRepository;
        _inventoryRepository = inventoryRepository;
        _ingredientRepository = ingredientRepository;
        _userRoleRepository = userRoleRepository;
        _proposalRepository = proposalRepository;
        _logger = logger;
    }

    public async Task<GenerateIngredientPrepFromAiResponse> Handle(
        GenerateIngredientPrepFromAiCommand command,
        CancellationToken cancellationToken)
    {
        var req = command.Request;
        if (req.Days < 1 || req.Days > 60)
            throw new ArgumentException("Days must be within 1..60.");

        var start = req.StartDate;
        var end = start.AddDays(req.Days - 1);

        // Permission: only roles who can create intake proposals can trigger AI planning
        var userRoles = await _userRoleRepository.GetActiveByUserIdAsync(command.ActorUserId);
        var roleNames = userRoles.Select(ur => ur.Role.Name).ToList();
        if (!IntakeProposalAccessHelper.CanCreateIntakeProposal(roleNames))
            throw new UnauthorizedAccessException("You are not allowed to generate ingredient prep suggestions.");

        var demand = await _orderRepository.GetUpcomingDishDemandAsync(start, end, cancellationToken);
        if (req.ContractId.HasValue)
            demand = demand.Where(x => x.ContractId == req.ContractId).ToList();

        var dishIds = demand.Select(d => d.DishId).Distinct().ToList();
        if (dishIds.Count == 0)
        {
            return new GenerateIngredientPrepFromAiResponse
            {
                Plan = new AiIndustrialIngredientPrepResponse
                {
                    StartDate = start.ToString("yyyy-MM-dd"),
                    Days = req.Days
                }
            };
        }

        // Load dishes with all DishIngredients & Ingredient refs.
        var dishes = await _dishRepository.GetByIdsWithIngredientsAsync(dishIds, cancellationToken);

        // Resolve contract -> DishValueId (tier) when possible
        var contractDishValueMap = new Dictionary<int, int?>();
        var contractIds = demand.Select(d => d.ContractId).Where(id => id.HasValue).Select(id => id!.Value).Distinct().ToList();
        foreach (var cid in contractIds)
        {
            var c = await _contractRepository.GetByIdAsync(cid);
            contractDishValueMap[cid] = c?.DishValueId;
        }

        // Build dish BOMs
        var dishBoms = dishes.Select(d =>
        {
            int? dishValueId = req.DishValueIdOverride;
            if (!dishValueId.HasValue)
            {
                // Prefer contract tier if demand has a single contract for this dish; else fallback.
                var cids = demand.Where(x => x.DishId == d.Id).Select(x => x.ContractId).Distinct().ToList();
                if (cids.Count == 1 && cids[0].HasValue)
                    dishValueId = contractDishValueMap.GetValueOrDefault(cids[0]!.Value);
            }

            var diLines = d.DishIngredients ?? new List<DishIngredient>();
            var eligible = dishValueId.HasValue
                ? diLines.Where(di => di.DishValueId == dishValueId.Value).ToList()
                : diLines
                    .GroupBy(di => di.DishValueId)
                    .OrderBy(g => g.Key) // smallest tier id
                    .FirstOrDefault()?.ToList()
                    ?? new List<DishIngredient>();

            var ingLines = eligible
                .Where(di => di.Ingredient != null)
                .Select(di => new AiIngredientBomLine
                {
                    IngredientName = string.IsNullOrWhiteSpace(di.Ingredient!.NameEnglish) ? di.Ingredient.Name : di.Ingredient.NameEnglish!,
                    // Assumption: dish_ingredients.Quantity is in KG per serving (per schema & current usage).
                    QuantityKgPerMeal = di.Quantity,
                    CostPerKg = di.Ingredient.CostPerUnit
                })
                .ToList();

            return new AiDishBom
            {
                DishId = d.Id,
                DishName = d.Name,
                Ingredients = ingLines
            };
        }).ToList();

        // Build order items list for AI
        var orderItems = demand.Select(x => new AiUpcomingOrderItem
        {
            ServiceDate = x.ServiceDate.ToString("yyyy-MM-dd"),
            DishId = x.DishId,
            QuantityMeals = x.QuantityMeals
        }).ToList();

        // Inventory for involved ingredients (optional but improves plan); only load ingredients present in BOM.
        var ingredientIds = dishes
            .SelectMany(d => d.DishIngredients.Select(di => di.IngredientId))
            .Distinct()
            .ToList();
        var inventories = await _inventoryRepository.GetByIngredientIdsAsync(ingredientIds, cancellationToken);
        var inventoryMap = inventories.ToDictionary(
            inv => string.IsNullOrWhiteSpace(inv.Ingredient.NameEnglish) ? inv.Ingredient.Name : inv.Ingredient.NameEnglish,
            inv => (double)inv.QuantityAvailable);
        var availableIngredients = inventoryMap
            .Where(kv => kv.Value > 0)
            .Select(kv => new AiAvailableIngredient { Name = kv.Key, QuantityKg = (decimal)kv.Value })
            .ToList();

        var aiReq = new AiIndustrialIngredientPrepRequest
        {
            StartDate = start.ToString("yyyy-MM-dd"),
            Days = req.Days,
            OrderItems = orderItems,
            DishBoms = dishBoms,
            AvailableIngredients = availableIngredients,
            Constraints = new AiIngredientPrepConstraints
            {
                LeadTimeDays = req.LeadTimeDays,
                SafetyStockKg = req.SafetyStockKg,
                MaxInventoryKg = req.MaxInventoryKg,
            }
        };

        _logger.LogInformation("Calling AI ingredient prep with {DishCount} dishes, {OrderItemCount} order rows, horizon {Days} days",
            dishBoms.Count, orderItems.Count, req.Days);

        var plan = await _aiClient.RecommendIndustrialIngredientPreparationAsync(aiReq, cancellationToken);

        var allIngredients = await _ingredientRepository.GetAllWithCategoryAsync(cancellationToken);
        var activeIngredients = allIngredients.Where(i => i.IsActive).ToList();
        var nameToId = activeIngredients
            .Select(i => new
            {
                Id = i.Id,
                Key = (i.NameEnglish ?? i.Name).Trim().ToLowerInvariant()
            })
            .GroupBy(x => x.Key)
            .ToDictionary(g => g.Key, g => g.First().Id);
        var englishKeyToVietnameseName = activeIngredients
            .Select(i => new
            {
                Key = (i.NameEnglish ?? i.Name).Trim().ToLowerInvariant(),
                DisplayName = i.Name
            })
            .GroupBy(x => x.Key)
            .ToDictionary(g => g.Key, g => g.First().DisplayName);

        IngredientIntakeProposalDetailDto? proposalDto = null;
        if (req.PersistAsProposal)
        {
            // Map AI total_buy_kg -> IngredientId, then persist as intake proposal

            var lines = new List<IngredientIntakeProposalLine>();
            foreach (var ing in plan.Ingredients)
            {
                var key = (ing.IngredientName ?? "").Trim().ToLowerInvariant();
                if (string.IsNullOrWhiteSpace(key))
                    continue;

                if (!nameToId.TryGetValue(key, out var ingId))
                    continue;

                if (ing.TotalBuyKg <= 0)
                    continue;

                lines.Add(new IngredientIntakeProposalLine
                {
                    IngredientId = ingId,
                    Quantity = ing.TotalBuyKg,
                    LineNote = $"AI plan {plan.StartDate} (+{plan.Days}d)"
                });
            }

            if (lines.Count > 0)
            {
                var headerNote = string.IsNullOrWhiteSpace(req.ProposalHeaderNote)
                    ? $"AI ingredient prep plan from {plan.StartDate} ({plan.Days} days)"
                    : req.ProposalHeaderNote.Trim();

                var proposal = new IngredientIntakeProposal
                {
                    ProposalCode = $"DXN-AI-{VietnamTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}",
                    Status = IntakeProposalStatus.Submitted,
                    HeaderNote = headerNote.Length > 500 ? headerNote[..500] : headerNote,
                    CreatedByUserId = command.ActorUserId,
                    CreatedAt = VietnamTime.Now,
                    Lines = lines
                };

                await _proposalRepository.CreateAsync(proposal, cancellationToken);
                var reloaded = await _proposalRepository.GetByIdWithDetailsAsync(proposal.Id, cancellationToken);
                proposalDto = reloaded == null ? null : IngredientIntakeProposalMapping.ToDetail(reloaded);
            }
        }

        foreach (var ing in plan.Ingredients)
        {
            var key = (ing.IngredientName ?? "").Trim().ToLowerInvariant();
            if (!string.IsNullOrWhiteSpace(key) && englishKeyToVietnameseName.TryGetValue(key, out var vnName))
                ing.IngredientName = vnName;
        }

        return new GenerateIngredientPrepFromAiResponse
        {
            Plan = plan,
            Proposal = proposalDto
        };
    }
}

