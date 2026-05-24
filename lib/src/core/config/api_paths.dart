/// Tiền tố REST bản [1.0] — khớp `ApiVersion` backend và bảng ánh xạ trong `UI_MOB.md`.
///
/// Gốc URL: `--dart-define=SMARTLUNCH_API=...` ([AppEnv.apiBaseUrl]); path luôn bắt đầu bằng [prefix].
abstract final class ApiPaths {
  static const String prefix = '/api/v1';

  // --- Auth (AuthController) ---
  static const String authLogin = '$prefix/Auth/login';
  static const String authLoginAdmin = '$prefix/Auth/login-admin';
  static const String authFirebaseLogin = '$prefix/Auth/firebase-login';
  static const String authRegister = '$prefix/Auth/register';
  static const String authResetPassword = '$prefix/Auth/reset-password';
  static const String authProfile = '$prefix/Auth/profile';
  static const String authLogout = '$prefix/Auth/logout';
  static const String authRefreshToken = '$prefix/Auth/refresh-token';

  // --- Master data: thực đơn tuần (Customer) ---
  static String weeklyMenuList() => '$prefix/master-data/WeeklyMenu';
  static String weeklyMenuDetail(int id) =>
      '$prefix/master-data/WeeklyMenu/$id/detail';

  // --- Master data: thống kê đơn (OrderController) ---
  static const String orderStatisticsMealCount =
      '$prefix/master-data/Order/statistics/meal-count';
  static const String orderStatisticsDetails =
      '$prefix/master-data/Order/statistics/details';

  // --- Shipper ---
  static const String shipperDeliveries = '$prefix/shipper/deliveries';
  static String shipperDelivery(int id) => '$prefix/shipper/deliveries/$id';
  static String shipperDeliveryStatus(int id) =>
      '$prefix/shipper/deliveries/$id/status';
  static String shipperDeliveryProof(int id) =>
      '$prefix/shipper/deliveries/$id/proof';
  static const String shipperRoutesOptimize = '$prefix/shipper/routes/optimize';

  // --- Org: Meal Order & Contracts ---
  static const String orgMealOrderDishCategory = '$prefix/organization/meal-order/dish-category';
  static const String orgMealOrderDishByCategory = '$prefix/organization/meal-order/dish/category';
  static const String orgMealOrderContract = '$prefix/organization/meal-order/contract';
  static const String orgMealOrderCheckout = '$prefix/organization/meal-order/checkout';
  static const String orgMealOrderPay = '$prefix/organization/meal-order/pay';
  static const String orgContracts = '$prefix/company/contracts';
  static String orgContractDetail(int id) => '$prefix/company/contracts/$id';
  static String orgContractSign(int id) => '$prefix/company/contracts/$id/sign';
  static String orgContractPayments(int id) => '$prefix/company/contracts/$id/payments';

  // --- Manager: Finance ---
  static const String managerCashFlowSummary = '$prefix/finance/cashflow-summary';
  static const String managerPaymentReconciliation = '$prefix/finance/payment-reconciliation';
  static const String managerOrgReceivables = '$prefix/finance/organization-receivables';
  static const String managerPaymentHistory = '$prefix/finance/payment-history';
  static const String managerSupplierPayables = '$prefix/finance/supplier-payables';

  // --- Manager: Master Data ---
  static const String managerReviews = '$prefix/master-data/Review';
  static const String managerComplaints = '$prefix/master-data/Complaint';

  // --- Media ---
  static const String mediaUploadUrl = '$prefix/Media/upload-url';
}
