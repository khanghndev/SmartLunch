# smartlunch_mobile

SmartLunch mobile app for three roles: individual customers, shippers, and business customers.

## App flow
- Welcome screen -> choose a role
- Role login -> role-specific home dashboard
- Feature pages per role (menu, orders, delivery, reports, etc.)

## Entry points
- `lib/main.dart`: single app with role selection
- `lib/main_customer.dart`: customer-only build
- `lib/main_shipper.dart`: shipper-only build
- `lib/main_org.dart`: organization-only build

## Folder structure
- `lib/src/app/`: app shell, routing, theme, and the welcome screen
  - `app.dart`: root widget
  - `app_config.dart`: flavor config (all/customer/shipper/org)
  - `app_routes.dart`: route names
  - `router.dart`: route mapping
  - `theme/`: theme configuration
- `lib/src/core/`: shared building blocks
  - `constants/`: app colors and constants
  - `errors/`: shared error types
  - `network/`: API client and interceptors
  - `storage/`: local + secure storage
  - `services/`: integrations (maps, payment, push, analytics, AI)
  - `utils/`: helpers
  - `widgets/`: reusable UI components
- `lib/src/di/`: dependency injection setup
- `lib/src/features/`: feature-first modules
  - `auth/`: login, register, password recovery
  - `profile/`: account details and settings
  - `organization/chatbot/`: chatbot chăm sóc khách hàng (doanh nghiệp)
  - `notifications/`: push notifications
  - `customer/`: menu, orders, payments, ratings, history
  - `shipper/`: delivery list, detail, route map, proof of delivery
  - `manager/`: statistics, reports, reconciliation (quản lý công ty)
  - `organization/`: staff list, bulk orders; home/profile/notifications
  - Each feature follows clean architecture:
    - `data/`: models, datasources, repository implementations
    - `domain/`: entities, repository contracts, use cases
    - `presentation/`: UI pages, widgets, state
- `assets/`: images, icons, fonts, localization
- `test/` and `integration_test/`: unit and integration tests

## Notes
- Role dashboards are in:
  - `lib/src/features/customer/presentation/pages/customer_home_page.dart`
  - `lib/src/features/shipper/presentation/pages/shipper_home_page.dart`
  - `lib/src/features/organization/presentation/pages/org_home_page.dart`
- Welcome + role selection screen:
  - `lib/src/app/welcome_page.dart`
- Role-specific login screen:
  - `lib/src/features/auth/presentation/pages/role_login_page.dart`
