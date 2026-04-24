# Tài Liệu Tổng Quan Dự Án SmartLunch

Dự án **SmartLunch** là một hệ thống đa dịch vụ (microservices-oriented) tiên tiến, chuyên biệt trong việc hỗ trợ quản lý suất ăn, kết hợp giữa các tính năng quản lý truyền thống và trí tuệ nhân tạo (AI/Gợi ý món/Xử lý ngôn ngữ tự nhiên). 

Dưới đây là một cái nhìn tổng quan toàn diện, chi tiết về toàn bộ hệ thống SmartLunch đang có trong kho lưu trữ.

---

## 1. Kiến Trúc Tổng Thể

Hệ thống được chia làm hai Service chính cùng với một hệ sinh thái Cơ sở hạ tầng đi kèm:

1. **SmartLunch Backend Service**: Dịch vụ nền tảng cốt lõi phục vụ nghiệp vụ (Business Logic), quản lý thông tin, Authentication/Authorization, phân quyền hệ thống (RBAC), và giao tiếp Real-time.
2. **SmartLunch AI Service**: Dịch vụ máy học chuyên biệt nằm tách rời, được gọi để giải quyết các bài toán tối ưu hoá (Lập lịch ăn, AI Recommendation) và hiểu ngôn ngữ, phân tích cảm xúc của người dùng.
3. **SmartLunch Shared Message Queue**: Thư viện dùng chung (`.dll` / package) cho .NET giúp đồng nhất việc giao tiếp bất đồng bộ qua hệ thống Message Broker (RabbitMQ).

### Sơ đồ luồng giao tiếp cơ bản

Người dùng (Client/App) 
-> `Backend Service (ASP.NET Core)` 
-> Giao tiếp với `DB/Cache/Storage` 
-> Có thể đẩy Event vào `RabbitMQ` hoặc gọi trực tiếp `AI Service (FastAPI)` thông qua REST API để thực hiện tính toán gợi ý.

---

## 2. Công Nghệ Sử Dụng

### 2.1 Backend Service
- **Nền tảng**: .NET 8.0 (C#) & ASP.NET Core API
- **Giao tiếp Real-time**: SignalR (ChatHub)
- **Xác thực & Bảo mật (Auth)**: JWT Token, tích hợp Firebase Authentication (đăng nhập bằng Token bên thứ 3)
- **Lưu trữ tệp (Storage)**: Tích hợp với **Appwrite Storage** (Quản lý các bucket cloud thông qua API ký URL upload/download)
- **RabbitMQ API / Broker**: Xử lý các tác vụ bất đồng bộ, sử dụng thư viện `SmartLunch-Shared-MessageQueue-Dotnet` custom tự xây.

### 2.2 AI Service
- **Nền tảng**: Python 3.10+ & FastAPI
- **Web Server**: Uvicorn
- **Logic / Thuật toán AI**: 
  - Sentiment Analysis (Phân tích cảm xúc phản hồi).
  - Hệ tư vấn món ăn rèn luyện / luật (Rule-based kết hợp CP-SAT / Constraint Programming) nhằm chọn ra tổ hợp món ăn thỏa mãn điều kiện dinh dưỡng, sở thích và dị ứng (ví dụ: tối thiểu 1 món canh + 1 món mặn).

### 2.3 Cơ Sở Hạ Tầng (Infrastructure - Docker Compose)
- **MySQL 8.0**: RDBMS chính của hệ thống. Chạy qua port `3307` để tránh xung đột với MySQL máy ảo mặc định. (Database: `SmartLunch`).
- **Redis 7-alpine**: Cơ sở dữ liệu In-Memory dùng làm hệ thống Caching, tăng tốc độ truy vấn data.
- **RabbitMQ 3-management**: Message Broker, dùng để truyền tải các Message Bus/Event Driven, quản lý qua port UI `15672` và kết nối AMQP qua port `5672`.
- **Docker Compose**: Đóng gói và chạy tự động toàn bộ MySQL, Redis, RabbitMQ và AI Service bằng file `docker-compose.yml`.

---

## 3. Các Tính Năng / Modules Cốt Lõi

### A. Hệ Thống Backend (.NET 8)
Backend cung cấp đầy đủ các endpoint theo chuẩn RESTful API, có đánh version (`/api/v1/...`).
- **Quản lý Định danh & Phân quyền (Identity & Master Data)**:
  - Đăng ký, Đăng nhập (Local Email/Pass hoặc Firebase Token), Đăng xuất.
  - CRUD User, Role, Permission.
  - Phân quyền động, gán quyền cho Role/User một cách linh hoạt (Grant/Revoke API).
- **Quản lý Media (Appwrite)**:
  - Tự động sinh Pre-signed URL để Upload/Download hình ảnh (món ăn, avatar), tối ưu quá trình truyền tải dữ liệu lớn bằng cách uỷ thác trực tiếp về máy chủ Appwrite thay vì đi qua Backend.
- **Tính năng Trò chuyện (Realtime Chat)**:
  - Triển khai Socket thông qua SignalR (`/hubs/chat`), cho phép user tương tác và chat với AI hoặc với hệ thống ngay lập tức. Client xác thực Socket qua query `access_token`.

### B. Chi tiết thuật toán và nghiệp vụ của AI Service (FastAPI)

Hiện tại, `AI Service` đóng vai trò module lõi cho các tính năng thông minh. Ở phiên bản hiện tại, để tối ưu tốc độ và không phụ thuộc vào sức mạnh phần cứng lớn, hệ thống sử dụng các thuật toán cơ sở (Baseline Algorithms), Rule-based kết hợp Toán học tối ưu (Mathematical Optimization) thay cho Deep Learning nặng:

**1. Hệ thống Lập kế hoạch Bữa ăn (Recommendation & Meal Planner)**: 
*(Thực thi chính tại `recommendation_service.py`)*
- **Chấm điểm cá nhân hoá (Heuristic Scoring)**: Mỗi món ăn có điểm cơ sở (Base score = 0.35). Hệ thống sẽ duyệt qua các thẻ (Tags) của món ăn đó, nếu có nhãn tốt cho sức khoẻ (`healthy`, `high_protein`...) hệ thống sẽ cộng các trọng số tương ứng dựa trên cấu hình tại `rules.json`.
- **Phễu Dị ứng (Allergy Matcher)**: Dùng kỹ thuật lọc chuỗi cơ sở (Substring matching). Bất kì món ăn hay nguyên liệu nào có tên chứa từ khóa dị ứng của người dùng sẽ bị loại triệt để từ đầu.
- **Lập lịch thông minh bằng Constraint Programming (CP-SAT qua Google OR-Tools)**: Đây là một bài toán tối ưu quan trọng nhằm đảm bảo suất ăn đủ chất và cân bằng (Ví dụ luôn phải có 1 Món Canh và 1 Món Mặn đi kèm).
  - *Biến quyết định*: Hệ thống sinh ra một ma trận biến bool đại diện cho mọi cặp ghép (Món chính i + Canh j).
  - *Hàm Mục tiêu (Objective)*: Tối đa hóa (Maximize) tổng điểm Heuristic của cặp món ăn đó.
  - *Ràng buộc cứng (Constraints)*: 
    - Chỉ được chọn đúng 1 cặp món / 1 bữa.
    - Loại bỏ triệt để các cặp món kỵ nhau (Ví dụ món có tính hàn kỵ món tính nhiệt tuỳ theo custom của Admin).
    - Lên lịch theo tuần (Weekly Plan): Đưa vào ràng buộc `All-Different` để đảm bảo thực đơn Món Chính cũng như Món Canh không bị lặp lại vào bất kì ngày nào trong tuần. Hệ thống giải phương trình để tìm ra chuỗi nghiệm (Top K giải pháp thực đơn tốt nhất).

**2. Chatbot & Trợ lý thông minh (`chat_service.py`)**:
- Hiện tại sử dụng nguyên lý **Rule-based NLP Pipeline & Keyword Inference**.
- Mô-đun sẽ phân tách văn bản (Tokenizer), làm sạch, và phân loại "Ý định người dùng" (Intent Inference). Khi phát hiện các thẻ từ như `suggest`, `today`, `menu`, hệ thống suy luận ý định trả lời là `recommend_food`. Đối với `review`, `taste` thì suy luận ý định là `evaluate_feedback`.
- Kiến trúc này được thiết kế theo dạng Dependency Injection linh hoạt để có thể dễ dàng thay lõi (plug-in) các hệ thống Prompt/LLM (HuggingFace / OpenAI GPT) sau này.

**3. Phân Tích Cảm Xúc Phản Hồi (`sentiment_service.py`)**:
- Sử dụng phương pháp **Lexicon-based Sentiment Analysis** (Phân tích dựa trên từ điển cảm xúc cơ sở).
- Hệ thống duy trì một tập hợp các từ khóa Tích cực (`POSITIVE`) và Tiêu cực (`NEGATIVE`). Đoạn chat nhận xét của người dùng sẽ được tách từ, và đối chiếu với tập từ vựng này.
- **Công thức tính Polarity**: Điểm được nội suy thông qua công thức trừ chênh lệch từ biểu cảm cực trị `(Số lượng từ Positive - Số lượng từ Negative) / Tổng số từ biểu cảm`. Nếu người dùng có Vote Sao từ 1 -> 5, hệ thống sẽ thực hiện Nudge Rating tuyến tính dời điểm đó về phạm vi `[-1, 1]`.
- **Khai thác kết quả**: Điểm `> 0.2` được phân bổ nhãn `Positive`, `< -0.2` tương ứng `Negative` và còn lại là Trung lập (`Neutral`), cung cấp báo cáo thống kê chính xác về chất lượng món ăn cho đội ngũ bếp.

---

## 4. Cách Thức Hoạt Động Của Hệ Thống

1. **Khởi chạy Hệ Sinh Thái (Infra)**: 
   Toàn bộ DB MySQL, Redis, RabbitMQ có thể được Start up thông qua 1 câu lệnh `docker compose up -d`. AI Service cũng được đóng gói trong này.
   
2. **Khởi chạy API Backend Core**:
   Thực thi thông thường qua CLI `.NET` tại `SmartLunch-Backend-Service/SmartLunch-Backend-Service-API`. Nó sẽ kết nối tới các Services trên thông qua file `appsettings.json`. Nó cung cấp cổng (localhost:5001) phục vụ App/Web Frontend.

3. **Luồng Cập nhật & Gợi ý (The Recommendation Flow)**:
   - User trên App yêu cầu gợi ý món cho tuần này.
   - Ứng dụng Frontend gọi tới Backend hoặc gọi thẳng AI Service.
   - Dịch vụ AI Service sử dụng Token phân quyền để Call back sang Backend, lấy dữ liệu Menu.
   - AI Service chạy mô hình giải thuật toán (Constraint Programming - CP-SAT), lấy rule tại file cấu hình để ghép nối món ăn hoàn chỉnh theo chuẩn dinh dưỡng/sở thích và trả lại cho User/Backend lưu trữ quá trình mua.
   
4. **Luồng Realtime Chat / Notification**:
   - Khi có người đặt đồ án, hoặc gửi thông báo qua lại giữa các phòng ban ăn uống. 
   - Thông báo được đẩy về RabbitMQ -> Các Service/Microservice khác Subscribe và phát tín hiệu -> Thông báo Real-time được Backend truyền tải qua hệ thống kết nối mở SignalR Socket.

---

## 5. Tài liệu đính kèm bạn nên xem qua trong Codebase
- `RUN_AND_API.md`: Danh sách toàn bộ Endpoint của Backend lẫn AI Service, cách khởi động từng Services.
- `docker-compose.yml`: Nơi cấu hình Network, các container (MySQL, RMQ, Redis, FastAPI).
- Thư mục `.github/` hoặc `docs/`: Có thể chứa cấu hình CI/CD và tài liệu riêng của luồng Backend.
