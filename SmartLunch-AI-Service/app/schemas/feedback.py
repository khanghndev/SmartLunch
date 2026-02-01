from __future__ import annotations

from pydantic import BaseModel, Field


class SentimentRequest(BaseModel):
    user_id: str | None = Field(default=None, examples=["u_123"])
    text: str = Field(..., min_length=1, examples=["The meal was delicious and the portion was great."])
    rating: int | None = Field(default=None, ge=1, le=5, examples=[5])


class SentimentResponse(BaseModel):
    sentiment: str = Field(..., examples=["positive", "neutral", "negative"])
    score: float = Field(..., ge=-1.0, le=1.0)
    keywords: list[str] = Field(default_factory=list, examples=[["delicious", "great"]])
    summary: str = Field(..., examples=["Mostly positive feedback about taste and portion."])

