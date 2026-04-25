# SmartLunch AI Service (FastAPI)

SmartLunch AI Service là một dịch vụ cung cấp các tính năng trí tuệ nhân tạo (AI) cho hệ thống quản lý suất ăn công nghiệp SmartLunch. Dịch vụ này được xây dựng trên nền tảng **FastAPI**, tập trung vào việc tối ưu hóa thực đơn, phân tích phản hồi người dùng và hỗ trợ tương tác thông qua chatbot.

## 🚀 Các Tính Năng Chính

Dịch vụ hiện cung cấp 3 nhóm chức năng cốt lõi:

1.  **Hệ Thống Gợi Ý Món Ăn (Recommendation Service)**: Tự động đề xuất món ăn và xây dựng kế hoạch ăn uống dựa trên sở thích, dị ứng và các quy tắc dinh dưỡng.
2.  **Phân Tích Cảm Xúc (Sentiment Service)**: Đánh giá phản hồi của người dùng về chất lượng món ăn (ngon/dở, thái độ phục vụ, giá cả, thời gian giao hàng) bằng phương pháp phân tích từ vựng (lexicon-based).
3.  **Trợ Lý Chat (Chat Service)**: Hỗ trợ giải đáp thắc mắc và điều hướng người dùng dựa trên nhận diện ý định (intent recognition).

---

## 🥗 Chi Tiết Về Recommendation Service

Đây là thành phần phức tạp nhất của dự án, sử dụng kết hợp giữa logic tính điểm dựa trên thẻ (tag-based) và tối ưu hóa tổ hợp (combinatorial optimization).

### 1. Cơ Chế Tính Điểm (Scoring Logic)
Hệ thống đánh giá mỗi món ăn dựa trên các tiêu chí sau:
-   **Lọc Dị Ứng (Hard Constraint)**: Tự động loại bỏ các món ăn có chứa thành phần gây dị ứng cho người dùng (kiểm tra theo tên món và thẻ).
-   **Điểm Cơ Bản (Base Score)**: Mỗi món ăn bắt đầu với một mức điểm mặc định (mặc định 0.35).
-   **Trọng Số Thẻ (Tag Weights)**: Các thẻ đặc thù như `healthy`, `high_protein`, `low_carb` có thể được cấu hình trọng số để tăng/giảm điểm ưu tiên.
-   **Sở Thích Cá Nhân (Preference Match)**: 
    -   Thưởng điểm (`preference_match_bonus`) nếu món ăn khớp với sở thích người dùng.
    -   Phạt điểm nhẹ (`preference_miss_penalty`) nếu người dùng có sở thích nhưng món ăn không đáp ứng được thẻ nào.
-   **Chuẩn Hóa**: Điểm số cuối cùng được đưa về khoảng [0, 1].

### 2. Tối Ưu Hóa Với Google OR-Tools (CP-SAT)
Thay vì chỉ gợi ý các món lẻ tẻ, hệ thống có khả năng giải quyết các bài toán tối ưu hóa phức tạp:
-   **Đề Xuất Cặp Món (Main + Soup)**: Tìm kiếm sự kết hợp tốt nhất giữa món mặn và món canh sao cho tổng điểm là cao nhất, đồng thời tránh các cặp món không tương thích (ví dụ: hai món cùng quá cay hoặc cùng một loại nguyên liệu chính).
-   **Lập Kế Hoạch Tuần (Weekly Planning)**:
    -   Tự động chọn 1 món mặn + 1 món canh cho mỗi ngày trong tuần.
    -   **Variety Constraints**: Ràng buộc món ăn không lặp lại trong tuần (`all_different_main`, `all_different_soup`).
    -   Đảm bảo tính đa dạng và cân bằng dinh dưỡng xuyên suốt.

### 3. Tích Hợp Hệ Thống (Backend Integration)
Dịch vụ có khả năng kết nối trực tiếp với `SmartLunch-Backend-Service`:
-   Lấy dữ liệu món ăn theo danh mục.
-   Phân tích thành phần nguyên liệu từ cơ sở dữ liệu để tự động gắn thẻ (tagging).
-   Lưu các đề xuất thực đơn (menu suggestions) trực tiếp vào hệ thống quản lý để người quản trị phê duyệt.

---

## 🛠 Kiến Trúc Dự Án

```text
app/
├── api/v1/             # Định nghĩa các endpoints API
│   └── endpoints/      # Xử lý logic cụ thể cho chat, recommend, feedback
├── core/               # Cấu hình hệ thống, nạp quy tắc (rules profile)
├── schemas/            # Định nghĩa kiểu dữ liệu (Pydantic models)
└── services/           # Logic nghiệp vụ chính (Core AI Logic)
    ├── recommendation_service.py
    ├── chat_service.py
    └── sentiment_service.py
```

---

## 💻 Hướng Dẫn Cài Đặt

### Yêu cầu hệ thống
-   Python 3.10+
-   (Tùy chọn) Google OR-Tools (để sử dụng tính năng tối ưu hóa nâng cao)

### Các bước cài đặt
1.  **Tạo môi trường ảo và cài đặt thư viện**:
    ```bash
    cd SmartLunch-AI-Service
    python -m venv .venv
    source .venv/bin/activate  # Trên Windows: .venv\Scripts\activate
    pip install -r requirements.txt
    ```

2.  **Cấu hình môi trường**:
    Sao chép file `.env.example` thành `.env` và cập nhật các thông số cần thiết (Backend URL, API Keys...).

3.  **Chạy dịch vụ**:
    ```bash
    uvicorn app.main:app --reload --host 0.0.0.0 --port 8001
    ```

4.  **Truy cập tài liệu API**:
    -   Swagger UI: `http://localhost:8001/docs`

---

## 📝 Ghi Chú Phát Triển
-   Hiện tại, các dịch vụ đang sử dụng logic **Baseline** (rule-based và lexicon-based).
-   Hệ thống được thiết kế theo dạng module, dễ dàng thay thế các service hiện tại bằng các mô hình học máy (Machine Learning) hoặc Large Language Models (LLMs) trong tương lai mà không cần thay đổi cấu trúc API.
