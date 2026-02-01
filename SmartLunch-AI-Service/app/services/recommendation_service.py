from __future__ import annotations

from dataclasses import dataclass


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

    DEFAULT_MENU: list[tuple[str, list[str]]] = [
        ("Grilled chicken salad", ["healthy", "high_protein", "low_oil"]),
        ("Pho bo (beef noodle soup)", ["soup", "comfort", "high_protein"]),
        ("Vegetable tofu bowl", ["vegetarian", "healthy", "high_fiber"]),
        ("Banh mi (pork)", ["quick", "budget"]),
        ("Salmon rice bowl", ["healthy", "omega3", "high_protein"]),
        ("Fruit yogurt parfait", ["light", "dessert", "healthy"]),
    ]

    def recommend(
        self,
        menu: list[tuple[str, list[str]]],
        dietary_preferences: list[str],
        allergies: list[str],
        top_k: int,
    ) -> list[ScoredItem]:
        prefs = {p.strip().lower() for p in dietary_preferences if p.strip()}
        alls = {a.strip().lower() for a in allergies if a.strip()}

        results: list[ScoredItem] = []
        for name, tags in menu:
            tagset = {t.strip().lower() for t in tags}
            reasons: list[str] = []

            # Allergy filtering (very simple heuristic: if allergy word appears in tags or name).
            if any(a in name.lower() or a in tagset for a in alls):
                continue

            score = 0.35  # base
            if "healthy" in tagset:
                score += 0.15
                reasons.append("Healthy option")
            if "high_protein" in tagset:
                score += 0.12
                reasons.append("High protein")
            if "high_fiber" in tagset:
                score += 0.08
                reasons.append("High fiber")

            # Preferences boost
            if prefs:
                matched = sorted(prefs.intersection(tagset))
                if matched:
                    score += 0.2
                    reasons.append(f"Matches preferences: {', '.join(matched)}")
                else:
                    # Light penalty if user has preferences but item doesn't match any
                    score -= 0.05

            score = max(0.0, min(1.0, score))
            if not reasons:
                reasons.append("Popular choice")
            results.append(ScoredItem(name=name, score=score, reasons=reasons))

        results.sort(key=lambda x: x.score, reverse=True)
        return results[:top_k]

    def recommend_today(
        self,
        menu_items: list[tuple[str, list[str]]] | None,
        dietary_preferences: list[str],
        allergies: list[str],
        top_k: int,
    ) -> list[ScoredItem]:
        menu = menu_items or self.DEFAULT_MENU
        return self.recommend(menu=menu, dietary_preferences=dietary_preferences, allergies=allergies, top_k=top_k)

