from fastapi import APIRouter

from app.schemas.feedback import SentimentRequest, SentimentResponse
from app.services.sentiment_service import SentimentService

router = APIRouter()
sentiment_service = SentimentService()


@router.post("/feedback/sentiment", response_model=SentimentResponse)
def feedback_sentiment(req: SentimentRequest):
    result = sentiment_service.analyze(req.text, rating=req.rating)
    return SentimentResponse(**result)

