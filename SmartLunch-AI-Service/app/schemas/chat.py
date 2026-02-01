from __future__ import annotations

from pydantic import BaseModel, Field


class ChatMessage(BaseModel):
    role: str = Field(..., examples=["user", "assistant"])
    content: str = Field(..., min_length=1, examples=["Hello"])


class ChatRequest(BaseModel):
    user_id: str | None = Field(default=None, examples=["u_123"])
    message: str = Field(..., min_length=1, examples=["Suggest a healthy lunch today"])
    history: list[ChatMessage] = Field(default_factory=list)


class ChatResponse(BaseModel):
    reply: str
    confidence: float = Field(..., ge=0.0, le=1.0)
    intent: str | None = Field(default=None, examples=["recommend_food"])

