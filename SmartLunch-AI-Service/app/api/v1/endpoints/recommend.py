from fastapi import APIRouter

from app.schemas.recommendation import RecommendTodayRequest, RecommendTodayResponse, Recommendation
from app.services.recommendation_service import RecommendationService

router = APIRouter()
recommender = RecommendationService()


@router.post("/recommend/today", response_model=RecommendTodayResponse)
def recommend_today(req: RecommendTodayRequest):
    menu_items = None
    if req.menu is not None:
        menu_items = [(m.name, m.tags) for m in req.menu]

    scored = recommender.recommend_today(
        menu_items=menu_items,
        dietary_preferences=req.dietary_preferences,
        allergies=req.allergies,
        top_k=req.top_k,
    )

    return RecommendTodayResponse(
        recommendations=[
            Recommendation(name=s.name, score=s.score, reasons=s.reasons)
            for s in scored
        ]
    )

