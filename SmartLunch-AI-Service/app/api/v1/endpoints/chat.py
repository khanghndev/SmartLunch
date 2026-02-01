from fastapi import APIRouter

from app.schemas.chat import ChatRequest, ChatResponse
from app.services.chat_service import ChatService

router = APIRouter()
chat_service = ChatService()


@router.post("/chat", response_model=ChatResponse)
def chat(req: ChatRequest):
    reply, confidence, intent = chat_service.reply(req.message)
    return ChatResponse(reply=reply, confidence=confidence, intent=intent)

