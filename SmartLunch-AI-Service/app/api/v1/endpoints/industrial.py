from fastapi import APIRouter, HTTPException

from app.core.rules_loader import RulesLoader
from app.schemas.industrial import (
    IndustrialIngredientPrepRequest,
    IndustrialIngredientPrepResponse,
    IndustrialMenuPlansRequest,
    IndustrialMenuPlansResponse,
    IndustrialWeekPlanRequest,
    IndustrialWeekPlanResponse,
    PlanConstraints,
)
from app.services.industrial_planner_service import IndustrialPlannerService
from app.services.ingredient_prep_planner_service import IngredientPrepPlannerService

router = APIRouter()
planner = IndustrialPlannerService()
prep_planner = IngredientPrepPlannerService()

def _merge_constraints(req_constraints: PlanConstraints | None, profile_constraints: dict) -> PlanConstraints:
    """
    Merge constraint defaults from rules.json with request overrides.

    - Start from `profile_constraints` (rules.json)
    - Override by values explicitly present in request
    """
    base = dict(profile_constraints or {})
    if req_constraints is None:
        return PlanConstraints(**base)

    try:
        overrides = req_constraints.model_dump(exclude_unset=True)  # pydantic v2
    except Exception:
        overrides = req_constraints.dict(exclude_unset=True)  # fallback
    base.update({k: v for k, v in overrides.items() if v is not None})
    return PlanConstraints(**base)


@router.post(
    "/recommend/industrial/week",
    response_model=IndustrialWeekPlanResponse,
    summary="Lập kế hoạch thực đơn tuần cho bếp công nghiệp",
    description=(
        "Sử dụng Google OR-Tools CP-SAT để sinh thực đơn tuần tối ưu "
        "dựa trên ràng buộc nguyên liệu, chi phí, và đa dạng cách chế biến. "
        "Trả về: thực đơn tuần + danh sách mua hàng + ước tính chi phí."
    ),
)
def recommend_industrial_week(req: IndustrialWeekPlanRequest):
    try:
        rules_profile = RulesLoader().load_profile(req.rules_key)

        # Build available_ingredients lookup from request
        available_ingredients: dict[str, float] = {
            ing.name.strip().lower(): ing.quantity_kg
            for ing in req.available_ingredients
        }

        effective_constraints = _merge_constraints(req.constraints, rules_profile.constraints)

        result = planner.plan_week(
            dishes=req.dishes,
            days=req.days,
            meal_structure=req.meal_structure,
            constraints=effective_constraints,
            budget_per_serving=req.budget_per_serving,
            servings_per_day=req.servings_per_day,
            available_ingredients=available_ingredients,
            rules_profile=rules_profile,
            top_k=req.top_k,
            request_ingredient_groups=req.ingredient_groups,
        )

        return result

    except Exception as e:
        raise HTTPException(
            status_code=500,
            detail=f"Industrial planner failed: {e}",
        )


@router.post(
    "/recommend/industrial/menus",
    response_model=IndustrialMenuPlansResponse,
    summary="Sinh top-K thực đơn (không shopping/cost)",
    description=(
        "Sinh nhiều phương án thực đơn theo số ngày và meal_structure mong muốn. "
        "Kết quả trả về danh sách K menu plans kèm điểm, sắp xếp giảm dần."
    ),
)
def recommend_industrial_menus(req: IndustrialMenuPlansRequest):
    try:
        rules_profile = RulesLoader().load_profile(req.rules_key)

        available_ingredients: dict[str, float] = {
            ing.name.strip().lower(): ing.quantity_kg
            for ing in req.available_ingredients
        }

        effective_constraints = _merge_constraints(req.constraints, rules_profile.constraints)

        result = planner.plan_menus_top_k(
            dishes=req.dishes,
            days=req.days,
            meal_structure=req.meal_structure,
            constraints=effective_constraints,
            budget_per_serving=req.budget_per_serving,
            available_ingredients=available_ingredients,
            rules_profile=rules_profile,
            top_k=req.top_k,
            time_limit_seconds=req.time_limit_seconds,
            request_ingredient_groups=req.ingredient_groups,
        )

        return result
    except Exception as e:
        raise HTTPException(
            status_code=500,
            detail=f"Industrial menus (top-k) failed: {e}",
        )


@router.post(
    "/recommend/industrial/ingredients/prepare",
    response_model=IndustrialIngredientPrepResponse,
    summary="Gợi ý lượng nguyên liệu cần chuẩn bị từ đơn hàng sắp tới",
    description=(
        "Tính nhu cầu nguyên liệu theo BOM (định mức kg/suất) và upcoming orders theo ngày, "
        "sau đó dùng Google OR-Tools CP-SAT để đề xuất kế hoạch mua/tồn kho theo từng ngày "
        "(có safety stock, lead time, và làm mượt lượng mua)."
    ),
)
def recommend_ingredient_preparation(req: IndustrialIngredientPrepRequest):
    try:
        return prep_planner.plan(req)
    except Exception as e:
        raise HTTPException(
            status_code=500,
            detail=f"Ingredient preparation planner failed: {e}",
        )
