from __future__ import annotations

import json

from pydantic import field_validator
from pydantic_settings import BaseSettings, SettingsConfigDict


class Settings(BaseSettings):
    app_name: str = "SmartLunch AI Service"
    api_v1_prefix: str = "/api/v1"

    # Accepts:
    # - "*" (allow all)
    # - "http://a,http://b" (comma-separated)
    # - '["http://a","http://b"]' (JSON list)
    cors_origins: list[str] = ["*"]

    @field_validator("cors_origins", mode="before")
    @classmethod
    def _parse_cors_origins(cls, v):
        if v is None:
            return ["*"]
        if isinstance(v, list):
            return v
        if isinstance(v, str):
            s = v.strip()
            if not s:
                return ["*"]
            if s == "*":
                return ["*"]
            if s.startswith("["):
                try:
                    parsed = json.loads(s)
                    if isinstance(parsed, list):
                        return [str(x) for x in parsed]
                except Exception:
                    # fall back to comma split
                    pass
            return [p.strip() for p in s.split(",") if p.strip()]
        return ["*"]

    model_config = SettingsConfigDict(
        env_file=".env",
        env_file_encoding="utf-8",
        extra="ignore",
    )


settings = Settings()

