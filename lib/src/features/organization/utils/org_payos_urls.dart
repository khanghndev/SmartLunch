import '../../../core/config/app_env.dart';

/// URL PayOS redirect — khớp web `OrganizationMealOrderController.PayDeposit`.
class OrgPayOsUrls {
  final String returnUrl;
  final String cancelUrl;

  const OrgPayOsUrls({required this.returnUrl, required this.cancelUrl});

  factory OrgPayOsUrls.forOrder(int orderId) {
    final base = AppEnv.webBaseUrl.replaceAll(RegExp(r'/+$'), '');
    return OrgPayOsUrls(
      returnUrl: '$base/OrganizationMealOrder/PaymentResult?orderId=$orderId',
      cancelUrl: '$base/Profile/Contracts?orderId=$orderId',
    );
  }
}
