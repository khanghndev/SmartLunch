from fastapi import APIRouter

from app.api.v1.endpoints.chat import router as chat_router
from app.api.v1.endpoints.feedback import router as feedback_router
from app.api.v1.endpoints.recommend import router as recommend_router
from app.api.v1.endpoints.industrial import router as industrial_router

api_router = APIRouter()

api_router.include_router(chat_router, tags=["chat"])
api_router.include_router(recommend_router, tags=["recommendation"])
api_router.include_router(feedback_router, tags=["feedback"])
api_router.include_router(industrial_router, tags=["industrial-kitchen"])