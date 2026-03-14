 SmartLunch – How to Run & API Endpoints

SmartLunch has two main services:

- SmartLunch Backend Service: ASP.NET Core 8.0 API (business logic, DB, auth, SignalR, RabbitMQ).
- SmartLunch AI Service: FastAPI service (chat, recommendation, sentiment analysis).

---

 1. Prerequisites

| Requirement | Version / Details |
|-------------|-------------------|
| .NET SDK | .NET 8.0 or later |
| Python | 3.10+ (recommended) |
| MySQL | Running on `localhost:3307`, database `SmartLunch` |

 MySQL connection

- Host: `localhost`
- Port: `3307`
- Database: `SmartLunch`
- User: `root`
- Password: `DevMySQLRoot@SmartLunch2026`

Ensure MySQL is running and the `SmartLunch` database exists before starting the backend.

---

 2. Running with Docker

You can run infrastructure (MySQL, Kafka, Redis) and the AI service with Docker Compose. The backend can be run on the host (see section 3) or built and run in Docker.

 2.1 Prerequisites

- [Docker](https://docs.docker.com/get-docker/) and [Docker Compose](https://docs.docker.com/compose/install/) installed.

 2.2 Environment variables

Create a `.env` file in the repository root (same folder as `docker-compose.yml`)

 2.3 Start infrastructure and AI service

From the repository root:

```powershell
# Start MySQL, RabbitMQ, Redis, and AI service
docker compose up -d

# View logs
docker compose logs -f
```

| Service    | Port  | URL / Notes                          |
|------------|-------|--------------------------------------|
| MySQL      | 3307  | `localhost:3307` (root + env password) |
| RabbitMQ   | 5672  | AMQP `localhost:5672`                |
| RabbitMQ UI | 15672 | http://localhost:15672 (guest/guest or env) |
| Redis      | 6379  | `localhost:6379` (password from env) |
| AI Service | 8001  | http://localhost:8001 — Docs: http://localhost:8001/docs |

 2.4 Stop Docker services

```powershell
docker compose down
# Optional: remove volumes (deletes DB data)
# docker compose down -v
```

 3. Running the Backend Service (ASP.NET Core)

Path: `SmartLunch-Backend-Service/SmartLunch-Backend-Service-API`

 Steps

```powershell
cd SmartLunch-Backend-Service/SmartLunch-Backend-Service-API

 Restore NuGet packages
dotnet restore

 Run the API (Development)
dotnet run
```

 URLs

| Resource | URL |
|----------|-----|
| Base URL | http://localhost:5001 |
| Swagger UI | http://localhost:5001/swagger |

 Config (from `appsettings.json`)

- Connection string: `Server=localhost;Port=3307;Database=SmartLunch;User=root;Password=DevMySQLRoot@SmartLunch2026`
- JWT: Issuer `SmartLunch-API`, Audience `SmartLunch-Users`
- API versioning: URL segment `/api/v1/...`, query `?api-version=1.0`, header `X-API-Version: 1.0`

---

 4. Running the AI Service (FastAPI)

Path: `SmartLunch-AI-Service`

 Steps

```powershell
cd SmartLunch-AI-Service

 Create virtual environment (Windows)
python -m venv .venv
.venv\Scripts\activate

 Install dependencies
pip install -r requirements.txt

 Run with Uvicorn
uvicorn app.main:app --reload --host 0.0.0.0 --port 8001
```

 URLs

| Resource | URL |
|----------|-----|
| Base URL | http://localhost:8001 |
| Swagger UI | http://localhost:8001/docs |
| OpenAPI JSON | http://localhost:8001/openapi.json |

---

 5. Running Both Services

1. Start MySQL on `localhost:3307` with database `SmartLunch`.
2. Terminal 1 – Backend:
   ```powershell
   cd SmartLunch-Backend-Service/SmartLunch-Backend-Service-API
   dotnet run
   ```
   → Backend: http://localhost:5001 — Swagger: http://localhost:5001/swagger
3. Terminal 2 – AI Service:
   ```powershell
   cd SmartLunch-AI-Service
   .venv\Scripts\activate
   uvicorn app.main:app --reload --host 0.0.0.0 --port 8001
   ```
   → AI Service: http://localhost:8001 — Docs: http://localhost:8001/docs

---

 6. API Endpoints Reference

Base paths:

- Backend: `http://localhost:5001/api/v1`
- AI Service: `http://localhost:8001/api/v1`

---

 6.1 Backend Service (`http://localhost:5001/api/v1`)

 Auth – `api/v1/Auth`

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/v1/Auth/login` | No | Login (email/password) |
| POST | `/api/v1/Auth/firebase-login` | No | Login with Firebase IdToken |
| POST | `/api/v1/Auth/register` | No | Register new user |
| POST | `/api/v1/Auth/logout` | Bearer | Logout |
| POST | `/api/v1/Auth/refresh-token` | No | Refresh JWT |

 Media – `api/v1/Media` (all require Bearer)

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/v1/Media/upload-url` | Create signed upload URL (Firebase Storage) |
| POST | `/api/v1/Media/confirm-upload` | Confirm upload completed |
| GET | `/api/v1/Media/{id}/download-url` | Get signed download URL (optional `?expiresMinutes`) |

 Master Data – Users – `api/v1/master-data/User` (Admin + permission)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/v1/master-data/User` | List users (pagination, search) |
| GET | `/api/v1/master-data/User/{id}` | Get user by ID |

 Master Data – Roles – `api/v1/master-data/Role` (Admin + permission)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/v1/master-data/Role` | List roles |
| GET | `/api/v1/master-data/Role/{id}` | Get role by ID |
| POST | `/api/v1/master-data/Role/grant` | Grant role to user |
| POST | `/api/v1/master-data/Role/revoke` | Revoke role from user |

 Master Data – Permissions – `api/v1/master-data/Permission` (Admin + permission)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/v1/master-data/Permission` | List permissions (optional filters) |
| GET | `/api/v1/master-data/Permission/{id}` | Get permission by ID |
| POST | `/api/v1/master-data/Permission/grant-to-user` | Grant permission to user |
| POST | `/api/v1/master-data/Permission/revoke-from-user` | Revoke permission from user |
| POST | `/api/v1/master-data/Permission/grant-to-role` | Grant permission to role |
| POST | `/api/v1/master-data/Permission/revoke-from-role` | Revoke permission from role |

 SignalR

| Endpoint | Description |
|----------|-------------|
| `/hubs/chat` | Real-time chat hub (token via query `access_token` or header) |

---

 6.2 AI Service (`http://localhost:8001`)

 Health

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/health` | Health check (`{"status":"ok"}`) |

 API v1 – `api/v1`

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/v1/chat` | Chat: send message, get reply with confidence and intent |
| POST | `/api/v1/recommend/today` | Food recommendation for today (menu, preferences, allergies, top_k) |
| POST | `/api/v1/feedback/sentiment` | Sentiment analysis for feedback (text + optional rating) |

Request/response schemas are in Swagger: http://localhost:8001/docs.

---

 7. Quick Reference

| Service | Base URL | Docs |
|---------|----------|------|
| Backend | http://localhost:5001 | http://localhost:5001/swagger |
| AI Service | http://localhost:8001 | http://localhost:8001/docs |

Use the Swagger UIs to explore and test all endpoints interactively.
