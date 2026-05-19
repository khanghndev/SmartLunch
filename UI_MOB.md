### Nghiệp vụ của hệ thống 

Tên ứng dụng: HUITMeal

Quy trình khi truy cập ứng dụng mobile, đến login page, sẽ có chỗ truyền email, password để đăng nhập cho Manager, Shipper và Organization, 
Bên cạnh đó thêm một nút phía dưới là Xem thực đơn (phần này dành cho customer)

Manager
- Báo cáo thống kê:
  + Xuất báo cáo Excel / PDF
  + Thống kê suất ăn: Theo ngày / ca / bộ phận
  + Xem thống kê doanh thu, số suất ăn theo ngày/tháng/năm

- Quản lý thu chi:
  + Theo ngày / tuần / tháng
  + Đối soát thanh toán

- Quản lý phản hồi & khiếu nại:
  + Xem đánh giá của khách hàng

Shipper
- Xem danh sách đơn cần giao
- Xem thực đơn: theo tuần
- Xem chi tiết đơn: 
  + Địa điểm giao
  + Số suất ăn
  + Thời gian giao
- Cập nhật trạng thái đơn: 
  + Đang giao
  + Đã giao thành công
  + Giao thất bại (Ghi chú lý do giao thất bại)
  + Từ chối đơn hàng
  + Đã nhận hàng
- Google Maps:
  + Tích hợp Google Maps
  + Xem bản đồ điểm giao
  + Tự động sắp xếp tuyến giao hàng tối ưu
- Giao hàng:
  + Xác nhận giao hàng
  + Chụp ảnh xác nhận giao hàng
  + Ghi nhận thời gian giao thực tế

Organization
- Đặt suất ăn 
- Chatbot chăm sóc khách hàng
- Thanh toán & đối soát: Theo hợp đồng
- Đánh giá suất ăn

Customer
- Xem thực đơn

---

### Luôn cập nhật router

Để đảm bảo hệ thống navigation hoạt động mượt mà và không có màn hình nào bị "mồ côi" (disconnected), bắt buộc tuân thủ các quy tắc sau khi tạo mới bất kỳ màn hình (Page/Screen) nào:
1. **Khai báo Route**: Mọi màn hình mới phải được khai báo định danh bằng một hằng số `static const` trong file `lib/src/app/app_routes.dart`.
2. **Đăng ký Router**: Bắt buộc phải thêm một `case` xử lý trong khối `switch (settings.name)` tại file `lib/src/app/router.dart` (bên trong hàm `onGenerateRoute`).
3. **Truyền dữ liệu (Arguments)**: Nếu màn hình cần nhận tham số (ví dụ: ID của đơn hàng, ID thực đơn), phải xử lý ép kiểu an toàn thông qua `settings.arguments` trong `router.dart` hoặc `ModalRoute.of(context)?.settings.arguments` tại màn hình đích.
4. **Không bỏ quên điều hướng**: Trước khi hoàn thành task tạo UI, luôn kiểm tra xem các nút (button) hoặc menu trỏ đến trang đó đã được gắn lệnh `Navigator.of(context).pushNamed(...)` chưa.

---

### Phong cách thiết kế (Design System — đồng bộ module Auth)

Nguồn chuẩn: `lib/src/core/theme/app_design_system.dart` + `lib/src/features/auth/presentation/widgets/auth_theme.dart` (khớp web `Khoa_Luan_KS_Web/Views/Auth/`).

#### Nguyên tắc
- **Font**: Google **Outfit** — `AppDesignSystem.font` / `AuthTheme.displayFont`.
- **Nền trang nội dung**: `#F9FAFB` (`AppDesignSystem.gray50`).
- **Thẻ (Card)**: nền trắng, viền `#F3F4F6`, bo góc `16px`, shadow nhẹ — `AppDesignSystem.card()`.
- **Input**: nền `#FAFAFA`, viền `1.5px #E5E7EB`, bo góc `12px`, focus theo accent role.
- **Nút chính**: nền solid, bo góc `12px`, chữ `FontWeight.w900`, có shadow nhẹ (giống web `rounded-xl font-black`).

#### Bảng màu brand & theo module (`RolePalette`)

| Module | Primary | Primary Alt | Dùng cho |
| :--- | :--- | :--- | :--- |
| **Brand / Auth / Customer** | `#F97316` | `#FB923C` | Đăng nhập, CTA, Khách xem thực đơn |
| **Manager** | `#D97706` | `#F59E0B` | Dashboard, báo cáo, thu chi |
| **Shipper** | `#2563EB` | `#3B82F6` | Giao hàng, bản đồ |
| **Organization** | `#0D9488` | `#2DD4BF` | Đặt suất, hợp đồng B2B |

#### Màn Auth
- **Login**: hero ảnh + chữ **HUITMeal** (không hiển thị role chip); form **Đăng nhập**; nút phụ **Xem thực đơn**.
- **Register / Forgot password**: layout `AuthShell` — hero panel + form card trắng.
- File: `auth_shell.dart`, `auth_widgets.dart`, `login_page.dart`, `register_page.dart`, `forgot_password_page.dart`.

##### UX scroll & bàn phím (AuthShell)
- **Một** `CustomScrollView`: hero + form + footer cùng scroll — không dùng `Column` cố định + `Expanded` (tránh che field khi bàn phím mở).
- `resizeToAvoidBottomInset: true`; padding đáy = `viewInsets.bottom` + safe area + `24`.
- Hero **thu nhỏ** khi `viewInsets.bottom > 0` (login ~112px, register/forgot ~88px).
- `keyboardDismissBehavior: onDrag`; tap ngoài form (`GestureDetector`) để ẩn bàn phím.
- Ảnh hero: `cacheWidth` theo `devicePixelRatio` — tránh méo / mờ pixel.
- Snackbar: `showAuthSnackBar` (floating, bo góc, không đè layout cố định).
- `AuthRememberRow`: trên màn hẹp (<360) xếp dọc; link footer dùng `Wrap` tránh overflow pixel.

#### Widget dùng chung cho module nội bộ
| Widget | File | Mô tả |
| :--- | :--- | :--- |
| `ModulePageShell` | `lib/src/core/widgets/module_page_shell.dart` | AppBar gradient + body `gray50` |
| `ModuleCard` | ↑ | Card chuẩn Auth |
| `RoleDashboardBody` | `lib/src/core/widgets/role_dashboard.dart` | Dashboard home các role |
| `SectionCard` | `lib/src/core/widgets/section_card.dart` | Section trong form |
| `ManagerPageShell` / `manager_ui.dart` | `lib/src/features/manager/presentation/widgets/manager_ui.dart` | Wrapper Manager → `RolePalette.manager` |

#### Theme Material (`app_theme.dart`)
- `ThemeData` dùng **Outfit**, `scaffoldBackgroundColor: gray50`, input/button/card khớp Auth.
- Accent theo `AppFlavor`: `RolePalette.customer` | `.shipper` | `.manager` | `.organization`.

#### Quy tắc khi tạo UI mới
1. Import `app_design_system.dart`; chọn `RolePalette` đúng module.
2. Dùng `ModulePageShell` + `ModuleCard` thay Scaffold/AppBar tự phối màu.
3. Text: `AppDesignSystem.title()` / `.sectionTitle()` / `.body()` / `.label()`.
4. Không dùng `Colors.amber` / `Colors.blueAccent` hard-code — lấy từ `RolePalette`.

#### Dashboard theo module (tổng quan từ BE)
Mỗi role dùng `RoleDashboardBody` (`role_dashboard.dart`) — **không chỉ là menu điều hướng**:

| Thành phần | Mô tả |
| :--- | :--- |
| **Header** | Gradient role + quick actions (Đặt suất, Thu chi, …) |
| **Số liệu tổng quan** | Lưới 2×2 `DashboardMetricCard` — gọi API thật khi `initState` / pull-to-refresh |
| **Hoạt động gần đây** | `DashboardRecentItem` — HĐ mới, đơn giao, khiếu nại |
| **Truy cập module** | Danh sách compact `_FeatureNavTile` + `preview` (số liệu gợi ý từ BE) |

**Nguồn dữ liệu dashboard:**

| Module | API / Repository |
| :--- | :--- |
| Manager | `ManagerRepository` (cashflow, reconciliation, complaints) + `MealStatisticsRepository` |
| Shipper | `ShipperRepository.getDeliveries(scheduledOn: hôm nay)` |
| Organization | `OrgRepository.getContracts`, `getContractPayments`, `getDishCategories` |
| Customer | `OrgRepository.getDishCategories`, `getDishesByCategory` |

File hỗ trợ: `dashboard_format.dart` (`formatDashboardVnd`, `formatDashboardPercent`).

---

Ánh xạ Nghiệp vụ của hệ thống ↔ File `.dart` & API Backend thực tế

### Manager

| Đầu mục Nghiệp vụ | Màn hình UI (.dart) | File Data Layer (.dart) | Endpoint API Backend | Trạng thái |
| :--- | :--- | :--- | :--- | :--- |
| **Xuất báo cáo Excel / PDF** | [reports_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/manager/presentation/pages/reports_page.dart)<br>[report_export_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/manager/presentation/pages/report_export_page.dart) | [manager_remote_datasource.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/manager/data/datasources/manager_remote_datasource.dart)<br>[manager_repository.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/manager/data/repositories/manager_repository.dart) | Local Task / Download URL | ✅ Hoàn thành |
| **Thống kê suất ăn** *(Theo ngày / ca / bộ phận)* | [statistics_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/manager/presentation/pages/statistics_page.dart) | [meal_statistics_repository.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/core/analytics/meal_statistics_repository.dart) | `GET /api/v1/master-data/Order/statistics/meal-count`<br>`GET /api/v1/master-data/Order/statistics/details` | ✅ Hoàn thành (Gọi API thực) |
| **Thống kê doanh thu & suất ăn** *(Ngày/tháng/năm)* | [statistics_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/manager/presentation/pages/statistics_page.dart) | [meal_statistics_repository.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/core/analytics/meal_statistics_repository.dart) | `GET /api/v1/master-data/Order/statistics/meal-count`<br>`GET /api/v1/master-data/Order/statistics/details` | ✅ Hoàn thành (Gọi API thực) |
| **Quản lý thu chi** *(Theo ngày / tuần / tháng)* | [cash_flow_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/manager/presentation/pages/cash_flow_page.dart) | [manager_remote_datasource.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/manager/data/datasources/manager_remote_datasource.dart)<br>[manager_repository.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/manager/data/repositories/manager_repository.dart)<br>[finance_models.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/manager/data/models/finance_models.dart) | `GET /api/v1/finance/cashflow-summary`<br>`GET /api/v1/finance/payment-history` | ✅ Hoàn thành (Gọi API thực) |
| **Đối soát thanh toán** | [reconciliation_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/manager/presentation/pages/reconciliation_page.dart) | [manager_remote_datasource.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/manager/data/datasources/manager_remote_datasource.dart)<br>[manager_repository.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/manager/data/repositories/manager_repository.dart)<br>[finance_models.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/manager/data/models/finance_models.dart) | `GET /api/v1/finance/payment-reconciliation`<br>`GET /api/v1/finance/organization-receivables`<br>`GET /api/v1/finance/supplier-payables` | ✅ Hoàn thành (Gọi API thực) |
| **Quản lý phản hồi & khiếu nại** | [feedback_complaints_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/manager/presentation/pages/feedback_complaints_page.dart) | [manager_remote_datasource.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/manager/data/datasources/manager_remote_datasource.dart)<br>[manager_repository.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/manager/data/repositories/manager_repository.dart)<br>[review_complaint_models.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/manager/data/models/review_complaint_models.dart) | `GET /api/v1/master-data/Review`<br>`GET /api/v1/master-data/Complaint` | ✅ Hoàn thành (Gọi API thực) |
| **Dashboard Manager Home** | [manager_home_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/manager/presentation/pages/manager_home_page.dart) | — | Thống kê nhanh / Điều hướng | ✅ Hoàn thành |

### Shipper

| Đầu mục Nghiệp vụ | Màn hình UI (.dart) | File Data Layer (.dart) | Endpoint API Backend | Trạng thái |
| :--- | :--- | :--- | :--- | :--- |
| **Đăng nhập & Xác thực** | [login_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/auth/presentation/pages/login_page.dart) | [auth_remote_datasource.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/auth/data/datasources/auth_remote_datasource.dart)<br>[auth_repository.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/auth/data/repositories/auth_repository.dart)<br>[auth_models.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/auth/data/models/auth_models.dart) | `POST /api/v1/Auth/login` | ✅ Hoàn thành (Gọi API thực) |
| **Danh sách đơn cần giao** | [delivery_list_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/presentation/pages/delivery_list_page.dart)<br>[courier_home_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/presentation/pages/shipper_home_page.dart) | [shipper_remote_datasource.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/data/datasources/shipper_remote_datasource.dart)<br>[shipper_repository.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/data/repositories/shipper_repository.dart)<br>[shipper_delivery_models.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/data/models/shipper_delivery_models.dart) | `GET /api/v1/shipper/deliveries` | ✅ Hoàn thành (Gọi API thực) |
| **Xem thực đơn tuần** | [shipper_weekly_menu_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/presentation/pages/shipper_weekly_menu_page.dart) | [customer_menu_remote_datasource.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/customer/data/datasources/customer_menu_remote_datasource.dart)<br>[customer_menu_repository.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/customer/data/repositories/customer_menu_repository.dart) | `GET /api/v1/master-data/WeeklyMenu` | ✅ Hoàn thành (Gọi API thực) |
| **Xem chi tiết đơn giao** *(Địa điểm, suất, thời gian)* | [delivery_detail_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/presentation/pages/delivery_detail_page.dart) | [shipper_remote_datasource.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/data/datasources/shipper_remote_datasource.dart)<br>[shipper_repository.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/data/repositories/shipper_repository.dart)<br>[shipper_delivery_models.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/data/models/shipper_delivery_models.dart) | `GET /api/v1/shipper/deliveries/{id}` | ✅ Hoàn thành (Gọi API thực) |
| **Cập nhật trạng thái đơn** *(Đang giao/Thành công/Thất bại...)* | [delivery_detail_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/presentation/pages/delivery_detail_page.dart)<br>[delivery_schedule_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/presentation/pages/delivery_schedule_page.dart) | [shipper_remote_datasource.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/data/datasources/shipper_remote_datasource.dart)<br>[shipper_repository.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/data/repositories/shipper_repository.dart) | `PATCH /api/v1/shipper/deliveries/{id}/status` | ✅ Hoàn thành (Gọi API thực) |
| **Bản đồ giao hàng & Tối ưu tuyến** | [route_map_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/presentation/pages/route_map_page.dart) | [shipper_remote_datasource.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/data/datasources/shipper_remote_datasource.dart)<br>[shipper_repository.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/data/repositories/shipper_repository.dart) | `POST /api/v1/shipper/routes/optimize` | ✅ Hoàn thành (Gọi API thực) |
| **Xác nhận giao, chụp ảnh PoD** | [proof_of_delivery_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/presentation/pages/proof_of_delivery_page.dart) | [shipper_remote_datasource.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/data/datasources/shipper_remote_datasource.dart)<br>[shipper_repository.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/data/repositories/shipper_repository.dart) | `POST /api/v1/shipper/deliveries/{id}/proof` | ✅ Hoàn thành (Gọi API thực) |
| **Lịch sử giao hàng** | [delivery_history_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/presentation/pages/delivery_history_page.dart) | [shipper_remote_datasource.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/data/datasources/shipper_remote_datasource.dart) | `GET /api/v1/shipper/deliveries?status=completed\|failed` | ✅ Hoàn thành (Gọi API thực) |
| **Trang chủ & Cá nhân Shipper** | [shipper_home_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/presentation/pages/shipper_home_page.dart)<br>[shipper_profile_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/presentation/pages/shipper_profile_page.dart)<br>[shipper_notifications_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/presentation/pages/shipper_notifications_page.dart) | — | Dashboard Shipper / Thông báo | ✅ Hoàn thành |

### Organization

| Đầu mục Nghiệp vụ | Màn hình UI (.dart) | File Data Layer (.dart) | Endpoint API Backend | Trạng thái |
| :--- | :--- | :--- | :--- | :--- |
| **Chatbot CSKH** | [chatbot_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/organization/presentation/pages/chatbot_page.dart) | — | Tích hợp dịch vụ chatbot AI | ✅ Hoàn thành |
| **Đặt suất ăn tập trung** *(3 bước — khớp web `OrganizationMealOrder`)* | [bulk_order_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/organization/presentation/pages/bulk_order_page.dart) *(Bước 1–2: giá, ngày, khuyến mãi, thực đơn theo ngày)*<br>[org_meal_order_review_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/organization/presentation/pages/org_meal_order_review_page.dart) *(Bước 3: xem nháp, % đặt cọc, checkout + PayOS)*<br>[org_dish_picker_sheet.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/organization/presentation/widgets/org_dish_picker_sheet.dart) | [org_remote_datasource.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/organization/data/datasources/org_remote_datasource.dart)<br>[org_repository.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/organization/data/org_repository.dart)<br>[bulk_order_models.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/organization/data/models/bulk_order_models.dart)<br>[org_meal_order_date_rules.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/organization/utils/org_meal_order_date_rules.dart) | `GET /api/v1/organization/meal-order/dish-category`<br>`GET /api/v1/organization/meal-order/dish/category?categoryId=`<br>`POST /api/v1/organization/meal-order/contract`<br>`POST /api/v1/organization/meal-order/checkout`<br>`POST /api/v1/organization/meal-order/pay` | ✅ Hoàn thành (Gọi API thực) |
| **Thanh toán & đối soát hợp đồng** | [contract_settlement_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/organization/presentation/pages/contract_settlement_page.dart) | [org_remote_datasource.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/organization/data/datasources/org_remote_datasource.dart)<br>[org_repository.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/organization/data/org_repository.dart)<br>[contract_models.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/organization/data/models/contract_models.dart) | `GET /api/v1/company/contracts`<br>`GET /api/v1/company/contracts/{id}`<br>`POST /api/v1/company/contracts/{id}/sign`<br>`POST /api/v1/organization/meal-order/pay` | ✅ Hoàn thành (Gọi API thực) |
| **Đánh giá suất ăn** | [meal_detail_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/customer/presentation/pages/meal_detail_page.dart) | [customer_menu_repository.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/customer/data/repositories/customer_menu_repository.dart) | `POST /api/v1/master-data/Review` | ✅ Hoàn thành (Gọi API thực) |
| **Danh sách nhân sự đơn vị** | [staff_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/organization/presentation/pages/staff_page.dart) | `GET /api/v1/Auth/profile` (`unit`) | Thông tin đơn vị từ hồ sơ; danh sách NV chi tiết trên web Admin | ✅ Hoàn thành (mức hồ sơ) |
| **Thống kê / Báo cáo / Đối soát** | [org_statistics_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/organization/presentation/pages/org_statistics_page.dart)<br>[org_reports_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/organization/presentation/pages/org_reports_page.dart)<br>[org_reconciliation_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/organization/presentation/pages/org_reconciliation_page.dart) | [meal_statistics_repository.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/core/analytics/meal_statistics_repository.dart)<br>[org_repository.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/organization/data/org_repository.dart) | `GET .../Order/statistics/meal-count`<br>`GET .../company/contracts` + payments | ✅ Hoàn thành |
| **Trang chủ & Cá nhân đơn vị** | [organization_home_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/organization/presentation/pages/organization_home_page.dart)<br>[organization_profile_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/organization/presentation/pages/organization_profile_page.dart)<br>[organization_notifications_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/organization/presentation/pages/organization_notifications_page.dart) | — | Dashboard đơn vị / Hồ sơ / Thông báo | ✅ Hoàn thành |

### Customer (Giao diện gồm 2 Tab chính)

| Đầu mục Nghiệp vụ | Màn hình UI (.dart) | File Data Layer (.dart) | Endpoint API Backend | Trạng thái |
| :--- | :--- | :--- | :--- | :--- |
| **Xem thực đơn theo loại món & Category** | [menu_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/customer/presentation/pages/menu_page.dart)<br>[meal_detail_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/customer/presentation/pages/meal_detail_page.dart) | [org_remote_datasource.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/organization/data/datasources/org_remote_datasource.dart)<br>[org_repository.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/organization/data/org_repository.dart)<br>[bulk_order_models.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/organization/data/models/bulk_order_models.dart) | `GET /api/v1/organization/meal-order/dish-category`<br>`GET /api/v1/organization/meal-order/dish/category` | ✅ Hoàn thành (Gọi API thực) |

### Các Module dùng chung & Ngoài UI_MOB

| Đầu mục Nghiệp vụ | Màn hình UI (.dart) | File Data Layer (.dart) | Endpoint API Backend | Trạng thái |
| :--- | :--- | :--- | :--- | :--- |
| **Quản lý hồ sơ cá nhân** | [profile_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/profile/presentation/pages/profile_page.dart)<br>[personal_info_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/profile/presentation/pages/personal_info_page.dart)<br>[security_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/profile/presentation/pages/security_page.dart)<br>[address_book_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/profile/presentation/pages/address_book_page.dart)<br>[notification_settings_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/profile/presentation/pages/notification_settings_page.dart)<br>[help_center_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/profile/presentation/pages/help_center_page.dart) | [profile_remote_datasource.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/profile/data/datasources/profile_remote_datasource.dart)<br>[profile_repository.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/profile/data/profile_repository.dart)<br>[user_profile_model.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/profile/data/models/user_profile_model.dart) | `GET /api/v1/Auth/profile`<br>`PUT /api/v1/Auth/profile`<br>`POST /api/v1/Auth/profile/avatar` | ✅ Hoàn thành (Gọi API thực) |
| **Thông báo hệ thống chung** | [notifications_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/notifications/presentation/pages/notifications_page.dart) | — | Hiển thị thông báo đẩy / thông báo chung | ✅ Hoàn thành |

---

## ## Ánh xạ màn hình ↔ API Backend

Nguồn: `Khoa_Luan_KS_BE/SmartLunch-Backend-Service/SmartLunch-Backend-Service-API/Controllers/v1/`.  
Tiền tố gọi thực tế: **`/api/v1/...`** (ApiVersion 1.0). Tất cả request yêu cầu xác thực cần gửi kèm Header:  
`Authorization: Bearer <JWT_TOKEN>`.

### Envelope phản hồi chung (BaseApiResponse<T>)
Tất cả các API trả về cấu trúc chuẩn JSON như sau:
```json
{
  "success": true,
  "status": 200,
  "message": "Success message details",
  "timestamp": "2026-05-17T12:00:00Z",
  "data": { ... }, // Dữ liệu payload kiểu T
  "errors": null // Hoặc mảng string chứa chi tiết lỗi khi success = false
}
```

---

### 1. Auth

| Chức năng UI | Method | Endpoint thực tế | Payload & Tham số | Vai trò & Quyền JWT |
|--------------|--------|------------------|-------------------|---------------------|
| Đăng nhập (Mọi Role) | POST | `/api/v1/Auth/login` | Body: `{ "email", "password" }` | Không yêu cầu |
| Đăng nhập Admin/Manager | POST | `/api/v1/Auth/login-admin` | Body: `{ "username", "password" }` | Không yêu cầu |
| Đăng ký tài khoản | POST | `/api/v1/Auth/register` | Body: `{ "username", "email", "password", "firstName", "lastName", "phoneNumber", "address" }` | Không yêu cầu |
| Đăng nhập Firebase | POST | `/api/v1/Auth/firebase-login` | Body: `{ "idToken" }` | Không yêu cầu |
| Đổi / Reset mật khẩu | PUT | `/api/v1/Auth/reset-password` | Body: `{ "email", "newPassword", "token" }` | Không yêu cầu |
| Xem hồ sơ cá nhân | GET | `/api/v1/Auth/profile` | — | `[Authorize]` |
| Cập nhật hồ sơ | PUT | `/api/v1/Auth/profile` | Body: `{ "firstName", "lastName", "phoneNumber", "address" }` | `[Authorize]` |
| Tải lên ảnh đại diện | POST | `/api/v1/Auth/profile/avatar` | Form-data: `file` (jpeg/png/webp) | `[Authorize]` |
| Đăng xuất | POST | `/api/v1/Auth/logout` | Body: `{ "refreshToken" }` | `[Authorize]` |
| Làm mới token | POST | `/api/v1/Auth/refresh-token` | Body: `{ "token", "refreshToken" }` | `[Authorize]` |
| Upload ảnh trực tiếp | POST | `/api/v1/Media/upload` | Form-data: `file`, `mediaType` ("image"\|"video"), `purpose`, `isPublic` (bool) | `[Authorize]` |
| Tải xuống URL ảnh | GET | `/api/v1/Media/{id}/download-url` | Query: `expiresMinutes` (int) | `[Authorize]` |
| Upload ảnh nhanh | POST | `/api/v1/Media/upload-image` | Form-data: `file` | `[Authorize]` |

---

### 2. Manager

| Chức năng UI | Method | Endpoint thực tế | Payload & Tham số | Vai trò & Quyền JWT |
|--------------|--------|------------------|-------------------|---------------------|
| Thống kê số suất ăn | GET | `/api/v1/master-data/Order/statistics/meal-count` | Query: `startDate`, `endDate` (yyyy-MM-dd), `organizationId` (int) | `roles:Admin,Company` + `orders.read` |
| Thống kê món chi tiết | GET | `/api/v1/master-data/Order/statistics/details` | Query: `startDate`, `endDate` (yyyy-MM-dd), `organizationId` (int) | `roles:Admin,Company` + `orders.read` |
| Báo cáo thu chi tổng hợp | GET | `/api/v1/finance/cashflow-summary` | Query: `startDate`, `endDate`, `period` ("daily"\|"weekly"\|"monthly") | `roles:Admin,Manager,WarehouseStaff` + `payments.read`, `transactions.read` |
| Đối soát thanh toán | GET | `/api/v1/finance/payment-reconciliation` | Query: `startDate`, `endDate`, `status` | `roles:Admin,Manager,WarehouseStaff` + `payments.read`, `orders.read` |
| Lịch sử thanh toán | GET | `/api/v1/finance/payment-history` | Query: `startDate`, `endDate`, `type` | `roles:Admin,Manager,WarehouseStaff` + `payments.read`, `partner_payments.read` |
| Công nợ theo đơn vị | GET | `/api/v1/finance/organization-receivables` | Query: `organizationId` (optional) | `roles:Admin,Manager,WarehouseStaff` + `payments.read`, `orders.read`, `organizations.read` |
| Công nợ nhà cung cấp | GET | `/api/v1/finance/supplier-payables` | Query: `partnerId` (optional) | `roles:Admin,Manager,WarehouseStaff` + `partner_payments.read`, `partners.read`, `contracts.read` |
| Xem phản hồi của khách | GET | `/api/v1/master-data/Review` | Query: `page`, `pageSize`, `searchTerm` | `roles:Admin` + `reviews.read` |
| Xem khiếu nại (Complaints) | GET | `/api/v1/master-data/Complaint` | Query: `page`, `pageSize` | `roles:Admin` + `complaints.read` |

---

### 3. Shipper

| Chức năng UI | Method | Endpoint thực tế | Payload & Tham số | Vai trò & Quyền JWT |
|--------------|--------|------------------|-------------------|---------------------|
| Danh sách đơn cần giao | GET | `/api/v1/shipper/deliveries` | Query: `page`, `pageSize`, `status` (pending\|assigned\|received\|in_transit\|completed\|failed\|rejected), `scheduledOn` (yyyy-MM-dd) | `roles:Shipper` + `deliveries.list` |
| Chi tiết đơn giao | GET | `/api/v1/shipper/deliveries/{id}` | Lấy chi tiết qua Route parameter `{id}` | `roles:Shipper` + `deliveries.read` |
| Cập nhật trạng thái giao | PATCH | `/api/v1/shipper/deliveries/{id}/status` | Body: `{ "status": "received"\|"in_transit"\|"failed"\|"rejected", "notes" }` *(notes bắt buộc nếu failed/rejected)* | `roles:Shipper` + `deliveries.update` |
| Xác nhận giao + chụp ảnh PoD | POST | `/api/v1/shipper/deliveries/{id}/proof` | Form-data: `file` (ảnh chụp thực tế), text field `notes` (optional). Tự động cập nhật status sang `completed`. | `roles:Shipper` + `deliveries.update` |
| Tối ưu tuyến đường giao | POST | `/api/v1/shipper/routes/optimize` | Body: `{ "start": { "latitude", "longitude" }, "stops": [ { "deliveryId", "latitude", "longitude" } ] }` | `roles:Shipper` + `deliveries.list` |

---

### 4. Organization

| Chức năng UI | Method | Endpoint thực tế | Payload & Tham số | Vai trò & Quyền JWT |
|--------------|--------|------------------|-------------------|---------------------|
| Xem thể loại suất ăn | GET | `/api/v1/organization/meal-order/dish-category` | Lấy các category và khung ngày được đặt | `roles:Company,Organization` |
| Xem danh sách món ăn | GET | `/api/v1/organization/meal-order/dish/category` | Query: `categoryId`, `page`, `pageSize` | `roles:Company,Organization` |
| Chuẩn bị nháp hợp đồng | POST | `/api/v1/organization/meal-order/contract` | Body: `{ "organizationId", "price", "promotionCode"?, "mealDays": [ { "serviceDate": "yyyy-MM-dd", "mealPlan": { "main"\|"side"\|"soup": [ { "dishId", "quantity" } ] } } ] }` — `organizationId` lấy từ `profile.unit.id` | `roles:Company,Organization` |
| Xác nhận đặt hàng (checkout) | POST | `/api/v1/organization/meal-order/checkout` | Body: `{ "draftId", "depositPercent" }` — `draftId` từ bước contract | `roles:Company,Organization` |
| Tạo liên kết thanh toán PayOS | POST | `/api/v1/organization/meal-order/pay` | Body: `{ "orderId" }` | `roles:Company,Organization` |
| Xem danh sách hợp đồng đơn vị | GET | `/api/v1/company/contracts` | Response `data.contracts[]` (theo membership `user_organizations`) | `roles:Company,Organization` |
| Thanh toán / đối soát theo HĐ | GET | `/api/v1/company/contracts/{id}/payments` | Response `data.customer.orders[]` + `data.supplier.paymentLines[]` | `roles:Company,Organization` |
| Xem chi tiết hợp đồng | GET | `/api/v1/company/contracts/{id}` | Lấy chi tiết qua Route parameter `{id}` | `roles:Company,Organization` |
| Ký hợp đồng số (B2B) | POST | `/api/v1/company/contracts/{id}/sign` | Body: `{ "signatureImageUrl", "signedName" }` | `roles:Company,Organization` |

---

### 5. Customer

| Chức năng UI | Method | Endpoint thực tế | Payload & Tham số | Vai trò & Quyền JWT |
|--------------|--------|------------------|-------------------|---------------------|
| **Tab 1: Theo Category** - Lấy thể loại món & khung đặt | GET | `/api/v1/organization/meal-order/dish-category` | Lấy các category và khung ngày được đặt | `[Authorize]` |
| **Tab 1: Theo Category** - Lấy món theo thể loại (categoryId) | GET | `/api/v1/organization/meal-order/dish/category` | Query: `categoryId`, `page`, `pageSize` | `[Authorize]` |

---

### 📂 Cấu trúc thư mục lib/src/features (Đã cấu trúc phẳng)
Đường dẫn gốc: `lib/src/features/`

```
lib/src/features/
├── auth/
│   ├── data/
│   │   ├── datasources/
│   │   │   └── auth_remote_datasource.dart
│   │   ├── models/
│   │   │   └── auth_models.dart
│   │   └── repositories/
│   │       └── auth_repository.dart
│   ├── auth_storage.dart
│   ├── auth_types.dart
│   └── presentation/
│       └── pages/
│           ├── forgot_password_page.dart
│           ├── login_page.dart
│           └── register_page.dart
├── customer/
│   ├── data/
│   │   ├── datasources/
│   │   │   └── customer_menu_remote_datasource.dart
│   │   ├── models/
│   │   │   └── customer_menu_models.dart
│   │   └── repositories/
│   │       └── customer_menu_repository.dart
│   └── presentation/
│       ├── pages/
│       │   ├── customer_home_page.dart
│       │   ├── meal_detail_page.dart
│       │   └── menu_page.dart
│       └── widgets/
│           ├── customer_header.dart
│           ├── meal_cards.dart
│           ├── quick_actions_section.dart
│           └── suggested_meal_card.dart
├── manager/
│   ├── data/
│   │   ├── datasources/
│   │   │   └── manager_remote_datasource.dart
│   │   ├── models/
│   │   │   ├── finance_models.dart
│   │   │   └── review_complaint_models.dart
│   │   └── repositories/
│   │       └── manager_repository.dart
│   └── presentation/
│       └── pages/
│           ├── cash_flow_page.dart
│           ├── feedback_complaints_page.dart
│           ├── manager_home_page.dart
│           ├── reconciliation_page.dart
│           ├── reports_page.dart
│           ├── report_export_page.dart
│           └── statistics_page.dart
├── notifications/
│   └── presentation/
│       └── pages/
│           └── notifications_page.dart
├── organization/
│   ├── data/
│   │   ├── datasources/
│   │   │   └── org_remote_datasource.dart
│   │   ├── models/
│   │   │   ├── bulk_order_models.dart
│   │   │   └── contract_models.dart
│   │   └── org_repository.dart
│   ├── utils/
│   │   └── org_meal_order_date_rules.dart
│   └── presentation/
│       ├── pages/
│       │   ├── bulk_order_page.dart
│       │   ├── org_meal_order_review_page.dart
│       │   ├── chatbot_page.dart
│       │   ├── contract_settlement_page.dart
│       │   ├── organization_home_page.dart
│       │   ├── organization_notifications_page.dart
│       │   ├── organization_profile_page.dart
│       │   └── people_organization_page.dart
│       └── widgets/
│           ├── organization_ui.dart
│           └── org_dish_picker_sheet.dart
├── profile/
│   ├── data/
│   │   ├── datasources/
│   │   │   └── profile_remote_datasource.dart
│   │   ├── models/
│   │   │   └── user_profile_model.dart
│   │   └── profile_repository.dart
│   └── presentation/
│       └── pages/
│           ├── address_book_page.dart
│           ├── help_center_page.dart
│           ├── notification_settings_page.dart
│           ├── personal_info_page.dart
│           ├── profile_page.dart
│           └── security_page.dart
└── shipper/
    ├── data/
    │   ├── datasources/
    │   │   └── shipper_remote_datasource.dart
    │   ├── models/
    │   │   └── shipper_delivery_models.dart
    │   └── repositories/
    │       └── shipper_repository.dart
    ├── utils/
    │   └── shipper_geo.dart
    └── presentation/
        ├── pages/
        │   ├── delivery_detail_page.dart
        │   ├── delivery_history_page.dart
        │   ├── delivery_list_page.dart
        │   ├── delivery_schedule_page.dart
        │   ├── proof_of_delivery_page.dart
        │   ├── route_map_page.dart
        │   ├── shipper_home_page.dart
        │   ├── shipper_notifications_page.dart
        │   ├── shipper_profile_page.dart
        │   └── shipper_weekly_menu_page.dart
        └── widgets/
            └── shipper_ui.dart
```