import 'package:flutter/material.dart';
import 'package:url_launcher/url_launcher.dart';

import '../../../core/theme/app_design_system.dart';
import '../data/org_repository.dart';
import '../presentation/widgets/organization_ui.dart';
import 'org_payos_urls.dart';

/// Mở PayOS thanh toán cọc đơn B2B — khớp web `OrganizationMealOrder/PayDeposit`.
///
/// Thời gian phụ thuộc BE + PayOS (có thể 10–45s khi PayOS rate-limit 429).
class OrgMealPayDeposit {
  static Future<void> launch({
    required BuildContext context,
    required int orderId,
    double? depositAmountHint,
    void Function(bool loading)? onLoadingChanged,
  }) async {
    onLoadingChanged?.call(true);
    try {
      final payUrls = OrgPayOsUrls.forOrder(orderId);
      final pay = await OrgRepository.instance.initiateMealPayment(
        orderId: orderId,
        returnUrl: payUrls.returnUrl,
        cancelUrl: payUrls.cancelUrl,
      );

      if (!context.mounted) return;

      if (!pay.hasCheckoutUrl) {
        final msg = pay.payOsMessage?.trim().isNotEmpty == true
            ? pay.payOsMessage!
            : 'Không tạo được liên kết thanh toán PayOS. Kiểm tra cấu hình server.';
        await showDialog<void>(
          context: context,
          builder: (ctx) => AlertDialog(
            title: const Text('Chưa mở được PayOS'),
            content: Text(msg),
            actions: [
              TextButton(
                onPressed: () => Navigator.of(ctx).pop(),
                child: const Text('Đóng'),
              ),
            ],
          ),
        );
        return;
      }

      final uri = Uri.tryParse(pay.checkoutUrl!);
      if (uri == null || !await canLaunchUrl(uri)) {
        if (!context.mounted) return;
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Không mở được trang PayOS trên thiết bị.'),
            backgroundColor: AppDesignSystem.danger,
          ),
        );
        return;
      }

      await launchUrl(uri, mode: LaunchMode.externalApplication);
      if (!context.mounted) return;

      final amount = pay.depositAmountVnd > 0
          ? pay.depositAmountVnd
          : depositAmountHint?.round() ?? 0;

      await showDialog<void>(
        context: context,
        builder: (ctx) => AlertDialog(
          title: const Text('Thanh toán đặt cọc'),
          content: Text(
            amount > 0
                ? 'Đã chuyển sang PayOS để thanh toán cọc '
                    '${formatOrgVnd(amount.toDouble())}.\n\n'
                    'Hoàn tất thanh toán trên trình duyệt. '
                    'Đơn #$orderId chỉ chuyển "đã đặt cọc" sau khi PayOS xác nhận.'
                : 'Đã chuyển sang PayOS. Hoàn tất thanh toán trên trình duyệt.',
          ),
          actions: [
            TextButton(
              onPressed: () => Navigator.of(ctx).pop(),
              child: const Text('Đóng'),
            ),
          ],
        ),
      );
    } catch (e) {
      if (!context.mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(orgApiError(e)),
          backgroundColor: AppDesignSystem.danger,
        ),
      );
    } finally {
      onLoadingChanged?.call(false);
    }
  }
}
