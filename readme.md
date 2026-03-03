## SmartLunch – Backend & AI Services

SmartLunch is composed of two main services:

- **SmartLunch Backend Service**: ASP.NET Core 8.0 API (business logic, DB, auth, SignalR, Kafka).
- **SmartLunch AI Service**: FastAPI service providing chat, recommendation, and sentiment analysis.

---

## 1. Prerequisites

- **.NET SDK**: .NET 8.0 or later
- **Python**: 3.10+ (recommended)
- **MySQL**:
  - **Host**: `localhost`
  - **Port**: `3307`
  - **Database**: `SmartLunch`
  - **User**: `root`
  - **Password**: `DevMySQLRoot@SmartLunch2026`

Make sure the MySQL instance is running and the `SmartLunch` database exists before starting the backend.

---

## 2. Running the SmartLunch Backend Service (ASP.NET Core)

**Project path**: `SmartLunch-Backend-Service/SmartLunch-Backend-Service-API`

### 2.1. Restore and run

cd SmartLunch-Backend-Service/SmartLunch-Backend-Service-API

# Restore NuGet packages
dotnet restore

# Run the API (Development profile)
dotnet run

By default (from launchSettings.json):
Base URL: http://localhost:5001
Swagger UI: http://localhost:5001/swagger
2.2. Database & configuration
Connection string (from appsettings.json):
Server=localhost;Port=3307;Database=SmartLunch;User=root;Password=DevMySQLRoot@SmartLunch2026;
JWT configuration (from appsettings.json):
Issuer: SmartLunch-API
Audience: SmartLunch-Users
When the app starts in Development:
It exposes Swagger at http://localhost:5001/swagger
API versioning is enabled:
URL segment versioning: /api/v{version}/... (e.g. /api/v1/...)
Query string: ?api-version=1.0
Header: X-API-Version: 1.0
2.3. Backend API overview
Base path (versioned): http://localhost:5001/api/v1/...
API documentation: see Swagger UI at http://localhost:5001/swagger
All controllers and endpoints are listed there (methods, request/response schemas, auth requirements).
3. Running the SmartLunch AI Service (FastAPI)
Project path: SmartLunch-AI-Service
3.1. Create virtual environment and install dependencies
cd SmartLunch-AI-Service# Create virtualenv (Windows)python -m venv .venv.venv\Scripts\activate# Install dependenciespip install -r requirements.txt
3.2. Run the FastAPI service (Uvicorn)
uvicorn app.main:app --reload --host 0.0.0.0 --port 8001
The service will be available at:
Base URL: http://localhost:8001
Swagger UI: http://localhost:8001/docs
OpenAPI JSON: http://localhost:8001/openapi.json
3.3. AI API endpoints
Base API prefix: /api/v1
Health check
GET /health
Example: http://localhost:8001/health
Chat
POST /api/v1/chat
Food recommendation (for today)
POST /api/v1/recommend/today
Feedback sentiment analysis
POST /api/v1/feedback/sentiment
Request/response schemas are defined in the FastAPI app and visible at http://localhost:8001/docs.
4. Running both services together
Step 1: Start MySQL on localhost:3307 with the SmartLunch database.
Step 2: Start SmartLunch Backend Service:
  cd SmartLunch-Backend-Service/SmartLunch-Backend-Service-API  dotnet run
Backend available at http://localhost:5001
Swagger at http://localhost:5001/swagger
Step 3: Start SmartLunch AI Service in another terminal:
  cd SmartLunch-AI-Service  .venv\Scripts\activate  uvicorn app.main:app --reload --host 0.0.0.0 --port 8001
AI service available at http://localhost:8001
Docs at http://localhost:8001/docs
Use the backend Swagger and AI Swagger UIs to explore all available endpoints and test them interactively.