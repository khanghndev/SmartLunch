# SmartLunch Backend Service

Tài liệu tổng hợp cho **SmartLunch-Backend-Service** — API nghiệp vụ chính của hệ thống SmartLunch (đặt cơm, kho, đối tác, RBAC, v.v.), xây dựng trên **ASP.NET Core 8.0**.

> **Chạy nhanh, cổng, Docker, AI Service:** xem file ở thư mục gốc repo: [`RUN_AND_API.md`](../RUN_AND_API.md).

---

## 1. Vai trò trong hệ thống

- REST API **versioned** (`/api/v1/...`) cho web admin, mobile, và tích hập khác.
- **Xác thực JWT** (Bearer), phân quyền **RBAC** (role + permission động).
- **Entity Framework Core** + **MySQL** (schema tham chiếu trong `SmartLunch-Backend-Service-Infrastructure/Data/db.txt`).
- **SignalR** hub chat (`/hubs/chat`).
- **RabbitMQ** (publish/consume sự kiện; consumer chạy nền trong API).
- **Redis** (tùy cấu hình; fallback bộ nhớ nếu không có Redis).
- Tích hợp **Firebase** (đăng nhập) và **Appwrite Storage** (media).

---

## 2. Kiến trúc solution (Clean Architecture)

| Project | Mô tả ngắn |
|---------|------------|
| **SmartLunch.Backend.Service.API** | HTTP controllers, Swagger, JWT, SignalR, middleware, authorization policies. |
| **SmartLunch.Backend.Service.Application** | Use cases: MediatR commands/queries, DTOs, interfaces, AutoMapper profiles. |
| **SmartLunch.Backend.Service.Domain** | Entities (User, Order, Dish, Unit, …). |
| **SmartLunch.Backend.Service.Infrastructure** | EF Core `DbContext`, repositories, `db.txt`, adapters (cache, message queue, …). |

Thư viện dùng chung (ngoài folder này): `SmartLunch-Shared-MessageQueue-Dotnet` — hàng đợi tin nhắn.

**Luồng điển hình:** `Controller` → `MediatR` → `Handler` → `I*Repository` → `DbContext` / dịch vụ ngoài.

---

## 3. Công nghệ chính

- **.NET 8**, **ASP.NET Core Web API**
- **MediatR** — CQRS nhẹ (command/query)
- **Pomelo.EntityFrameworkCore.MySql** — MySQL
- **JWT Bearer** — authentication
- **Dynamic authorization** — policy dạng `roles:Admin,SuperAdmin` và `permission:users.read`, …
- **API Versioning** — URL `api/v{version}`, header `X-API-Version`, query `api-version`
- **Swagger / Swashbuckle** — `/swagger`
- **Serilog** — logging theo cấu hình
- **StackExchange.Redis** — distributed cache (optional)
- **SignalR** — real-time
- **Scrutor** — đăng ký repository/interface trong Application & Infrastructure

---

## 4. Cấu trúc thư mục (rút gọn)

```
SmartLunch-Backend-Service/
├── SmartLunch-Backend-Service-API/
│   ├── Controllers/v1/          # API v1 (Auth, master-data, sales, inventory, …)
│   ├── Authorization/           # Role & Permission handlers, dynamic policy provider
│   ├── Hubs/                    # SignalR
│   ├── Middlewares/
│   ├── Program.cs
│   └── appsettings*.json
├── SmartLunch-Backend-Service-Application/
│   ├── Commands/                # MediatR commands
│   ├── Queries/                 # MediatR queries
│   ├── DTOs/
│   ├── Interfaces/              # IUserRepository, IOrderRepository, …
│   └── Constants/               # Order status, catalog roles, …
├── SmartLunch-Backend-Service-Domain/
│   └── Entities/
├── SmartLunch-Backend-Service-Infrastructure/
│   ├── Data/
│   │   ├── SmartLunchDBContext.cs
│   │   └── db.txt               # Script MySQL (schema + seed mẫu)
│   └── Repositories/
└── README.md                    # (file này)
```

---

## 5. Chạy & cấu hình

1. Cài **.NET 8 SDK**, **MySQL** (database và connection string khớp `appsettings`).
2. Từ thư mục API:
   ```powershell
   cd SmartLunch-Backend-Service-API
   dotnet restore
   dotnet run
   ```
3. Swagger (mặc định dev): `http://localhost:5001/swagger` (cổng có thể khác theo `launchSettings.json`).

**Connection string, JWT Issuer/Audience, Redis, RabbitMQ:** xem `appsettings.json` / `appsettings.Development.json`.

Chi tiết cổng, Docker, AI service: [`RUN_AND_API.md`](../RUN_AND_API.md).

---

## 6. Xác thực & phân quyền

- **Đăng nhập:** `AuthController` — ví dụ `POST /api/v1/Auth/login-admin`, `login`, `register`, `firebase-login`, `refresh-token`, …
- Token JWT chứa `NameIdentifier` (user id) và các claim role/permission (theo handler đăng nhập).
- **Policy động:**
  - `roles:A,B,C` — user phải có **ít nhất một** role trong danh sách (kiểm tra qua DB/cache theo user id).
  - `permission:resource.action` — user phải có permission tương ứng (gộp từ role + gán trực tiếp user).

Nhiều endpoint **master-data** yêu cầu role **Admin** hoặc **SuperAdmin** cộng thêm permission cụ thể (ví dụ `users.read`).

---

## 7. Nhóm API nghiệp vụ (tổng quan)

Base URL mẫu: `http://localhost:5001/api/v1`

| Nhóm | Route prefix / Controller | Ghi chú |
|------|---------------------------|--------|
| **Auth** | `/api/v1/Auth/*` | Login admin/user, register, Firebase, refresh, logout. |
| **Người dùng & RBAC** | `/api/v1/master-data/User`, `Role`, `Permission`, `UserRole`, `UserPermission`, `RolePermission` | CRUD user (tạo/sửa/khóa), gán role/quyền. |
| **Đơn vị / đối tác** | `Unit`, `Partner`, `Contract`, `PartnerPayment`, … | |
| **Thực đơn & lịch** | `Dish`, `DishIngredient`, `WeeklyMenu`, `MenuSchedule`, `MenuSuggestion` | |
| **Đơn hàng** | `Order`, `OrderItem`, `Delivery`, `Payment`, `Transaction` | |
| **Hóa đơn POS** | `/api/v1/sales/invoices` | Tạo hóa đơn (nhân viên bán). |
| **Kho & nguyên liệu** | `Ingredient`, `IngredientSource`, `Inventory`, `IngredientInventory`, `IngredientIntakeProposal`, … | Phiếu nhập, tồn kho, cảnh báo. |
| **Media** | `/api/v1/Media/*` | Upload/download (Appwrite Storage: ảnh, video, file media). |
| **Khác** | `Review`, `Complaint`, `Sentiment`, `Chat` (hub), `Catalog`, `Finance`, … | |

Danh sách đầy đủ endpoint: mở **Swagger** hoặc xem bảng rút gọn trong [`RUN_AND_API.md`](../RUN_AND_API.md) (một số endpoint mới có thể chỉ có trên Swagger).

---

## 8. Cơ sở dữ liệu

- **EF Core** map entity → bảng MySQL (chuẩn hóa tên bảng trong `SmartLunchDBContext`).
- File **`SmartLunch-Backend-Service-Infrastructure/Data/db.txt`**: script tạo bảng + dữ liệu mẫu (roles, permissions, user thử nghiệm, …).
- Khi đổi model: cập nhật entity + `DbContext` + migration thủ công hoặc script SQL tương ứng (theo quy trình team).

---

## 9. Tích hợp ngoài API

- **SmartLunch AI Service** (FastAPI): chat, gợi ý món, sentiment — gọi từ client hoặc từ backend tùy thiết kế; xem [`RUN_AND_API.md`](../RUN_AND_API.md).
- **RabbitMQ:** cấu hình trong `Program.cs` / `appsettings`; consumer hosted service trong project API.
- **Appwrite Storage (Media):** bucket quản trị tại `https://cloud.appwrite.io/console/project-syd-69bab7660023ca1dd830/storage/bucket-69bfa6de000fdacda87d` (lưu ảnh/video/media).

---

## 10. Kiểm thử & chất lượng

- Build: `dotnet build SmartLunch.Backend.Service.API.csproj`
- Kiểm tra hợp đồng API: Swagger UI.
- Log: Serilog (file/console tùy cấu hình).

---

## 11. Tài liệu liên quan

| File | Nội dung |
|------|----------|
| [`../RUN_AND_API.md`](../RUN_AND_API.md) | Cách chạy, Docker, bảng endpoint tham khảo, AI service. |
| `SmartLunch-Backend-Service-Infrastructure/Data/db.txt` | Schema & seed MySQL. |
| `SmartLunch-Backend-Service-API/Program.cs` | Đăng ký dịch vụ, JWT, CORS, SignalR, RabbitMQ. |

---

*Tài liệu này mô tả trạng thái tổng quan của backend; chi tiết từng use case nên đối chiếu mã nguồn (handlers) và Swagger.*
