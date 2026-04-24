from __future__ import annotations

from pydantic import BaseModel, Field


class MenuItem(BaseModel):
    name: str = Field(..., min_length=1, examples=["Grilled chicken salad"])
    tags: list[str] = Field(default_factory=list, examples=[["healthy", "high_protein"]])


class RecommendTodayRequest(BaseModel):
    user_id: str | None = Field(default=None, examples=["u_123"])
    # If provided, use these menu items; otherwise fallback to a built-in demo menu.
    menu: list[MenuItem] | None = None
    dietary_preferences: list[str] = Field(default_factory=list, examples=[["vegetarian"]])
    allergies: list[str] = Field(default_factory=list, examples=[["peanut"]])
    rules_key: str = Field(default="default", examples=["default"])
    top_k: int = Field(default=3, ge=1, le=10)


class Recommendation(BaseModel):
    name: str
    score: float = Field(..., ge=0.0, le=1.0)
    reasons: list[str] = Field(default_factory=list)


class RecommendTodayResponse(BaseModel):
    recommendations: list[Recommendation]

class IncompatibleMainSoupPair(BaseModel):
    """
    Disallow selecting (main_index, soup_index) together.
    Indices refer to the provided `main_menu` and `soup_menu` arrays.
    """

    main_index: int = Field(..., ge=0, examples=[0])
    soup_index: int = Field(..., ge=0, examples=[1])


class RecommendMealPlanTodayRequest(BaseModel):
    """
    CP-SAT optimized plan selection: choose exactly 1 main + 1 soup.
    """

    user_id: str | None = Field(default=None, examples=["u_123"])
    main_menu: list[MenuItem] = Field(..., min_length=1)
    soup_menu: list[MenuItem] = Field(..., min_length=1)
    dietary_preferences: list[str] = Field(default_factory=list, examples=[["vegetarian"]])
    allergies: list[str] = Field(default_factory=list, examples=[["peanut"]])
    incompatible_pairs: list[IncompatibleMainSoupPair] = Field(default_factory=list)
    rules_key: str = Field(default="default", examples=["default"])
    top_k: int = Field(default=3, ge=1, le=10)


class RecommendMealPlanTodayFromBackendRequest(BaseModel):
    """
    Endpoint dùng để AI-Service tự gọi Backend lấy `dishes` + `dish_ingredients`,
    sau đó áp rules.json (theo rules_key) và chạy CP-SAT.
    """

    user_id: str | None = Field(default=None, examples=["u_123"])
    dietary_preferences: list[str] = Field(default_factory=list, examples=[["vegetarian"]])
    allergies: list[str] = Field(default_factory=list, examples=[["peanut"]])
    rules_key: str = Field(default="default", examples=["default"])
    top_k: int = Field(default=1, ge=1, le=5)
    main_categories: list[str] = Field(default_factory=lambda: ["man", "xao"])
    soup_categories: list[str] = Field(default_factory=lambda: ["canh"])
    # If true, AI-Service will POST the recommendation to SmartLunch backend
    # into `menu_suggestions` table.
    save_to_backend: bool = Field(default=False)
    algorithm_version: str = Field(default="ortools-cpsat")
    # If not provided, AI-Service will compute Monday of the current week (UTC).
    week_start: str | None = Field(default=None, examples=["2026-03-25"])


class RecommendMealPlanTodayFromBackendResponse(BaseModel):
    plans: list[MealPlanPair]
    saved_menu_suggestion_id: str | None = Field(default=None)


class MealPlanPair(BaseModel):
    main: Recommendation
    soup: Recommendation
    plan_score: float = Field(..., ge=0.0, le=1.0)


class RecommendMealPlanTodayResponse(BaseModel):
    plans: list[MealPlanPair]


class DayPlan(BaseModel):
    day: str
    main: Recommendation
    soup: Recommendation


class WeekPlan(BaseModel):
    days: list[DayPlan]
    plan_score: float = Field(..., ge=0.0, le=1.0)


class RecommendWeekPlanRequest(BaseModel):
    """
    CP-SAT optimized weekly schedule: choose exactly 1 main + 1 soup per day.
    """

    user_id: str | None = Field(default=None, examples=["u_123"])
    main_menu: list[MenuItem] = Field(..., min_length=1)
    soup_menu: list[MenuItem] = Field(..., min_length=1)
    dietary_preferences: list[str] = Field(default_factory=list, examples=[["vegetarian"]])
    allergies: list[str] = Field(default_factory=list, examples=[["peanut"]])
    incompatible_pairs: list[IncompatibleMainSoupPair] = Field(default_factory=list)
    days: list[str] = Field(
        default_factory=lambda: ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday"]
    )
    all_different_main: bool = Field(default=True)
    all_different_soup: bool = Field(default=True)
    rules_key: str = Field(default="default", examples=["default"])
    top_k: int = Field(default=1, ge=1, le=5)


class RecommendWeekPlanResponse(BaseModel):
    plans: list[WeekPlan]
