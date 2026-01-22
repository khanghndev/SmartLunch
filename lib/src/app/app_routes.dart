import 'app_config.dart';

class AppRoutes {
  static const root = '/';
  static const splash = '/splash';
  static const welcome = '/welcome';
  static const loginCustomer = '/login/customer';
  static const loginCourier = '/login/courier';
  static const loginOrg = '/login/org';
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
  static const customerOrder = '/customer/order';
  static const customerPayment = '/customer/payment';
  static const customerRating = '/customer/rating';
  static const customerHistory = '/customer/history';
  static const customerMealDetail = '/customer/meal-detail';

  static const courierHome = '/courier';
  static const courierDeliveryList = '/courier/deliveries';
  static const courierDeliveryDetail = '/courier/delivery-detail';
  static const courierRouteMap = '/courier/route-map';
  static const courierProof = '/courier/proof-of-delivery';
  static const courierProfile = '/courier/profile';
  static const courierNotifications = '/courier/notifications';

  static const orgHome = '/org';
  static const orgStaff = '/org/staff';
  static const orgBulkOrder = '/org/bulk-order';
  static const orgStatistics = '/org/statistics';
  static const orgReports = '/org/reports';
  static const orgReconciliation = '/org/reconciliation';
  static const orgProfile = '/org/profile';
  static const orgNotifications = '/org/notifications';

  static const chatbot = '/chatbot';
  static const notifications = '/notifications';

  static String initialFor(AppFlavor flavor) {
    switch (flavor) {
      case AppFlavor.customer:
        return loginCustomer;
      case AppFlavor.courier:
        return loginCourier;
      case AppFlavor.org:
        return loginOrg;
      case AppFlavor.all:
      default:
        return splash;
    }
  }
}
