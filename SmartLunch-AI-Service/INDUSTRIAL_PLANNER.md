# SmartLunch AI Service — Hệ Thống Lập Kế Hoạch Thực Đơn Bếp Công Nghiệp

## 1. Tổng Quan

SmartLunch AI Service là dịch vụ trí tuệ nhân tạo chuyên dụng cho hệ thống quản lý suất ăn công nghiệp. Dịch vụ sử dụng **Google OR-Tools CP-SAT Solver** (Constraint Programming – Satisfiability) để tự động sinh thực đơn tuần tối ưu, đảm bảo cân bằng dinh dưỡng, kiểm soát chi phí và tận dụng nguyên liệu hiệu quả.

### Tại sao không dùng Random?

Trong môi trường bếp KCN (800–2000 suất/ngày), việc "random ghép món" sẽ gây ra:
- **Trùng nguyên liệu** không kiểm soát → khó mua hàng
- **Chi phí dao động mạnh** → vượt budget
- **Combo kỳ lạ** (2 món cùng dầu mỡ, toàn thịt đỏ) → công nhân phàn nàn
- **Lãng phí nguyên liệu** → tồn kho cao

Hệ thống này giải quyết triệt để bằng cách sử dụng **ràng buộc toán học (constraints)** thay vì ngẫu nhiên.

---

## 2. Kiến Trúc Hệ Thống

```
SmartLunch-AI-Service/
├── app/
│   ├── api/v1/endpoints/
│   │   ├── industrial.py      ← Endpoint chính: POST /recommend/industrial/menus
│   │   ├── recommend.py       ← Endpoint cũ (giữ lại): POST /recommend/week/plan
│   │   ├── chat.py            ← Chatbot Q&A
│   │   └── feedback.py        ← Phân tích cảm xúc
│   ├── core/
│   │   ├── config.py          ← Cấu hình FastAPI (CORS, env)
│   │   ├── rules.json         ← File cấu hình trọng số & ràng buộc (HOT-RELOAD)
│   │   └── rules_loader.py    ← Nạp rules.json mỗi request
│   ├── schemas/
│   │   ├── industrial.py      ← Data models cho bếp công nghiệp
│   │   └── recommendation.py  ← Data models cho API cũ
│   └── services/
│       ├── industrial_planner_service.py  ← BỘ NÃO AI (CP-SAT Solver)
│       ├── recommendation_service.py      ← Service cũ (tag-based)
│       ├── chat_service.py
│       └── sentiment_service.py
├── requirements.txt
├── Dockerfile
└── README.md
```

---

## 3. Luồng Xử Lý Chính (Pipeline)

```
┌─────────────────────────────────────────────────────────────────┐
│                    CLIENT / BACKEND SERVICE                      │
│                                                                  │
│  POST /api/v1/recommend/industrial/menus                         │
│  Body: { top_k, days[], meal_structure[], dishes[], constraints,  │
│          budget_per_serving, rules_key, ... }                     │
└──────────────────────────┬──────────────────────────────────────┘
                           │
                           ▼
┌──────────────────────────────────────────────────────────────────┐
│  BƯỚC 1: VALIDATE & PARSE INPUT                                  │
│  • Xác định slot cần cover: meal_structure (mặc định: main/side/soup/dessert) │
│  • Với món “tổng hợp” (phở/mì/bánh canh): dùng `covers_categories` để cover nhiều slot │
│  • Kiểm tra mỗi slot trong meal_structure có ít nhất 1 món      │
│  • Nạp rules profile từ rules.json                               │
└──────────────────────────┬──────────────────────────────────────┘
                           │
                           ▼
┌──────────────────────────────────────────────────────────────────┐
│  BƯỚC 2: CHẤM ĐIỂM MỖI MÓN ĂN (Scoring)                        │
│                                                                   │
│  score = base_score                                               │
│        + budget_fit_weight × (1 - |1 - cost/budget|)             │
│        + popularity_weight × (popularity / 5)                     │
│        + available_ingredient_bonus (nếu nguyên liệu có sẵn)    │
│        + tag_weights (theo thẻ: traditional, comfort, ...)       │
│                                                                   │
│  Kết quả: Mỗi món có điểm ∈ [0, 1]                              │
└──────────────────────────┬──────────────────────────────────────┘
                           │
                           ▼
┌──────────────────────────────────────────────────────────────────┐
│  BƯỚC 3: XÂY DỰNG MÔ HÌNH TỐI ƯU (CP-SAT)                      │
│                                                                   │
│  Biến quyết định:                                                 │
│    y[ngày][món] = 0 hoặc 1                                       │
│    (một món có thể cover nhiều slot: main/side/soup)             │
│                                                                   │
│  Ràng buộc:                                                       │
│    ① Mỗi ngày, mỗi slot phải được cover ĐÚNG 1 lần              │
│    ② Mỗi món xuất hiện ≤ max_per_week lần/tuần                  │
│    ③ Mỗi loại protein (heo/gà/cá) ≤ N bữa/tuần                 │
│    ④ Cá ≥ M bữa/tuần (đảm bảo dinh dưỡng)                      │
│    ⑤ Không lặp nguyên liệu chính 2 ngày liên tiếp               │
│    ⑥ Không lặp cách chế biến 2 ngày liên tiếp                   │
│    ⑦ Tổng cost/ngày ≤ budget_per_serving                        │
│                                                                   │
│  Hàm mục tiêu: Maximize tổng điểm + bonus tận dụng nguyên liệu │
└──────────────────────────┬──────────────────────────────────────┘
                           │
                           ▼
┌──────────────────────────────────────────────────────────────────┐
│  BƯỚC 4: GIẢI & SINH KẾT QUẢ                                     │
│                                                                   │
│  → Top-K phương án thực đơn theo số ngày yêu cầu                │
│  → Mỗi phương án có điểm (plan_score + objective_value)         │
│  → Sắp xếp theo điểm giảm dần để chọn menu hợp lý nhất          │
└──────────────────────────────────────────────────────────────────┘
```

---

## 4. Chi Tiết Các Ràng Buộc (Constraints)

### 4.1 Khung Bữa Ăn (Meal Structure)
Mỗi ngày **bắt buộc** phải có đủ các slot theo `meal_structure`. Mặc định:

| Slot | Category | Ý nghĩa |
|------|----------|---------|
| 1 | `main` | Món mặn chính (gà kho, cá chiên, thịt kho...) |
| 2 | `side` | Món phụ / xào (bắp cải xào, đậu que xào...) |
| 3 | `soup` | Món canh (canh bí, canh chua...) |
| 4 | `dessert` | Tráng miệng (sữa chua, trái cây, pudding...) |

#### Món “tổng hợp” (phở/mì/bánh canh)
Có những món có thể thay thế nhiều slot cùng lúc. Ví dụ:
- **Phở bò** cover `main + side + soup`
- Khi đó, trong 1 ngày chỉ cần chọn thêm `dessert` (ví dụ sữa chua)

Để khai báo, dùng field `covers_categories` trong `IndustrialDish`.

### 4.2 Giới Hạn Protein (`max_same_protein_per_week`)
- **Mặc định:** 3
- **Ý nghĩa:** Không quá 3 bữa cùng loại protein chính trong tuần
- **Ví dụ:** Thịt heo xuất hiện ở Thứ 2, Thứ 4, Thứ 6 → OK (3 lần). Thêm Thứ 7 → Vi phạm!

### 4.3 Tối Thiểu Cá (`min_fish_per_week`)
- **Mặc định:** 2
- **Ý nghĩa:** Phải có ít nhất 2 bữa dùng cá làm nguyên liệu chính
- **Lý do:** Đảm bảo đa dạng protein và cung cấp omega-3

### 4.4 Không Lặp Nguyên Liệu Liên Tiếp (`no_repeat_main_ingredient_consecutive_days`)
- **Mặc định:** `true`
- **Ý nghĩa:** Nếu Thứ 2 ăn gà → Thứ 3 **không được** ăn gà
- **Lý do:** Tránh nhàm chán, đảm bảo đa dạng

### 4.5 Xen Kẽ Cách Chế Biến (`alternate_cooking_methods`)
- **Mặc định:** `true`
- **Ý nghĩa:** Nếu Thứ 2 món mặn là chiên → Thứ 3 **không được** chiên
- **Các cách chế biến:** `fried` (chiên), `stewed` (kho), `boiled` (luộc), `stir_fried` (xào), `grilled` (nướng), `steamed` (hấp)

### 4.6 Tận Dụng Nguyên Liệu (`prefer_ingredient_reuse`)
- **Mặc định:** `true`
- **Ý nghĩa:** AI sẽ **thưởng điểm** nếu cùng một loại nguyên liệu được dùng trong 2-3 ngày gần nhau
- **Lợi ích thực tế:**
  - Mua số lượng lớn → giá tốt hơn
  - Giảm tồn kho / waste
  - Bếp sơ chế 1 lần dùng nhiều món

### 4.7 Giới Hạn Budget (`budget_per_serving`)
- **Mặc định:** 25,000 VND
- **Ý nghĩa:** Tổng cost của tất cả các món trong 1 ngày **không được vượt** budget/suất
- **Tính toán:** `cost(main) + cost(side) + cost(soup) + cost(dessert) ≤ budget`
  - Với món tổng hợp (phở/mì): `cost(phở) + cost(dessert) ≤ budget`

---

## 5. Cơ Chế Chấm Điểm (Scoring)

Mỗi món ăn được chấm điểm dựa trên công thức:

```
score = base_score (0.3)
      + budget_fit_weight × budget_fitness
      + popularity_weight × (popularity / 5)
      + available_ingredient_bonus (nếu có sẵn trong kho)
      + Σ tag_weights (theo thẻ món ăn)
```

Các trọng số mặc định (trong `rules.json`, profile `industrial`):

| Tham số | Giá trị | Ý nghĩa |
|---------|---------|---------|
| `base_score` | 0.3 | Điểm khởi đầu của mọi món |
| `budget_fit_weight` | 0.25 | Trọng số cho mức độ phù hợp budget |
| `popularity_weight` | 0.15 | Trọng số cho độ phổ biến |
| `ingredient_reuse_bonus` | 0.2 | Thưởng khi tận dụng nguyên liệu |
| `available_ingredient_bonus` | 0.15 | Thưởng khi nguyên liệu có sẵn trong kho |

> **Hot-Reload:** Thay đổi `rules.json` → Kết quả thay đổi ngay lập tức ở request tiếp theo, không cần restart server.

---

## 6. Metadata Món Ăn (IndustrialDish)

Mỗi món ăn cần cung cấp đầy đủ thông tin sau:

| Trường | Kiểu | Bắt buộc | Mô tả | Ví dụ |
|--------|------|----------|-------|-------|
| `name` | string | ✅ | Tên món | `"Gà kho gừng"` |
| `category` | enum | ✅ | Loại món | `main`, `side`, `soup`, `dessert` |
| `covers_categories` | enum[] | ❌ | Slot mà món có thể cover | `["main","side","soup"]` (phở/mì) |
| `main_ingredient` | string | ✅ | Nguyên liệu chính | `"chicken"` |
| `sub_ingredients` | string[] | ❌ | Nguyên liệu phụ | `["gừng", "nước mắm"]` |
| `cooking_method` | enum | ✅ | Cách chế biến | `fried`, `stewed`, `boiled`, ... |
| `cost_per_serving` | float | ✅ | Chi phí / suất (VND) | `12000` |
| `popularity` | int 1-5 | ❌ | Độ phổ biến | `4` |
| `max_per_week` | int 1-7 | ❌ | Tần suất tối đa / tuần | `2` |
| `tags` | string[] | ❌ | Thẻ bổ sung | `["traditional", "comfort"]` |

---

## 7. API Reference

### `POST /api/v1/recommend/industrial/menus`

**Mô tả:** Sinh **top-K** phương án thực đơn theo số ngày yêu cầu, tập trung vào menu + điểm, bỏ qua shopping/cost.

#### Request Body (ví dụ cho cả tuần):

```json
{
  "budget_per_serving": 26000,
  "days": ["Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "Chủ nhật"],
  "meal_structure": ["main", "side", "soup", "dessert"],
  "top_k": 3,
  "time_limit_seconds": 5.0,
  "dishes": [
    {
      "name": "Mực xào",
      "category": "main",
      "covers_categories": [],
      "main_ingredient": "squid",
      "sub_ingredients": ["hành", "tỏi"],
      "cooking_method": "stir_fried",
      "cost_per_serving": 12000,
      "popularity": 4,
      "max_per_week": 2,
      "tags": ["comfort"]
    },
    {
      "name": "Gà kho gừng",
      "category": "main",
      "covers_categories": [],
      "main_ingredient": "chicken",
      "sub_ingredients": ["gừng", "nước mắm"],
      "cooking_method": "stewed",
      "cost_per_serving": 12000,
      "popularity": 4,
      "max_per_week": 2,
      "tags": ["traditional", "comfort"]
    },
    {
      "name": "Thịt heo kho trứng",
      "category": "main",
      "covers_categories": [],
      "main_ingredient": "pork",
      "sub_ingredients": ["trứng", "nước dừa"],
      "cooking_method": "stewed",
      "cost_per_serving": 12000,
      "popularity": 5,
      "max_per_week": 2,
      "tags": ["traditional"]
    },
    {
      "name": "Cá kho tộ",
      "category": "main",
      "covers_categories": [],
      "main_ingredient": "fish",
      "sub_ingredients": ["tiêu", "nước mắm"],
      "cooking_method": "stewed",
      "cost_per_serving": 13000,
      "popularity": 4,
      "max_per_week": 2,
      "tags": ["traditional", "healthy"]
    },
    {
      "name": "Phở bò",
      "category": "main",
      "covers_categories": ["main", "side", "soup"],
      "main_ingredient": "beef",
      "sub_ingredients": ["bánh phở", "hành", "rau thơm"],
      "cooking_method": "boiled",
      "cost_per_serving": 22000,
      "popularity": 5,
      "max_per_week": 2,
      "tags": ["comfort"]
    },
    {
      "name": "Măng xào",
      "category": "side",
      "covers_categories": [],
      "main_ingredient": "bamboo_shoot",
      "sub_ingredients": ["tỏi"],
      "cooking_method": "stir_fried",
      "cost_per_serving": 5000,
      "popularity": 3,
      "max_per_week": 3,
      "tags": ["traditional"]
    },
    {
      "name": "Bắp cải xào",
      "category": "side",
      "covers_categories": [],
      "main_ingredient": "cabbage",
      "sub_ingredients": ["tỏi"],
      "cooking_method": "stir_fried",
      "cost_per_serving": 4000,
      "popularity": 3,
      "max_per_week": 3,
      "tags": ["budget"]
    },
    {
      "name": "Đậu que xào tỏi",
      "category": "side",
      "covers_categories": [],
      "main_ingredient": "green_beans",
      "sub_ingredients": ["tỏi"],
      "cooking_method": "stir_fried",
      "cost_per_serving": 4000,
      "popularity": 4,
      "max_per_week": 3,
      "tags": ["healthy"]
    },
    {
      "name": "Canh rau ngót",
      "category": "soup",
      "covers_categories": [],
      "main_ingredient": "rau ngót",
      "sub_ingredients": ["thịt băm"],
      "cooking_method": "boiled",
      "cost_per_serving": 6000,
      "popularity": 4,
      "max_per_week": 3,
      "tags": ["healthy"]
    },
    {
      "name": "Canh bí đỏ",
      "category": "soup",
      "covers_categories": [],
      "main_ingredient": "pumpkin",
      "sub_ingredients": ["hành"],
      "cooking_method": "boiled",
      "cost_per_serving": 5000,
      "popularity": 4,
      "max_per_week": 3,
      "tags": ["healthy", "budget"]
    },
    {
      "name": "Canh cải xanh",
      "category": "soup",
      "covers_categories": [],
      "main_ingredient": "mustard_greens",
      "sub_ingredients": ["gừng"],
      "cooking_method": "boiled",
      "cost_per_serving": 5000,
      "popularity": 3,
      "max_per_week": 3,
      "tags": ["healthy"]
    },
    {
      "name": "Sữa chua",
      "category": "dessert",
      "covers_categories": [],
      "main_ingredient": "yogurt",
      "sub_ingredients": [],
      "cooking_method": "raw",
      "cost_per_serving": 4000,
      "popularity": 4,
      "max_per_week": 7,
      "tags": ["kid_friendly"]
    }
  ],
  "available_ingredients": [],
  "constraints": {
    "max_same_protein_per_week": 3,
    "min_fish_per_week": 0,
    "no_repeat_main_ingredient_consecutive_days": true,
    "alternate_cooking_methods": true,
    "prefer_ingredient_reuse": true,
    "max_consecutive_same_main_dish": 3
  },
  "rules_key": "industrial"
}
```

#### Response (top-K):

```json
{
    "plans": [
        {
            "rank": 1,
            "plan_score": 0.598,
            "objective_value": 17047.0,
            "week_menu": [
                {
                    "day": "Thứ 2",
                    "dishes": [
                        {
                            "name": "Gà kho gừng",
                            "category": "main",
                            "score": 0.715,
                            "reasons": [
                                "Món phổ biến",
                                "Điểm phù hợp cao",
                                "Chế biến: stewed",
                                "Nguyên liệu chính: chicken"
                            ],
                            "cost_per_serving": 12000.0
                        },
                        {
                            "name": "Măng xào",
                            "category": "side",
                            "score": 0.538,
                            "reasons": [
                                "Chế biến: stir_fried",
                                "Nguyên liệu chính: bamboo_shoot"
                            ],
                            "cost_per_serving": 5000.0
                        },
                        {
                            "name": "Canh bí đỏ",
                            "category": "soup",
                            "score": 0.638,
                            "reasons": [
                                "Món phổ biến",
                                "Chế biến: boiled",
                                "Nguyên liệu chính: pumpkin"
                            ],
                            "cost_per_serving": 5000.0
                        },
                        {
                            "name": "Sữa chua",
                            "category": "dessert",
                            "score": 0.458,
                            "reasons": [
                                "Món phổ biến",
                                "Chế biến: raw",
                                "Nguyên liệu chính: yogurt"
                            ],
                            "cost_per_serving": 4000.0
                        }
                    ],
                    "day_cost_per_serving": 26000.0
                },
                {
                    "day": "Thứ 3",
                    "dishes": [
                        {
                            "name": "Mực xào",
                            "category": "main",
                            "score": 0.615,
                            "reasons": [
                                "Món phổ biến",
                                "Chế biến: stir_fried",
                                "Nguyên liệu chính: squid"
                            ],
                            "cost_per_serving": 12000.0
                        },
                        {
                            "name": "Bắp cải xào",
                            "category": "side",
                            "score": 0.548,
                            "reasons": [
                                "Chế biến: stir_fried",
                                "Nguyên liệu chính: cabbage"
                            ],
                            "cost_per_serving": 4000.0
                        },
                        {
                            "name": "Canh rau ngót",
                            "category": "soup",
                            "score": 0.528,
                            "reasons": [
                                "Món phổ biến",
                                "Chế biến: boiled",
                                "Nguyên liệu chính: rau ngót"
                            ],
                            "cost_per_serving": 6000.0
                        },
                        {
                            "name": "Sữa chua",
                            "category": "dessert",
                            "score": 0.458,
                            "reasons": [
                                "Món phổ biến",
                                "Chế biến: raw",
                                "Nguyên liệu chính: yogurt"
                            ],
                            "cost_per_serving": 4000.0
                        }
                    ],
                    "day_cost_per_serving": 26000.0
                },
                {
                    "day": "Thứ 4",
                    "dishes": [
                        {
                            "name": "Gà kho gừng",
                            "category": "main",
                            "score": 0.715,
                            "reasons": [
                                "Món phổ biến",
                                "Điểm phù hợp cao",
                                "Chế biến: stewed",
                                "Nguyên liệu chính: chicken"
                            ],
                            "cost_per_serving": 12000.0
                        },
                        {
                            "name": "Bắp cải xào",
                            "category": "side",
                            "score": 0.548,
                            "reasons": [
                                "Chế biến: stir_fried",
                                "Nguyên liệu chính: cabbage"
                            ],
                            "cost_per_serving": 4000.0
                        },
                        {
                            "name": "Canh rau ngót",
                            "category": "soup",
                            "score": 0.528,
                            "reasons": [
                                "Món phổ biến",
                                "Chế biến: boiled",
                                "Nguyên liệu chính: rau ngót"
                            ],
                            "cost_per_serving": 6000.0
                        },
                        {
                            "name": "Sữa chua",
                            "category": "dessert",
                            "score": 0.458,
                            "reasons": [
                                "Món phổ biến",
                                "Chế biến: raw",
                                "Nguyên liệu chính: yogurt"
                            ],
                            "cost_per_serving": 4000.0
                        }
                    ],
                    "day_cost_per_serving": 26000.0
                },
                {
                    "day": "Thứ 5",
                    "dishes": [
                        {
                            "name": "Phở bò",
                            "category": "main",
                            "score": 0.742,
                            "reasons": [
                                "Món phổ biến",
                                "Điểm phù hợp cao",
                                "Chế biến: boiled",
                                "Nguyên liệu chính: beef"
                            ],
                            "cost_per_serving": 22000.0
                        },
                        {
                            "name": "Phở bò",
                            "category": "side",
                            "score": 0.742,
                            "reasons": [
                                "Món phổ biến",
                                "Điểm phù hợp cao",
                                "Chế biến: boiled",
                                "Nguyên liệu chính: beef"
                            ],
                            "cost_per_serving": 22000.0
                        },
                        {
                            "name": "Phở bò",
                            "category": "soup",
                            "score": 0.742,
                            "reasons": [
                                "Món phổ biến",
                                "Điểm phù hợp cao",
                                "Chế biến: boiled",
                                "Nguyên liệu chính: beef"
                            ],
                            "cost_per_serving": 22000.0
                        },
                        {
                            "name": "Sữa chua",
                            "category": "dessert",
                            "score": 0.458,
                            "reasons": [
                                "Món phổ biến",
                                "Chế biến: raw",
                                "Nguyên liệu chính: yogurt"
                            ],
                            "cost_per_serving": 4000.0
                        }
                    ],
                    "day_cost_per_serving": 26000.0
                },
                {
                    "day": "Thứ 6",
                    "dishes": [
                        {
                            "name": "Cá kho tộ",
                            "category": "main",
                            "score": 0.695,
                            "reasons": [
                                "Món phổ biến",
                                "Chế biến: stewed",
                                "Nguyên liệu chính: fish"
                            ],
                            "cost_per_serving": 13000.0
                        },
                        {
                            "name": "Đậu que xào tỏi",
                            "category": "side",
                            "score": 0.508,
                            "reasons": [
                                "Món phổ biến",
                                "Chế biến: stir_fried",
                                "Nguyên liệu chính: green_beans"
                            ],
                            "cost_per_serving": 4000.0
                        },
                        {
                            "name": "Canh bí đỏ",
                            "category": "soup",
                            "score": 0.638,
                            "reasons": [
                                "Món phổ biến",
                                "Chế biến: boiled",
                                "Nguyên liệu chính: pumpkin"
                            ],
                            "cost_per_serving": 5000.0
                        },
                        {
                            "name": "Sữa chua",
                            "category": "dessert",
                            "score": 0.458,
                            "reasons": [
                                "Món phổ biến",
                                "Chế biến: raw",
                                "Nguyên liệu chính: yogurt"
                            ],
                            "cost_per_serving": 4000.0
                        }
                    ],
                    "day_cost_per_serving": 26000.0
                },
                {
                    "day": "Thứ 7",
                    "dishes": [
                        {
                            "name": "Phở bò",
                            "category": "main",
                            "score": 0.742,
                            "reasons": [
                                "Món phổ biến",
                                "Điểm phù hợp cao",
                                "Chế biến: boiled",
                                "Nguyên liệu chính: beef"
                            ],
                            "cost_per_serving": 22000.0
                        },
                        {
                            "name": "Phở bò",
                            "category": "side",
                            "score": 0.742,
                            "reasons": [
                                "Món phổ biến",
                                "Điểm phù hợp cao",
                                "Chế biến: boiled",
                                "Nguyên liệu chính: beef"
                            ],
                            "cost_per_serving": 22000.0
                        },
                        {
                            "name": "Phở bò",
                            "category": "soup",
                            "score": 0.742,
                            "reasons": [
                                "Món phổ biến",
                                "Điểm phù hợp cao",
                                "Chế biến: boiled",
                                "Nguyên liệu chính: beef"
                            ],
                            "cost_per_serving": 22000.0
                        },
                        {
                            "name": "Sữa chua",
                            "category": "dessert",
                            "score": 0.458,
                            "reasons": [
                                "Món phổ biến",
                                "Chế biến: raw",
                                "Nguyên liệu chính: yogurt"
                            ],
                            "cost_per_serving": 4000.0
                        }
                    ],
                    "day_cost_per_serving": 26000.0
                },
                {
                    "day": "Chủ nhật",
                    "dishes": [
                        {
                            "name": "Cá kho tộ",
                            "category": "main",
                            "score": 0.695,
                            "reasons": [
                                "Món phổ biến",
                                "Chế biến: stewed",
                                "Nguyên liệu chính: fish"
                            ],
                            "cost_per_serving": 13000.0
                        },
                        {
                            "name": "Bắp cải xào",
                            "category": "side",
                            "score": 0.548,
                            "reasons": [
                                "Chế biến: stir_fried",
                                "Nguyên liệu chính: cabbage"
                            ],
                            "cost_per_serving": 4000.0
                        },
                        {
                            "name": "Canh bí đỏ",
                            "category": "soup",
                            "score": 0.638,
                            "reasons": [
                                "Món phổ biến",
                                "Chế biến: boiled",
                                "Nguyên liệu chính: pumpkin"
                            ],
                            "cost_per_serving": 5000.0
                        },
                        {
                            "name": "Sữa chua",
                            "category": "dessert",
                            "score": 0.458,
                            "reasons": [
                                "Món phổ biến",
                                "Chế biến: raw",
                                "Nguyên liệu chính: yogurt"
                            ],
                            "cost_per_serving": 4000.0
                        }
                    ],
                    "day_cost_per_serving": 26000.0
                }
            ]
        },
        {
            "rank": 2,
            "plan_score": 0.598,
            "objective_value": 17047.0,
            "week_menu": [
                {
                    "day": "Thứ 2",
                    "dishes": [
                        {
                            "name": "Cá kho tộ",
                            "category": "main",
                            "score": 0.695,
                            "reasons": [
                                "Món phổ biến",
                                "Chế biến: stewed",
                                "Nguyên liệu chính: fish"
                            ],
                            "cost_per_serving": 13000.0
                        },
                        {
                            "name": "Bắp cải xào",
                            "category": "side",
                            "score": 0.548,
                            "reasons": [
                                "Chế biến: stir_fried",
                                "Nguyên liệu chính: cabbage"
                            ],
                            "cost_per_serving": 4000.0
                        },
                        {
                            "name": "Canh bí đỏ",
                            "category": "soup",
                            "score": 0.638,
                            "reasons": [
                                "Món phổ biến",
                                "Chế biến: boiled",
                                "Nguyên liệu chính: pumpkin"
                            ],
                            "cost_per_serving": 5000.0
                        },
                        {
                            "name": "Sữa chua",
                            "category": "dessert",
                            "score": 0.458,
                            "reasons": [
                                "Món phổ biến",
                                "Chế biến: raw",
                                "Nguyên liệu chính: yogurt"
                            ],
                            "cost_per_serving": 4000.0
                        }
                    ],
                    "day_cost_per_serving": 26000.0
                },
                {
                    "day": "Thứ 3",
                    "dishes": [
                        {
                            "name": "Mực xào",
                            "category": "main",
                            "score": 0.615,
                            "reasons": [
                                "Món phổ biến",
                                "Chế biến: stir_fried",
                                "Nguyên liệu chính: squid"
                            ],
                            "cost_per_serving": 12000.0
                        },
                        {
                            "name": "Măng xào",
                            "category": "side",
                            "score": 0.538,
                            "reasons": [
                                "Chế biến: stir_fried",
                                "Nguyên liệu chính: bamboo_shoot"
                            ],
                            "cost_per_serving": 5000.0
                        },
                        {
                            "name": "Canh bí đỏ",
                            "category": "soup",
                            "score": 0.638,
                            "reasons": [
                                "Món phổ biến",
                                "Chế biến: boiled",
                                "Nguyên liệu chính: pumpkin"
                            ],
                            "cost_per_serving": 5000.0
                        },
                        {
                            "name": "Sữa chua",
                            "category": "dessert",
                            "score": 0.458,
                            "reasons": [
                                "Món phổ biến",
                                "Chế biến: raw",
                                "Nguyên liệu chính: yogurt"
                            ],
                            "cost_per_serving": 4000.0
                        }
                    ],
                    "day_cost_per_serving": 26000.0
                },
                {
                    "day": "Thứ 4",
                    "dishes": [
                        {
                            "name": "Cá kho tộ",
                            "category": "main",
                            "score": 0.695,
                            "reasons": [
                                "Món phổ biến",
                                "Chế biến: stewed",
                                "Nguyên liệu chính: fish"
                            ],
                            "cost_per_serving": 13000.0
                        },
                        {
                            "name": "Bắp cải xào",
                            "category": "side",
                            "score": 0.548,
                            "reasons": [
                                "Chế biến: stir_fried",
                                "Nguyên liệu chính: cabbage"
                            ],
                            "cost_per_serving": 4000.0
                        },
                        {
                            "name": "Canh bí đỏ",
                            "category": "soup",
                            "score": 0.638,
                            "reasons": [
                                "Món phổ biến",
                                "Chế biến: boiled",
                                "Nguyên liệu chính: pumpkin"
                            ],
                            "cost_per_serving": 5000.0
                        },
                        {
                            "name": "Sữa chua",
                            "category": "dessert",
                            "score": 0.458,
                            "reasons": [
                                "Món phổ biến",
                                "Chế biến: raw",
                                "Nguyên liệu chính: yogurt"
                            ],
                            "cost_per_serving": 4000.0
                        }
                    ],
                    "day_cost_per_serving": 26000.0
                },
                {
                    "day": "Thứ 5",
                    "dishes": [
                        {
                            "name": "Phở bò",
                            "category": "main",
                            "score": 0.742,
                            "reasons": [
                                "Món phổ biến",
                                "Điểm phù hợp cao",
                                "Chế biến: boiled",
                                "Nguyên liệu chính: beef"
                            ],
                            "cost_per_serving": 22000.0
                        },
                        {
                            "name": "Phở bò",
                            "category": "side",
                            "score": 0.742,
                            "reasons": [
                                "Món phổ biến",
                                "Điểm phù hợp cao",
                                "Chế biến: boiled",
                                "Nguyên liệu chính: beef"
                            ],
                            "cost_per_serving": 22000.0
                        },
                        {
                            "name": "Phở bò",
                            "category": "soup",
                            "score": 0.742,
                            "reasons": [
                                "Món phổ biến",
                                "Điểm phù hợp cao",
                                "Chế biến: boiled",
                                "Nguyên liệu chính: beef"
                            ],
                            "cost_per_serving": 22000.0
                        },
                        {
                            "name": "Sữa chua",
                            "category": "dessert",
                            "score": 0.458,
                            "reasons": [
                                "Món phổ biến",
                                "Chế biến: raw",
                                "Nguyên liệu chính: yogurt"
                            ],
                            "cost_per_serving": 4000.0
                        }
                    ],
                    "day_cost_per_serving": 26000.0
                },
                {
                    "day": "Thứ 6",
                    "dishes": [
                        {
                            "name": "Gà kho gừng",
                            "category": "main",
                            "score": 0.715,
                            "reasons": [
                                "Món phổ biến",
                                "Điểm phù hợp cao",
                                "Chế biến: stewed",
                                "Nguyên liệu chính: chicken"
                            ],
                            "cost_per_serving": 12000.0
                        },
                        {
                            "name": "Bắp cải xào",
                            "category": "side",
                            "score": 0.548,
                            "reasons": [
                                "Chế biến: stir_fried",
                                "Nguyên liệu chính: cabbage"
                            ],
                            "cost_per_serving": 4000.0
                        },
                        {
                            "name": "Canh rau ngót",
                            "category": "soup",
                            "score": 0.528,
                            "reasons": [
                                "Món phổ biến",
                                "Chế biến: boiled",
                                "Nguyên liệu chính: rau ngót"
                            ],
                            "cost_per_serving": 6000.0
                        },
                        {
                            "name": "Sữa chua",
                            "category": "dessert",
                            "score": 0.458,
                            "reasons": [
                                "Món phổ biến",
                                "Chế biến: raw",
                                "Nguyên liệu chính: yogurt"
                            ],
                            "cost_per_serving": 4000.0
                        }
                    ],
                    "day_cost_per_serving": 26000.0
                },
                {
                    "day": "Thứ 7",
                    "dishes": [
                        {
                            "name": "Phở bò",
                            "category": "main",
                            "score": 0.742,
                            "reasons": [
                                "Món phổ biến",
                                "Điểm phù hợp cao",
                                "Chế biến: boiled",
                                "Nguyên liệu chính: beef"
                            ],
                            "cost_per_serving": 22000.0
                        },
                        {
                            "name": "Phở bò",
                            "category": "side",
                            "score": 0.742,
                            "reasons": [
                                "Món phổ biến",
                                "Điểm phù hợp cao",
                                "Chế biến: boiled",
                                "Nguyên liệu chính: beef"
                            ],
                            "cost_per_serving": 22000.0
                        },
                        {
                            "name": "Phở bò",
                            "category": "soup",
                            "score": 0.742,
                            "reasons": [
                                "Món phổ biến",
                                "Điểm phù hợp cao",
                                "Chế biến: boiled",
                                "Nguyên liệu chính: beef"
                            ],
                            "cost_per_serving": 22000.0
                        },
                        {
                            "name": "Sữa chua",
                            "category": "dessert",
                            "score": 0.458,
                            "reasons": [
                                "Món phổ biến",
                                "Chế biến: raw",
                                "Nguyên liệu chính: yogurt"
                            ],
                            "cost_per_serving": 4000.0
                        }
                    ],
                    "day_cost_per_serving": 26000.0
                },
                {
                    "day": "Chủ nhật",
                    "dishes": [
                        {
                            "name": "Gà kho gừng",
                            "category": "main",
                            "score": 0.715,
                            "reasons": [
                                "Món phổ biến",
                                "Điểm phù hợp cao",
                                "Chế biến: stewed",
                                "Nguyên liệu chính: chicken"
                            ],
                            "cost_per_serving": 12000.0
                        },
                        {
                            "name": "Đậu que xào tỏi",
                            "category": "side",
                            "score": 0.508,
                            "reasons": [
                                "Món phổ biến",
                                "Chế biến: stir_fried",
                                "Nguyên liệu chính: green_beans"
                            ],
                            "cost_per_serving": 4000.0
                        },
                        {
                            "name": "Canh rau ngót",
                            "category": "soup",
                            "score": 0.528,
                            "reasons": [
                                "Món phổ biến",
                                "Chế biến: boiled",
                                "Nguyên liệu chính: rau ngót"
                            ],
                            "cost_per_serving": 6000.0
                        },
                        {
                            "name": "Sữa chua",
                            "category": "dessert",
                            "score": 0.458,
                            "reasons": [
                                "Món phổ biến",
                                "Chế biến: raw",
                                "Nguyên liệu chính: yogurt"
                            ],
                            "cost_per_serving": 4000.0
                        }
                    ],
                    "day_cost_per_serving": 26000.0
                }
            ]
        },
        {
            "rank": 3,
            "plan_score": 0.598,
            "objective_value": 17047.0,
            "week_menu": [
                {
                    "day": "Thứ 2",
                    "dishes": [
                        {
                            "name": "Cá kho tộ",
                            "category": "main",
                            "score": 0.695,
                            "reasons": [
                                "Món phổ biến",
                                "Chế biến: stewed",
                                "Nguyên liệu chính: fish"
                            ],
                            "cost_per_serving": 13000.0
                        },
                        {
                            "name": "Bắp cải xào",
                            "category": "side",
                            "score": 0.548,
                            "reasons": [
                                "Chế biến: stir_fried",
                                "Nguyên liệu chính: cabbage"
                            ],
                            "cost_per_serving": 4000.0
                        },
                        {
                            "name": "Canh bí đỏ",
                            "category": "soup",
                            "score": 0.638,
                            "reasons": [
                                "Món phổ biến",
                                "Chế biến: boiled",
                                "Nguyên liệu chính: pumpkin"
                            ],
                            "cost_per_serving": 5000.0
                        },
                        {
                            "name": "Sữa chua",
                            "category": "dessert",
                            "score": 0.458,
                            "reasons": [
                                "Món phổ biến",
                                "Chế biến: raw",
                                "Nguyên liệu chính: yogurt"
                            ],
                            "cost_per_serving": 4000.0
                        }
                    ],
                    "day_cost_per_serving": 26000.0
                },
                {
                    "day": "Thứ 3",
                    "dishes": [
                        {
                            "name": "Phở bò",
                            "category": "main",
                            "score": 0.742,
                            "reasons": [
                                "Món phổ biến",
                                "Điểm phù hợp cao",
                                "Chế biến: boiled",
                                "Nguyên liệu chính: beef"
                            ],
                            "cost_per_serving": 22000.0
                        },
                        {
                            "name": "Phở bò",
                            "category": "side",
                            "score": 0.742,
                            "reasons": [
                                "Món phổ biến",
                                "Điểm phù hợp cao",
                                "Chế biến: boiled",
                                "Nguyên liệu chính: beef"
                            ],
                            "cost_per_serving": 22000.0
                        },
                        {
                            "name": "Phở bò",
                            "category": "soup",
                            "score": 0.742,
                            "reasons": [
                                "Món phổ biến",
                                "Điểm phù hợp cao",
                                "Chế biến: boiled",
                                "Nguyên liệu chính: beef"
                            ],
                            "cost_per_serving": 22000.0
                        },
                        {
                            "name": "Sữa chua",
                            "category": "dessert",
                            "score": 0.458,
                            "reasons": [
                                "Món phổ biến",
                                "Chế biến: raw",
                                "Nguyên liệu chính: yogurt"
                            ],
                            "cost_per_serving": 4000.0
                        }
                    ],
                    "day_cost_per_serving": 26000.0
                },
                {
                    "day": "Thứ 4",
                    "dishes": [
                        {
                            "name": "Cá kho tộ",
                            "category": "main",
                            "score": 0.695,
                            "reasons": [
                                "Món phổ biến",
                                "Chế biến: stewed",
                                "Nguyên liệu chính: fish"
                            ],
                            "cost_per_serving": 13000.0
                        },
                        {
                            "name": "Bắp cải xào",
                            "category": "side",
                            "score": 0.548,
                            "reasons": [
                                "Chế biến: stir_fried",
                                "Nguyên liệu chính: cabbage"
                            ],
                            "cost_per_serving": 4000.0
                        },
                        {
                            "name": "Canh bí đỏ",
                            "category": "soup",
                            "score": 0.638,
                            "reasons": [
                                "Món phổ biến",
                                "Chế biến: boiled",
                                "Nguyên liệu chính: pumpkin"
                            ],
                            "cost_per_serving": 5000.0
                        },
                        {
                            "name": "Sữa chua",
                            "category": "dessert",
                            "score": 0.458,
                            "reasons": [
                                "Món phổ biến",
                                "Chế biến: raw",
                                "Nguyên liệu chính: yogurt"
                            ],
                            "cost_per_serving": 4000.0
                        }
                    ],
                    "day_cost_per_serving": 26000.0
                },
                {
                    "day": "Thứ 5",
                    "dishes": [
                        {
                            "name": "Phở bò",
                            "category": "main",
                            "score": 0.742,
                            "reasons": [
                                "Món phổ biến",
                                "Điểm phù hợp cao",
                                "Chế biến: boiled",
                                "Nguyên liệu chính: beef"
                            ],
                            "cost_per_serving": 22000.0
                        },
                        {
                            "name": "Phở bò",
                            "category": "side",
                            "score": 0.742,
                            "reasons": [
                                "Món phổ biến",
                                "Điểm phù hợp cao",
                                "Chế biến: boiled",
                                "Nguyên liệu chính: beef"
                            ],
                            "cost_per_serving": 22000.0
                        },
                        {
                            "name": "Phở bò",
                            "category": "soup",
                            "score": 0.742,
                            "reasons": [
                                "Món phổ biến",
                                "Điểm phù hợp cao",
                                "Chế biến: boiled",
                                "Nguyên liệu chính: beef"
                            ],
                            "cost_per_serving": 22000.0
                        },
                        {
                            "name": "Sữa chua",
                            "category": "dessert",
                            "score": 0.458,
                            "reasons": [
                                "Món phổ biến",
                                "Chế biến: raw",
                                "Nguyên liệu chính: yogurt"
                            ],
                            "cost_per_serving": 4000.0
                        }
                    ],
                    "day_cost_per_serving": 26000.0
                },
                {
                    "day": "Thứ 6",
                    "dishes": [
                        {
                            "name": "Gà kho gừng",
                            "category": "main",
                            "score": 0.715,
                            "reasons": [
                                "Món phổ biến",
                                "Điểm phù hợp cao",
                                "Chế biến: stewed",
                                "Nguyên liệu chính: chicken"
                            ],
                            "cost_per_serving": 12000.0
                        },
                        {
                            "name": "Măng xào",
                            "category": "side",
                            "score": 0.538,
                            "reasons": [
                                "Chế biến: stir_fried",
                                "Nguyên liệu chính: bamboo_shoot"
                            ],
                            "cost_per_serving": 5000.0
                        },
                        {
                            "name": "Canh bí đỏ",
                            "category": "soup",
                            "score": 0.638,
                            "reasons": [
                                "Món phổ biến",
                                "Chế biến: boiled",
                                "Nguyên liệu chính: pumpkin"
                            ],
                            "cost_per_serving": 5000.0
                        },
                        {
                            "name": "Sữa chua",
                            "category": "dessert",
                            "score": 0.458,
                            "reasons": [
                                "Món phổ biến",
                                "Chế biến: raw",
                                "Nguyên liệu chính: yogurt"
                            ],
                            "cost_per_serving": 4000.0
                        }
                    ],
                    "day_cost_per_serving": 26000.0
                },
                {
                    "day": "Thứ 7",
                    "dishes": [
                        {
                            "name": "Mực xào",
                            "category": "main",
                            "score": 0.615,
                            "reasons": [
                                "Món phổ biến",
                                "Chế biến: stir_fried",
                                "Nguyên liệu chính: squid"
                            ],
                            "cost_per_serving": 12000.0
                        },
                        {
                            "name": "Đậu que xào tỏi",
                            "category": "side",
                            "score": 0.508,
                            "reasons": [
                                "Món phổ biến",
                                "Chế biến: stir_fried",
                                "Nguyên liệu chính: green_beans"
                            ],
                            "cost_per_serving": 4000.0
                        },
                        {
                            "name": "Canh rau ngót",
                            "category": "soup",
                            "score": 0.528,
                            "reasons": [
                                "Món phổ biến",
                                "Chế biến: boiled",
                                "Nguyên liệu chính: rau ngót"
                            ],
                            "cost_per_serving": 6000.0
                        },
                        {
                            "name": "Sữa chua",
                            "category": "dessert",
                            "score": 0.458,
                            "reasons": [
                                "Món phổ biến",
                                "Chế biến: raw",
                                "Nguyên liệu chính: yogurt"
                            ],
                            "cost_per_serving": 4000.0
                        }
                    ],
                    "day_cost_per_serving": 26000.0
                },
                {
                    "day": "Chủ nhật",
                    "dishes": [
                        {
                            "name": "Gà kho gừng",
                            "category": "main",
                            "score": 0.715,
                            "reasons": [
                                "Món phổ biến",
                                "Điểm phù hợp cao",
                                "Chế biến: stewed",
                                "Nguyên liệu chính: chicken"
                            ],
                            "cost_per_serving": 12000.0
                        },
                        {
                            "name": "Bắp cải xào",
                            "category": "side",
                            "score": 0.548,
                            "reasons": [
                                "Chế biến: stir_fried",
                                "Nguyên liệu chính: cabbage"
                            ],
                            "cost_per_serving": 4000.0
                        },
                        {
                            "name": "Canh rau ngót",
                            "category": "soup",
                            "score": 0.528,
                            "reasons": [
                                "Món phổ biến",
                                "Chế biến: boiled",
                                "Nguyên liệu chính: rau ngót"
                            ],
                            "cost_per_serving": 6000.0
                        },
                        {
                            "name": "Sữa chua",
                            "category": "dessert",
                            "score": 0.458,
                            "reasons": [
                                "Món phổ biến",
                                "Chế biến: raw",
                                "Nguyên liệu chính: yogurt"
                            ],
                            "cost_per_serving": 4000.0
                        }
                    ],
                    "day_cost_per_serving": 26000.0
                }
            ]
        }
    ]
}
```

> **Lưu ý:** Ví dụ response ở trên chỉ hiển thị 1 ngày (`Thứ 2`) để ngắn gọn.
> Khi bạn gửi `days` đủ 7 ngày, trường `week_menu` sẽ có đủ 7 phần tử tương ứng (`Thứ 2` → `Chủ nhật`).

---

## 8. Cấu Hình (rules.json)

File `app/core/rules.json` cho phép thay đổi hành vi AI **mà không cần sửa code** hoặc **restart server**.

### Profile `industrial`:

```json
{
  "profiles": {
    "industrial": {
      "scoring": {
        "base_score": 0.3,
        "budget_fit_weight": 0.25,
        "popularity_weight": 0.15,
        "ingredient_reuse_bonus": 0.2,
        "available_ingredient_bonus": 0.15,
        "tag_weights": {
          "traditional": 0.10,
          "comfort": 0.08,
          "healthy": 0.05,
          "budget": 0.12
        }
      },
      "constraints": {
        "max_same_protein_per_week": 3,
        "min_fish_per_week": 2,
        "no_repeat_main_ingredient_consecutive_days": true,
        "alternate_cooking_methods": true,
        "prefer_ingredient_reuse": true
      }
    },
    "industrial": {
      "scoring": {
        "base_score": 0.3,
        "budget_fit_weight": 0.25,
        "popularity_weight": 0.15,
        "ingredient_reuse_bonus": 0.2,
        "available_ingredient_bonus": 0.15,
        "tag_weights": {
          "traditional": 0.10,
          "comfort": 0.08,
          "healthy": 0.05,
          "budget": 0.12
        }
      },
      "constraints": {
        "max_same_protein_per_week": 3,
        "min_fish_per_week": 2,
        "no_repeat_main_ingredient_consecutive_days": true,
        "alternate_cooking_methods": true,
        "prefer_ingredient_reuse": true
      }
    }
  }
}
```

### Cách thay đổi:
1. Mở file `app/core/rules.json`
2. Sửa các giá trị (ví dụ: tăng `budget_fit_weight` lên `0.4`)
3. Gửi lại request → Kết quả thay đổi ngay lập tức

---

## 9. Danh Sách Mua Hàng (Shopping List)

> **Ghi chú:** Phần Shopping List thuộc luồng response “3-in-1” (legacy). Với luồng mới
> `POST /api/v1/recommend/industrial/menus`, hệ thống tập trung trả về **top-K thực đơn + điểm**,
> **bỏ qua** mua hàng và ước tính chi phí.

Hệ thống tự động sinh danh sách mua hàng dựa trên:

1. **Gom nguyên liệu** từ tất cả các món đã chọn trong tuần
2. **Nhân với số suất** (`servings_per_day`)
3. **Trừ đi tồn kho** (`available_ingredients`)
4. **Phân loại** theo nhóm: `protein` / `vegetable` / `spice` / `other`

### Công thức tính kg:

```
total_kg = kg_per_serving × servings_per_day × số_ngày_sử_dụng

Trong đó kg_per_serving:
  - Protein: 0.15 kg/suất
  - Rau:     0.10 kg/suất
  - Gia vị:  0.01 kg/suất
  - Khác:    0.05 kg/suất
```

---

## 10. Cài Đặt & Chạy

### Yêu cầu:
- Python 3.10+
- Google OR-Tools (`pip install ortools`)

### Các bước:

```bash
cd SmartLunch-AI-Service

# 1. Tạo môi trường ảo
python -m venv .venv
.venv\Scripts\activate     # Windows
# source .venv/bin/activate  # Linux/Mac

# 2. Cài thư viện
pip install -r requirements.txt

# 3. Chạy server
uvicorn app.main:app --reload --host 0.0.0.0 --port 8001

# 4. Mở Swagger UI
# http://localhost:8001/docs
```

---

## 11. Roadmap

| Phase | Tính năng | Trạng thái |
|-------|-----------|------------|
| 1 | Thực đơn tuần + Danh sách mua hàng + Ước tính chi phí | ✅ Hoàn thành |
| 2 | Plan sơ chế (prep plan) — Lịch sơ chế nguyên liệu tối ưu | 🔜 Sắp tới |
| 3 | Tích hợp Backend — Tự động lấy pool món từ DB | 🔜 Sắp tới |
| 4 | Học từ feedback — Điều chỉnh popularity dựa trên phản hồi công nhân | 🔜 Sắp tới |
