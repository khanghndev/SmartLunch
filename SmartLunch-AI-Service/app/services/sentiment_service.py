from __future__ import annotations

import re


class SentimentService:
    """
    Baseline lexicon-based sentiment evaluation.

    Replace with your real NLP model later (transformer, fine-tuned classifier, etc.).
    """

    POSITIVE = {
        "good",
        "great",
        "excellent",
        "amazing",
        "delicious",
        "tasty",
        "fresh",
        "fast",
        "perfect",
        "nice",
        "love",
        "friendly",
        "clean",
        "hot",
        "warm",
        "reasonable",
        "worth",
    }
    NEGATIVE = {
        "bad",
        "terrible",
        "awful",
        "disgusting",
        "cold",
        "late",
        "slow",
        "salty",
        "bland",
        "stale",
        "small",
        "expensive",
        "dirty",
        "hate",
        "poor",
        "burnt",
        "raw",
    }

    def _tokenize(self, text: str) -> list[str]:
        return re.findall(r"[a-zA-Z']+", text.lower())

    def analyze(self, text: str, rating: int | None = None) -> dict:
        tokens = self._tokenize(text)
        pos = [t for t in tokens if t in self.POSITIVE]
        neg = [t for t in tokens if t in self.NEGATIVE]

        raw = (len(pos) - len(neg)) / max(4, (len(pos) + len(neg)))
        score = max(-1.0, min(1.0, raw))

        # Optional rating nudge
        if rating is not None:
            # map 1..5 to -0.6..+0.6
            score = max(-1.0, min(1.0, score + (rating - 3) * 0.3))

        if score >= 0.2:
            sentiment = "positive"
            summary = "Mostly positive feedback."
        elif score <= -0.2:
            sentiment = "negative"
            summary = "Mostly negative feedback."
        else:
            sentiment = "neutral"
            summary = "Mixed or neutral feedback."

        keywords = sorted(set(pos + neg))[:12]
        return {"sentiment": sentiment, "score": float(score), "keywords": keywords, "summary": summary}

