import 'package:flutter/material.dart';
import 'app_config.dart';
import 'app_routes.dart';
import 'not_found_page.dart';

// Auth
import '../features/auth/presentation/pages/login_page.dart';
import '../features/auth/presentation/pages/register_page.dart';
import '../features/auth/presentation/pages/forgot_password_page.dart';

// Profile
import '../features/profile/presentation/pages/profile_page.dart';
import '../features/profile/presentation/pages/personal_info_page.dart';
import '../features/profile/presentation/pages/security_page.dart';
import '../features/profile/presentation/pages/address_book_page.dart';
import '../features/profile/presentation/pages/notification_settings_page.dart';
import '../features/profile/presentation/pages/help_center_page.dart';

// Customer
import '../features/customer/presentation/pages/customer_home_page.dart';
import '../features/customer/presentation/pages/menu_page.dart';
import '../features/customer/presentation/pages/meal_detail_page.dart';

// Shipper
import '../features/shipper/presentation/pages/shipper_home_page.dart';
import '../features/shipper/presentation/pages/delivery_list_page.dart';
import '../features/shipper/presentation/pages/delivery_detail_page.dart';
import '../features/shipper/presentation/pages/route_map_page.dart';
import '../features/shipper/presentation/pages/proof_of_delivery_page.dart';
import '../features/shipper/presentation/pages/shipper_handover_pdf_page.dart';
import '../features/shipper/presentation/pages/shipper_profile_page.dart';
import '../features/shipper/presentation/pages/shipper_notifications_page.dart';
import '../features/shipper/presentation/pages/delivery_schedule_page.dart';
import '../features/shipper/presentation/pages/delivery_history_page.dart';
// Organization
import '../features/organization/presentation/pages/organization_home_page.dart';
import '../features/organization/presentation/pages/staff_page.dart';
import '../features/organization/presentation/pages/bulk_order_page.dart';
import '../features/organization/presentation/pages/org_meal_order_review_page.dart';
import '../features/organization/presentation/pages/org_order_annex_sign_page.dart';
import '../features/organization/presentation/pages/org_bulk_order_mode_page.dart';
import '../features/organization/presentation/pages/org_meal_period_contract_index_page.dart';
import '../features/organization/presentation/pages/org_meal_period_contract_review_page.dart';
import '../features/organization/presentation/pages/org_meal_period_weekly_page.dart';
import '../features/organization/data/models/org_order_annex_sign_args.dart';
import '../features/organization/data/models/bulk_order_models.dart';
import '../features/organization/data/models/org_meal_contract_models.dart';
import '../features/organization/presentation/pages/org_statistics_page.dart';
import '../features/organization/presentation/pages/org_reports_page.dart';
import '../features/organization/presentation/pages/org_reconciliation_page.dart';
import '../features/organization/presentation/pages/organization_profile_page.dart';
import '../features/organization/presentation/pages/org_notifications_page.dart';
import '../features/organization/presentation/pages/contract_settlement_page.dart';
import '../features/organization/presentation/pages/org_reviews_page.dart';
import '../features/organization/presentation/pages/chatbot_page.dart';

// Manager
import '../features/manager/presentation/pages/manager_home_page.dart';
import '../features/manager/presentation/pages/manager_statistics_page.dart';
import '../features/manager/presentation/pages/manager_reports_page.dart';
import '../features/manager/presentation/pages/manager_reconciliation_page.dart';
import '../features/manager/presentation/pages/report_export_page.dart';
import '../features/manager/presentation/pages/cash_flow_page.dart';
import '../features/manager/presentation/pages/feedback_complaints_page.dart';

// Notifications
import '../features/notifications/presentation/pages/notifications_page.dart';

class AppRouter {
  final AppConfig config;

  AppRouter(this.config);

  Route<dynamic> onGenerateRoute(RouteSettings settings) {
    switch (settings.name) {
      case AppRoutes.root:
        return _routeForInitial();
        
      // Auth
      case AppRoutes.login:
        return MaterialPageRoute(builder: (_) => const LoginPage(), settings: settings);
      case AppRoutes.register:
        return MaterialPageRoute(builder: (_) => const RegisterPage(), settings: settings);
      case AppRoutes.forgotPassword:
        return MaterialPageRoute(builder: (_) => const ForgotPasswordPage(), settings: settings);

      // Profile
      case AppRoutes.profile:
        return MaterialPageRoute(builder: (_) => const ProfilePage(), settings: settings);
      case AppRoutes.profilePersonalInfo:
        return MaterialPageRoute(builder: (_) => const PersonalInfoPage(), settings: settings);
      case AppRoutes.profileSecurity:
        return MaterialPageRoute(builder: (_) => const SecurityPage(), settings: settings);
      case AppRoutes.profileAddresses:
        return MaterialPageRoute(builder: (_) => const AddressBookPage(), settings: settings);
      case AppRoutes.profileNotifications:
        return MaterialPageRoute(builder: (_) => const NotificationSettingsPage(), settings: settings);
      case AppRoutes.profileHelp:
        return MaterialPageRoute(builder: (_) => const HelpCenterPage(), settings: settings);

      // Customer
      case AppRoutes.customerHome:
        return MaterialPageRoute(builder: (_) => const CustomerHomePage(), settings: settings);
      case AppRoutes.customerMenu:
        return MaterialPageRoute(builder: (_) => const MenuPage(), settings: settings);
      case AppRoutes.customerMealDetail:
        return MaterialPageRoute(builder: (_) => const MealDetailPage(), settings: settings);

      // Shipper
      case AppRoutes.shipperHome:
        return MaterialPageRoute(builder: (_) => const ShipperHomePage(), settings: settings);
      case AppRoutes.shipperDeliveryList:
        return MaterialPageRoute(builder: (_) => const DeliveryListPage(), settings: settings);
      case AppRoutes.shipperDeliveryDetail: {
        final id = settings.arguments is int ? settings.arguments as int : 0;
        return MaterialPageRoute(
          builder: (_) => DeliveryDetailPage(deliveryId: id),
          settings: settings,
        );
      }
      case AppRoutes.shipperRouteMap:
        return MaterialPageRoute(builder: (_) => const RouteMapPage(), settings: settings);
      case AppRoutes.shipperProof: {
        final id = settings.arguments is int ? settings.arguments as int : 0;
        return MaterialPageRoute(
          builder: (_) => ProofOfDeliveryPage(deliveryId: id),
          settings: settings,
        );
      }
      case AppRoutes.shipperHandoverPdf: {
        final args = settings.arguments;
        String? pdfUrl;
        String? subtitle;
        var title = 'Biên bản bàn giao';
        if (args is Map) {
          pdfUrl = args['pdfUrl'] as String?;
          subtitle = args['subtitle'] as String?;
          title = args['title'] as String? ?? title;
        } else if (args is String) {
          pdfUrl = args;
        }
        return MaterialPageRoute(
          builder: (_) => ShipperHandoverPdfPage(
            title: title,
            pdfUrl: pdfUrl,
            subtitle: subtitle,
          ),
          settings: settings,
        );
      }
      case AppRoutes.shipperProfile:
        return MaterialPageRoute(builder: (_) => const ShipperProfilePage(), settings: settings);
      case AppRoutes.shipperNotifications:
        return MaterialPageRoute(builder: (_) => const ShipperNotificationsPage(), settings: settings);
      case AppRoutes.shipperSchedule:
        return MaterialPageRoute(builder: (_) => const DeliverySchedulePage(), settings: settings);
      case AppRoutes.shipperHistory:
        return MaterialPageRoute(builder: (_) => const DeliveryHistoryPage(), settings: settings);

      // Organization
      case AppRoutes.orgHome:
        return MaterialPageRoute(builder: (_) => const OrganizationHomePage(), settings: settings);
      case AppRoutes.orgStaff:
        return MaterialPageRoute(builder: (_) => const StaffPage(), settings: settings);
      case AppRoutes.orgBulkOrder:
        return MaterialPageRoute(builder: (_) => const BulkOrderPage(), settings: settings);
      case AppRoutes.orgBulkOrderMode:
        return MaterialPageRoute(builder: (_) => const OrgBulkOrderModePage(), settings: settings);
      case AppRoutes.orgMealOrderReview:
        final draft = settings.arguments;
        if (draft is! PrepareMealDraftModel) {
          return MaterialPageRoute(
            builder: (_) => const Scaffold(
              body: Center(child: Text('Thiếu dữ liệu nháp hợp đồng.')),
            ),
            settings: settings,
          );
        }
        return MaterialPageRoute(
          builder: (_) => OrgMealOrderReviewPage(draft: draft),
          settings: settings,
        );
      case AppRoutes.orgOrderAnnexSign:
        final args = settings.arguments;
        if (args is! OrgOrderAnnexSignArgs) {
          return MaterialPageRoute(
            builder: (_) => const Scaffold(
              body: Center(child: Text('Thiếu thông tin đơn hàng để ký phụ lục.')),
            ),
            settings: settings,
          );
        }
        return MaterialPageRoute(
          builder: (_) => OrgOrderAnnexSignPage(args: args),
          settings: settings,
        );
      case AppRoutes.orgMealPeriodContractIndex:
        return MaterialPageRoute(builder: (_) => const OrgMealPeriodContractIndexPage(), settings: settings);
      case AppRoutes.orgMealPeriodContractReview: {
        final draft = settings.arguments;
        if (draft is! PrepareMealPeriodDraftModel) {
          return MaterialPageRoute(
            builder: (_) => const Scaffold(
              body: Center(child: Text('Thiếu dữ liệu nháp hợp đồng theo kỳ.')),
            ),
            settings: settings,
          );
        }
        return MaterialPageRoute(
          builder: (_) => OrgMealPeriodContractReviewPage(draft: draft),
          settings: settings,
        );
      }
      case AppRoutes.orgMealPeriodWeekly: {
        final contractId = settings.arguments is int ? settings.arguments as int : 0;
        return MaterialPageRoute(
          builder: (_) => OrgMealPeriodWeeklyPage(contractId: contractId),
          settings: settings,
        );
      }
      case AppRoutes.orgStatistics:
        return MaterialPageRoute(builder: (_) => const OrgStatisticsPage(), settings: settings);
      case AppRoutes.orgReports:
        return MaterialPageRoute(builder: (_) => const OrgReportsPage(), settings: settings);
      case AppRoutes.orgReconciliation:
        return MaterialPageRoute(builder: (_) => const OrgReconciliationPage(), settings: settings);
      case AppRoutes.orgProfile:
        return MaterialPageRoute(builder: (_) => const OrganizationProfilePage(), settings: settings);
      case AppRoutes.orgNotifications:
        return MaterialPageRoute(builder: (_) => const OrgNotificationsPage(), settings: settings);
      case AppRoutes.orgContractSettlement:
        return MaterialPageRoute(builder: (_) => const ContractSettlementPage(), settings: settings);
      case AppRoutes.orgReviews:
        return MaterialPageRoute(builder: (_) => const OrgReviewsPage(), settings: settings);

      // Shared
      case AppRoutes.chatbot:
        return MaterialPageRoute(builder: (_) => const ChatbotPage(), settings: settings);
      case AppRoutes.notifications:
        return MaterialPageRoute(builder: (_) => const NotificationsPage(), settings: settings);

      // Manager
      case AppRoutes.managerHome:
        return MaterialPageRoute(builder: (_) => const ManagerHomePage(), settings: settings);
      case AppRoutes.managerStatistics:
        return MaterialPageRoute(builder: (_) => const ManagerStatisticsPage(), settings: settings);
      case AppRoutes.managerReports:
        return MaterialPageRoute(builder: (_) => const ManagerReportsPage(), settings: settings);
      case AppRoutes.managerReconciliation:
        return MaterialPageRoute(builder: (_) => const ManagerReconciliationPage(), settings: settings);
      case AppRoutes.managerReportExport:
        return MaterialPageRoute(builder: (_) => const ReportExportPage(), settings: settings);
      case AppRoutes.managerCashFlow:
        return MaterialPageRoute(builder: (_) => const CashFlowPage(), settings: settings);
      case AppRoutes.managerFeedbackComplaints:
        return MaterialPageRoute(builder: (_) => const FeedbackComplaintsPage(), settings: settings);
    }

    return MaterialPageRoute(
      builder: (_) => NotFoundPage(routeName: settings.name ?? ''),
    );
  }

  Route<dynamic> _routeForInitial() {
    final routeName = AppRoutes.initialFor(config.flavor);
    return onGenerateRoute(RouteSettings(name: routeName));
  }
}
