from fastapi import APIRouter, HTTPException

from app.core.rules_loader import RulesLoader
from app.schemas.industrial import (
    IndustrialWeekPlanRequest,
    IndustrialWeekPlanResponse,
)
from app.services.industrial_planner_service import IndustrialPlannerService

router = APIRouter()
planner = IndustrialPlannerService()


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

        result = planner.plan_week(
            dishes=req.dishes,
            days=req.days,
            meal_structure=req.meal_structure,
            constraints=req.constraints,
            budget_per_serving=req.budget_per_serving,
            servings_per_day=req.servings_per_day,
            available_ingredients=available_ingredients,
            rules_profile=rules_profile,
            top_k=req.top_k,
        )

        return result

    except Exception as e:
        raise HTTPException(
            status_code=500,
            detail=f"Industrial planner failed: {e}",
        )
