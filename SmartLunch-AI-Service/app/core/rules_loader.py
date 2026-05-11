from __future__ import annotations

import json
import os
from dataclasses import dataclass, field
from typing import Any


@dataclass(frozen=True)
class RulesProfile:
    scoring: dict[str, Any]
    allergy_filter: dict[str, Any]
    incompatible_pairs: dict[str, Any]
    constraints: dict[str, Any] = field(default_factory=dict)
    ingredient_groups: dict[str, list[str]] = field(default_factory=dict)
    """Groups of ingredient NameEnglish. 
    Keys: protein, seafood, vegetable, spice, etc. 
    Values: list of ingredient keywords."""


def _default_profile() -> RulesProfile:
    return RulesProfile(
        scoring={
            "base_score": 0.35,
            "tag_weights": {
                "healthy": 0.15,
                "high_protein": 0.12,
                "high_fiber": 0.08,
                "low_carb": 0.10,
                "vegetarian": 0.08,
                "vegan": 0.10,
                "spicy": 0.05,
                "comfort": 0.05,
                "budget": 0.10
            },
            "preference_match_bonus": 0.2,
            "preference_miss_penalty": 0.05,
        },
        allergy_filter={"enabled": True, "substring_match": True},
        incompatible_pairs={"mode": "ingredient_keyword_overlap", "keywords": []},
        constraints={},
    )


class RulesLoader:
    """
    Load rules.json from disk.
    Requirement: read rules file on each request so you can edit the file without redeploy.
    """

    def __init__(self, rules_path: str | None = None):
        self._rules_path = rules_path or os.path.join(
            os.path.dirname(__file__),
            "rules.json",
        )

    def load_profile(self, profile_key: str = "default") -> RulesProfile:
        try:    
            with open(self._rules_path, "r", encoding="utf-8") as f:
                raw = json.load(f)
            
            profiles = raw.get("profiles", {})
            default_raw = profiles.get("default", {})
            # Profile keys in rules.json are lowercase; UI may send Org_elementary, etc.
            key = (profile_key or "default").strip()
            lookup_key = key.lower() if key != "default" else "default"
            profile_raw = profiles.get(lookup_key, {}) if lookup_key != "default" else {}
            if not profile_raw and lookup_key != "default" and key != lookup_key:
                profile_raw = profiles.get(key, {})
            
            if not profile_raw and lookup_key != "default":
                # If profile not found, fallback to default
                profile_raw = default_raw

            # ── Inherit and Merge ──
            # 1. Ingredient Groups: Always take from default, then override/extend with profile-specific
            groups = default_raw.get("ingredient_groups", {}).copy()
            groups.update(profile_raw.get("ingredient_groups", {}))
            
            # 2. Constraints: Same logic
            constraints = default_raw.get("constraints", {}).copy()
            constraints.update(profile_raw.get("constraints", {}))

            return RulesProfile(
                scoring=profile_raw.get("scoring", default_raw.get("scoring", {})),
                allergy_filter=profile_raw.get("allergy_filter", default_raw.get("allergy_filter", {})),
                incompatible_pairs=profile_raw.get("incompatible_pairs", default_raw.get("incompatible_pairs", {})),
                constraints=constraints,
                ingredient_groups=groups,
            )
        except FileNotFoundError:
            return _default_profile()
        except Exception:
            # Never break recommendation flow because of rules parsing.
            return _default_profile()
