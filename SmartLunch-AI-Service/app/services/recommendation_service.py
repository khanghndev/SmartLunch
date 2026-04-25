from __future__ import annotations

from dataclasses import dataclass

from app.core.rules_loader import RulesProfile, _default_profile
from app.schemas.recommendation import MealPlanPair, WeekPlan, Recommendation, DayPlan
import random

@dataclass(frozen=True)
class ScoredItem:
    name: str
    score: float
    reasons: list[str]


class RecommendationService:
    """
    Baseline, tag-based recommender.

    Replace with your model later (collaborative filtering, content-based, LLM ranking, etc.).
    """

    def recommend(
        self,
        menu: list[tuple[str, list[str]]],
        dietary_preferences: list[str],
        allergies: list[str],
        top_k: int,
        rules_profile: RulesProfile | None = None,
    ) -> list[ScoredItem]:
        prefs = self._normalize_set(dietary_preferences)
        alls = self._normalize_set(allergies)

        results: list[ScoredItem] = []
        for name, tags in menu:
            score, reasons, allowed = self._score_item_by_tags(
                name, tags, prefs, alls, rules_profile=rules_profile
            )
            if allowed:
                results.append(ScoredItem(name=name, score=score, reasons=reasons))

        results.sort(key=lambda x: x.score, reverse=True)
        return results[:top_k]

    def recommend_today(
        self,
        menu_items: list[tuple[str, list[str]]] | None,
        dietary_preferences: list[str],
        allergies: list[str],
        top_k: int,
        rules_profile: RulesProfile | None = None,
    ) -> list[ScoredItem]:
        menu = menu_items or []
        if not menu:
            return []
            
        return self.recommend(
            menu=menu, 
            dietary_preferences=dietary_preferences, 
            allergies=allergies, 
            top_k=top_k, 
            rules_profile=rules_profile
        )

    @staticmethod
    def _normalize_set(items: list[str]) -> set[str]:
        return {x.strip().lower() for x in items if x.strip()}

    @staticmethod
    def _matches_allergies(name: str, tags: set[str], allergies: set[str]) -> bool:
        if not allergies:
            return False
        n = name.lower()

        # Substring match is more tolerant than exact tag equality.
        # (tags should already be lowercased tokens)
        return any((a in n) or any(a in t for t in tags) for a in allergies)

    @staticmethod
    def _score_item_by_tags(
        name: str,
        tags: list[str],
        dietary_preferences: set[str],
        allergies: set[str],
        rules_profile: RulesProfile | None = None,
    ) -> tuple[float, list[str], bool]:
        """
        Returns (score, reasons, allowed).
        - allowed=False if the item violates allergies.
        - score is clamped into [0, 1] for stable optimization.
        """

        profile = rules_profile or _default_profile()
        scoring_cfg = profile.scoring
        allergy_cfg = profile.allergy_filter

        tagset = {t.strip().lower() for t in tags}
        if allergy_cfg.get("enabled", True) and RecommendationService._matches_allergies(
            name=name, tags=tagset, allergies=allergies
        ):
            return 0.0, [], False

        reasons: list[str] = []
        score = float(scoring_cfg.get("base_score", 0.35))
        tag_weights: dict[str, float] = scoring_cfg.get("tag_weights", {})

        for tag, w in tag_weights.items():
            t = tag.strip().lower()
            if t and t in tagset:
                score += float(w)
                reasons.append(tag)

        # Preferences boost
        if dietary_preferences:
            matched = sorted(dietary_preferences.intersection(tagset))
            if matched:
                score += float(scoring_cfg.get("preference_match_bonus", 0.2))
                reasons.append(f"Matches preferences: {', '.join(matched)}")
            else:
                # Light penalty if user has preferences but item doesn't match any
                score -= float(scoring_cfg.get("preference_miss_penalty", 0.05))

        score = max(0.0, min(1.0, score))
        if not reasons:
            reasons.append("Popular choice")
            
        # Add randomness for variety if configured
        randomness = float(scoring_cfg.get("randomness", 0.0))
        if randomness > 0:
            noise = random.uniform(0, randomness)
            score += noise
            score = max(0.0, min(1.0, score))  # Re-clamp
            reasons.append("Variety boost")

        return score, reasons, True

    def recommend_meal_plan_today_ortools(
        self,
        main_menu: list[tuple[str, list[str]]],
        soup_menu: list[tuple[str, list[str]]],
        dietary_preferences: list[str],
        allergies: list[str],
        incompatible_pairs: list[tuple[int, int]],
        top_k: int = 1,
        time_limit_seconds: float = 2.0,
        rules_profile: RulesProfile | None = None,
    ) -> list[MealPlanPair]:
        """
        Choose the best (mặn, canh) pair using Google OR-Tools CP-SAT.
        """

        try:
            from ortools.sat.python import cp_model  # type: ignore
        except ImportError:  # pragma: no cover
            # Fallback to baseline pairing by combining scores independently.
            prefs = self._normalize_set(dietary_preferences)
            alls = self._normalize_set(allergies)

            main_scored = []
            for name, tags in main_menu:
                score, reasons, allowed = self._score_item_by_tags(
                    name, tags, prefs, alls, rules_profile=rules_profile
                )
                if allowed:
                    main_scored.append(ScoredItem(name=name, score=score, reasons=reasons))
            soup_scored = []
            for name, tags in soup_menu:
                score, reasons, allowed = self._score_item_by_tags(
                    name, tags, prefs, alls, rules_profile=rules_profile
                )
                if allowed:
                    soup_scored.append(ScoredItem(name=name, score=score, reasons=reasons))

            # Only remove incompatible pairs by index if provided.
            incompatible_set = set(incompatible_pairs)
            best: list[MealPlanPair] = []
            for i, m in enumerate(main_scored):
                for j, s in enumerate(soup_scored):
                    if (i, j) in incompatible_set:
                        continue
                    plan_score = max(0.0, min(1.0, (m.score + s.score) / 2.0))
                    # Chuyển đổi sang Pydantic model
                    main_rec = Recommendation(name=m.name, score=m.score, reasons=m.reasons)
                    soup_rec = Recommendation(name=s.name, score=s.score, reasons=s.reasons)
                    best.append(MealPlanPair(main=main_rec, soup=soup_rec, plan_score=plan_score))
            best.sort(key=lambda x: x.plan_score, reverse=True)
            return best[:top_k]

        prefs = self._normalize_set(dietary_preferences)
        alls = self._normalize_set(allergies)

        main_names = [x[0] for x in main_menu]
        soup_names = [x[0] for x in soup_menu]
        main_tags = [x[1] for x in main_menu]
        soup_tags = [x[1] for x in soup_menu]

        main_scores: list[float] = []
        soup_scores: list[float] = []
        main_reasons: list[list[str]] = []
        soup_reasons: list[list[str]] = []
        allowed_main: list[bool] = []
        allowed_soup: list[bool] = []

        for name, tags in main_menu:
            score, reasons, allowed = self._score_item_by_tags(
                name, tags, prefs, alls, rules_profile=rules_profile
            )
            main_scores.append(score)
            main_reasons.append(reasons)
            allowed_main.append(allowed)
        for name, tags in soup_menu:
            score, reasons, allowed = self._score_item_by_tags(
                name, tags, prefs, alls, rules_profile=rules_profile
            )
            soup_scores.append(score)
            soup_reasons.append(reasons)
            allowed_soup.append(allowed)

        incompatible_set = set(incompatible_pairs)

        # Build CP-SAT: one pair must be chosen: main i + soup j.
        model = cp_model.CpModel()

        scale = 1000
        m = len(main_menu)
        s = len(soup_menu)
        pair_vars: dict[tuple[int, int], cp_model.IntVar] = {}

        allowed_pairs: list[tuple[int, int]] = []
        for i in range(m):
            for j in range(s):
                if not allowed_main[i] or not allowed_soup[j]:
                    continue
                if (i, j) in incompatible_set:
                    continue
                allowed_pairs.append((i, j))

        if not allowed_pairs:
            return []

        for i, j in allowed_pairs:
            pair_vars[(i, j)] = model.NewBoolVar(f"pair_main{i}_soup{j}")

        # Exactly one pair.
        model.Add(sum(pair_vars[(i, j)] for (i, j) in allowed_pairs) == 1)

        # Maximize sum of independent scores.
        objective_terms = []
        for i, j in allowed_pairs:
            pair_score_int = int(main_scores[i] * scale) + int(soup_scores[j] * scale)
            objective_terms.append(pair_vars[(i, j)] * pair_score_int)
        model.Maximize(sum(objective_terms))

        solver = cp_model.CpSolver()
        solver.parameters.max_time_in_seconds = time_limit_seconds
        solver.parameters.num_search_workers = 8

        solutions: list[MealPlanPair] = []
        for _ in range(top_k):
            status = solver.Solve(model)
            if status not in (cp_model.OPTIMAL, cp_model.FEASIBLE):
                break

            chosen_pair = None
            for (i, j), var in pair_vars.items():
                if solver.Value(var) == 1:
                    chosen_pair = (i, j)
                    break
            if chosen_pair is None:
                break

            i, j = chosen_pair
            main_item = Recommendation(name=main_names[i], score=main_scores[i], reasons=main_reasons[i])
            soup_item = Recommendation(name=soup_names[j], score=soup_scores[j], reasons=soup_reasons[j])
            plan_score = max(0.0, min(1.0, (main_scores[i] + soup_scores[j]) / 2.0))
            solutions.append(MealPlanPair(main=main_item, soup=soup_item, plan_score=plan_score))

            # No-good constraint: exclude this exact pair in next iterations.
            model.Add(pair_vars[(i, j)] == 0)

        solutions.sort(key=lambda x: x.plan_score, reverse=True)
        return solutions

    def recommend_week_plan_ortools(
        self,
        main_menu: list[tuple[str, list[str]]],
        soup_menu: list[tuple[str, list[str]]],
        dietary_preferences: list[str],
        allergies: list[str],
        incompatible_pairs: list[tuple[int, int]],
        days: list[str],
        all_different_main: bool = True,
        all_different_soup: bool = True,
        top_k: int = 1,
        time_limit_seconds: float = 3.0,
        rules_profile: RulesProfile | None = None,
    ) -> list[WeekPlan]:
        """
        Choose a weekly schedule using CP-SAT:
        - 1 main + 1 soup per day
        - avoid incompatible pairs (main_i, soup_j)
        - optional all-different across days for mains and soups
        """

        try:
            from ortools.sat.python import cp_model  # type: ignore
        except ImportError:  # pragma: no cover
            # Fallback: greedy daily picks using recommend_meal_plan_today_ortools (top_k=1).
            # This keeps the API functional even without OR-Tools installed.
            daily_plans: list[DayPlan] = []
            incompatible_set = set(incompatible_pairs)
            used_main: set[int] = set()
            used_soup: set[int] = set()
            prefs = self._normalize_set(dietary_preferences)
            alls = self._normalize_set(allergies)

            for d in days:
                # Filter incompatible pairs dynamically by excluding used indices.
                dyn_incompatible = list(incompatible_set)
                if all_different_main:
                    dyn_incompatible.extend([(mi, sj) for mi in used_main for sj in range(len(soup_menu))])
                if all_different_soup:
                    dyn_incompatible.extend([(mi, sj) for sj in used_soup for mi in range(len(main_menu))])
                picked = self.recommend_meal_plan_today_ortools(
                    main_menu=main_menu,
                    soup_menu=soup_menu,
                    dietary_preferences=list(prefs),
                    allergies=list(alls),
                    incompatible_pairs=dyn_incompatible,
                    top_k=1,
                    time_limit_seconds=time_limit_seconds,
                )
                if not picked:
                    return []
                # Map picked main/soup by matching name back to indices.
                picked_pair = picked[0]
                mi = next(i for i, (n, _) in enumerate(main_menu) if n == picked_pair.main.name)
                sj = next(j for j, (n, _) in enumerate(soup_menu) if n == picked_pair.soup.name)
                
                # Sử dụng Recommendation model từ picked_pair
                daily_plans.append(DayPlan(day=d, main=picked_pair.main, soup=picked_pair.soup))
                used_main.add(mi)
                used_soup.add(sj)

            plan_score = sum((x.main.score + x.soup.score) / 2.0 for x in daily_plans) / max(1, len(daily_plans))
            return [WeekPlan(days=daily_plans, plan_score=plan_score)]

        prefs = self._normalize_set(dietary_preferences)
        alls = self._normalize_set(allergies)
        incompatible_set = set(incompatible_pairs)

        D = len(days)
        m = len(main_menu)
        s = len(soup_menu)
        if D <= 0:
            return []

        main_names = [x[0] for x in main_menu]
        soup_names = [x[0] for x in soup_menu]

        main_scores: list[float] = []
        soup_scores: list[float] = []
        main_reasons: list[list[str]] = []
        soup_reasons: list[list[str]] = []
        allowed_main: list[bool] = []
        allowed_soup: list[bool] = []

        for name, tags in main_menu:
            score, reasons, allowed = self._score_item_by_tags(
                name, tags, prefs, alls, rules_profile=rules_profile
            )
            main_scores.append(score)
            main_reasons.append(reasons)
            allowed_main.append(allowed)
        for name, tags in soup_menu:
            score, reasons, allowed = self._score_item_by_tags(
                name, tags, prefs, alls, rules_profile=rules_profile
            )
            soup_scores.append(score)
            soup_reasons.append(reasons)
            allowed_soup.append(allowed)

        scale = 1000
        model = cp_model.CpModel()

        pair_vars_by_day: list[dict[tuple[int, int], cp_model.IntVar]] = []
        allowed_pairs_by_day: list[list[tuple[int, int]]] = []

        for _d in range(D):
            allowed_pairs: list[tuple[int, int]] = []
            pair_vars: dict[tuple[int, int], cp_model.IntVar] = {}
            for i in range(m):
                for j in range(s):
                    if not allowed_main[i] or not allowed_soup[j]:
                        continue
                    if (i, j) in incompatible_set:
                        continue
                    allowed_pairs.append((i, j))
                    pair_vars[(i, j)] = model.NewBoolVar(f"pair_day{_d}_main{i}_soup{j}")
            if not allowed_pairs:
                return []

            # Exactly one pair per day.
            model.Add(sum(pair_vars[(i, j)] for (i, j) in allowed_pairs) == 1)
            pair_vars_by_day.append(pair_vars)
            allowed_pairs_by_day.append(allowed_pairs)

        # All-different constraints across days (optional).
        if all_different_main:
            for i in range(m):
                # If this main is used on any day, it must be <= 1 day total.
                model.Add(
                    sum(
                        pair_vars_by_day[d][(i, j)]
                        for d in range(D)
                        for j in range(s)
                        if (i, j) in pair_vars_by_day[d]
                    )
                    <= 1
                )
        if all_different_soup:
            for j in range(s):
                model.Add(
                    sum(
                        pair_vars_by_day[d][(i, j)]
                        for d in range(D)
                        for i in range(m)
                        if (i, j) in pair_vars_by_day[d]
                    )
                    <= 1
                )

        # Objective: maximize sum of per-day (main_score + soup_score).
        objective_terms = []
        for d in range(D):
            for (i, j), var in pair_vars_by_day[d].items():
                pair_score_int = int(main_scores[i] * scale) + int(soup_scores[j] * scale)
                objective_terms.append(var * pair_score_int)
        model.Maximize(sum(objective_terms))

        solver = cp_model.CpSolver()
        solver.parameters.max_time_in_seconds = time_limit_seconds
        solver.parameters.num_search_workers = 8

        solutions: list[WeekPlan] = []
        for _ in range(top_k):
            status = solver.Solve(model)
            if status not in (cp_model.OPTIMAL, cp_model.FEASIBLE):
                break

            chosen_pairs: list[tuple[int, int]] = []
            for d in range(D):
                chosen = None
                for (i, j), var in pair_vars_by_day[d].items():
                    if solver.Value(var) == 1:
                        chosen = (i, j)
                        break
                if chosen is None:
                    return solutions
                chosen_pairs.append(chosen)

            daily_plans: list[DayPlan] = []
            for d, (i, j) in enumerate(chosen_pairs):
                main_rec = Recommendation(name=main_names[i], score=main_scores[i], reasons=main_reasons[i])
                soup_rec = Recommendation(name=soup_names[j], score=soup_scores[j], reasons=soup_reasons[j])
                daily_plans.append(DayPlan(day=days[d], main=main_rec, soup=soup_rec))

            plan_score = (
                sum((x.main.score + x.soup.score) / 2.0 for x in daily_plans) / max(1, len(daily_plans))
            )
            solutions.append(WeekPlan(days=daily_plans, plan_score=plan_score))

            # No-good constraint: forbid repeating the exact same schedule.
            # Since we choose exactly one pair per day, sum over the selected day-pairs must be D.
            selected_sum = []
            for d, (i, j) in enumerate(chosen_pairs):
                selected_sum.append(pair_vars_by_day[d][(i, j)])
            model.Add(sum(selected_sum) <= D - 1)

        solutions.sort(key=lambda x: x.plan_score, reverse=True)
        return solutions