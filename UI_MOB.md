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
- Bản đồ giao hàng:
  + **OSM (OpenStreetMap)** in-app — `flutter_map`, xem điểm giao & tuyến nối
  + Tự động sắp xếp tuyến tối ưu (`POST /shipper/routes/optimize`)
  + **Google Maps** (ngoài app) — chỉ đường / điều hướng qua `url_launcher`
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
| `ModulePageShell` | `lib/src/core/widgets/module_page_shell.dart` | AppBar gradient + body `gray50`, `resizeToAvoidBottomInset` |
| `ModuleListView` / `moduleListPadding` | `lib/src/core/widgets/module_scroll.dart` | Padding đáy = safe area + bàn phím + extra |
| `RoleTabShell` / `RoleTabScope` | `lib/src/core/widgets/role_tab_shell.dart` | Home 2 tab + bottom nav; `bottomInset` cho scroll |
| `RoleModuleHeader` / `RoleModuleTabPage` | `lib/src/core/widgets/role_module_header.dart` | Hero ảnh + gradient theo role; menu, HUITMeal, title/subtitle; panel trắng tùy chọn |
| `RoleModuleTabShell` | `lib/src/core/widgets/role_module_shell.dart` | `PremiumDrawer` + bottom nav + `RoleTabScope` (Manager / Org / Shipper) |
| `CustomerTabShell` | `customer_shell.dart` | Shell Customer (tương đương) |
| `ManagerShellConfig` / `OrganizationShellConfig` / `ShipperShellConfig` | `*_shell.dart` trong từng feature | Cấu hình drawer + bottom nav đồng bộ |
| `ModuleCard` | `module_page_shell.dart` | Card chuẩn Auth |
| `RoleDashboardBody` | `lib/src/core/widgets/role_dashboard.dart` | Dashboard home các role; `showInternalHeader: false` khi dùng `RoleModuleHeader` |
| `SectionCard` | `lib/src/core/widgets/section_card.dart` | Section trong form |
| `ManagerPageShell` / `manager_ui.dart` | `lib/src/features/manager/presentation/widgets/manager_ui.dart` | Wrapper Manager → `RolePalette.manager` |
| `CustomerPageShell` / `customer_ui.dart` | `lib/src/features/customer/presentation/widgets/customer_ui.dart` | Wrapper Customer + `CustomerDishCard` |
| `PremiumDrawer` | `lib/src/core/widgets/premium_drawer.dart` | Drawer module: header gradient, avatar API, menu theo section, đăng xuất |
| `showAppLogoutDialog` / `performAppLogout` | `lib/src/core/widgets/app_confirm_dialog.dart` | Dialog xác nhận bo góc 20, Outfit, nút **Ở lại** / **Đăng xuất** |
| `ApiClient` | `lib/src/core/network/api_client.dart` | HTTP client; Bearer token từ `AuthStorage` |
| `SessionGuard` | `lib/src/core/navigation/session_guard.dart` | Refresh thất bại → xóa session → `AppRoutes.login` |
| `TokenRefreshCoordinator` | `lib/src/core/auth/token_refresh_coordinator.dart` | 401 → `POST /api/v1/Auth/refresh-token` → lưu JWT mới |

##### HTTP 401 — refresh token rồi mới về login
1. Request (trừ login/register) nhận **401** → `TokenRefreshCoordinator.tryRefresh()`:
   - `POST /api/v1/Auth/refresh-token` body `{ "refreshToken" }`, header `Authorization: Bearer <accessToken hiện tại>` (khớp BE `[Authorize]`).
   - Thành công: lưu `accessToken`, `refreshToken`, `refreshTokenExpiresAt` vào `AuthStorage` → **retry** request một lần.
2. Refresh thất bại hoặc retry vẫn 401 → `SessionGuard.handleUnauthorized()`: `clearSession()` + `pushNamedAndRemoveUntil(/login)`.
3. Nhiều request 401 đồng thời: chỉ một lần refresh (shared `Completer`).
4. `MaterialApp.navigatorKey` = `SessionGuard.instance.navigatorKey` (`app.dart`).
5. Ném `ApiUnauthorizedException` khi buộc đăng nhập lại.

##### Header + shell tab (Customer, Manager, Organization, Shipper)
- Tab shell: `CustomerTabShell` hoặc `RoleModuleTabShell` — drawer + bottom nav + `RoleTabScope`.
- Mỗi tab: `Column` = `RoleModuleHeader` (cố định) + `Expanded` body scroll.
- Dashboard tab: shortcut trong `RoleHeaderQuickActionsPanel`; thông báo ở `headerTrailing`.
- Tab Hồ sơ: header `Hồ sơ cá nhân` + `ProfilePage(embeddedInModuleShell: true)` (không AppBar trùng).
- Trang con (push): vẫn `ModulePageShell` / `ManagerPageShell` / `OrgPageShell`.

##### UX scroll (mọi module tab)
- Tab home: `RoleTabShell` — nội dung cuộn có `RoleTabScope.bottomInset` (không bị bottom nav che).
- Trang con: `ModulePageShell` + `ModuleListView` / `managerListPadding` — padding đáy gồm bàn phím khi nhập liệu.
- `RoleDashboardBody`: tự cộng `RoleTabScope` + `viewInsets.bottom`.
- Ảnh món: `cacheWidth` theo DPR — tránh méo pixel.
- Tránh `ListView` lồng `SizedBox(height: cố định)` + `TabBarView` — dùng `Expanded` + tab scroll riêng.

#### Theme Material (`app_theme.dart`)
- `ThemeData` dùng **Outfit**, `scaffoldBackgroundColor: gray50`, input/button/card khớp Auth.
- Accent theo `AppFlavor`: `RolePalette.customer` | `.shipper` | `.manager` | `.organization`.

#### PremiumDrawer (điều hướng module)
- Header gradient theo `RolePalette` module; nút đóng; logo **HUITMeal**.
- Avatar: `GET /api/v1/Auth/profile` → `avatarUrl`, fallback `assets/images/linh_vat.png`.
- Role badge tiếng Việt qua `formatDrawerRoleBadge`.
- Mục menu: card trắng, icon accent, tab đang chọn có viền + radio checked.
- Footer brand + nút **Đăng nhập** / **Đăng xuất** full-width.
- Dùng chung: Customer, Manager, Organization, Shipper (`DrawerSection` + `DrawerItem`).

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

#### UI báo cáo & module con (`manager_ui.dart`)
- `ManagerPageIntro` — banner mô tả đầu mỗi màn báo cáo.
- `ManagerTabBar` / `ManagerTabbedBody` — tab trong card trắng; KPI cố định phía trên.
- `ManagerSectionHeader`, `ManagerDataRow`, `ManagerStatusBadge` — danh sách & trạng thái thống nhất.
- `ManagerBarChart`, `ManagerStatTile`, `ManagerPeriodChips` — số liệu & biểu đồ.
- `ManagerReportNavCard`, `ManagerPrimaryButton` — xuất báo cáo.
- Màn con: `ManagerPageShell` + `AppDesignSystem` (không dùng `Colors.grey` / `Theme.of` rời).

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

#### Drawer Shipper (`shipper_shell.dart` → `ShipperShellConfig.drawerSections`)
| Section | Mục menu |
| :--- | :--- |
| **Điều phối** | Dashboard, Danh sách đơn hàng, Bản đồ tuyến đường |
| **Công việc** | Lịch trình giao, Lịch sử giao hàng |
| **Cá nhân** | Hồ sơ cá nhân |

*(Không có mục Thực đơn tuần.)*

#### UI giao hàng & module con (`shipper_ui.dart`)
- `ShipperPageIntro` — banner mô tả đầu mỗi màn.
- `ShipperSectionHeader`, `ShipperDataRow`, `ShipperStatusBadge`, `ShipperDetailField` — danh sách & chi tiết.
- `ShipperStatTile`, `ShipperPeriodChips`, `ShipperInfoBanner` — KPI và lọc.
- `ShipperPrimaryButton`, `ShipperOutlineButton`, `ShipperDeliveryTile` — hành động & tile đơn.
- `ShipperOsmMap` / `ShipperRouteMapPanel` — bản đồ OSM (fit bounds, polyline tuyến, attribution).
- `ShipperMapsLauncher` — mở Google Maps chỉ đường (app/web).
- `ShipperProofImage` — xem ảnh PoD sau khi giao hoàn tất.
- Phụ thuộc: `flutter_map`, `latlong2`, `url_launcher`.

| Đầu mục Nghiệp vụ | Màn hình UI (.dart) | File Data Layer (.dart) | Endpoint API Backend | Trạng thái |
| :--- | :--- | :--- | :--- | :--- |
| **Danh sách đơn cần giao** | [delivery_list_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/presentation/pages/delivery_list_page.dart)<br>[shipper_home_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/presentation/pages/shipper_home_page.dart) | [shipper_remote_datasource.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/data/datasources/shipper_remote_datasource.dart)<br>[shipper_repository.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/data/repositories/shipper_repository.dart)<br>[shipper_delivery_models.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/data/models/shipper_delivery_models.dart) | `GET /api/v1/shipper/deliveries` | ✅ Hoàn thành (Gọi API thực) |
| **Xem chi tiết đơn giao** *(Địa điểm, suất, thời gian)* | [delivery_detail_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/presentation/pages/delivery_detail_page.dart) | [shipper_remote_datasource.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/data/datasources/shipper_remote_datasource.dart)<br>[shipper_repository.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/data/repositories/shipper_repository.dart)<br>[shipper_delivery_models.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/data/models/shipper_delivery_models.dart) | `GET /api/v1/shipper/deliveries/{id}` | ✅ Hoàn thành (Gọi API thực) |
| **Cập nhật trạng thái đơn** *(Đang giao/Thành công/Thất bại...)* | [delivery_detail_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/presentation/pages/delivery_detail_page.dart)<br>[delivery_schedule_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/presentation/pages/delivery_schedule_page.dart) | [shipper_remote_datasource.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/data/datasources/shipper_remote_datasource.dart)<br>[shipper_repository.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/data/repositories/shipper_repository.dart) | `PATCH /api/v1/shipper/deliveries/{id}/status` | ✅ Hoàn thành (Gọi API thực) |
| **Bản đồ giao hàng & Tối ưu tuyến** | [route_map_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/presentation/pages/route_map_page.dart)<br>[shipper_route_map_panel.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/presentation/widgets/shipper_route_map_panel.dart)<br>[shipper_maps_launcher.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/utils/shipper_maps_launcher.dart) | [shipper_remote_datasource.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/data/datasources/shipper_remote_datasource.dart)<br>[shipper_repository.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/data/repositories/shipper_repository.dart) | `POST /api/v1/shipper/routes/optimize` + OSM map + Google Maps (url_launcher) | ✅ Hoàn thành |
| **Xác nhận giao, chụp ảnh PoD** | [proof_of_delivery_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/presentation/pages/proof_of_delivery_page.dart) | [shipper_remote_datasource.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/data/datasources/shipper_remote_datasource.dart)<br>[shipper_repository.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/data/repositories/shipper_repository.dart) | `POST /api/v1/shipper/deliveries/{id}/proof` | ✅ Hoàn thành (Gọi API thực) |
| **Lịch sử giao hàng** | [delivery_history_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/presentation/pages/delivery_history_page.dart) | [shipper_remote_datasource.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/data/datasources/shipper_remote_datasource.dart) | `GET /api/v1/shipper/deliveries?status=completed\|failed` | ✅ Hoàn thành (Gọi API thực) |
| **Trang chủ & Cá nhân Shipper** | [shipper_home_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/presentation/pages/shipper_home_page.dart)<br>[shipper_profile_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/presentation/pages/shipper_profile_page.dart)<br>[shipper_notifications_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/shipper/presentation/pages/shipper_notifications_page.dart) | — | Dashboard Shipper / Thông báo | ✅ Hoàn thành |

### Organization

#### UI báo cáo & module con (`organization_ui.dart`)
- `OrgPageIntro` — banner mô tả đầu mỗi màn báo cáo / nghiệp vụ.
- `OrgSectionHeader`, `OrgDataRow`, `OrgStatusBadge`, `OrgDetailField` — danh sách & chi tiết thống nhất.
- `OrgStatTile`, `OrgPeriodChips`, `OrgInfoBanner` — KPI và lọc kỳ.
- `OrgPrimaryButton`, `OrgCard`, `OrgLoadingBody`, `OrgErrorBody`, `OrgEmptyList` — trạng thái & hành động.
- `orgListPadding` / `ModuleListView` — scroll chuẩn trong `OrgPageShell`.
- Màn con: `OrgPageShell` + `AppDesignSystem` + `RolePalette.organization` (không màu hard-code).

| Đầu mục Nghiệp vụ | Màn hình UI (.dart) | File Data Layer (.dart) | Endpoint API Backend | Trạng thái |
| :--- | :--- | :--- | :--- | :--- |
| **Chatbot CSKH** | [chatbot_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/organization/presentation/pages/chatbot_page.dart) | — | Tích hợp dịch vụ chatbot AI | ✅ Hoàn thành |
| **Đặt suất ăn tập trung** *(khớp web `OrganizationMealOrder`)* | [bulk_order_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/organization/presentation/pages/bulk_order_page.dart) *(4 bước: Thiết lập → Thực đơn → KM → Giao hàng)*<br>[org_meal_order_review_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/organization/presentation/pages/org_meal_order_review_page.dart) *(nháp HĐ + giao hàng + % cọc)*<br>[org_order_annex_sign_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/organization/presentation/pages/org_order_annex_sign_page.dart) *(checkout → ký phụ lục → PayOS `returnUrl`/`cancelUrl`)*<br>[org_signature_pad.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/organization/presentation/widgets/org_signature_pad.dart) | `POST …/meal-order/contract` (+ `delivery`), `checkout`, `sign-annex`, `pay-deposit` | ✅ Hoàn thành |
| **Thanh toán & đối soát hợp đồng** | [contract_settlement_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/organization/presentation/pages/contract_settlement_page.dart) *(nút **Thanh toán đặt cọc** → PayOS; ký HĐ bằng pad)* | [org_remote_datasource.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/organization/data/datasources/org_remote_datasource.dart)<br>[org_repository.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/organization/data/org_repository.dart)<br>[contract_models.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/organization/data/models/contract_models.dart)<br>[org_meal_pay_deposit.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/organization/utils/org_meal_pay_deposit.dart) | `GET /api/v1/company/contracts`<br>`GET …/contracts/{id}/payments`<br>`POST …/contracts/{id}/sign`<br>`POST …/meal-order/pay` | ✅ Hoàn thành |
| **Đánh giá suất ăn** | [org_reviews_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/organization/presentation/pages/org_reviews_page.dart) | [org_remote_datasource.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/organization/data/datasources/org_remote_datasource.dart)<br>[org_repository.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/organization/data/org_repository.dart)<br>[customer_review_models.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/organization/data/models/customer_review_models.dart) | `GET /api/v1/customer-reviews/public`<br>`GET /api/v1/customer-reviews/me`<br>`POST /api/v1/customer-reviews` | ✅ Hoàn thành (Gọi API thực) |
| **Danh sách nhân sự đơn vị** | [staff_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/organization/presentation/pages/staff_page.dart) | `GET /api/v1/Auth/profile` (`unit`) | Thông tin đơn vị từ hồ sơ; danh sách NV chi tiết trên web Admin | ✅ Hoàn thành (mức hồ sơ) |
| **Thống kê / Báo cáo / Đối soát** | [org_statistics_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/organization/presentation/pages/org_statistics_page.dart)<br>[org_reports_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/organization/presentation/pages/org_reports_page.dart)<br>[org_reconciliation_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/organization/presentation/pages/org_reconciliation_page.dart) | [meal_statistics_repository.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/core/analytics/meal_statistics_repository.dart)<br>[org_repository.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/organization/data/org_repository.dart) | `GET .../Order/statistics/meal-count`<br>`GET .../company/contracts` + payments | ✅ Hoàn thành |
| **Trang chủ & Cá nhân đơn vị** | [organization_home_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/organization/presentation/pages/organization_home_page.dart)<br>[organization_profile_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/organization/presentation/pages/organization_profile_page.dart)<br>[organization_notifications_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/organization/presentation/pages/organization_notifications_page.dart) | — | Dashboard đơn vị / Hồ sơ / Thông báo | ✅ Hoàn thành |

### Customer (Giao diện gồm 2 Tab chính)

| Đầu mục Nghiệp vụ | Màn hình UI (.dart) | File Data Layer (.dart) | Endpoint API Backend | Trạng thái |
| :--- | :--- | :--- | :--- | :--- |
| **Xem thực đơn theo loại món & Category** | [customer_home_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/customer/presentation/pages/customer_home_page.dart)<br>[menu_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/customer/presentation/pages/menu_page.dart)<br>[meal_detail_page.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/customer/presentation/pages/meal_detail_page.dart) | [org_repository.dart](file:///h:/EngineeringThesis/Khoa_Luan_KS_Mobile/lib/src/features/organization/data/org_repository.dart) | `GET /api/v1/organization/meal-order/dish-category`<br>`GET /api/v1/organization/meal-order/dish/category` | ✅ Hoàn thành (Gọi API thực) |

##### Giao diện Customer (đồng bộ module — `customer_shell.dart` + `customer_ui.dart`)
- **Shell**: `CustomerTabShell` — `PremiumDrawer` + `BottomNavigationBar` + `RoleTabScope` cấu hình tại `CustomerShellConfig` (cùng mục Trang chủ / Thực đơn).
- **Header chung**: `CustomerModuleHeader` — hero ảnh + gradient giống nhau mọi tab; chỉ đổi `title` / `subtitle`; panel trắng: `CustomerHeaderSearchPanel` (Trang chủ) hoặc `CustomerHeaderCategoryPanel` (Thực đơn) — chip danh mục dùng `RoleHeaderChip`, **không** giới hạn `height` cố định (tránh cắt chữ).

#### UI module con Customer (`customer_ui.dart`)
- `CustomerPageIntro`, `CustomerGlassCard`, `CustomerSectionHeader` — section rõ ràng.
- `CustomerLoadingBody` / `CustomerErrorBody` — trạng thái tải & lỗi chuẩn.
- `CustomerQuickActions` — shortcut dùng `RoleHeaderQuickActionsPanel`.
- `CustomerFeaturedCard`, `CustomerDishCard`, `CustomerCategoryCard` — thẻ món & danh mục.
- `CustomerDishDetailArgs` — truyền `id`, `name`, `imageUrl`, `categoryName` sang màn chi tiết.
- `CustomerEmptyState` / `CustomerPrimaryButton` — empty & CTA đồng bộ Manager.
- Màn con push: `CustomerPageShell` (chi tiết món, v.v.).
- **Trang chủ**: nội dung scroll dưới header cố định — gợi ý, danh mục.
- **Thực đơn**: header `title: Thực đơn`, chip category trong panel; grid món (không giá, không filter text).
- **Tab shell**: `RoleTabShell` — scroll không bị bottom nav che.

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
| Danh sách KM đủ điều kiện | POST | `/api/v1/organization/meal-order/promotions/eligible` | Body `PreviewPromotionRequest`: `channel: "b2b_org"`, `organizationId`, `subtotal`, `totalQuantity`, `lines[]` (chỉ món main) | `roles:Company,Organization` |
| Xem trước mã KM | POST | `/api/v1/organization/meal-order/promotions/preview` | Cùng body + `promotionCode` | `roles:Company,Organization` |
| Chuẩn bị nháp hợp đồng | POST | `/api/v1/organization/meal-order/contract` | Body: `{ "organizationId", "price", "promotionCode"?, "promotionId"?, "mealDays": … }` — `organizationId` từ `profile.unit.id` | `roles:Company,Organization` |
| Xác nhận đặt hàng (checkout) | POST | `/api/v1/organization/meal-order/checkout` | Body: `{ "draftId", "depositPercent" }` — `draftId` từ bước contract | `roles:Company,Organization` |
| Tạo liên kết thanh toán PayOS | POST | `/api/v1/organization/meal-order/pay` | Body: `{ "orderId", "returnUrl", "cancelUrl" }` (bắt buộc nếu BE chưa cấu hình `PayOS:DefaultReturnUrl`) | `roles:Company,Organization` |
| Ký phụ lục đặt hàng (PNG data URL) | POST | `/api/v1/master-data/Order/{id}/sign-annex` | Body: `{ "digitalSignature" }` — sau checkout, trước PayOS | `roles:Organization` + `orders.create` |
| Xem trước PDF phụ lục | GET | `/api/v1/master-data/Order/{id}/annex-preview` | Trả `application/pdf` — mobile `OrgAnnexPdfPage` (pdfx) | `orders.read` |
| Xem danh sách hợp đồng đơn vị | GET | `/api/v1/company/contracts` | Response `data.contracts[]` (theo membership `user_organizations`) | `roles:Company,Organization` |
| Thanh toán / đối soát theo HĐ | GET | `/api/v1/company/contracts/{id}/payments` | Response `data.customer.orders[]` + `data.supplier.paymentLines[]` | `roles:Company,Organization` |
| Xem chi tiết hợp đồng | GET | `/api/v1/company/contracts/{id}` | Lấy chi tiết qua Route parameter `{id}` | `roles:Company,Organization` |
| Ký hợp đồng số (B2B) | POST | `/api/v1/company/contracts/{id}/sign` | Body: `{ "signatureImageUrl", "signedName" }` | `roles:Company,Organization` |
| **Đánh giá suất ăn (xem công khai)** | GET | `/api/v1/customer-reviews/public` | Query: `page`, `pageSize` — không cần đăng nhập | `[AllowAnonymous]` |
| **Ngữ cảnh gửi đánh giá** | GET | `/api/v1/customer-reviews/me` | Trả `canSubmitReview`, `reviewableOrders[]` | `[Authorize]` |
| **Gửi đánh giá theo đơn** | POST | `/api/v1/customer-reviews` | Body: `{ "orderId", "rating", "comment" }` | `roles:Organization,Khách hàng doanh nghiệp,Company` |

**Hiệu năng (contract / checkout / pay):**

| Endpoint | Thời gian chủ yếu | Ghi chú |
|----------|-------------------|---------|
| `POST …/contract` | 3–15s | BE tạo PDF HĐ + upload cloud — **web & mobile cùng BE** |
| `POST …/checkout` | 2–10s | Tạo đơn DB + gửi email xác nhận |
| `POST …/pay` | 5–45s+ | BE ↔ PayOS (retry 429, orderCode trùng, tạo payment mới) — **không phải do Flutter chậm hơn web** |

**Ngrok khi dev (web + mobile cùng môi trường):**

```
PayOS (webhook) ──HTTPS──► ngrok ──► BE :5001
PayOS (return sau thanh toán) ──► URL web public (thường cùng host ngrok)
App mobile ──HTTPS──► ngrok ──► BE :5001          (SMARTLUNCH_API)
Web MVC ──► BackendApi:BaseUrl ──► BE              (repo mặc định localhost:5001;
                                                    nếu bạn đổi sang URL ngrok thì = mobile)
```

- **Webhook** chỉ cần ngrok trỏ vào **BE** — không liên quan app mobile.
- **returnUrl / cancelUrl** mobile gửi qua `SMARTLUNCH_WEB` (ngrok) — giống web build URL từ `Request.Host` khi bạn mở site qua ngrok.
- Khi **cả web và mobile đều gọi cùng một URL ngrok tới BE**, thời gian `POST …/pay` trên server **giống nhau** (log ~44s là thời gian xử lý BE, không phải “mobile đi đường vòng”).

**Vì sao web vẫn *cảm giác* nhanh hơn khi PayOS lỗi (dù cùng ngrok)?**

| | Web | Mobile |
|---|-----|--------|
| Gọi API | Form POST → MVC → `InitiateOrganizationMealPayment` → **cùng** `POST …/pay` | `await` trực tiếp **cùng** `POST …/pay` |
| Trong lúc chờ ~44s | Tab trình duyệt loading (toàn trang) | Nút “Đang mở PayOS…” trên **cùng màn** |
| Khi lỗi (không có checkoutUrl) | `Redirect` sang OrderDetail + `TempData` → **trang mới**, dễ tưởng “xong” | Dialog lỗi trên màn cũ → dễ tưởng app “treo 1 phút” |
| Khi thành công | Redirect **ngay** sang PayOS (rời site) | Chờ API xong rồi mới `launchUrl` |

Thêm: mobile dễ bấm **Thanh toán cọc** nhiều lần trên cùng đơn → BE log `orderCode already exists` + PayOS 429 → nhánh xử lý **rất lâu**; web thường bấm một lần từ Orders sau khi vừa ký phụ lục.

**Tối ưu tùy chọn (không bắt buộc nếu đã dùng ngrok cho mọi thứ):** `SMARTLUNCH_API` = LAN/`10.0.2.2` (API nhanh hơn vài trăm ms), `SMARTLUNCH_WEB` = ngrok (chỉ redirect PayOS). Cache JWT RAM (`AuthStorage`), payments dashboard gọi song song.

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
        │   └── shipper_profile_page.dart
        └── widgets/
            └── shipper_ui.dart
```
