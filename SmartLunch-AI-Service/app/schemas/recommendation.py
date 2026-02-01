from __future__ import annotations

from pydantic import BaseModel, Field


class MenuItem(BaseModel):
    name: str = Field(..., min_length=1, examples=["Grilled chicken salad"])
    tags: list[str] = Field(default_factory=list, examples=[["healthy", "high_protein"]])


class RecommendTodayRequest(BaseModel):
    user_id: str | None = Field(default=None, examples=["u_123"])
    # If provided, use these menu items; otherwise fallback to a built-in demo menu.
    menu: list[MenuItem] | None = None
    dietary_preferences: list[str] = Field(default_factory=list, examples=[["vegetarian"]])
    allergies: list[str] = Field(default_factory=list, examples=[["peanut"]])
    top_k: int = Field(default=3, ge=1, le=10)


class Recommendation(BaseModel):
    name: str
    score: float = Field(..., ge=0.0, le=1.0)
    reasons: list[str] = Field(default_factory=list)


class RecommendTodayResponse(BaseModel):
    recommendations: list[Recommendation]

