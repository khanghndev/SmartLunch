from fastapi import APIRouter

from app.schemas.recommendation import (
    RecommendWeekPlanRequest,
    RecommendWeekPlanResponse,
)
from app.services.recommendation_service import RecommendationService
from app.core.rules_loader import RulesLoader

router = APIRouter()
recommender = RecommendationService()


@router.post("/recommend/week/plan", response_model=RecommendWeekPlanResponse)
def recommend_week_plan(req: RecommendWeekPlanRequest):
    rules_profile = RulesLoader().load_profile(req.rules_key)
    plans = recommender.recommend_week_plan_ortools(
        main_menu=[(m.name, m.tags) for m in req.main_menu],
        soup_menu=[(m.name, m.tags) for m in req.soup_menu],
        dietary_preferences=req.dietary_preferences,
        allergies=req.allergies,
        incompatible_pairs=[(p.main_index, p.soup_index) for p in req.incompatible_pairs],
        days=req.days,
        all_different_main=req.all_different_main,
        all_different_soup=req.all_different_soup,
        top_k=req.top_k,
        rules_profile=rules_profile,
    )
    return RecommendWeekPlanResponse(plans=plans)
