# Tài liệu Chi tiết: Recommendation Service

Tài liệu này mô tả chi tiết cách thức hoạt động của hệ thống gợi ý món ăn trong `SmartLunch-AI-Service` và cách cấu hình linh hoạt thông qua file quy tắc (`rules.json`).

## 1. Tổng quan về Cơ chế Hoạt động

Hệ thống gợi ý hoạt động dựa trên phương pháp **Tính điểm theo thẻ (Tag-based scoring)** kết hợp với **Tối ưu hóa tổ hợp (Combinatorial Optimization)**.

### Quy trình xử lý (Pipeline)
1.  **Thu nhận dữ liệu**: Nhận danh sách thực đơn (tên món, thẻ/nguyên liệu), sở thích người dùng và danh sách dị ứng.
2.  **Lọc dị ứng (Hard Constraint)**: Loại bỏ ngay lập tức bất kỳ món ăn nào chứa từ khóa dị ứng trong tên hoặc thẻ.
3.  **Tính điểm (Scoring)**: Áp dụng các trọng số và thưởng/phạt dựa trên `rules.json`.
4.  **Tối ưu hóa (Optimization)**: Nếu yêu cầu là một thực đơn (Meal Plan), hệ thống sử dụng Google OR-Tools để tìm tổ hợp món mặn + món canh tốt nhất.
5.  **Trả kết quả**: Trả về danh sách đã xếp hạng kèm theo lý do (reasons) tại sao món đó được chọn.

---

## 2. Cấu hình Linh hoạt (Dynamic Configuration)

Để đáp ứng yêu cầu "setup 1 lần, cấu hình nhiều nơi", hệ thống sử dụng file **`app/core/rules.json`**. File này cho phép bạn thay đổi toàn bộ hành vi của AI mà không cần khởi động lại server hoặc thay đổi mã nguồn.

### Cấu trúc file `rules.json`
Bạn có thể định nghĩa nhiều "profile" khác nhau cho các đối tượng khách hàng khác nhau (ví dụ: `office`, `factory`, `hospital`).

```json
{
  "profiles": {
    "default": {
      "scoring": {
        "base_score": 0.35,
        "tag_weights": {
          "healthy": 0.15,
          "high_protein": 0.12,
          "budget": 0.10
        },
        "preference_match_bonus": 0.2,
        "preference_miss_penalty": 0.05
      },
      "allergy_filter": {
        "enabled": true
      },
      "incompatible_pairs": {
        "mode": "ingredient_keyword_overlap",
        "keywords": ["cay", "nóng"]
      }
    },
    "gym_user": {
      "scoring": {
        "tag_weights": {
          "high_protein": 0.5,
          "low_carb": 0.3
        }
      }
    }
  }
}
```

### Các tham số quan trọng:
-   **`base_score`**: Điểm khởi đầu của mỗi món ăn.
-   **`tag_weights`**: Định nghĩa tầm quan trọng của từng loại thẻ. Bạn có thể thêm bất kỳ thẻ nào vào đây.
-   **`preference_match_bonus`**: Điểm thưởng nếu món ăn khớp với sở thích cá nhân của người dùng.
-   **`incompatible_pairs`**: 
    -   `mode`: `ingredient_keyword_overlap` (kiểm tra sự trùng lặp nguyên liệu).
    -   `keywords`: Danh sách từ khóa không nên xuất hiện cùng lúc trong cả món mặn và món canh (ví dụ: tránh thực đơn toàn món "cay").

---

## 3. Cách thức "Setup Một lần"

Hệ thống đã được thiết lập sẵn lớp `RulesLoader` với cơ chế **Hot-Reloading**:

1.  **Trong Code**: `RulesLoader` được gọi mỗi khi có request API (`POST /recommend/...`). Nó sẽ đọc lại file `rules.json` từ đĩa.
2.  **Sử dụng API**: Khi gọi API, bạn chỉ cần truyền thêm trường `rules_key` trong request body:
    ```json
    {
      "rules_key": "gym_user",
      "dietary_preferences": ["protein"],
      "menu": [...]
    }
    ```
    Hệ thống sẽ tự động áp dụng các trọng số của profile `gym_user` để tính toán.

### Lợi ích:
-   **Không downtime**: Thay đổi `rules.json` và kết quả gợi ý sẽ thay đổi ngay lập tức ở request tiếp theo.
-   **Đa dạng hóa**: Một service AI duy nhất có thể phục vụ nhiều nhóm người dùng với tiêu chuẩn dinh dưỡng hoàn toàn khác nhau chỉ bằng cách thêm profile vào JSON.
-   **Dễ bảo trì**: Logic AI phức tạp được tách rời khỏi code và quản lý dưới dạng cấu hình dữ liệu.

---

## 4. Các thuật toán tối ưu nâng cao

### Google OR-Tools (CP-SAT Solver)
Khi bạn cần gợi ý thực đơn tuần hoặc cặp món, service sử dụng Constraint Programming để đảm bảo:
-   **Tính đa dạng**: Không lặp lại món mặn quá N lần trong tuần.
-   **Sự tương thích**: Món mặn và món canh phải "hợp" nhau dựa trên các quy tắc `incompatible_pairs`.
-   **Hiệu năng**: Tìm ra lời giải tối ưu trong hàng ngàn tổ hợp chỉ trong vài mili giây.
