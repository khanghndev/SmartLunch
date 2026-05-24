using MediatR;
using Microsoft.Extensions.Configuration;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Dishes;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationMealOrders;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.Services;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Queries.OrganizationMealOrders.GetDishSuggestions;

public sealed class GetDishSuggestionsQueryHandler
    : IRequestHandler<GetDishSuggestionsQuery, GetDishSuggestionsResponse>
{
    private const int CandidatePoolLimit = 500;
    private const int FallbackPoolLimit  = 500;
    private const double ScoreBandEpsilon = 0.04;

    private readonly IDishRepository    _dishRepository;
    private readonly IStorageService    _storage;
    private readonly IConfiguration     _configuration;

    public GetDishSuggestionsQueryHandler(
        IDishRepository dishRepository,
        IStorageService storage,
        IConfiguration configuration)
    {
        _dishRepository = dishRepository;
        _storage        = storage;
        _configuration  = configuration;
    }

    public async Task<GetDishSuggestionsResponse> Handle(
        GetDishSuggestionsQuery request,
        CancellationToken cancellationToken)
    {
        if (request.DishId <= 0)
            throw new ArgumentException("DishId is required.");

        var anchor = await _dishRepository.GetByIdWithIngredientsAsync(request.DishId)
            ?? throw new KeyNotFoundException("Dish not found.");

        if (!anchor.IsActive)
            throw new KeyNotFoundException("Dish not found.");

        var anchorSlots   = DishPairingScorer.ResolveAnchorSlots(anchor);
        var anchorPrimary = DishPairingScorer.ResolveAnchorPrimary(anchorSlots);

        var targetSlots = DishPairingScorer.ResolveTargetSlots(anchorSlots);
        if (targetSlots.Count == 0)
            targetSlots = ["main", "soup", "side", "dessert"];

        var candidates = await _dishRepository.GetActiveDishesForSlotKeysAsync(
            targetSlots,
            excludeDishId: anchor.Id,
            limit: CandidatePoolLimit,
            cancellationToken);

        if (candidates.Count == 0)
        {
            candidates = await _dishRepository.GetActiveDishesForPairingPoolAsync(
                anchor.Id,
                FallbackPoolLimit,
                cancellationToken);
        }

        if (candidates.Count == 0)
        {
            return new GetDishSuggestionsResponse
            {
                AnchorDishId      = anchor.Id,
                AnchorPrimarySlotKey = anchorPrimary,
                Items             = [],
            };
        }

        // ── Bước quan trọng: Tính IDF từ toàn bộ corpus (anchor + candidates) ──
        // IDF(family) = log(1 + N / (1 + df))
        // Cho phép TF-IDF vector phân biệt được NVL phổ biến vs hiếm gặp.
        var corpus    = candidates.Prepend(anchor).ToList();
        var idfWeights = DishPairingScorer.ComputeIdfWeights(corpus);

        var candidateIds  = candidates.Select(c => c.Id).ToList();
        var popularity    = await _dishRepository.GetPopularityScoresAsync(candidateIds, lookbackDays: 28, cancellationToken);
        var coOccurrence  = await _dishRepository.GetMenuCoOccurrenceCountsAsync(
            anchor.Id,
            candidateIds,
            cancellationToken);

        var picked = PickBalancedMeal(
            anchor,
            candidates,
            targetSlots,
            popularity,
            coOccurrence,
            request.MaxItems,
            idfWeights);

        var items = new List<SuggestedDishItemDto>(picked.Count);
        foreach (var (dish, slot, _) in picked)
        {
            var dto = DishDtoMapping.ToDto(dish);
            dto = await WithSignedImageAsync(dto, cancellationToken);
            var pop = popularity.GetValueOrDefault(dish.Id, 3);
            var co  = coOccurrence.GetValueOrDefault(dish.Id, 0);

            items.Add(new SuggestedDishItemDto
            {
                Id             = dto.Id,
                Name           = dto.Name,
                Description    = dto.Description,
                ImageUrl       = dto.ImageUrl,
                PrimarySlotKey = dto.PrimarySlotKey ?? slot,
                CategoryLabel  = DishDtoMapping.FormatMealSlotNamesDisplay(dish),
                DietaryLabel   = dto.DietaryLabel,
                MatchReason    = DishPairingScorer.BuildMatchReason(anchor, slot, co, pop, dish),
            });
        }

        return new GetDishSuggestionsResponse
        {
            AnchorDishId         = anchor.Id,
            AnchorPrimarySlotKey = anchorPrimary,
            Items                = items,
        };
    }

    private static List<(Dish Dish, string TargetSlot, double Score)> PickBalancedMeal(
        Dish anchor,
        List<Dish> candidates,
        IReadOnlyList<string> targetSlots,
        Dictionary<int, int> popularity,
        Dictionary<int, int> coOccurrence,
        int maxItems,
        IReadOnlyDictionary<string, double> idfWeights)
    {
        var picked    = new List<(Dish Dish, string TargetSlot, double Score)>();
        var usedIds   = new HashSet<int> { anchor.Id };
        var mealSoFar = new List<Dish> { anchor };

        // Pass 1: Chọn 1 món tốt nhất cho mỗi target slot (có IDF weights)
        foreach (var slot in targetSlots)
        {
            if (picked.Count >= maxItems)
                break;

            var best = SelectBestForSlot(
                candidates, usedIds, slot, anchor, mealSoFar,
                popularity, coOccurrence, rejectUnacceptable: true, idfWeights);

            best ??= SelectBestForSlot(
                candidates, usedIds, slot, anchor, mealSoFar,
                popularity, coOccurrence, rejectUnacceptable: false, idfWeights);

            if (best == null)
                continue;

            var chosen = best.Value;
            picked.Add((chosen.Dish, slot, chosen.Score));
            usedIds.Add(chosen.Dish.Id);
            mealSoFar.Add(chosen.Dish);
        }

        // Pass 2: Điền thêm nếu chưa đủ maxItems
        if (picked.Count < maxItems)
        {
            var fillers = candidates
                .Where(c => !usedIds.Contains(c.Id))
                .Select(c =>
                {
                    var slot = ResolveBestSlotForCandidate(c, targetSlots);
                    var co   = coOccurrence.GetValueOrDefault(c.Id, 0);
                    return new
                    {
                        Dish  = c,
                        Slot  = slot,
                        Score = DishPairingScorer.ScoreCandidate(
                            anchor, c, slot,
                            popularity.GetValueOrDefault(c.Id, 3),
                            co, mealSoFar, idfWeights),
                        Co = co,
                    };
                })
                .Where(x => !DishPairingScorer.IsUnacceptablePairing(anchor, x.Dish, mealSoFar, x.Co, idfWeights))
                .OrderByDescending(x => x.Score)
                .ThenBy(x => DishPairingScorer.StablePairingRank(anchor.Id, x.Slot, x.Dish.Id))
                .Take(maxItems - picked.Count);

            foreach (var f in fillers)
            {
                picked.Add((f.Dish, f.Slot, f.Score));
                usedIds.Add(f.Dish.Id);
                mealSoFar.Add(f.Dish);
            }
        }

        return picked
            .OrderByDescending(p => p.Score)
            .ToList();
    }

    private static (Dish Dish, double Score)? SelectBestForSlot(
        List<Dish> candidates,
        HashSet<int> usedIds,
        string slot,
        Dish anchor,
        List<Dish> mealSoFar,
        Dictionary<int, int> popularity,
        Dictionary<int, int> coOccurrence,
        bool rejectUnacceptable,
        IReadOnlyDictionary<string, double> idfWeights)
    {
        var scored = candidates
            .Where(c => !usedIds.Contains(c.Id) && DishPairingScorer.CandidateMatchesSlot(c, slot))
            .Select(c =>
            {
                var co = coOccurrence.GetValueOrDefault(c.Id, 0);
                return new
                {
                    Dish = c,
                    Co = co,
                    Score = DishPairingScorer.ScoreCandidate(
                        anchor, c, slot,
                        popularity.GetValueOrDefault(c.Id, 3),
                        co, mealSoFar, idfWeights),
                };
            })
            .Where(x => !rejectUnacceptable ||
                        !DishPairingScorer.IsUnacceptablePairing(anchor, x.Dish, mealSoFar, x.Co, idfWeights))
            .OrderByDescending(x => x.Score)
            .ToList();

        if (scored.Count == 0)
            return null;

        var topScore = scored[0].Score;
        var band = scored
            .Where(x => x.Score >= topScore - ScoreBandEpsilon)
            .OrderByDescending(x => DishPairingScorer.ComputeNutritionRankScore(anchor, mealSoFar, x.Dish, slot))
            .ThenBy(x => DishPairingScorer.StablePairingRank(anchor.Id, slot, x.Dish.Id))
            .First();

        return (band.Dish, band.Score);
    }

    private static string ResolveBestSlotForCandidate(Dish candidate, IReadOnlyList<string> targetSlots)
    {
        var slots = DishAiEnglishCatalog.BuildSlotKeysFromJunction(candidate);
        var match = targetSlots.FirstOrDefault(t =>
            slots.Any(s => string.Equals(s, t, StringComparison.OrdinalIgnoreCase)));
        return match ?? DishPairingScorer.ResolveCandidatePrimarySlot(candidate);
    }

    private async Task<DishDto> WithSignedImageAsync(DishDto dto, CancellationToken cancellationToken)
    {
        var raw = dto.ImageUrl;
        if (string.IsNullOrWhiteSpace(raw))
            return dto;

        if (raw.StartsWith("http://",  StringComparison.OrdinalIgnoreCase) ||
            raw.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            return dto;

        var expiresMinutes = int.TryParse(_configuration["Media:DownloadUrlExpireMinutes"], out var m) ? m : 15;
        var expiresIn      = TimeSpan.FromMinutes(Math.Clamp(expiresMinutes, 1, 60));
        var signed         = await _storage.CreateSignedUrlAsync(
            raw.Trim(), HttpMethod.Get, contentType: null, expiresIn: expiresIn);

        var resolvedUrl = string.IsNullOrWhiteSpace(signed.Url) ? raw : signed.Url;
        return new DishDto
        {
            Id                  = dto.Id,
            Code                = dto.Code,
            Name                = dto.Name,
            NameEnglish         = dto.NameEnglish,
            Description         = dto.Description,
            PrimarySlotKey      = dto.PrimarySlotKey,
            DishSlotCategoryCodes = [.. dto.DishSlotCategoryCodes],
            CookingMethod       = dto.CookingMethod,
            Price               = dto.Price,
            DietaryLabel        = dto.DietaryLabel,
            ImageUrl            = resolvedUrl,
            Images              = dto.Images.ToList(),
            Calories            = dto.Calories,
            Protein             = dto.Protein,
            Fat                 = dto.Fat,
            Carbs               = dto.Carbs,
            IsActive            = dto.IsActive,
            CreatedAt           = dto.CreatedAt,
            UpdatedAt           = dto.UpdatedAt,
        };
    }
}
