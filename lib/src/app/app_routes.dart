import 'app_config.dart';

/// Tên route named — bám cấu trúc `lib/src/features/` trong `UI_MOB.md`.
class AppRoutes {
  static const root = '/';
  static const login = '/login';
  static const register = '/register';
  static const forgotPassword = '/forgot-password';
  static const profile = '/profile';
  static const profilePersonalInfo = '/profile/personal-info';
  static const profileSecurity = '/profile/security';
  static const profileAddresses = '/profile/addresses';
  static const profileNotifications = '/profile/notifications';
  static const profileHelp = '/profile/help';

  static const customerHome = '/customer';
  static const customerMenu = '/customer/menu';
  static const customerMealDetail = '/customer/meal-detail';

  static const shipperHome = '/shipper';
  static const shipperDeliveryList = '/shipper/deliveries';
  static const shipperDeliveryDetail = '/shipper/delivery-detail';
  static const shipperRouteMap = '/shipper/route-map';
  static const shipperProof = '/shipper/proof-of-delivery';
  static const shipperProfile = '/shipper/profile';
  static const shipperNotifications = '/shipper/notifications';
  static const shipperSchedule = '/shipper/schedule';
  static const shipperHistory = '/shipper/history';

  static const orgHome = '/org';
  static const orgStaff = '/org/staff';
  static const orgBulkOrder = '/org/bulk-order';
  static const orgMealOrderReview = '/org/meal-order/review';
  static const orgOrderAnnexSign = '/org/meal-order/sign';
  static const orgStatistics = '/org/statistics';
  static const orgReports = '/org/reports';
  static const orgReconciliation = '/org/reconciliation';
  static const orgProfile = '/org/profile';
  static const orgNotifications = '/org/notifications';
  static const orgContractSettlement = '/org/contract-settlement';
  static const orgReviews = '/org/reviews';

  static const chatbot = '/chatbot';
  static const notifications = '/notifications';

  static const managerHome = '/manager';
  static const managerStatistics = '/manager/statistics';
  static const managerReports = '/manager/reports';
  static const managerReconciliation = '/manager/reconciliation';
  static const managerReportExport = '/manager/report-export';
  static const managerCashFlow = '/manager/cash-flow';
  static const managerFeedbackComplaints = '/manager/feedback-complaints';

  static String initialFor(AppFlavor flavor) {
    switch (flavor) {
      case AppFlavor.customer:
        return customerHome;
      case AppFlavor.shipper:
        return login;
      case AppFlavor.organization:
        return login;
      case AppFlavor.manager:
        return login;
      case AppFlavor.all:
        return login;
    }
  }
}
