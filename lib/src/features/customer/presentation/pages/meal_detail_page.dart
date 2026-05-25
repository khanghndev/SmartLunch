import 'package:flutter/material.dart';

import '../../../../core/theme/app_design_system.dart';
import '../widgets/customer_ui.dart';

class MealDetailPage extends StatelessWidget {
  const MealDetailPage({super.key});

  @override
  Widget build(BuildContext context) {
    final args = CustomerDishDetailArgs.fromRoute(
      ModalRoute.of(context)?.settings.arguments,
    );
    if (args == null) {
      return CustomerPageShell(
        title: 'Chi tiết món',
        body: CustomerEmptyState(
          title: 'Không tìm thấy món',
          actionLabel: 'Quay lại',
          onAction: () => Navigator.of(context).pop(),
        ),
      );
    }

    final dpr = MediaQuery.devicePixelRatioOf(context);
    final cacheWidth = (MediaQuery.sizeOf(context).width * dpr).round();

    return CustomerPageShell(
      title: 'Chi tiết món',
      body: ModuleListView(
        padding: customerListPadding(context),
        children: [
          ClipRRect(
            borderRadius: BorderRadius.circular(20),
            child: AspectRatio(
              aspectRatio: 4 / 3,
              child: CustomerDishImage(imageUrl: args.imageUrl, cacheWidth: cacheWidth),
            ),
          ),
          const SizedBox(height: 14),
          CustomerGlassCard(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                if (args.categoryName != null && args.categoryName!.isNotEmpty) ...[
                  Container(
                    padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 5),
                    decoration: BoxDecoration(
                      color: kCustomerRole.primary.withValues(alpha: 0.12),
                      borderRadius: BorderRadius.circular(999),
                    ),
                    child: Text(
                      args.categoryName!,
                      style: AppDesignSystem.body(size: 11, color: kCustomerRole.link)
                          .copyWith(fontWeight: FontWeight.w700),
                    ),
                  ),
                  const SizedBox(height: 10),
                ],
                Text(args.name, style: AppDesignSystem.title(size: 26)),
                const SizedBox(height: 12),
                Wrap(
                  spacing: 8,
                  runSpacing: 8,
                  children: [
                    _DetailChip(icon: Icons.eco_rounded, label: 'Suất ăn sạch'),
                    _DetailChip(icon: Icons.visibility_rounded, label: 'Xem miễn phí'),
                  ],
                ),
              ],
            ),
          ),
          const SizedBox(height: 12),
          CustomerGlassCard(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const CustomerSectionHeader(
                  title: 'Về món ăn',
                  subtitle: 'Thông tin dành cho khách tham khảo',
                ),
                Text(
                  'Món được phục vụ theo thực đơn tuần của đơn vị. Bạn có thể xem thực đơn đầy đủ mà không cần đăng nhập.',
                  style: AppDesignSystem.body(size: 14, color: AppDesignSystem.gray700),
                ),
                const SizedBox(height: 10),
                _InfoRow(
                  icon: Icons.calendar_today_rounded,
                  text: 'Thực đơn có thể thay đổi theo tuần',
                ),
                const _InfoRow(
                  icon: Icons.groups_rounded,
                  text: 'Đặt suất tập thể — liên hệ quản lý đơn vị',
                ),
              ],
            ),
          ),
          const SizedBox(height: 12),
          CustomerPrimaryButton(
            label: 'Quay lại thực đơn',
            icon: Icons.restaurant_menu_rounded,
            onPressed: () => Navigator.of(context).pop(),
          ),
          const SizedBox(height: 8),
          CustomerSecondaryButton(
            label: 'Về trang chủ',
            icon: Icons.home_rounded,
            onPressed: () => Navigator.of(context).popUntil((route) => route.isFirst),
          ),
        ],
      ),
    );
  }
}

class _DetailChip extends StatelessWidget {
  final IconData icon;
  final String label;

  const _DetailChip({required this.icon, required this.label});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
      decoration: BoxDecoration(
        color: kCustomerRole.primary.withValues(alpha: 0.1),
        borderRadius: BorderRadius.circular(999),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(icon, size: 14, color: kCustomerRole.primary),
          const SizedBox(width: 6),
          Text(label, style: AppDesignSystem.body(size: 11, color: kCustomerRole.link)),
        ],
      ),
    );
  }
}

class _InfoRow extends StatelessWidget {
  final IconData icon;
  final String text;

  const _InfoRow({required this.icon, required this.text});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.only(top: 8),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Icon(icon, size: 18, color: kCustomerRole.primary),
          const SizedBox(width: 10),
          Expanded(
            child: Text(text, style: AppDesignSystem.body(size: 13)),
          ),
        ],
      ),
    );
  }
}
