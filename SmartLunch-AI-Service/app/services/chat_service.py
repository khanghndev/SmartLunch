from __future__ import annotations

import re


class ChatService:
    """
    Baseline, rule-based chat service.

    Replace this with your real LLM / retrieval / tool-calling pipeline later.
    """

    def infer_intent(self, message: str) -> str | None:
        m = message.lower()
        if any(k in m for k in ["recommend", "suggest", "today", "menu", "eat"]):
            return "recommend_food"
        if any(k in m for k in ["feel", "feedback", "review", "rate", "experience", "taste"]):
            return "evaluate_feedback"
        return None

    def reply(self, message: str) -> tuple[str, float, str | None]:
        intent = self.infer_intent(message)

        cleaned = re.sub(r"\s+", " ", message).strip()
        if not cleaned:
            return ("Please send a message.", 0.2, None)

        if intent == "recommend_food":
            return (
                "I can recommend today's meal. If you send your available menu (or preferences/allergies), "
                "I’ll rank the best options for you.",
                0.7,
                intent,
            )
        if intent == "evaluate_feedback":
            return (
                "Tell me how today’s meal was (taste, portion, price, delivery time). "
                "I can evaluate the sentiment and summarize your experience.",
                0.7,
                intent,
            )

        return (
            "Hi! I can help you chat, recommend food for today, or evaluate your meal experience. "
            "What would you like to do?",
            0.55,
            None,
        )

