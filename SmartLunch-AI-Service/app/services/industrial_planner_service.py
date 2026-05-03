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
    IndustrialMenuPlan,
    IndustrialMenuPlansResponse,
    IndustrialWeekPlanResponse,
    PlanConstraints,
    ShoppingItem,
    GroupFrequencyConstraint,
)

# ── Ingredient classification keywords ──────────────────────────
# Loaded from rules.json `ingredient_groups` section at runtime.
# No hardcoded defaults here to ensure full configurability via JSON.


def _extract_ingredient_categories(profile: "RulesProfile") -> dict[str, frozenset[str]]:
    """Extract ingredient classification keywords from rules profile.
    Returns a mapping of group_name -> set of keywords."""
    groups = profile.ingredient_groups
    if not groups:
        return {}
    
    return {name: frozenset(k.lower() for k in keywords) for name, keywords in groups.items()}



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
        request_ingredient_groups: dict[str, list[str]] | None = None,
    ) -> IndustrialWeekPlanResponse:
        """Entry point: solve, then build response with shopping list & cost."""

        profile = rules_profile or _default_profile()
        scoring_cfg = profile.scoring
        
        # Merge ingredient groups from profile and request
        kw_sets = _extract_ingredient_categories(profile)
        if request_ingredient_groups:
            for name, keywords in request_ingredient_groups.items():
                kw_sets[name] = frozenset(k.lower() for k in keywords)

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
            kw_sets=kw_sets,
            time_limit_seconds=time_limit_seconds,
        )

        if chosen is None:
            return self._empty_response(days)

        # ── Build response ────────────────────────────────────────
        week_menu = self._build_week_menu(chosen, dishes, days, meal_structure, base_scores)
        shopping_list = self._build_shopping_list(
            chosen, dishes, days, meal_structure, servings_per_day, available_ingredients, kw_sets,
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

    def plan_menus_top_k(
        self,
        dishes: list[IndustrialDish],
        days: list[str],
        meal_structure: list[DishCategory],
        constraints: PlanConstraints,
        budget_per_serving: float,
        available_ingredients: dict[str, float],
        rules_profile: RulesProfile | None = None,
        top_k: int = 3,
        time_limit_seconds: float = 5.0,
        request_ingredient_groups: dict[str, list[str]] | None = None,
    ) -> IndustrialMenuPlansResponse:
        """
        Generate top-K alternative menu plans (ranked).

        Output focuses on `week_menu` + scores only (no shopping list / cost).
        """

        profile = rules_profile or _default_profile()
        scoring_cfg = profile.scoring
        
        # Merge ingredient groups from profile and request
        kw_sets = _extract_ingredient_categories(profile)
        if request_ingredient_groups:
            for name, keywords in request_ingredient_groups.items():
                kw_sets[name] = frozenset(k.lower() for k in keywords)

        dishes_by_cat: dict[DishCategory, list[int]] = defaultdict(list)
        for idx, dish in enumerate(dishes):
            dishes_by_cat[dish.category].append(idx)

        for slot_cat in meal_structure:
            if not dishes_by_cat.get(slot_cat):
                return IndustrialMenuPlansResponse(plans=[])

        base_scores = self._compute_base_scores(
            dishes, budget_per_serving, available_ingredients, scoring_cfg,
        )

        solutions = self._solve_cpsat_top_k(
            dishes=dishes,
            days=days,
            meal_structure=meal_structure,
            dishes_by_cat=dishes_by_cat,
            constraints=constraints,
            budget_per_serving=budget_per_serving,
            base_scores=base_scores,
            scoring_cfg=scoring_cfg,
            kw_sets=kw_sets,
            top_k=top_k,
            time_limit_seconds=time_limit_seconds,
        )

        plans: list[IndustrialMenuPlan] = []
        for idx, (chosen, obj) in enumerate(solutions, start=1):
            week_menu = self._build_week_menu_multi_cover(
                chosen, dishes, days, meal_structure, base_scores,
            )
            plan_score = self._compute_plan_score(week_menu)
            plans.append(IndustrialMenuPlan(
                rank=idx,
                plan_score=plan_score,
                objective_value=float(obj),
                week_menu=week_menu,
            ))

        # Ensure sorted by best first (objective desc, then plan_score desc)
        plans_sorted = sorted(plans, key=lambda p: (p.objective_value, p.plan_score), reverse=True)
        plans_ranked = [
            IndustrialMenuPlan(
                rank=i,
                plan_score=p.plan_score,
                objective_value=p.objective_value,
                week_menu=p.week_menu,
            )
            for i, p in enumerate(plans_sorted, start=1)
        ]

        return IndustrialMenuPlansResponse(plans=plans_ranked)

    @staticmethod
    def _dish_covers(dish: IndustrialDish) -> set[DishCategory]:
        covers = set(dish.covers_categories or [])
        if not covers:
            covers.add(dish.category)
        return covers

    def _build_week_menu_multi_cover(
        self,
        chosen_by_day: list[dict[DishCategory, int]],
        dishes: list[IndustrialDish],
        days: list[str],
        meal_structure: list[DishCategory],
        base_scores: list[float],
    ) -> list[IndustrialDayMenu]:
        week: list[IndustrialDayMenu] = []
        for d, day_name in enumerate(days):
            day_dishes: list[DishRecommendation] = []
            day_cost = 0.0

            picked: set[int] = set(chosen_by_day[d].values())
            for dish_idx in picked:
                day_cost += dishes[dish_idx].cost_per_serving

            # Render by slot order; if same dish covers multiple slots, it will appear multiple times.
            for cat in meal_structure:
                dish_idx = chosen_by_day[d][cat]
                dish = dishes[dish_idx]
                reasons = self._build_reasons(dish, base_scores[dish_idx])
                day_dishes.append(DishRecommendation(
                    name=dish.name,
                    category=cat,
                    score=round(base_scores[dish_idx], 3),
                    reasons=reasons,
                    cost_per_serving=dish.cost_per_serving,
                ))

            week.append(IndustrialDayMenu(
                day=day_name,
                dishes=day_dishes,
                day_cost_per_serving=round(day_cost, 0),
            ))
        return week

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
        kw_sets: dict[str, frozenset[str]],
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

        primary_slot_indices = [
            s_idx for s_idx, cat in enumerate(meal_structure)
            if cat in (DishCategory.main, DishCategory.noodle_soup)
        ]

        # ── Group dishes by main_ingredient for reuse/consecutive logic ──
        ingredient_groups: dict[str, list[int]] = defaultdict(list)
        for i, dish in enumerate(dishes):
            ing = dish.main_ingredient.strip().lower()
            ingredient_groups[ing].append(i)

        # ── Generic Group Frequency Constraints ──
        group_freqs = list(constraints.group_frequencies)
        # Fallback for legacy fields if group_frequencies is empty
        if not group_freqs:
            if constraints.max_same_protein_per_week is not None:
                group_freqs.append(GroupFrequencyConstraint(group_name="protein", max_count=constraints.max_same_protein_per_week))
            if constraints.min_fish_per_week is not None:
                group_freqs.append(GroupFrequencyConstraint(group_name="seafood", min_count=constraints.min_fish_per_week))

        for cfg in group_freqs:
            keywords = kw_sets.get(cfg.group_name)
            if not keywords:
                continue
            # All dishes whose main ingredient belongs to this group
            idxs = [i for i, dish in enumerate(dishes) if dish.main_ingredient.strip().lower() in keywords]
            if not idxs:
                continue
            
            terms = [
                x[(d, s_idx, i)]
                for d in range(D)
                for s_idx in primary_slot_indices
                for i in idxs
                if (d, s_idx, i) in x
            ]
            if terms:
                if cfg.max_count is not None:
                    model.Add(sum(terms) <= cfg.max_count)
                if cfg.min_count is not None:
                    model.Add(sum(terms) >= cfg.min_count)

        # ── Constraint: no_repeat_main_ingredient_consecutive_days ──
        if constraints.no_repeat_main_ingredient_consecutive_days:
            for ing, idxs in ingredient_groups.items():
                for d in range(D - 1):
                    today_terms = [
                        x[(d, s_idx, i)]
                        for s_idx in primary_slot_indices
                        for i in idxs
                        if (d, s_idx, i) in x
                    ]
                    tomorrow_terms = [
                        x[(d + 1, s_idx, i)]
                        for s_idx in primary_slot_indices
                        for i in idxs
                        if (d + 1, s_idx, i) in x
                    ]
                    if today_terms and tomorrow_terms:
                        model.Add(sum(today_terms) + sum(tomorrow_terms) <= 1)

        # ── Constraint: alternate_cooking_methods ──
        if constraints.alternate_cooking_methods:
            method_groups: dict[CookingMethod, list[int]] = defaultdict(list)
            for i, dish in enumerate(dishes):
                if dish.category in (DishCategory.main, DishCategory.noodle_soup):
                    method_groups[dish.cooking_method].append(i)

            for method, idxs in method_groups.items():
                for d in range(D - 1):
                    today_terms = [
                        x[(d, s_idx, i)]
                        for s_idx in primary_slot_indices
                        for i in idxs
                        if (d, s_idx, i) in x
                    ]
                    tomorrow_terms = [
                        x[(d + 1, s_idx, i)]
                        for s_idx in primary_slot_indices
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
                for d in range(D - 1):
                    for d2 in range(d + 1, min(d + 3, D)):
                        # Create auxiliary bool: both days use this ingredient
                        for s1 in primary_slot_indices:
                            for s2 in primary_slot_indices:
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

    def _solve_cpsat_top_k(
        self,
        dishes: list[IndustrialDish],
        days: list[str],
        meal_structure: list[DishCategory],
        dishes_by_cat: dict[DishCategory, list[int]],
        constraints: PlanConstraints,
        budget_per_serving: float,
        base_scores: list[float],
        scoring_cfg: dict[str, Any],
        kw_sets: dict[str, frozenset[str]],
        top_k: int,
        time_limit_seconds: float,
    ) -> list[tuple[list[dict[DishCategory, int]], float]]:
        """
        Solve CP-SAT repeatedly to get K distinct plans.

        Model supports "multi-cover" dishes, e.g. phở/mì can cover (main, side, soup),
        so a single chosen dish can satisfy multiple daily slots.

        Implementation: solve repeatedly and add a "no-good cut" to exclude the last
        exact selection set, then resolve.

        Returns list of (chosen_by_day_category, objective_value), best-first.
        """

        from ortools.sat.python import cp_model

        D = len(days)
        SCALE = 1000

        model = cp_model.CpModel()

        required_cats = list(meal_structure)
        required_set = set(required_cats)

        covers_by_dish: list[set[DishCategory]] = [self._dish_covers(d) for d in dishes]

        # Candidate dish indices: covers at least 1 required slot
        candidate_idxs = [
            i for i, cov in enumerate(covers_by_dish)
            if cov.intersection(required_set)
        ]

        # Validate: for each required slot, there must be at least 1 dish that can cover it
        cover_pool: dict[DishCategory, list[int]] = {}
        for cat in required_cats:
            cover_pool[cat] = [i for i in candidate_idxs if cat in covers_by_dish[i]]
            if not cover_pool[cat]:
                return []

        # Dishes that can cover primary slots (main / noodle_soup) — mirrors
        # primary_slot_indices in _solve_cpsat; used for protein/group freq & diversity.
        _primary_cats = frozenset({DishCategory.main, DishCategory.noodle_soup})
        main_cover_idxs = [
            i for i in candidate_idxs if covers_by_dish[i].intersection(_primary_cats)
        ]

        # Variables: y[d,i] = 1 if dish i is selected on day d
        y: dict[tuple[int, int], cp_model.IntVar] = {}
        for d in range(D):
            for i in candidate_idxs:
                y[(d, i)] = model.NewBoolVar(f"y_d{d}_i{i}")

        # Coverage constraints: each required slot must be covered exactly once
        for d in range(D):
            for cat in required_cats:
                model.AddExactlyOne(y[(d, i)] for i in cover_pool[cat])

        # max_per_week: limit dish appearance across days
        for i in candidate_idxs:
            model.Add(sum(y[(d, i)] for d in range(D)) <= dishes[i].max_per_week)

        # ── Generic Group Frequency Constraints ──
        group_freqs = list(constraints.group_frequencies)
        if not group_freqs:
            if constraints.max_same_protein_per_week is not None:
                group_freqs.append(GroupFrequencyConstraint(group_name="protein", max_count=constraints.max_same_protein_per_week))
            if constraints.min_fish_per_week is not None:
                group_freqs.append(GroupFrequencyConstraint(group_name="seafood", min_count=constraints.min_fish_per_week))

        for cfg in group_freqs:
            keywords = kw_sets.get(cfg.group_name)
            if not keywords:
                continue
            idxs = [i for i in main_cover_idxs if dishes[i].main_ingredient.strip().lower() in keywords]
            if not idxs:
                continue
            
            terms = [y[(d, i)] for d in range(D) for i in idxs]
            if terms:
                if cfg.max_count is not None:
                    model.Add(sum(terms) <= cfg.max_count)
                if cfg.min_count is not None:
                    model.Add(sum(terms) >= cfg.min_count)

        ingredient_groups: dict[str, list[int]] = defaultdict(list)
        for i in candidate_idxs:
            ing = dishes[i].main_ingredient.strip().lower()
            ingredient_groups[ing].append(i)

        if constraints.no_repeat_main_ingredient_consecutive_days:
            for ing, idxs in ingredient_groups.items():
                idxs_main = [i for i in idxs if i in main_cover_idxs]
                if not idxs_main:
                    continue
                for d in range(D - 1):
                    model.Add(
                        sum(y[(d, i)] for i in idxs_main)
                        + sum(y[(d + 1, i)] for i in idxs_main)
                        <= 1
                    )

        if constraints.alternate_cooking_methods:
            method_groups: dict[CookingMethod, list[int]] = defaultdict(list)
            for i in main_cover_idxs:
                method_groups[dishes[i].cooking_method].append(i)

            for method, idxs in method_groups.items():
                for d in range(D - 1):
                    model.Add(
                        sum(y[(d, i)] for i in idxs) + sum(y[(d + 1, i)] for i in idxs)
                        <= 1
                    )

        # ── Constraint: main dish not more than N consecutive days ──
        # Apply to dishes that can cover the `main` slot.
        n = int(constraints.max_consecutive_same_main_dish)
        if n > 0 and main_cover_idxs and D > n:
            window = n + 1
            for i in main_cover_idxs:
                for start in range(0, D - window + 1):
                    model.Add(sum(y[(start + t, i)] for t in range(window)) <= n)

        for d in range(D):
            model.Add(
                sum(y[(d, i)] * int(dishes[i].cost_per_serving) for i in candidate_idxs)
                <= int(budget_per_serving)
            )

        reuse_bonus_w = float(scoring_cfg.get("ingredient_reuse_bonus", 0.0))
        repeat_penalty_w = float(scoring_cfg.get("repeat_main_dish_penalty", 0.0))
        objective_terms = []
        for d in range(D):
            for i in candidate_idxs:
                cover_count = len(covers_by_dish[i].intersection(required_set))
                score_int = int(base_scores[i] * SCALE * max(1, cover_count))
                objective_terms.append(y[(d, i)] * score_int)

        # Soft diversity: penalize repeating the same main-cover dish on consecutive days.
        if repeat_penalty_w > 0 and main_cover_idxs:
            penalty = int(repeat_penalty_w * SCALE)
            for i in main_cover_idxs:
                for d in range(D - 1):
                    rep = model.NewBoolVar(f"rep_main_i{i}_d{d}")
                    model.AddBoolAnd([y[(d, i)], y[(d + 1, i)]]).OnlyEnforceIf(rep)
                    model.AddBoolOr([y[(d, i)].Not(), y[(d + 1, i)].Not()]).OnlyEnforceIf(rep.Not())
                    objective_terms.append(rep * (-penalty))

        if constraints.prefer_ingredient_reuse and main_cover_idxs:
            for ing, idxs in ingredient_groups.items():
                for d in range(D - 1):
                    for d2 in range(d + 1, min(d + 3, D)):
                        idxs_main = [i for i in idxs if i in main_cover_idxs]
                        for i1 in idxs_main:
                            for i2 in idxs_main:
                                reuse_var = model.NewBoolVar(
                                    f"reuse_{ing}_d{d}_d{d2}_i{i1}_i{i2}"
                                )
                                model.AddBoolAnd([
                                    y[(d, i1)],
                                    y[(d2, i2)],
                                ]).OnlyEnforceIf(reuse_var)
                                model.AddBoolOr([
                                    y[(d, i1)].Not(),
                                    y[(d2, i2)].Not(),
                                ]).OnlyEnforceIf(reuse_var.Not())
                                bonus = int(reuse_bonus_w * SCALE * 0.5)
                                objective_terms.append(reuse_var * bonus)

        model.Maximize(sum(objective_terms))

        solver = cp_model.CpSolver()
        solver.parameters.max_time_in_seconds = time_limit_seconds
        solver.parameters.num_search_workers = 8

        results: list[tuple[list[dict[DishCategory, int]], float]] = []
        # no-good cut uses the count of actually selected dishes, which may be
        # smaller than number of slots when multi-cover dishes are used.

        for plan_idx in range(max(1, int(top_k))):
            # Many optimal/near-optimal menus share the same objective value; varying the
            # CP-SAT random seed explores different branches so top-K plans diverge more for chefs.
            solver.parameters.random_seed = 10_007 + plan_idx * 9_973
            status = solver.Solve(model)
            if status not in (cp_model.OPTIMAL, cp_model.FEASIBLE):
                break

            chosen_by_day: list[dict[DishCategory, int]] = []
            picked_literals: list[cp_model.IntVar] = []

            for d in range(D):
                selected_idxs = [i for i in candidate_idxs if solver.Value(y[(d, i)]) == 1]
                for i in selected_idxs:
                    picked_literals.append(y[(d, i)])

                day_map: dict[DishCategory, int] = {}
                for cat in required_cats:
                    # Find the (unique) selected dish that covers this category
                    picked_i = next(
                        (i for i in selected_idxs if cat in covers_by_dish[i]),
                        None,
                    )
                    if picked_i is None:
                        # Should not happen because of AddExactlyOne coverage.
                        picked_i = cover_pool[cat][0]
                    day_map[cat] = picked_i

                chosen_by_day.append(day_map)

            obj_val = float(solver.ObjectiveValue())
            results.append((chosen_by_day, obj_val))

            # No-good cut: cannot pick exactly the same set again.
            if picked_literals:
                model.Add(sum(picked_literals) <= len(picked_literals) - 1)
            else:
                break

        # Best-first
        results.sort(key=lambda t: t[1], reverse=True)
        return results

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
                        cat = self._classify_ingredient(ing_lower, kw_sets)
                        ingredient_usage[ing_lower] = {"days_count": 0, "category": cat}
                    ingredient_usage[ing_lower]["days_count"] += 1

        shopping: list[ShoppingItem] = []
        available_lower = {k.lower(): v for k, v in available_ingredients.items()}

        for ing_name, info in sorted(ingredient_usage.items()):
            cat = info["category"]
            kg_table: dict = kw_sets.get("kg_per_serving", _DEFAULT_KG_PER_SERVING)  # type: ignore[assignment]
            kg_per_serving = kg_table.get(cat, 0.05)
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

    def _classify_ingredient(self, name: str, kw_sets: dict) -> str:
        """Classify an ingredient name (English NameEnglish from DB)
        into protein / vegetable / spice / other.
        Uses keyword sets loaded from the active rules profile — no hardcoded Vietnamese."""
        n = name.strip().lower()
        if n in kw_sets["protein"]:
            return "protein"
        if n in kw_sets["vegetable"]:
            return "vegetable"
        if n in kw_sets["spice"]:
            return "spice"
        # Substring fallback for compound names like 'snakehead_fish', 'coconut_water'
        for kw in kw_sets["protein"]:
            if kw in n:
                return "protein"
        for kw in kw_sets["vegetable"]:
            if kw in n:
                return "vegetable"
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
