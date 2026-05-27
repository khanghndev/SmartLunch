from __future__ import annotations

from dataclasses import dataclass
from datetime import date, timedelta
from typing import Iterable

from ortools.sat.python import cp_model

from app.schemas.industrial import (
    AvailableIngredient,
    DishBom,
    IndustrialIngredientPrepRequest,
    IndustrialIngredientPrepResponse,
    IngredientPrepItem,
    IngredientPrepPlanDay,
)


def _parse_date_yyyy_mm_dd(s: str) -> date:
    return date.fromisoformat(s)


def _daterange(start: date, days: int) -> list[date]:
    return [start + timedelta(days=i) for i in range(days)]


def _to_int_grams(kg: float) -> int:
    # Keep the model integral; use grams to preserve decimals.
    return int(round(max(0.0, kg) * 1000.0))


def _to_float_kg(g: int) -> float:
    return round(max(0, g) / 1000.0, 3)


@dataclass(frozen=True)
class _DemandRow:
    ingredient: str
    day_idx: int
    grams: int


class IngredientPrepPlannerService:
    """
    Suggest ingredient preparation / purchase quantities from upcoming orders.

    Model (per ingredient, per day):
      inv[d] = inv[d-1] + buy[d] - demand[d]
      inv[d] >= safety_stock
      buy[d] >= 0
      optional: inv[d] <= max_inventory

    Optional lead time L (days):
      demand at day d consumes inventory that was bought at day d-L or earlier.
      Implemented by shifting demand forward: inv uses effective_demand[d] = demand[d+L].
    """

    def plan(self, req: IndustrialIngredientPrepRequest) -> IndustrialIngredientPrepResponse:
        start = _parse_date_yyyy_mm_dd(req.start_date)
        horizon_days = int(req.days)
        dates = _daterange(start, horizon_days)

        dish_bom_by_id = {b.dish_id: b for b in req.dish_boms}
        inventory_by_ing: dict[str, int] = {
            a.name.strip().lower(): _to_int_grams(a.quantity_kg) for a in req.available_ingredients
        }

        demand_rows = self._compute_demand_rows(req, dates, dish_bom_by_id)
        demand_by_ing_day = self._aggregate_demand(demand_rows)

        # Solve a single CP-SAT model for all ingredients for better smoothing (optional).
        solved = self._solve_model(
            dates=dates,
            inventory_by_ing=inventory_by_ing,
            demand_by_ing_day=demand_by_ing_day,
            constraints=req.constraints,
            dish_boms=req.dish_boms,
        )

        # Build response
        summary_by_day: list[IngredientPrepPlanDay] = []
        for d, day in enumerate(dates):
            total_demand = sum(demand_by_ing_day.get((ing, d), 0) for ing in solved.ingredients)
            total_buy = sum(solved.buy.get((ing, d), 0) for ing in solved.ingredients)
            total_inv = sum(solved.inv.get((ing, d), 0) for ing in solved.ingredients)
            summary_by_day.append(IngredientPrepPlanDay(
                date=day.isoformat(),
                total_demand_kg=_to_float_kg(total_demand),
                total_buy_kg=_to_float_kg(total_buy),
                total_end_inventory_kg=_to_float_kg(total_inv),
            ))

        ingredient_items: list[IngredientPrepItem] = []
        for ing in sorted(solved.ingredients):
            start_inv = inventory_by_ing.get(ing, 0)
            end_inv = solved.inv.get((ing, horizon_days - 1), start_inv)
            total_demand = sum(demand_by_ing_day.get((ing, d), 0) for d in range(horizon_days))
            total_buy = sum(solved.buy.get((ing, d), 0) for d in range(horizon_days))

            daily = []
            for d, day in enumerate(dates):
                daily.append({
                    "date": day.isoformat(),
                    "demand_kg": _to_float_kg(demand_by_ing_day.get((ing, d), 0)),
                    "buy_kg": _to_float_kg(solved.buy.get((ing, d), 0)),
                    "end_inventory_kg": _to_float_kg(solved.inv.get((ing, d), 0)),
                })

            ingredient_items.append(IngredientPrepItem(
                ingredient_name=ing,
                total_demand_kg=_to_float_kg(total_demand),
                total_buy_kg=_to_float_kg(total_buy),
                start_inventory_kg=_to_float_kg(start_inv),
                end_inventory_kg=_to_float_kg(end_inv),
                daily=daily,
            ))

        return IndustrialIngredientPrepResponse(
            start_date=req.start_date,
            days=horizon_days,
            summary_by_day=summary_by_day,
            ingredients=ingredient_items,
        )

    def _compute_demand_rows(
        self,
        req: IndustrialIngredientPrepRequest,
        dates: list[date],
        dish_bom_by_id: dict[int, DishBom],
    ) -> list[_DemandRow]:
        idx_by_date = {d.isoformat(): i for i, d in enumerate(dates)}
        rows: list[_DemandRow] = []

        for item in req.order_items:
            d_idx = idx_by_date.get(item.service_date)
            if d_idx is None:
                continue
            bom = dish_bom_by_id.get(item.dish_id)
            if bom is None:
                continue
            qty = max(0, int(item.quantity_meals))
            if qty == 0:
                continue

            for line in bom.ingredients:
                ing = line.ingredient_name.strip().lower()
                grams = _to_int_grams(line.quantity_kg_per_meal) * qty
                if grams <= 0:
                    continue
                rows.append(_DemandRow(ingredient=ing, day_idx=d_idx, grams=grams))

        return rows

    @staticmethod
    def _aggregate_demand(rows: Iterable[_DemandRow]) -> dict[tuple[str, int], int]:
        out: dict[tuple[str, int], int] = {}
        for r in rows:
            k = (r.ingredient, r.day_idx)
            out[k] = out.get(k, 0) + r.grams
        return out

    @dataclass
    class _Solved:
        ingredients: set[str]
        buy: dict[tuple[str, int], int]
        inv: dict[tuple[str, int], int]

    def _solve_model(
        self,
        *,
        dates: list[date],
        inventory_by_ing: dict[str, int],
        demand_by_ing_day: dict[tuple[str, int], int],
        constraints,
        dish_boms: list[DishBom],
    ) -> _Solved:
        D = len(dates)
        lead = int(getattr(constraints, "lead_time_days", 0) or 0)
        safety = _to_int_grams(float(getattr(constraints, "safety_stock_kg", 0.0) or 0.0))
        max_inv = getattr(constraints, "max_inventory_kg", None)
        max_inv_g = _to_int_grams(float(max_inv)) if max_inv is not None else None
        holding_w = float(getattr(constraints, "holding_weight", 0.0) or 0.0)
        smooth_w = float(getattr(constraints, "smooth_weight", 0.0) or 0.0)

        # Ingredient universe: from BOMs + inventory + demand
        ingredients: set[str] = set(inventory_by_ing.keys())
        for b in dish_boms:
            for line in b.ingredients:
                ingredients.add(line.ingredient_name.strip().lower())
        for (ing, _d) in demand_by_ing_day.keys():
            ingredients.add(ing)

        model = cp_model.CpModel()
        buy: dict[tuple[str, int], cp_model.IntVar] = {}
        inv: dict[tuple[str, int], cp_model.IntVar] = {}

        # Create vars
        for ing in ingredients:
            for d in range(D):
                buy[(ing, d)] = model.NewIntVar(0, 10**9, f"buy_{ing}_{d}")
                inv[(ing, d)] = model.NewIntVar(0, 10**9, f"inv_{ing}_{d}")

        # Inventory balance with lead time (shift demand forward)
        for ing in ingredients:
            start_inv = int(inventory_by_ing.get(ing, 0))
            for d in range(D):
                effective_demand = demand_by_ing_day.get((ing, d + lead), 0) if (d + lead) < D else 0
                if d == 0:
                    model.Add(inv[(ing, d)] == start_inv + buy[(ing, d)] - effective_demand)
                else:
                    model.Add(inv[(ing, d)] == inv[(ing, d - 1)] + buy[(ing, d)] - effective_demand)

                if safety > 0:
                    model.Add(inv[(ing, d)] >= safety)
                if max_inv_g is not None:
                    model.Add(inv[(ing, d)] <= max_inv_g)

        # Objective: holding + smoothness; optionally include ingredient cost if provided in BOM
        cost_per_kg: dict[str, float] = {}
        for b in dish_boms:
            for line in b.ingredients:
                if line.cost_per_kg is not None:
                    cost_per_kg[line.ingredient_name.strip().lower()] = float(line.cost_per_kg)

        objective_terms = []
        for ing in ingredients:
            cpk = cost_per_kg.get(ing, 1.0)
            # Convert kg cost into gram cost in integer objective space
            buy_cost_per_g = cpk / 1000.0
            for d in range(D):
                if buy_cost_per_g > 0:
                    # Use a scaled int coefficient to keep objective integral.
                    # SCALE=1000: cost_per_g * SCALE approx cost_per_kg
                    objective_terms.append(int(round(buy_cost_per_g * 1000.0)) * buy[(ing, d)])
                if holding_w > 0:
                    objective_terms.append(int(round(holding_w * 1000.0)) * inv[(ing, d)])

            if smooth_w > 0 and D > 1:
                for d in range(1, D):
                    diff = model.NewIntVar(-10**9, 10**9, f"diff_{ing}_{d}")
                    adiff = model.NewIntVar(0, 10**9, f"adiff_{ing}_{d}")
                    model.Add(diff == buy[(ing, d)] - buy[(ing, d - 1)])
                    model.AddAbsEquality(adiff, diff)
                    objective_terms.append(int(round(smooth_w * 1000.0)) * adiff)

        model.Minimize(sum(objective_terms) if objective_terms else 0)

        solver = cp_model.CpSolver()
        solver.parameters.max_time_in_seconds = 10.0
        solver.parameters.num_search_workers = 8
        status = solver.Solve(model)
        if status not in (cp_model.OPTIMAL, cp_model.FEASIBLE):
            # Fallback: buy exactly demand (no lead time), ignore safety
            buy_out: dict[tuple[str, int], int] = {}
            inv_out: dict[tuple[str, int], int] = {}
            for ing in ingredients:
                current = inventory_by_ing.get(ing, 0)
                for d in range(D):
                    dem = demand_by_ing_day.get((ing, d), 0)
                    b = max(0, dem - current)
                    current = current + b - dem
                    buy_out[(ing, d)] = b
                    inv_out[(ing, d)] = current
            return self._Solved(ingredients=ingredients, buy=buy_out, inv=inv_out)

        buy_out = {(ing, d): int(solver.Value(buy[(ing, d)])) for ing in ingredients for d in range(D)}
        inv_out = {(ing, d): int(solver.Value(inv[(ing, d)])) for ing in ingredients for d in range(D)}
        return self._Solved(ingredients=ingredients, buy=buy_out, inv=inv_out)

