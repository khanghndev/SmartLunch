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
    name_english: str = Field(
        default="",
        description=(
            "Tên món ăn tiếng Anh — dùng cho AI grouping (protein, ingredient reuse). "
            "Ví dụ: 'Grilled Pork Rib Rice', 'Sour Shrimp Soup'."
        ),
        examples=["Grilled Ginger Chicken"],
    )
    category: DishCategory = Field(..., examples=["main"])
    covers_categories: list[DishCategory] = Field(
        default_factory=list,
        description=(
            "Danh sách slot mà món này có thể thay thế/bao phủ. "
            "Nếu để trống, hệ thống mặc định cover đúng `category`. "
            "Ví dụ phở/mì: cover [main, side, soup] và chỉ cần thêm dessert."
        ),
        examples=[[ "main", "side", "soup" ]],
    )
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


class GroupFrequencyConstraint(BaseModel):
    """Ràng buộc tần suất xuất hiện của một nhóm nguyên liệu."""

    group_name: str = Field(..., examples=["protein", "seafood"])
    min_count: int | None = Field(default=None, ge=0, le=7)
    max_count: int | None = Field(default=None, ge=0, le=7)


class PlanConstraints(BaseModel):
    """Các ràng buộc cho bài toán lập kế hoạch."""

    # Generic constraints (Preferred)
    group_frequencies: list[GroupFrequencyConstraint] = Field(
        default_factory=list,
        description="Danh sách các ràng buộc tần suất theo nhóm nguyên liệu (linh hoạt hơn)",
    )

    # Legacy/Specific constraints (Will be mapped to group_frequencies if group_frequencies is empty)
    max_same_protein_per_week: int | None = Field(
        default=3, ge=1, le=7,
        description="[Legacy] Không quá N bữa cùng loại protein chính / tuần",
    )
    min_fish_per_week: int | None = Field(
        default=2, ge=0, le=7,
        description="[Legacy] Có ít nhất N bữa cá / tuần",
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

    max_consecutive_same_main_dish: int = Field(
        default=3, ge=1, le=7,
        description=(
            "Không cho phép cùng 1 món cover `main` lặp quá N ngày liên tiếp. "
            "Ví dụ N=3: không được ăn phở bò 4 ngày liên tục."
        ),
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
            DishCategory.dessert,
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
    constraints: PlanConstraints | None = Field(
        default=None,
        description="Nếu không truyền, hệ thống dùng constraints từ rules.json (theo rules_key).",
    )
    rules_key: str = Field(default="industrial", examples=["industrial"])
    ingredient_groups: dict[str, list[str]] = Field(
        default_factory=dict,
        description="Phân loại nguyên liệu (từ Database của Backend)",
    )
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


# ─── Lite (top-K) response models ────────────────────────────────────

class IndustrialMenuPlan(BaseModel):
    """1 phương án thực đơn (menu plan) để xếp hạng."""

    rank: int = Field(..., ge=1, description="Thứ hạng (1 là tốt nhất)")
    plan_score: float = Field(..., ge=0.0, le=1.0)
    objective_value: float = Field(..., description="Giá trị objective của CP-SAT (để so sánh tương đối)")
    week_menu: list[IndustrialDayMenu]


class IndustrialMenuPlansResponse(BaseModel):
    """Danh sách K phương án thực đơn, sắp xếp theo điểm giảm dần."""

    plans: list[IndustrialMenuPlan]


class IndustrialMenuPlansRequest(BaseModel):
    """
    Request để sinh nhiều phương án thực đơn (top-K).

    - `meal_structure` mặc định theo yêu cầu hiện tại:
      - Cơm: [main, side, soup, dessert]
      - Mì/phở/bánh canh: món chính sẽ cover [main, side, soup] (dùng `covers_categories`)
    """

    days: list[str] = Field(
        default_factory=lambda: [
            "Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "Chủ nhật",
        ],
        description="Danh sách ngày cần gợi ý (có thể truyền ít hơn 7 ngày)",
    )
    meal_structure: list[DishCategory] = Field(
        default_factory=lambda: [
            DishCategory.main,
            DishCategory.side,
            DishCategory.soup,
            DishCategory.dessert,
        ],
        description="Khung bữa ăn theo từng ngày",
    )
    dishes: list[IndustrialDish] = Field(..., min_length=1, description="Pool món ăn")
    available_ingredients: list[AvailableIngredient] = Field(default_factory=list)
    constraints: PlanConstraints | None = Field(
        default=None,
        description="Nếu không truyền, hệ thống dùng constraints từ rules.json (theo rules_key).",
    )
    budget_per_serving: float = Field(default=25000, gt=0, description="Budget tối đa / suất (VND)")
    rules_key: str = Field(default="industrial")
    ingredient_groups: dict[str, list[str]] = Field(
        default_factory=dict,
        description="Phân loại nguyên liệu (từ Database của Backend)",
    )
    top_k: int = Field(default=3, ge=1, le=10, description="Số lượng phương án menu muốn nhận")
    time_limit_seconds: float = Field(default=5.0, gt=0, le=3600.0, description="Giới hạn thời gian solve cho mỗi phương án (giây, tối đa 3600)")
