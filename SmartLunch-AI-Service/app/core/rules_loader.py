from __future__ import annotations

import json
import os
from dataclasses import dataclass
from typing import Any


@dataclass(frozen=True)
class RulesProfile:
    scoring: dict[str, Any]
    allergy_filter: dict[str, Any]
    incompatible_pairs: dict[str, Any]


def _default_profile() -> RulesProfile:
    return RulesProfile(
        scoring={
            "base_score": 0.35,
            "tag_weights": {"healthy": 0.15, "high_protein": 0.12, "high_fiber": 0.08},
            "preference_match_bonus": 0.2,
            "preference_miss_penalty": 0.05,
        },
        allergy_filter={"enabled": True, "substring_match": True},
        incompatible_pairs={"mode": "ingredient_keyword_overlap", "keywords": []},
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
            profile = raw.get("profiles", {}).get(profile_key)
            if not profile:
                return _default_profile()

            return RulesProfile(
                scoring=profile.get("scoring", {}),
                allergy_filter=profile.get("allergy_filter", {}),
                incompatible_pairs=profile.get("incompatible_pairs", {}),
            )
        except FileNotFoundError:
            return _default_profile()
        except Exception:
            # Never break recommendation flow because of rules parsing.
            return _default_profile()

