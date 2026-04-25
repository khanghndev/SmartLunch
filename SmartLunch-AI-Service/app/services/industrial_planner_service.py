from __future__ import annotations

import random
from collections import defaultdict
from typing import Any



from app.core.rules_loader import RulesProfile, _default_profile
from app.schemas.industrial import (
    CookingMethod,
    CostBreakdown,
    DishCategory,
    DishRecommendation,
    IndustrialDayMenu,
    IndustrialDish,
    IndustrialWeekPlanResponse,
    PlanConstraints,
    ShoppingItem,
)

# ── Constants ────────────────────────────────────────────────────────

_PROTEIN_INGREDIENTS: set[str] = {
    "chicken", "pork", "beef", "fish", "shrimp", "egg", "tofu",
    "duck", "squid", "crab", "clam", "goat",
    "gà", "heo", "bò", "cá", "tôm", "trứng", "đậu hũ", "đậu phụ",
    "vịt", "mực", "cua", "nghêu", "dê",
}

_FISH_KEYWORDS: set[str] = {
    "fish", "cá", "ca",
}

_VEGETABLE_KEYWORDS: set[str] = {
    "rau", "cải", "bắp cải", "rau muống", "bí", "bầu", "mồng tơi",
    "su su", "đậu", "cà", "khoai", "cabbage", "spinach", "gourd",
    "morning_glory", "pumpkin", "cucumber",
}

# Estimated kg per serving for ingredient types
_KG_PER_SERVING: dict[str, float] = {
    "protein": 0.15,
    "vegetable": 0.10,
    "spice": 0.01,
    "other": 0.05,
}


class IndustrialPlannerService:
    """
    Constraint-based meal planner for industrial kitchens (KCN).

    Uses Google OR-Tools CP-SAT solver to generate weekly meal plans
    that satisfy budget, ingredient, protein frequency, and cooking
    method diversity constraints.
    """

    def plan_week(
        self,
        dishes: list[IndustrialDish],
        days: list[str],
        meal_structure: list[DishCategory],
        constraints: PlanConstraints,
        budget_per_serving: float,
        servings_per_day: int,
        available_ingredients: dict[str, float],
        rules_profile: RulesProfile | None = None,
        top_k: int = 1,
        time_limit_seconds: float = 5.0,
    ) -> IndustrialWeekPlanResponse:
        """Entry point: solve, then build response with shopping list & cost."""

        profile = rules_profile or _default_profile()
        scoring_cfg = profile.scoring

        D = len(days)
        S = len(meal_structure)

        # ── Group dishes by category ──────────────────────────────
        dishes_by_cat: dict[DishCategory, list[int]] = defaultdict(list)
        for idx, dish in enumerate(dishes):
            dishes_by_cat[dish.category].append(idx)

        # Validate: every slot in meal_structure must have at least 1 dish
        for slot_cat in meal_structure:
            if not dishes_by_cat.get(slot_cat):
                # If a category has no dishes, return empty plan
                return self._empty_response(days)

        # ── Pre-compute scores ────────────────────────────────────
        base_scores = self._compute_base_scores(
            dishes, budget_per_serving, available_ingredients, scoring_cfg,
        )

        # ── Build & solve CP-SAT model ────────────────────────────
        chosen = self._solve_cpsat(
            dishes=dishes,
            days=days,
            meal_structure=meal_structure,
            dishes_by_cat=dishes_by_cat,
            constraints=constraints,
            budget_per_serving=budget_per_serving,
            base_scores=base_scores,
            scoring_cfg=scoring_cfg,
            time_limit_seconds=time_limit_seconds,
        )

        if chosen is None:
            return self._empty_response(days)

        # ── Build response ────────────────────────────────────────
        week_menu = self._build_week_menu(chosen, dishes, days, meal_structure, base_scores)
        shopping_list = self._build_shopping_list(
            chosen, dishes, days, meal_structure, servings_per_day, available_ingredients,
        )
        cost_estimate = self._build_cost_estimate(
            week_menu, budget_per_serving, servings_per_day, D,
        )

        plan_score = self._compute_plan_score(week_menu)

        return IndustrialWeekPlanResponse(
            week_menu=week_menu,
            shopping_list=shopping_list,
            cost_estimate=cost_estimate,
            plan_score=plan_score,
        )

    # ═══════════════════════════════════════════════════════════════
    # Scoring
    # ═══════════════════════════════════════════════════════════════

    def _compute_base_scores(
        self,
        dishes: list[IndustrialDish],
        budget: float,
        available_ingredients: dict[str, float],
        scoring_cfg: dict[str, Any],
    ) -> list[float]:
        """Pre-compute a base score ∈ [0,1] for each dish."""

        base = float(scoring_cfg.get("base_score", 0.3))
        budget_w = float(scoring_cfg.get("budget_fit_weight", 0.25))
        pop_w = float(scoring_cfg.get("popularity_weight", 0.15))
        avail_w = float(scoring_cfg.get("available_ingredient_bonus", 0.15))
        tag_weights: dict[str, float] = scoring_cfg.get("tag_weights", {})

        scores: list[float] = []
        for dish in dishes:
            s = base

            # Budget fit: closer to budget (but not over) is better
            if budget > 0:
                ratio = dish.cost_per_serving / budget
                if ratio <= 1.0:
                    s += budget_w * (1.0 - abs(1.0 - ratio))
                else:
                    # Over budget → penalty
                    s -= budget_w * min(1.0, ratio - 1.0)

            # Popularity
            s += pop_w * (dish.popularity / 5.0)

            # Available ingredient bonus
            main_ing = dish.main_ingredient.strip().lower()
            if main_ing in {k.lower() for k in available_ingredients}:
                s += avail_w

            # Tag weights
            dish_tags = {t.strip().lower() for t in dish.tags}
            for tag, w in tag_weights.items():
                if tag.strip().lower() in dish_tags:
                    s += float(w)

            scores.append(max(0.0, min(1.0, s)))

        return scores

    # ═══════════════════════════════════════════════════════════════
    # CP-SAT Solver
    # ═══════════════════════════════════════════════════════════════

    def _solve_cpsat(
        self,
        dishes: list[IndustrialDish],
        days: list[str],
        meal_structure: list[DishCategory],
        dishes_by_cat: dict[DishCategory, list[int]],
        constraints: PlanConstraints,
        budget_per_serving: float,
        base_scores: list[float],
        scoring_cfg: dict[str, Any],
        time_limit_seconds: float,
    ) -> list[list[int]] | None:
        """
        Returns chosen[d][s] = dish index, or None if infeasible.
        """

        D = len(days)
        S = len(meal_structure)
        N = len(dishes)
        SCALE = 1000

        from ortools.sat.python import cp_model

        model = cp_model.CpModel()

        # ── Variables: x[d][s][i] = 1 if day d, slot s uses dish i ──
        x: dict[tuple[int, int, int], cp_model.IntVar] = {}
        for d in range(D):
            for s_idx, cat in enumerate(meal_structure):
                for i in dishes_by_cat[cat]:
                    x[(d, s_idx, i)] = model.NewBoolVar(f"x_d{d}_s{s_idx}_i{i}")

        # ── Constraint: exactly 1 dish per day per slot ──
        for d in range(D):
            for s_idx, cat in enumerate(meal_structure):
                model.AddExactlyOne(
                    x[(d, s_idx, i)] for i in dishes_by_cat[cat]
                )

        # ── Constraint: max_per_week ──
        for i, dish in enumerate(dishes):
            terms = [
                x[(d, s_idx, i)]
                for d in range(D)
                for s_idx, cat in enumerate(meal_structure)
                if (d, s_idx, i) in x
            ]
            if terms:
                model.Add(sum(terms) <= dish.max_per_week)

        # ── Constraint: max_same_protein_per_week ──
        # Group dishes by main_ingredient (protein only)
        ingredient_groups: dict[str, list[int]] = defaultdict(list)
        for i, dish in enumerate(dishes):
            ing = dish.main_ingredient.strip().lower()
            if ing in _PROTEIN_INGREDIENTS:
                ingredient_groups[ing].append(i)

        for ing, idxs in ingredient_groups.items():
            # Count how many days use this protein (across all main slots)
            main_slot_indices = [
                s_idx for s_idx, cat in enumerate(meal_structure)
                if cat == DishCategory.main
            ]
            terms = [
                x[(d, s_idx, i)]
                for d in range(D)
                for s_idx in main_slot_indices
                for i in idxs
                if (d, s_idx, i) in x
            ]
            if terms:
                model.Add(sum(terms) <= constraints.max_same_protein_per_week)

        # ── Constraint: min_fish_per_week ──
        fish_idxs = [
            i for i, dish in enumerate(dishes)
            if dish.main_ingredient.strip().lower() in _FISH_KEYWORDS
        ]
        if fish_idxs and constraints.min_fish_per_week > 0:
            main_slot_indices = [
                s_idx for s_idx, cat in enumerate(meal_structure)
                if cat == DishCategory.main
            ]
            fish_terms = [
                x[(d, s_idx, i)]
                for d in range(D)
                for s_idx in main_slot_indices
                for i in fish_idxs
                if (d, s_idx, i) in x
            ]
            if fish_terms:
                model.Add(sum(fish_terms) >= constraints.min_fish_per_week)

        # ── Constraint: no_repeat_main_ingredient_consecutive_days ──
        if constraints.no_repeat_main_ingredient_consecutive_days:
            main_slot_indices = [
                s_idx for s_idx, cat in enumerate(meal_structure)
                if cat == DishCategory.main
            ]
            for ing, idxs in ingredient_groups.items():
                for d in range(D - 1):
                    today_terms = [
                        x[(d, s_idx, i)]
                        for s_idx in main_slot_indices
                        for i in idxs
                        if (d, s_idx, i) in x
                    ]
                    tomorrow_terms = [
                        x[(d + 1, s_idx, i)]
                        for s_idx in main_slot_indices
                        for i in idxs
                        if (d + 1, s_idx, i) in x
                    ]
                    if today_terms and tomorrow_terms:
                        model.Add(sum(today_terms) + sum(tomorrow_terms) <= 1)

        # ── Constraint: alternate_cooking_methods ──
        if constraints.alternate_cooking_methods:
            main_slot_indices = [
                s_idx for s_idx, cat in enumerate(meal_structure)
                if cat == DishCategory.main
            ]
            method_groups: dict[CookingMethod, list[int]] = defaultdict(list)
            for i, dish in enumerate(dishes):
                if dish.category == DishCategory.main:
                    method_groups[dish.cooking_method].append(i)

            for method, idxs in method_groups.items():
                for d in range(D - 1):
                    today_terms = [
                        x[(d, s_idx, i)]
                        for s_idx in main_slot_indices
                        for i in idxs
                        if (d, s_idx, i) in x
                    ]
                    tomorrow_terms = [
                        x[(d + 1, s_idx, i)]
                        for s_idx in main_slot_indices
                        for i in idxs
                        if (d + 1, s_idx, i) in x
                    ]
                    if today_terms and tomorrow_terms:
                        model.Add(sum(today_terms) + sum(tomorrow_terms) <= 1)

        # ── Constraint: budget per day ──
        for d in range(D):
            cost_terms = []
            for s_idx, cat in enumerate(meal_structure):
                for i in dishes_by_cat[cat]:
                    cost_int = int(dishes[i].cost_per_serving)
                    cost_terms.append(x[(d, s_idx, i)] * cost_int)
            model.Add(sum(cost_terms) <= int(budget_per_serving))

        # ── Objective: maximize total score ──
        # Include ingredient reuse bonus in objective
        reuse_bonus_w = float(scoring_cfg.get("ingredient_reuse_bonus", 0.2))
        cooking_div_w = float(scoring_cfg.get("cooking_diversity_bonus", 0.1))

        objective_terms = []
        for d in range(D):
            for s_idx, cat in enumerate(meal_structure):
                for i in dishes_by_cat[cat]:
                    score_int = int(base_scores[i] * SCALE)
                    objective_terms.append(x[(d, s_idx, i)] * score_int)

        # Ingredient reuse bonus: reward using same main_ingredient on nearby days
        if constraints.prefer_ingredient_reuse:
            for ing, idxs in ingredient_groups.items():
                main_slots = [
                    s_idx for s_idx, cat in enumerate(meal_structure)
                    if cat == DishCategory.main
                ]
                for d in range(D - 1):
                    for d2 in range(d + 1, min(d + 3, D)):
                        # Create auxiliary bool: both days use this ingredient
                        for s1 in main_slots:
                            for s2 in main_slots:
                                for i1 in idxs:
                                    for i2 in idxs:
                                        if (d, s1, i1) in x and (d2, s2, i2) in x:
                                            reuse_var = model.NewBoolVar(
                                                f"reuse_{ing}_d{d}_d{d2}_i{i1}_i{i2}"
                                            )
                                            model.AddBoolAnd([
                                                x[(d, s1, i1)],
                                                x[(d2, s2, i2)],
                                            ]).OnlyEnforceIf(reuse_var)
                                            model.AddBoolOr([
                                                x[(d, s1, i1)].Not(),
                                                x[(d2, s2, i2)].Not(),
                                            ]).OnlyEnforceIf(reuse_var.Not())
                                            bonus = int(reuse_bonus_w * SCALE * 0.5)
                                            objective_terms.append(reuse_var * bonus)

        model.Maximize(sum(objective_terms))

        # ── Solve ──
        solver = cp_model.CpSolver()
        solver.parameters.max_time_in_seconds = time_limit_seconds
        solver.parameters.num_search_workers = 8

        status = solver.Solve(model)
        if status not in (cp_model.OPTIMAL, cp_model.FEASIBLE):
            return None

        # Extract solution
        chosen: list[list[int]] = []
        for d in range(D):
            day_choices: list[int] = []
            for s_idx, cat in enumerate(meal_structure):
                for i in dishes_by_cat[cat]:
                    if solver.Value(x[(d, s_idx, i)]) == 1:
                        day_choices.append(i)
                        break
            chosen.append(day_choices)

        return chosen

    # ═══════════════════════════════════════════════════════════════
    # Response builders
    # ═══════════════════════════════════════════════════════════════

    def _build_week_menu(
        self,
        chosen: list[list[int]],
        dishes: list[IndustrialDish],
        days: list[str],
        meal_structure: list[DishCategory],
        base_scores: list[float],
    ) -> list[IndustrialDayMenu]:
        week: list[IndustrialDayMenu] = []
        for d, day_name in enumerate(days):
            day_dishes: list[DishRecommendation] = []
            day_cost = 0.0
            for s_idx, dish_idx in enumerate(chosen[d]):
                dish = dishes[dish_idx]
                reasons = self._build_reasons(dish, base_scores[dish_idx])
                day_dishes.append(DishRecommendation(
                    name=dish.name,
                    category=dish.category,
                    score=round(base_scores[dish_idx], 3),
                    reasons=reasons,
                    cost_per_serving=dish.cost_per_serving,
                ))
                day_cost += dish.cost_per_serving
            week.append(IndustrialDayMenu(
                day=day_name,
                dishes=day_dishes,
                day_cost_per_serving=round(day_cost, 0),
            ))
        return week

    @staticmethod
    def _build_reasons(dish: IndustrialDish, score: float) -> list[str]:
        reasons: list[str] = []
        if dish.popularity >= 4:
            reasons.append("Món phổ biến")
        if score >= 0.7:
            reasons.append("Điểm phù hợp cao")
        reasons.append(f"Chế biến: {dish.cooking_method.value}")
        reasons.append(f"Nguyên liệu chính: {dish.main_ingredient}")
        return reasons

    def _build_shopping_list(
        self,
        chosen: list[list[int]],
        dishes: list[IndustrialDish],
        days: list[str],
        meal_structure: list[DishCategory],
        servings_per_day: int,
        available_ingredients: dict[str, float],
    ) -> list[ShoppingItem]:
        # Aggregate all ingredients across the week
        ingredient_usage: dict[str, dict] = {}  # name -> {days_count, category}

        for d, day_choices in enumerate(chosen):
            for dish_idx in day_choices:
                dish = dishes[dish_idx]
                all_ings = [dish.main_ingredient] + dish.sub_ingredients
                for ing in all_ings:
                    ing_lower = ing.strip().lower()
                    if ing_lower not in ingredient_usage:
                        cat = self._classify_ingredient(ing_lower)
                        ingredient_usage[ing_lower] = {"days_count": 0, "category": cat}
                    ingredient_usage[ing_lower]["days_count"] += 1

        shopping: list[ShoppingItem] = []
        available_lower = {k.lower(): v for k, v in available_ingredients.items()}

        for ing_name, info in sorted(ingredient_usage.items()):
            cat = info["category"]
            kg_per_serving = _KG_PER_SERVING.get(cat, 0.05)
            total_kg = round(kg_per_serving * servings_per_day * info["days_count"], 1)
            avail = available_lower.get(ing_name, 0.0)
            to_buy = max(0.0, round(total_kg - avail, 1))

            shopping.append(ShoppingItem(
                ingredient_name=ing_name,
                total_kg=total_kg,
                available_kg=avail,
                to_buy_kg=to_buy,
                category=cat,
            ))

        return shopping

    @staticmethod
    def _classify_ingredient(name: str) -> str:
        n = name.lower()
        if any(kw in n for kw in _PROTEIN_INGREDIENTS):
            return "protein"
        if any(kw in n for kw in _VEGETABLE_KEYWORDS):
            return "vegetable"
        if any(kw in n for kw in {"muối", "tiêu", "ớt", "tỏi", "hành", "gừng",
                                    "nước mắm", "đường", "dầu", "salt", "pepper",
                                    "garlic", "onion", "sugar", "oil", "sauce"}):
            return "spice"
        return "other"

    @staticmethod
    def _build_cost_estimate(
        week_menu: list[IndustrialDayMenu],
        budget_per_serving: float,
        servings_per_day: int,
        num_days: int,
    ) -> CostBreakdown:
        if not week_menu:
            return CostBreakdown(
                avg_cost_per_serving=0, total_week_cost=0, within_budget=True,
            )
        avg = sum(d.day_cost_per_serving for d in week_menu) / len(week_menu)
        total = avg * servings_per_day * num_days
        return CostBreakdown(
            avg_cost_per_serving=round(avg, 0),
            total_week_cost=round(total, 0),
            within_budget=avg <= budget_per_serving,
        )

    @staticmethod
    def _compute_plan_score(week_menu: list[IndustrialDayMenu]) -> float:
        if not week_menu:
            return 0.0
        all_scores = [
            dish.score
            for day in week_menu
            for dish in day.dishes
        ]
        if not all_scores:
            return 0.0
        return round(min(1.0, sum(all_scores) / len(all_scores)), 3)

    @staticmethod
    def _empty_response(days: list[str]) -> IndustrialWeekPlanResponse:
        return IndustrialWeekPlanResponse(
            week_menu=[],
            shopping_list=[],
            cost_estimate=CostBreakdown(
                avg_cost_per_serving=0, total_week_cost=0, within_budget=True,
            ),
            plan_score=0.0,
        )
