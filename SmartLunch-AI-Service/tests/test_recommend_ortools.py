import pytest


def test_ortools_today_respects_incompatible_pairs():
    pytest.importorskip("ortools")

    from app.services.recommendation_service import RecommendationService

    svc = RecommendationService()

    main_menu = [
        ("main0", ["healthy"]),
        ("main1", ["high_protein"]),
    ]
    soup_menu = [
        ("soup0", ["high_protein"]),
        ("soup1", ["healthy"]),
    ]

    # Disallow (main0, soup0), but allow (main0, soup1).
    plans = svc.recommend_meal_plan_today_ortools(
        main_menu=main_menu,
        soup_menu=soup_menu,
        dietary_preferences=["healthy"],
        allergies=[],
        incompatible_pairs=[(0, 0)],
        top_k=1,
    )

    assert plans, "Expected at least one plan"
    assert plans[0].main.name == "main0"
    assert plans[0].soup.name == "soup1"


def test_ortools_today_filters_allergies():
    pytest.importorskip("ortools")

    from app.services.recommendation_service import RecommendationService

    svc = RecommendationService()

    main_menu = [("m0", ["healthy"])]
    soup_menu = [
        ("s0", ["peanut", "high_protein"]),
        ("s1", ["healthy"]),
    ]

    plans = svc.recommend_meal_plan_today_ortools(
        main_menu=main_menu,
        soup_menu=soup_menu,
        dietary_preferences=[],
        allergies=["peanut"],
        incompatible_pairs=[],
        top_k=1,
    )

    assert plans
    assert plans[0].soup.name == "s1"


def test_ortools_week_all_different_main():
    pytest.importorskip("ortools")

    from app.services.recommendation_service import RecommendationService

    svc = RecommendationService()

    main_menu = [
        ("m0", ["healthy"]),
        ("m1", ["high_protein"]),
        ("m2", ["high_fiber"]),
    ]
    soup_menu = [
        ("s0", ["healthy"]),
        ("s1", ["high_protein"]),
    ]

    days = ["Monday", "Tuesday", "Wednesday"]
    plans = svc.recommend_week_plan_ortools(
        main_menu=main_menu,
        soup_menu=soup_menu,
        dietary_preferences=["healthy"],
        allergies=[],
        incompatible_pairs=[],
        days=days,
        all_different_main=True,
        all_different_soup=False,
        top_k=1,
    )

    assert plans
    week = plans[0]
    assert [d.main.name for d in week.days] == ["m0", "m1", "m2"] or len(
        {d.main.name for d in week.days}
    ) == 3

