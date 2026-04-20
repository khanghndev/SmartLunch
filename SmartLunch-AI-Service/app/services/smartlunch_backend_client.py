from __future__ import annotations

from dataclasses import dataclass
from typing import Any

import httpx

from app.core.config import settings


@dataclass(frozen=True)
class BackendDish:
    id: str
    name: str
    category: str | None
    dietary_label: str | None
    is_active: bool | None = None


@dataclass(frozen=True)
class BackendDishIngredient:
    dish_id: str
    ingredient_name: str | None


def _require_token() -> str:
    token = settings.backend_admin_bearer_token.strip()
    if not token:
        raise RuntimeError(
            "Missing SmartLunch backend admin bearer token. Set "
            "`BACKEND_ADMIN_BEARER_TOKEN` in .env or env vars."
        )
    return token


class SmartLunchBackendClient:
    def __init__(self, base_url: str | None = None, token: str | None = None):
        self._base_url = (base_url or settings.backend_base_url).rstrip("/")
        self._token = (token or settings.backend_admin_bearer_token).strip()

    def _headers(self) -> dict[str, str]:
        return {"Authorization": f"Bearer {_require_token()}"}

    @staticmethod
    def _unwrap_base_api_response(payload: dict[str, Any]) -> Any:
        # Backend uses BaseApiResponse<T> wrapper.
        return payload.get("Data") if "Data" in payload else payload.get("data")

    async def _get_paginated_data(
        self,
        url: str,
        params: dict[str, Any],
        *,
        page_size: int,
    ) -> list[dict[str, Any]]:
        results: list[dict[str, Any]] = []
        page = int(params.get("page", 1))

        while True:
            params["page"] = page
            params["pageSize"] = page_size
            async with httpx.AsyncClient(timeout=30.0) as client:
                resp = await client.get(url, params=params, headers=self._headers())
            resp.raise_for_status()
            payload = resp.json()

            data = self._unwrap_base_api_response(payload) or {}
            items = data.get("Data") if "Data" in data else data.get("data") or data.get("Data", [])
            total_count = int(data.get("TotalCount") if "TotalCount" in data else data.get("totalCount") or 0)
            page_size_eff = int(data.get("PageSize") if "PageSize" in data else data.get("pageSize") or page_size)

            if not isinstance(items, list):
                break
            results.extend(items)

            if page_size_eff <= 0:
                break
            if len(results) >= total_count:
                break

            page += 1

        return results

    async def fetch_dishes_by_category(self, category: str, is_active: bool = True) -> list[BackendDish]:
        url = f"{self._base_url}/api/v1/master-data/Dish"
        page_size = settings.backend_page_size
        params = {"category": category, "isActive": is_active}
        rows = await self._get_paginated_data(url, params=params, page_size=page_size)

        dishes: list[BackendDish] = []
        for r in rows:
            dishes.append(
                BackendDish(
                    id=str(r.get("Id") or r.get("id")),
                    name=str(r.get("Name") or r.get("name")),
                    category=r.get("Category") or r.get("category"),
                    dietary_label=r.get("DietaryLabel") or r.get("dietaryLabel"),
                    is_active=r.get("IsActive") if "IsActive" in r else r.get("isActive"),
                )
            )
        return dishes

    async def fetch_all_dish_ingredients(self) -> list[BackendDishIngredient]:
        url = f"{self._base_url}/api/v1/master-data/DishIngredient"
        page_size = settings.backend_page_size
        params: dict[str, Any] = {}
        rows = await self._get_paginated_data(url, params=params, page_size=page_size)

        items: list[BackendDishIngredient] = []
        for r in rows:
            items.append(
                BackendDishIngredient(
                    dish_id=str(r.get("DishId") or r.get("dishId")),
                    ingredient_name=r.get("IngredientName") or r.get("ingredientName"),
                )
            )
        return items

    async def create_menu_suggestion(
        self,
        *,
        week_start,
        suggestion_text: str,
        algorithm_version: str | None = None,
    ) -> str | None:
        """
        POST to backend endpoint:
        POST /api/v1/master-data/MenuSuggestion
        """

        url = f"{self._base_url}/api/v1/master-data/MenuSuggestion"

        payload = {
            "weekStart": week_start.isoformat(),
            "suggestionText": suggestion_text,
            "algorithmVersion": algorithm_version,
        }

        async with httpx.AsyncClient(timeout=30.0) as client:
            resp = await client.post(url, json=payload, headers=self._headers())
        resp.raise_for_status()

        data = resp.json()
        unwrapped = self._unwrap_base_api_response(data) or {}

        menu = (
            unwrapped.get("MenuSuggestion")
            or unwrapped.get("menuSuggestion")
            or unwrapped.get("menusuggestion")
            or {}
        )
        if not isinstance(menu, dict):
            return None

        return str(menu.get("Id") or menu.get("id") or menu.get("ID") or "") or None

