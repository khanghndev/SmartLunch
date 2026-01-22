import 'package:flutter/material.dart';

import 'app_config.dart';
import 'app_routes.dart';
import 'not_found_page.dart';
import 'splash_screen.dart';
import 'welcome_page.dart';
import '../features/auth/presentation/pages/forgot_password_page.dart';
import '../features/auth/presentation/pages/register_page.dart';
import '../features/auth/presentation/pages/role_login_page.dart';
import '../features/chatbot/presentation/pages/chatbot_page.dart';
import '../features/courier/delivery_detail/presentation/pages/delivery_detail_page.dart';
import '../features/courier/delivery_list/presentation/pages/delivery_list_page.dart';
import '../features/courier/presentation/pages/courier_home_page.dart';
import '../features/courier/proof_of_delivery/presentation/pages/proof_of_delivery_page.dart';
import '../features/courier/route_map/presentation/pages/route_map_page.dart';
import '../features/courier/presentation/pages/courier_profile_page.dart';
import '../features/courier/presentation/pages/courier_notifications_page.dart';
import '../features/customer/history/presentation/pages/order_history_page.dart';
import '../features/customer/menu/presentation/pages/meal_detail_page.dart';
import '../features/customer/menu/presentation/pages/menu_page.dart';
import '../features/customer/order/presentation/pages/order_page.dart';
import '../features/customer/payment/presentation/pages/payment_page.dart';
import '../features/customer/presentation/pages/customer_home_page.dart';
import '../features/customer/presentation/widgets/meal_cards.dart';
import '../features/customer/rating/presentation/pages/rating_page.dart';
import '../features/notifications/presentation/pages/notifications_page.dart';
import '../features/org/bulk_order/presentation/pages/bulk_order_page.dart';
import '../features/org/presentation/pages/org_home_page.dart';
import '../features/org/reconciliation/presentation/pages/reconciliation_page.dart';
import '../features/org/reports/presentation/pages/reports_page.dart';
import '../features/org/staff/presentation/pages/staff_page.dart';
import '../features/org/statistics/presentation/pages/statistics_page.dart';
import '../features/org/presentation/pages/org_profile_page.dart';
import '../features/org/presentation/pages/org_notifications_page.dart';
import '../features/profile/presentation/pages/profile_page.dart';
import '../features/profile/presentation/pages/personal_info_page.dart';
import '../features/profile/presentation/pages/security_page.dart';
import '../features/profile/presentation/pages/address_book_page.dart';
import '../features/profile/presentation/pages/notification_settings_page.dart';
import '../features/profile/presentation/pages/help_center_page.dart';

class AppRouter {
  final AppConfig config;

  AppRouter(this.config);

  Route<dynamic> onGenerateRoute(RouteSettings settings) {
    switch (settings.name) {
      case AppRoutes.root:
        return _routeForInitial();
      case AppRoutes.splash:
        return MaterialPageRoute(builder: (_) => const SplashScreen());
      case AppRoutes.welcome:
        return MaterialPageRoute(builder: (_) => const WelcomePage());
      case AppRoutes.loginCustomer:
        return MaterialPageRoute(
          builder: (_) => RoleLoginPage(config: RoleLoginConfig.customer()),
        );
      case AppRoutes.loginCourier:
        return MaterialPageRoute(
          builder: (_) => RoleLoginPage(config: RoleLoginConfig.courier()),
        );
      case AppRoutes.loginOrg:
        return MaterialPageRoute(
          builder: (_) => RoleLoginPage(config: RoleLoginConfig.org()),
        );
      case AppRoutes.register:
        return MaterialPageRoute(builder: (_) => const RegisterPage());
      case AppRoutes.forgotPassword:
        return MaterialPageRoute(builder: (_) => const ForgotPasswordPage());
      case AppRoutes.profile:
        return MaterialPageRoute(builder: (_) => const ProfilePage());
      case AppRoutes.profilePersonalInfo:
        return MaterialPageRoute(builder: (_) => const PersonalInfoPage());
      case AppRoutes.profileSecurity:
        return MaterialPageRoute(builder: (_) => const SecurityPage());
      case AppRoutes.profileAddresses:
        return MaterialPageRoute(builder: (_) => const AddressBookPage());
      case AppRoutes.profileNotifications:
        return MaterialPageRoute(
          builder: (_) => const NotificationSettingsPage(),
        );
      case AppRoutes.profileHelp:
        return MaterialPageRoute(builder: (_) => const HelpCenterPage());

      case AppRoutes.customerHome:
        return MaterialPageRoute(builder: (_) => const CustomerHomePage());
      case AppRoutes.customerMenu:
        return MaterialPageRoute(builder: (_) => const MenuPage());
      case AppRoutes.customerMealDetail:
        final meal = settings.arguments;
        return MaterialPageRoute(
          builder:
              (_) => MealDetailPage(
                meal: meal is MealData ? meal : suggestedMeals.first,
              ),
        );
      case AppRoutes.customerOrder:
        return MaterialPageRoute(builder: (_) => const OrderPage());
      case AppRoutes.customerPayment:
        return MaterialPageRoute(builder: (_) => const PaymentPage());
      case AppRoutes.customerRating:
        return MaterialPageRoute(builder: (_) => const RatingPage());
      case AppRoutes.customerHistory:
        return MaterialPageRoute(builder: (_) => const OrderHistoryPage());

      case AppRoutes.courierHome:
        return MaterialPageRoute(builder: (_) => const CourierHomePage());
      case AppRoutes.courierDeliveryList:
        return MaterialPageRoute(builder: (_) => const DeliveryListPage());
      case AppRoutes.courierDeliveryDetail:
        return MaterialPageRoute(builder: (_) => const DeliveryDetailPage());
      case AppRoutes.courierRouteMap:
        return MaterialPageRoute(builder: (_) => const RouteMapPage());
      case AppRoutes.courierProof:
        return MaterialPageRoute(builder: (_) => const ProofOfDeliveryPage());
      case AppRoutes.courierProfile:
        return MaterialPageRoute(builder: (_) => const CourierProfilePage());
      case AppRoutes.courierNotifications:
        return MaterialPageRoute(
          builder: (_) => const CourierNotificationsPage(),
        );

      case AppRoutes.orgHome:
        return MaterialPageRoute(builder: (_) => const OrgHomePage());
      case AppRoutes.orgStaff:
        return MaterialPageRoute(builder: (_) => const StaffPage());
      case AppRoutes.orgBulkOrder:
        return MaterialPageRoute(builder: (_) => const BulkOrderPage());
      case AppRoutes.orgStatistics:
        return MaterialPageRoute(builder: (_) => const StatisticsPage());
      case AppRoutes.orgReports:
        return MaterialPageRoute(builder: (_) => const ReportsPage());
      case AppRoutes.orgReconciliation:
        return MaterialPageRoute(builder: (_) => const ReconciliationPage());
      case AppRoutes.orgProfile:
        return MaterialPageRoute(builder: (_) => const OrgProfilePage());
      case AppRoutes.orgNotifications:
        return MaterialPageRoute(builder: (_) => const OrgNotificationsPage());

      case AppRoutes.chatbot:
        return MaterialPageRoute(builder: (_) => const ChatbotPage());
      case AppRoutes.notifications:
        return MaterialPageRoute(builder: (_) => const NotificationsPage());
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
