from __future__ import annotations

from enum import Enum
from pydantic import BaseModel, Field


# ─── Enums ───────────────────────────────────────────────────────────

class DishCategory(str, Enum):
    main = "main"
    side = "side"
    soup = "soup"
    vegetable = "vegetable"
    noodle_soup = "noodle_soup"
    dessert = "dessert"


class CookingMethod(str, Enum):
    fried = "fried"
    stewed = "stewed"
    boiled = "boiled"
    stir_fried = "stir_fried"
    grilled = "grilled"
    steamed = "steamed"
    raw = "raw"


# ─── Input Models ────────────────────────────────────────────────────

class IndustrialDish(BaseModel):
    """Mô tả đầy đủ 1 món ăn trong pool."""

    name: str = Field(..., min_length=1, examples=["Gà kho gừng"])
    category: DishCategory = Field(..., examples=["main"])
    main_ingredient: str = Field(..., min_length=1, examples=["chicken"])
    sub_ingredients: list[str] = Field(default_factory=list, examples=[["gừng", "nước mắm"]])
    cooking_method: CookingMethod = Field(..., examples=["stewed"])
    cost_per_serving: float = Field(..., gt=0, examples=[12000], description="VND / suất")
    popularity: int = Field(default=3, ge=1, le=5, description="1-5, công nhân thích hay không")
    max_per_week: int = Field(default=2, ge=1, le=7, description="Tần suất tối đa / tuần")
    tags: list[str] = Field(default_factory=list, examples=[["comfort", "traditional"]])


class AvailableIngredient(BaseModel):
    """Nguyên liệu đã có sẵn trong kho."""

    name: str = Field(..., min_length=1, examples=["thịt heo"])
    quantity_kg: float = Field(..., ge=0, examples=[20.0])


class PlanConstraints(BaseModel):
    """Các ràng buộc cho bài toán lập kế hoạch."""

    max_same_protein_per_week: int = Field(
        default=3, ge=1, le=7,
        description="Không quá N bữa cùng loại protein chính / tuần",
    )
    min_fish_per_week: int = Field(
        default=2, ge=0, le=7,
        description="Có ít nhất N bữa cá / tuần",
    )
    no_repeat_main_ingredient_consecutive_days: bool = Field(
        default=True,
        description="Không lặp nguyên liệu chính liên tiếp 2 ngày",
    )
    alternate_cooking_methods: bool = Field(
        default=True,
        description="Xen kẽ chiên – luộc – kho (đỡ ngán)",
    )
    prefer_ingredient_reuse: bool = Field(
        default=True,
        description="Ưu tiên reuse nguyên liệu trong 2–3 ngày gần nhau",
    )


class IndustrialWeekPlanRequest(BaseModel):
    """Request chính cho endpoint lập kế hoạch bếp công nghiệp."""

    servings_per_day: int = Field(
        default=800, ge=1,
        description="Số suất / ngày",
    )
    budget_per_serving: float = Field(
        default=25000, gt=0,
        description="Budget tối đa / suất (VND)",
    )
    days: list[str] = Field(
        default_factory=lambda: [
            "Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "Chủ nhật",
        ],
    )
    meal_structure: list[DishCategory] = Field(
        default_factory=lambda: [
            DishCategory.main,
            DishCategory.side,
            DishCategory.soup,
            DishCategory.vegetable,
        ],
        description="Khung bữa ăn: mỗi ngày phải có đủ các slot này",
    )
    dishes: list[IndustrialDish] = Field(
        ..., min_length=1,
        description="Pool tất cả các món ăn",
    )
    available_ingredients: list[AvailableIngredient] = Field(
        default_factory=list,
        description="Nguyên liệu đã có sẵn trong kho",
    )
    constraints: PlanConstraints = Field(
        default_factory=PlanConstraints,
    )
    rules_key: str = Field(default="industrial", examples=["industrial"])
    top_k: int = Field(default=1, ge=1, le=3)


# ─── Output Models ───────────────────────────────────────────────────

class DishRecommendation(BaseModel):
    """1 món ăn trong thực đơn đã được chọn."""

    name: str
    category: DishCategory
    score: float = Field(..., ge=0.0, le=1.0)
    reasons: list[str] = Field(default_factory=list)
    cost_per_serving: float = Field(..., ge=0)


class IndustrialDayMenu(BaseModel):
    """Thực đơn 1 ngày."""

    day: str
    dishes: list[DishRecommendation]
    day_cost_per_serving: float = Field(
        ..., ge=0,
        description="Tổng chi phí / suất cho ngày này",
    )


class ShoppingItem(BaseModel):
    """1 dòng trong danh sách mua hàng."""

    ingredient_name: str
    total_kg: float = Field(..., ge=0, description="Tổng kg cần cho cả tuần")
    available_kg: float = Field(default=0, ge=0, description="Kg đã có sẵn")
    to_buy_kg: float = Field(..., ge=0, description="Kg cần mua thêm")
    category: str = Field(default="other", examples=["protein", "vegetable", "spice"])


class CostBreakdown(BaseModel):
    """Ước tính chi phí."""

    avg_cost_per_serving: float = Field(..., ge=0)
    total_week_cost: float = Field(..., ge=0)
    within_budget: bool


class IndustrialWeekPlanResponse(BaseModel):
    """Response tổng hợp: Thực đơn + Mua hàng + Chi phí."""

    week_menu: list[IndustrialDayMenu]
    shopping_list: list[ShoppingItem]
    cost_estimate: CostBreakdown
    plan_score: float = Field(..., ge=0.0, le=1.0)
