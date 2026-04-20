# SmartLunch AI Service (FastAPI)

FastAPI-based AI Model Service for SmartLunch with 3 capabilities:

- ChatBot (Q&A / assistant)
- Recommend food for today
- Evaluate customer feelings/experience about today's meal (sentiment)

## Quickstart (local)

Create a virtualenv, install dependencies, then run:

```bash
cd SmartLunch-AI-Service
python -m venv .venv
.venv\Scripts\activate
pip install -r requirements.txt
uvicorn app.main:app --reload --host 0.0.0.0 --port 8001
```

Open docs:

- Swagger UI: `http://localhost:8001/docs`
- OpenAPI JSON: `http://localhost:8001/openapi.json`

## API

Base path: `/api/v1`

- `GET /health`
- `POST /api/v1/chat`
- `POST /api/v1/recommend/today`
- `POST /api/v1/recommend/today/plan` (CP-SAT chọn 1 món mặn + 1 món canh)
- `POST /api/v1/recommend/today/plan/backend` (AI-Service gọi SmartLunch Backend để lấy `dishes` + `dish_ingredients`, sau đó áp `app/core/rules.json`)
- `POST /api/v1/recommend/week/plan` (CP-SAT lịch theo tuần)
- `POST /api/v1/feedback/sentiment`

## Notes
- Rules config được đặt tại `app/core/rules.json` và được AI-Service đọc lại theo từng request (sửa file là có hiệu lực ngay).
- Để endpoint `/recommend/today/plan/backend` gọi SmartLunch Backend lấy dữ liệu, cần cấu hình `.env`:
  - `BACKEND_BASE_URL`
  - `BACKEND_ADMIN_BEARER_TOKEN`
- This project ships with **simple baseline logic** (rule-based) to make the APIs usable immediately.
- Replace implementations inside `app/services/` with your real models later (LLM, recommender, sentiment model).

