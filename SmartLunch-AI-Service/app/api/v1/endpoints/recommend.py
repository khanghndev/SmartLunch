from fastapi import APIRouter, HTTPException

from app.schemas.recommendation import (
    RecommendTodayRequest, 
    RecommendTodayResponse, 
    Recommendation,
    RecommendMealPlanTodayRequest,
    RecommendMealPlanTodayResponse,
    MealPlanPair,
    RecommendWeekPlanRequest,
    RecommendWeekPlanResponse,
    RecommendMealPlanTodayFromBackendRequest,
    RecommendMealPlanTodayFromBackendResponse
)
from app.services.recommendation_service import RecommendationService
from app.core.rules_loader import RulesLoader

router = APIRouter()
recommender = RecommendationService()


@router.post("/recommend/today", response_model=RecommendTodayResponse)
def recommend_today(req: RecommendTodayRequest):
    menu_items = None
    if req.menu is not None:
        menu_items = [(m.name, m.tags) for m in req.menu]

    rules_profile = RulesLoader().load_profile(req.rules_key)
    scored = recommender.recommend_today(
        menu_items=menu_items,
        dietary_preferences=req.dietary_preferences,
        allergies=req.allergies,
        top_k=req.top_k,
        rules_profile=rules_profile,
    )

    return RecommendTodayResponse(
        recommendations=[
            Recommendation(name=s.name, score=s.score, reasons=s.reasons)
            for s in scored
        ]
    )

@router.post("/recommend/today/plan", response_model=RecommendMealPlanTodayResponse)
def recommend_today_plan(req: RecommendMealPlanTodayRequest):
    rules_profile = RulesLoader().load_profile(req.rules_key)
    plans = recommender.recommend_meal_plan_today_ortools(
        main_menu=[(m.name, m.tags) for m in req.main_menu],
        soup_menu=[(m.name, m.tags) for m in req.soup_menu],
        dietary_preferences=req.dietary_preferences,
        allergies=req.allergies,
        incompatible_pairs=[(p.main_index, p.soup_index) for p in req.incompatible_pairs],
        top_k=req.top_k,
        rules_profile=rules_profile,
    )

    return RecommendMealPlanTodayResponse(
        plans=[
            MealPlanPair(
                main=Recommendation(name=p.main.name, score=p.main.score, reasons=p.main.reasons),
                soup=Recommendation(name=p.soup.name, score=p.soup.score, reasons=p.soup.reasons),
                plan_score=p.plan_score,
            )
            for p in plans
        ]
    )


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


@router.post(
    "/recommend/today/plan/backend",
    response_model=RecommendMealPlanTodayFromBackendResponse,
)
async def recommend_today_plan_from_backend(req: RecommendMealPlanTodayFromBackendRequest):
    try:
        rules_profile = RulesLoader().load_profile(req.rules_key)

        backend = SmartLunchBackendClient()

        # Fetch dishes by categories
        main_dishes = []
        for c in req.main_categories:
            main_dishes.extend(await backend.fetch_dishes_by_category(c, is_active=True))

        soup_dishes = []
        for c in req.soup_categories:
            soup_dishes.extend(await backend.fetch_dishes_by_category(c, is_active=True))

        # Fetch all dish ingredients once, then build dishId -> ingredient names map.
        all_di = await backend.fetch_all_dish_ingredients()
        dish_to_ingredients: dict[str, list[str]] = {}
        for di in all_di:
            dish_to_ingredients.setdefault(di.dish_id, [])
            if di.ingredient_name:
                dish_to_ingredients[di.dish_id].append(di.ingredient_name.strip().lower())

        def tokenize_dietary_label(s: str | None) -> list[str]:
            if not s:
                return []
            # Split by common delimiters: comma/semicolon/slash/whitespace.
            import re

            parts = re.split(r"[,;/\\s]+", s.strip().lower())
            return [p for p in parts if p]

        def build_menu(dishes):
            menu: list[tuple[str, list[str]]] = []
            for d in dishes:
                tags: set[str] = set()
                if d.category:
                    tags.add(d.category.strip().lower())
                tags.update(tokenize_dietary_label(d.dietary_label))
                for ing in dish_to_ingredients.get(d.id, []):
                    tags.add(ing)
                menu.append((d.name, sorted(tags)))
            return menu

        main_menu = build_menu(main_dishes)
        soup_menu = build_menu(soup_dishes)

        # Compute incompatible pairs based on rules file.
        incompatible_pairs: list[tuple[int, int]] = []
        mode = (rules_profile.incompatible_pairs or {}).get("mode")
        raw_keywords = (rules_profile.incompatible_pairs or {}).get("keywords") or []
        keywords = [str(k).strip().lower() for k in raw_keywords if str(k).strip()]

        if mode == "ingredient_keyword_overlap" and keywords:
            main_ings = [dish_to_ingredients.get(d.id, []) for d in main_dishes]
            soup_ings = [dish_to_ingredients.get(d.id, []) for d in soup_dishes]

            for i in range(len(main_dishes)):
                for j in range(len(soup_dishes)):
                    incompatible = False
                    for k in keywords:
                        if any(k in ing for ing in main_ings[i]) and any(
                            k in ing for ing in soup_ings[j]
                        ):
                            incompatible = True
                            break
                    if incompatible:
                        incompatible_pairs.append((i, j))

        plans = recommender.recommend_meal_plan_today_ortools(
            main_menu=main_menu,
            soup_menu=soup_menu,
            dietary_preferences=req.dietary_preferences,
            allergies=req.allergies,
            incompatible_pairs=incompatible_pairs,
            top_k=req.top_k,
            rules_profile=rules_profile,
        )

        saved_menu_suggestion_id: str | None = None
        if req.save_to_backend:
            from datetime import date, datetime, timedelta, timezone
            import json

            def compute_week_start_dt(s: str | None):
                if s:
                    if len(s) == 10:
                        d = date.fromisoformat(s)
                    else:
                        d = datetime.fromisoformat(s).date()
                    return datetime(d.year, d.month, d.day, tzinfo=timezone.utc)

                now = datetime.now(timezone.utc).date()
                monday = now - timedelta(days=now.weekday())
                return datetime(monday.year, monday.month, monday.day, tzinfo=timezone.utc)

            week_start_dt = compute_week_start_dt(req.week_start)

            suggestion_recommendations = [
                {
                    "main": {
                        "name": p.main.name,
                        "score": p.main.score,
                        "reasons": p.main.reasons,
                    },
                    "soup": {
                        "name": p.soup.name,
                        "score": p.soup.score,
                        "reasons": p.soup.reasons,
                    },
                    "plan_score": p.plan_score,
                }
                for p in plans
            ]

            suggestion_text = json.dumps(
                {
                    "week_start": week_start_dt.date().isoformat(),
                    "algorithm_version": req.algorithm_version,
                    "recommendations": suggestion_recommendations,
                },
                ensure_ascii=False,
            )

            saved_menu_suggestion_id = await backend.create_menu_suggestion(
                week_start=week_start_dt,
                suggestion_text=suggestion_text,
                algorithm_version=req.algorithm_version,
            )

        return RecommendMealPlanTodayFromBackendResponse(
            plans=[
                MealPlanPair(
                    main=Recommendation(
                        name=p.main.name, score=p.main.score, reasons=p.main.reasons
                    ),
                    soup=Recommendation(
                        name=p.soup.name, score=p.soup.score, reasons=p.soup.reasons
                    ),
                    plan_score=p.plan_score,
                )
                for p in plans
            ],
            saved_menu_suggestion_id=saved_menu_suggestion_id,
        )
    except RuntimeError as e:
        raise HTTPException(status_code=500, detail=str(e))
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Backend integration failed: {e}")
