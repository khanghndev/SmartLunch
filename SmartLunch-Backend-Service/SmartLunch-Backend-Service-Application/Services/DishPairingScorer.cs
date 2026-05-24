using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Services;

/// <summary>
/// Gợi ý món đi kèm — Hybrid Recommendation System.
///
/// Thuật toán kết hợp 4 tín hiệu:
///   (1) TF-IDF Content-Based Filtering  — ingredient vector cosine similarity (đo độ ĐA DẠNG nguyên liệu)
///   (2) Popularity-weighted PMI Co-occurrence  — Collaborative Filtering từ lịch sử thực đơn
///   (3) Nutritional Balance Score  — bù thiếu hụt macro-nutrient của anchor
///   (4) Slot Complementarity  — đảm bảo cấu trúc bữa ăn hợp lý (main/soup/side/vegetable/dessert)
///
/// Trọng số mặc định (có thể tune):
///   w_content    = 0.35
///   w_collab     = 0.30
///   w_nutrition  = 0.20
///   w_popularity = 0.15
/// </summary>
public static class DishPairingScorer
{
    // ─── Trọng số hybrid (dinh dưỡng là tín hiệu chính) ─────────────────────────
    private const double W_Nutrition        = 0.45; // Cân bằng macro bữa ăn theo anchor
    private const double W_AnchorComplement = 0.18; // Khác NVL / không lặp protein chủ đạo
    private const double W_Content          = 0.15; // Đa dạng trong bữa
    private const double W_Collab           = 0.17; // Co-occurrence thực đơn
    private const double W_Popularity       = 0.05;

    // Nutritional targets cho một bữa ăn cân bằng (kcal / g)
    private const double TargetCalories = 600.0;
    private const double TargetProtein  = 25.0;
    private const double TargetFat      = 15.0;
    private const double TargetCarbs    = 60.0;

    // ─── Cấu trúc bữa ăn (slot complementarity) ─────────────────────────────────
    private static readonly Dictionary<string, string[]> ComplementarySlots = new(StringComparer.OrdinalIgnoreCase)
    {
        ["main"]       = ["soup", "vegetable", "side", "dessert"],
        ["noodle_soup"] = ["side", "vegetable", "dessert"],
        ["soup"]       = ["main", "vegetable", "side"],
        ["side"]       = ["main", "soup", "vegetable"],
        ["vegetable"]  = ["main", "soup", "side", "dessert"],
        ["dessert"]    = ["main", "soup", "side"],
    };

    private static readonly string[] DefaultComplement = ["main", "soup", "vegetable", "side"];

    private static readonly Dictionary<string, string> SlotReasonVi = new(StringComparer.OrdinalIgnoreCase)
    {
        ["main"]       = "Bổ sung món mặn / protein",
        ["soup"]       = "Canh đi kèm cân bằng",
        ["vegetable"]  = "Thêm món xanh",
        ["side"]       = "Món phụ hợp khẩu vị",
        ["dessert"]    = "Tráng miệng nhẹ",
        ["noodle_soup"] = "Món nước bổ sung",
    };

    // ─── IDF corpus ─────────────────────────────────────────────────────────────
    private static readonly string[] IngredientFamilyRoots =
    [
        "bí đỏ", "cà rốt", "khoai tây", "khoai lang", "cải ngọt", "rau muống", "mồng tơi", "cà chua",
        "dưa leo", "dưa hấu", "bắp cải", "súp lơ", "đậu cove", "đậu hũ", "nấm",
        "tôm", "cua", "mực", "cá basa", "cá lóc", "cá", "thịt heo", "thịt bò", "gà", "thịt bằm",
        "trứng", "gạo", "bún", "phở", "mì",
    ];

    private static readonly HashSet<string> MinorSeasoningFamilies = new(StringComparer.OrdinalIgnoreCase)
    {
        "tỏi", "hành", "hành tím", "hành tây", "gừng", "ớt", "tiêu", "muối", "đường", "nước mắm",
    };

    private static readonly string[] ProteinKeywords =
        ["thịt", "gà", "cá", "tôm", "bò", "heo", "sườn", "basa", "lóc", "bằm", "cua"];

    // ════════════════════════════════════════════════════════════════════════════
    // Public API (giữ nguyên signature để QueryHandler không cần sửa nhiều)
    // ════════════════════════════════════════════════════════════════════════════

    public static List<string> ResolveAnchorSlots(Dish anchor)
    {
        var fromDb = DishAiEnglishCatalog.BuildSlotKeysFromJunction(anchor);
        return fromDb.Count > 0 ? fromDb : InferSlotsFromMetadata(anchor);
    }

    public static string ResolveAnchorPrimary(IReadOnlyList<string> anchorSlots)
    {
        if (anchorSlots.Count > 0)
            return DishAiEnglishCatalog.PickPrimaryCategoryForAi(anchorSlots);
        return "main";
    }

    public static IReadOnlyList<string> ResolveTargetSlots(IReadOnlyList<string> anchorSlots)
    {
        var anchorPrimary = anchorSlots.Count > 0
            ? DishAiEnglishCatalog.PickPrimaryCategoryForAi(anchorSlots)
            : "main";

        var targets = ComplementarySlots.TryGetValue(anchorPrimary, out var list)
            ? list
            : DefaultComplement;

        var owned = new HashSet<string>(anchorSlots, StringComparer.OrdinalIgnoreCase);
        return targets
            .Where(s => !owned.Contains(s))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public static string ResolveCandidatePrimarySlot(Dish candidate)
    {
        var slots = DishAiEnglishCatalog.BuildSlotKeysFromJunction(candidate);
        if (slots.Count > 0)
            return DishAiEnglishCatalog.PickPrimaryCategoryForAi(slots);
        return InferSlotsFromMetadata(candidate).FirstOrDefault() ?? "main";
    }

    public static bool CandidateMatchesSlot(Dish candidate, string targetSlot)
    {
        var slots = DishAiEnglishCatalog.BuildSlotKeysFromJunction(candidate);
        if (slots.Any(s => string.Equals(s, targetSlot, StringComparison.OrdinalIgnoreCase)))
            return true;
        return string.Equals(ResolveCandidatePrimarySlot(candidate), targetSlot, StringComparison.OrdinalIgnoreCase);
    }

    // ════════════════════════════════════════════════════════════════════════════
    // CORE: Hybrid Scoring
    // ════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Tính điểm hybrid cho một candidate.
    ///
    /// Sử dụng:
    ///   - TF-IDF cosine diversity (ingredient vector)
    ///   - PMI-weighted co-occurrence (collaborative)
    ///   - Nutritional balance (macro-nutrient deficit fill)
    ///   - Popularity signal (1–5 scaled)
    ///   - Slot complementarity bonus
    /// </summary>
    public static double ScoreCandidate(
        Dish anchor,
        Dish candidate,
        string targetSlot,
        int popularity,
        int coOccurrenceCount,
        IReadOnlyList<Dish> mealSoFar,
        IReadOnlyDictionary<string, double>? idfWeights = null)
    {
        // (0) Điểm bổ sung theo MÓN GỐC — tách biệt kết quả giữa các anchor
        var anchorComplementScore = ComputeAnchorComplementScore(anchor, candidate, targetSlot, idfWeights);

        // (1) Đa dạng so với cả bữa (anchor + món đã chọn)
        var contentDiversityScore = ComputeContentDiversityScore(mealSoFar, candidate, idfWeights);

        // (2) Co-occurrence với anchor trên thực đơn
        var collabScore = ComputePmiCollabScore(coOccurrenceCount, popularity);

        // (3) Dinh dưỡng — điểm chính, phụ thuộc anchor + bữa hiện tại
        var mealBalanceScore = ComputeMealBalanceFitScore(anchor, mealSoFar, candidate, targetSlot);
        var deficitFillScore = ComputeNutritionalBalanceScore(anchor, mealSoFar, candidate, targetSlot);
        var nutritionScore = mealBalanceScore * 0.72 + deficitFillScore * 0.28;

        // (4) Popularity (trọng số thấp)
        var popScore = Math.Clamp((popularity - 1) / 4.0, 0.0, 1.0);

        var hybridScore = W_Nutrition * nutritionScore
                        + W_AnchorComplement * anchorComplementScore
                        + W_Content * contentDiversityScore
                        + W_Collab * collabScore
                        + W_Popularity * popScore;

        if (CandidateMatchesSlot(candidate, targetSlot))
            hybridScore += 0.08;

        if (!string.IsNullOrWhiteSpace(anchor.DietaryLabel) &&
            string.Equals(anchor.DietaryLabel, candidate.DietaryLabel, StringComparison.OrdinalIgnoreCase))
            hybridScore += 0.04;

        var anchorMethod = anchor.CookingMethod?.MethodKey ?? "";
        var candMethod = candidate.CookingMethod?.MethodKey ?? "";
        if (!string.IsNullOrEmpty(candMethod) &&
            !string.Equals(anchorMethod, candMethod, StringComparison.OrdinalIgnoreCase))
            hybridScore += 0.03;

        hybridScore -= ComputeAnchorProteinOverlapPenalty(anchor, candidate, idfWeights);

        return Math.Clamp(hybridScore, 0.0, 1.25);
    }

    /// <summary>Điểm dinh dưỡng thuần (dùng tie-break trong dải điểm gần nhau).</summary>
    public static double ComputeNutritionRankScore(
        Dish anchor,
        IReadOnlyList<Dish> mealSoFar,
        Dish candidate,
        string targetSlot) =>
        ComputeMealBalanceFitScore(anchor, mealSoFar, candidate, targetSlot) * 0.72
        + ComputeNutritionalBalanceScore(anchor, mealSoFar, candidate, targetSlot) * 0.28;

    /// <summary>Thứ tự ổn định theo anchor — không dùng tên món (tránh luôn chọn A→Z).</summary>
    public static int StablePairingRank(int anchorId, string slotKey, int candidateId)
    {
        unchecked
        {
            var h = anchorId;
            h = h * 31 + slotKey.GetHashCode(StringComparison.OrdinalIgnoreCase);
            h = h * 31 + candidateId;
            return h & int.MaxValue;
        }
    }

    /// <summary>
    /// Kiểm tra cặp ghép không chấp nhận được:
    /// Dùng TF-IDF cosine similarity thay vì rule cứng.
    /// Chỉ từ chối nếu similarity > 0.75 VÀ không có lịch sử co-occurrence.
    /// </summary>
    public static bool IsUnacceptablePairing(
        Dish anchor,
        Dish candidate,
        IReadOnlyList<Dish> mealSoFar,
        int coOccurrenceCount,
        IReadOnlyDictionary<string, double>? idfWeights = null)
    {
        if (coOccurrenceCount >= 2)
            return false;

        var anchorVec = BuildTfIdfVector(anchor, idfWeights);
        var candVec = BuildTfIdfVector(candidate, idfWeights);
        var anchorSim = CosineSimilarity(anchorVec, candVec);

        // Trùng NVL chủ đạo với món gốc (vd: gà + món gà khác) — trừ khi đã hay ghép thực đơn
        if (anchorSim > 0.72 && coOccurrenceCount < 1)
            return true;

        var combinedVec = BuildCombinedTfIdfVector(mealSoFar, idfWeights);
        var mealSim = CosineSimilarity(combinedVec, candVec);

        return mealSim > 0.78;
    }

    public static string BuildMatchReason(
        Dish anchor,
        string targetSlot,
        int coOccurrenceCount,
        int popularity,
        Dish candidate)
    {
        var baseReason = SlotReasonVi.TryGetValue(targetSlot, out var vi)
            ? vi
            : "Phù hợp bữa ăn trọn vẹn";

        var anchorPro = (double)(anchor.Protein ?? 0);
        var anchorFat = (double)(anchor.Fat ?? 0);
        var anchorCal = (double)(anchor.Calories ?? 0);
        var candPro = (double)(candidate.Protein ?? 0);
        var candFat = (double)(candidate.Fat ?? 0);
        var candCal = (double)(candidate.Calories ?? 0);

        if (coOccurrenceCount >= 3)
            return $"{baseReason} · thường phục vụ cùng nhau";
        if (coOccurrenceCount >= 1)
            return $"{baseReason} · đã được ghép trên thực đơn";

        if (string.Equals(targetSlot, "vegetable", StringComparison.OrdinalIgnoreCase))
        {
            if (anchorFat > 14 && candFat is > 0 and < 10)
                return "Bổ sung rau xanh · giảm chất béo bữa ăn";
            if (anchorPro > 22)
                return "Thêm món xanh · cân bằng đạm";
            return "Thêm món xanh · bổ sung vi chất";
        }

        if (string.Equals(targetSlot, "soup", StringComparison.OrdinalIgnoreCase))
        {
            if (anchorCal > 480 || anchorFat > 15)
                return "Canh thanh · cân bằng năng lượng";
            if (anchorPro > 22 && candPro < 12)
                return "Canh đi kèm · bổ sung nước, ít đạm";
            return baseReason;
        }

        if (string.Equals(targetSlot, "side", StringComparison.OrdinalIgnoreCase))
        {
            if (anchorPro > 24 && candPro < 10)
                return "Món phụ nhẹ · không tăng đạm";
            if (anchorPro > 24 && candPro > 14)
                return "Món phụ · cân nhắc khẩu phần đạm";
            return baseReason;
        }

        if (string.Equals(targetSlot, "dessert", StringComparison.OrdinalIgnoreCase))
        {
            if (anchorCal > 420 && candCal is > 0 and < 220)
                return "Tráng miệng nhẹ · hợp bữa nhiều năng lượng";
            return baseReason;
        }

        if (string.Equals(targetSlot, "main", StringComparison.OrdinalIgnoreCase) && HasProteinSignal(candidate))
            return $"{baseReason} · giàu đạm";

        if (popularity >= 4)
            return $"{baseReason} · được đặt nhiều";

        return baseReason;
    }

    // ════════════════════════════════════════════════════════════════════════════
    // (1) TF-IDF CONTENT-BASED FILTERING
    // ════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Xây dựng TF-IDF vector cho một món ăn.
    /// Chiều = mỗi ingredient family root (trong IngredientFamilyRoots).
    /// TF  = quantity_ingredient / total_quantity  (hoặc 0.35 nếu không có quantity)
    /// IDF = log(1 + N / (1 + df))  — smoothed IDF
    /// </summary>
    public static Dictionary<string, double> BuildTfIdfVector(
        Dish dish,
        IReadOnlyDictionary<string, double>? idfWeights)
    {
        var vector = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
        var ingredients = dish.DishIngredients ?? [];
        var totalQty = (double)ingredients.Sum(x => x.Quantity);

        foreach (var di in ingredients)
        {
            var name = di.Ingredient?.Name?.Trim();
            if (string.IsNullOrWhiteSpace(name))
                continue;

            foreach (var family in ExtractFamilyRootsFromText(name))
            {
                if (MinorSeasoningFamilies.Contains(family))
                    continue;

                var tf = totalQty > 0 ? (double)di.Quantity / totalQty : 0.35;
                var idf = idfWeights != null && idfWeights.TryGetValue(family, out var w) ? w : 1.0;
                var tfidf = tf * idf;
                UpsertMax(vector, family, tfidf);
            }
        }

        // Thêm tín hiệu từ tên món (name-based features)
        foreach (var family in ExtractFamilyRootsFromText(dish.Name))
        {
            if (MinorSeasoningFamilies.Contains(family))
                continue;
            var prominence = GetNameFamilyProminence(dish.Name, family);
            var idf = idfWeights != null && idfWeights.TryGetValue(family, out var w) ? w : 1.0;
            UpsertMax(vector, family, prominence * idf * 0.7); // name signal nhẹ hơn ingredient
        }

        return vector;
    }

    /// <summary>
    /// Tính IDF weights từ một corpus của món ăn (candidates).
    /// IDF(family) = log(1 + N / (1 + df))
    /// </summary>
    public static Dictionary<string, double> ComputeIdfWeights(IReadOnlyList<Dish> corpus)
    {
        var n = corpus.Count;
        if (n == 0)
            return new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

        // Đếm document frequency
        var df = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var dish in corpus)
        {
            var families = GetDishFamilies(dish);
            foreach (var fam in families)
            {
                df[fam] = df.GetValueOrDefault(fam, 0) + 1;
            }
        }

        // Tính smoothed IDF
        var idf = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
        foreach (var (family, count) in df)
        {
            idf[family] = Math.Log(1.0 + (double)n / (1.0 + count));
        }

        return idf;
    }

    private static HashSet<string> GetDishFamilies(Dish dish)
    {
        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var ingredients = dish.DishIngredients ?? [];
        foreach (var di in ingredients)
        {
            var name = di.Ingredient?.Name?.Trim();
            if (!string.IsNullOrWhiteSpace(name))
                foreach (var f in ExtractFamilyRootsFromText(name))
                    result.Add(f);
        }
        foreach (var f in ExtractFamilyRootsFromText(dish.Name))
            result.Add(f);
        return result;
    }

    private static Dictionary<string, double> BuildCombinedTfIdfVector(
        IReadOnlyList<Dish> dishes,
        IReadOnlyDictionary<string, double>? idfWeights)
    {
        var combined = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
        foreach (var dish in dishes)
        {
            var vec = BuildTfIdfVector(dish, idfWeights);
            foreach (var (key, val) in vec)
                UpsertMax(combined, key, val);
        }
        return combined;
    }

    private static double CosineSimilarity(
        Dictionary<string, double> vecA,
        Dictionary<string, double> vecB)
    {
        if (vecA.Count == 0 || vecB.Count == 0)
            return 0.0;

        double dot  = 0.0;
        double normA = 0.0;
        double normB = 0.0;

        foreach (var (key, valA) in vecA)
        {
            normA += valA * valA;
            if (vecB.TryGetValue(key, out var valB))
                dot += valA * valB;
        }
        foreach (var valB in vecB.Values)
            normB += valB * valB;

        if (normA <= 0 || normB <= 0)
            return 0.0;

        return dot / (Math.Sqrt(normA) * Math.Sqrt(normB));
    }

    /// <summary>
    /// Tính Content Diversity Score = 1 - cosine_similarity(mealSoFar, candidate).
    /// Giá trị cao → candidate có NVL ĐA DẠNG so với bữa hiện tại → tốt.
    /// </summary>
    private static double ComputeContentDiversityScore(
        IReadOnlyList<Dish> mealSoFar,
        Dish candidate,
        IReadOnlyDictionary<string, double>? idfWeights)
    {
        if (mealSoFar.Count == 0)
            return 0.5;

        var mealVec = BuildCombinedTfIdfVector(mealSoFar, idfWeights);
        var candVec = BuildTfIdfVector(candidate, idfWeights);
        var sim = CosineSimilarity(mealVec, candVec);

        return Math.Clamp(1.0 - sim, 0.0, 1.0);
    }

    /// <summary>
    /// Điểm bổ sung cho món gốc: khác NVL nhưng vẫn hợp khẩu vị (cosine 0.08–0.42 là tối ưu).
    /// </summary>
    private static double ComputeAnchorComplementScore(
        Dish anchor,
        Dish candidate,
        string targetSlot,
        IReadOnlyDictionary<string, double>? idfWeights)
    {
        var anchorVec = BuildTfIdfVector(anchor, idfWeights);
        var candVec = BuildTfIdfVector(candidate, idfWeights);

        if (anchorVec.Count == 0 && candVec.Count == 0)
            return ComputeSlotNutritionHeuristic(anchor, targetSlot) * 0.5;

        var sim = CosineSimilarity(anchorVec, candVec);

        // Quá giống món gốc
        if (sim > 0.68)
            return 0.05;

        // Vùng bổ sung lý tưởng
        if (sim is >= 0.06 and <= 0.38)
            return 0.95;

        if (sim < 0.06)
        {
            // Rất khác — tốt cho side/dessert, hơi lạ cho soup/main
            return targetSlot.ToLowerInvariant() switch
            {
                "side" or "dessert" or "vegetable" => 0.88,
                "soup" => 0.72,
                _ => 0.65,
            };
        }

        // sim 0.38–0.68: vẫn chấp nhận nhưng không tối ưu
        return 0.45;
    }

    /// <summary>Phạt món ứng viên lặp protein / họ NVL chủ đạo của anchor.</summary>
    private static double ComputeAnchorProteinOverlapPenalty(
        Dish anchor,
        Dish candidate,
        IReadOnlyDictionary<string, double>? idfWeights)
    {
        var anchorFamilies = GetDominantProteinFamilies(anchor, idfWeights);
        if (anchorFamilies.Count == 0)
            return 0.0;

        var candVec = BuildTfIdfVector(candidate, idfWeights);
        var penalty = 0.0;

        foreach (var family in anchorFamilies)
        {
            if (!candVec.TryGetValue(family, out var weight) || weight < 0.25)
                continue;

            penalty += weight >= 0.55 ? 0.22 : 0.12;
        }

        return Math.Min(penalty, 0.35);
    }

    private static HashSet<string> GetDominantProteinFamilies(
        Dish dish,
        IReadOnlyDictionary<string, double>? idfWeights)
    {
        var vec = BuildTfIdfVector(dish, idfWeights);
        var proteins = new[] { "gà", "thịt heo", "thịt bò", "thịt bằm", "cá", "cá basa", "cá lóc", "tôm", "cua", "mực", "trứng" };
        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var p in proteins)
        {
            if (vec.TryGetValue(p, out var w) && w >= 0.22)
                result.Add(p);
        }

        if (result.Count == 0 && HasProteinSignal(dish))
        {
            var text = (dish.Name + " " + (dish.Description ?? "")).ToLowerInvariant();
            foreach (var p in proteins)
            {
                if (text.Contains(p, StringComparison.OrdinalIgnoreCase))
                    result.Add(p);
            }
        }

        return result;
    }

    // ════════════════════════════════════════════════════════════════════════════
    // (2) PMI-WEIGHTED CO-OCCURRENCE (COLLABORATIVE FILTERING)
    // ════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Tính Collaborative Score dựa trên co-occurrence và popularity.
    /// Dùng Sigmoid(coOccurrence × popularity_factor) để đưa về [0, 1].
    ///
    /// Ý tưởng PMI: món nào hay được phục vụ cùng anchor → điểm cao hơn.
    /// </summary>
    private static double ComputePmiCollabScore(int coOccurrenceCount, int popularity)
    {
        if (coOccurrenceCount <= 0)
            return 0.0;

        // PMI-like signal: coOccurrence × log(1 + popularity)
        var rawSignal = coOccurrenceCount * Math.Log(1.0 + popularity);

        // Sigmoid để đưa về [0, 1]: σ(x) = 1/(1+e^(-kx))
        // k = 0.5 → score bão hòa khi rawSignal ≈ 6 (coOcc=3, pop=5)
        const double k = 0.5;
        return 1.0 / (1.0 + Math.Exp(-k * rawSignal));
    }

    // ════════════════════════════════════════════════════════════════════════════
    // (3) NUTRITIONAL BALANCE SCORING
    // ════════════════════════════════════════════════════════════════════════════

    private readonly record struct MacroProfile(double Calories, double Protein, double Fat, double Carbs);

    private static MacroProfile SumMacros(IEnumerable<Dish> dishes) =>
        new(
            dishes.Sum(d => (double)(d.Calories ?? 0)),
            dishes.Sum(d => (double)(d.Protein ?? 0)),
            dishes.Sum(d => (double)(d.Fat ?? 0)),
            dishes.Sum(d => (double)(d.Carbs ?? 0)));

    private static MacroProfile BuildIdealMealProfile(Dish anchor)
    {
        var a = SumMacros(new[] { anchor });
        if (a.Calories <= 0 && a.Protein <= 0)
        {
            return new MacroProfile(TargetCalories, TargetProtein, TargetFat, TargetCarbs);
        }

        return new MacroProfile(
            Math.Clamp(a.Calories > 0 ? a.Calories * 1.32 : TargetCalories, 620, 980),
            Math.Clamp(a.Protein > 0 ? a.Protein * 1.10 : TargetProtein, 30, 65),
            Math.Clamp(a.Fat > 0 ? a.Fat * 0.92 : TargetFat, 10, 32),
            Math.Clamp(a.Carbs > 0 ? a.Carbs * 1.15 : TargetCarbs, 45, 110));
    }

    private static double MacroDistance(MacroProfile actual, MacroProfile target)
    {
        static double Norm(double v, double t) => Math.Abs(v - t) / Math.Max(t, 1.0);

        return 0.25 * Norm(actual.Calories, target.Calories)
             + 0.30 * Norm(actual.Protein, target.Protein)
             + 0.20 * Norm(actual.Fat, target.Fat)
             + 0.25 * Norm(actual.Carbs, target.Carbs);
    }

    /// <summary>
    /// Điểm cải thiện cân bằng dinh dưỡng khi thêm candidate vào bữa (so với profile lý tưởng của anchor).
    /// </summary>
    private static double ComputeMealBalanceFitScore(
        Dish anchor,
        IReadOnlyList<Dish> mealSoFar,
        Dish candidate,
        string targetSlot)
    {
        var ideal = BuildIdealMealProfile(anchor);
        var before = SumMacros(mealSoFar);
        var after = SumMacros(mealSoFar.Append(candidate));

        var distBefore = MacroDistance(before, ideal);
        var distAfter = MacroDistance(after, ideal);

        double score;
        if (distBefore <= 1e-9)
            score = 0.5;
        else if (distAfter < distBefore)
            score = Math.Clamp((distBefore - distAfter) / distBefore, 0.0, 1.0);
        else
            score = 0.08;

        score += ComputeSlotNutritionBoost(anchor, candidate, targetSlot);
        score -= ComputeSlotNutritionPenalty(anchor, candidate, targetSlot);

        return Math.Clamp(score, 0.0, 1.0);
    }

    private static double ComputeSlotNutritionBoost(Dish anchor, Dish candidate, string targetSlot)
    {
        var anchorPro = (double)(anchor.Protein ?? 0);
        var anchorFat = (double)(anchor.Fat ?? 0);
        var anchorCal = (double)(anchor.Calories ?? 0);
        var candPro = (double)(candidate.Protein ?? 0);
        var candFat = (double)(candidate.Fat ?? 0);
        var candCal = (double)(candidate.Calories ?? 0);

        return targetSlot.ToLowerInvariant() switch
        {
            "vegetable" when anchorFat > 14 && candFat is > 0 and < 10 => 0.18,
            "vegetable" when anchorPro > 22 && candPro < 12 => 0.14,
            "soup" when anchorCal > 480 && candCal is > 0 and < 180 => 0.16,
            "soup" when anchorPro > 22 && candPro < 10 => 0.12,
            "dessert" when anchorCal > 420 && candCal is > 0 and < 200 => 0.14,
            "side" when anchorPro > 24 && candPro is > 0 and < 10 => 0.10,
            _ => 0.0,
        };
    }

    private static double ComputeSlotNutritionPenalty(Dish anchor, Dish candidate, string targetSlot)
    {
        var anchorPro = (double)(anchor.Protein ?? 0);
        var candPro = (double)(candidate.Protein ?? 0);
        var anchorCal = (double)(anchor.Calories ?? 0);
        var candCal = (double)(candidate.Calories ?? 0);

        var penalty = 0.0;

        // Anchor đã nhiều đạm → phạt món phụ/canh thêm đạm (vd: cơm gà + trứng chiên)
        if (anchorPro > 24 && candPro > 14 &&
            targetSlot is "side" or "soup" or "vegetable")
            penalty += 0.28;

        if (anchorCal > 520 && candCal > 350 &&
            targetSlot is "side" or "dessert")
            penalty += 0.18;

        return penalty;
    }

    /// <summary>
    /// Tính Nutritional Balance Score:
    /// Đo mức độ candidate bù đắp thiếu hụt macro-nutrient của mealSoFar.
    ///
    /// Deficit = max(0, Target - current)
    /// Score   = weighted sum của (candidate contribution / deficit)
    /// </summary>
    private static double ComputeNutritionalBalanceScore(
        Dish anchor,
        IReadOnlyList<Dish> mealSoFar,
        Dish candidate,
        string targetSlot)
    {
        var anchorCalories = (double)(anchor.Calories ?? 0);
        var anchorProtein = (double)(anchor.Protein ?? 0);
        var anchorFat = (double)(anchor.Fat ?? 0);

        // Mục tiêu bữa = anchor + phần bổ sung theo slot (phụ thuộc món gốc)
        var mealTargetCalories = anchorCalories > 0
            ? Math.Max(TargetCalories, anchorCalories * 1.15)
            : TargetCalories;
        var mealTargetProtein = anchorProtein > 0
            ? Math.Max(TargetProtein, anchorProtein * 1.1)
            : TargetProtein;
        var mealTargetFat = anchorFat > 18
            ? Math.Max(TargetFat, anchorFat * 0.85)
            : TargetFat;

        // Tổng hợp dinh dưỡng hiện có trong bữa
        var currentCalories = mealSoFar.Sum(d => (double)(d.Calories ?? 0));
        var currentProtein  = mealSoFar.Sum(d => (double)(d.Protein  ?? 0));
        var currentFat      = mealSoFar.Sum(d => (double)(d.Fat      ?? 0));
        var currentCarbs    = mealSoFar.Sum(d => (double)(d.Carbs    ?? 0));

        var candCalories = (double)(candidate.Calories ?? 0);
        var candProtein  = (double)(candidate.Protein  ?? 0);
        var candFat      = (double)(candidate.Fat      ?? 0);
        var candCarbs    = (double)(candidate.Carbs    ?? 0);

        var deficitCalories = Math.Max(0, mealTargetCalories - currentCalories);
        var deficitProtein = Math.Max(0, mealTargetProtein - currentProtein);
        var deficitFat = Math.Max(0, mealTargetFat - currentFat);
        var deficitCarbs = Math.Max(0, TargetCarbs - currentCarbs);

        // Nếu bữa ăn đã đủ dinh dưỡng → không cần bonus
        var totalDeficit = deficitCalories + deficitProtein * 4 + deficitFat * 3 + deficitCarbs;
        if (totalDeficit <= 1e-9)
            return 0.3; // bữa đã cân bằng → trung tính

        double score = 0.0;
        double totalWeight = 0.0;

        // Calories contribution (weight = 0.25)
        if (deficitCalories > 0)
        {
            var fill = Math.Min(candCalories, deficitCalories) / deficitCalories;
            score += 0.25 * fill;
            totalWeight += 0.25;
        }

        // Protein contribution (weight = 0.35 — quan trọng nhất)
        if (deficitProtein > 0)
        {
            var fill = Math.Min(candProtein, deficitProtein) / deficitProtein;
            score += 0.35 * fill;
            totalWeight += 0.35;

            // Bonus thêm cho món main với protein cao
            if (string.Equals(targetSlot, "main", StringComparison.OrdinalIgnoreCase)
                && candProtein >= 15)
                score += 0.05;
        }

        // Fat contribution (weight = 0.15)
        if (deficitFat > 0)
        {
            var fill = Math.Min(candFat, deficitFat) / deficitFat;
            score += 0.15 * fill;
            totalWeight += 0.15;
        }

        // Carbs contribution (weight = 0.25)
        if (deficitCarbs > 0)
        {
            var fill = Math.Min(candCarbs, deficitCarbs) / deficitCarbs;
            score += 0.25 * fill;
            totalWeight += 0.25;
        }

        // Nếu không có dữ liệu dinh dưỡng → slot-based heuristic
        if (totalWeight < 1e-9 || (candCalories == 0 && candProtein == 0))
            return ComputeSlotNutritionHeuristic(anchor, targetSlot);

        var balanced = Math.Clamp(score, 0.0, 1.0);

        // Anchor nhiều đạm/mỡ → ưu tiên canh/rau bổ sung
        if (anchorProtein > 22 && string.Equals(targetSlot, "soup", StringComparison.OrdinalIgnoreCase))
            balanced = Math.Min(1.0, balanced + 0.08);
        if (anchorFat > 16 && string.Equals(targetSlot, "vegetable", StringComparison.OrdinalIgnoreCase))
            balanced = Math.Min(1.0, balanced + 0.10);

        return balanced;
    }

    /// <summary>Fallback khi không có macro số — phụ thuộc anchor, không cố định theo slot.</summary>
    private static double ComputeSlotNutritionHeuristic(Dish anchor, string targetSlot)
    {
        var anchorFat = (double)(anchor.Fat ?? 0);
        var anchorCal = (double)(anchor.Calories ?? 0);
        var hasHeavyProtein = HasProteinSignal(anchor) || (anchor.Protein ?? 0) > 18;

        return targetSlot.ToLowerInvariant() switch
        {
            "main" => hasHeavyProtein ? 0.35 : 0.72,
            "soup" => anchorFat > 14 || anchorCal > 480 ? 0.78 : 0.52,
            "vegetable" => anchorFat > 12 ? 0.82 : 0.58,
            "side" => 0.50,
            "dessert" => anchorCal > 420 ? 0.62 : 0.38,
            _ => 0.45,
        };
    }

    // ════════════════════════════════════════════════════════════════════════════
    // UTILITY METHODS
    // ════════════════════════════════════════════════════════════════════════════

    private static List<string> InferSlotsFromMetadata(Dish dish)
    {
        var name = (dish.Name + " " + (dish.Description ?? "")).ToLowerInvariant();

        if (ContainsAny(name, "phở", "bún", "bánh canh", "hủ tiếu", "mì"))
            return ["noodle_soup"];
        if (ContainsAny(name, "canh", "súp", "cháo"))
            return ["soup"];
        if (ContainsAny(name, "chè", "trái cây", "tráng miệng", "yaourt"))
            return ["dessert"];
        if (ContainsAny(name, "dưa", "salad", "muối", "trộn", "chấm", "trứng chiên", "trứng luộc"))
            return ["side"];
        if (ContainsAny(name, "xào", "luộc", "rau", "cải", "bí đỏ", "cà rốt", "mồng tơi", "đậu"))
            return ["vegetable"];

        return ["main"];
    }

    private static bool HasProteinSignal(Dish dish)
    {
        if (dish.Protein is > 12)
            return true;
        var text = (dish.Name + " " + (dish.Description ?? "")).ToLowerInvariant();
        return ProteinKeywords.Any(k => text.Contains(k, StringComparison.OrdinalIgnoreCase));
    }

    private static bool ContainsAny(string text, params string[] parts) =>
        parts.Any(p => text.Contains(p, StringComparison.OrdinalIgnoreCase));

    private static IEnumerable<string> ExtractFamilyRootsFromText(string text)
    {
        var normalized = text.Trim().ToLowerInvariant();
        if (normalized.Length == 0)
            yield break;

        foreach (var root in IngredientFamilyRoots.OrderByDescending(r => r.Length))
        {
            if (normalized.Contains(root, StringComparison.OrdinalIgnoreCase))
                yield return root;
        }
    }

    private static double GetNameFamilyProminence(string dishName, string family)
    {
        var name = dishName.Trim().ToLowerInvariant();
        if (name.Length == 0 || !name.Contains(family, StringComparison.OrdinalIgnoreCase))
            return 0;

        if (name.StartsWith(family, StringComparison.OrdinalIgnoreCase))
            return 1.0;

        if (ContainsAny(name,
                $"canh {family}", $"chè {family}", $"súp {family}",
                $"{family} xào", $"{family} luộc", $"{family} muối",
                $"{family} kho", $"{family} chiên", $"{family} hấp", $"{family} nướng"))
            return 0.92;

        return 0.48;
    }

    private static void UpsertMax(Dictionary<string, double> map, string key, double value)
    {
        if (map.TryGetValue(key, out var existing))
            map[key] = Math.Max(existing, value);
        else
            map[key] = value;
    }
}
