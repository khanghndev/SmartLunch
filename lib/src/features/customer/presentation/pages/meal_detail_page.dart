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
            borderRadius: BorderRadius.circular(AppDesignSystem.radiusLg),
            child: AspectRatio(
              aspectRatio: 16 / 10,
              child: CustomerDishImage(imageUrl: args.imageUrl, cacheWidth: cacheWidth),
            ),
          ),
          const SizedBox(height: 16),
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
                Text(args.name, style: AppDesignSystem.title(size: 24)),
                const SizedBox(height: 8),
                Row(
                  children: [
                    Icon(Icons.verified_rounded, color: kCustomerRole.primary, size: 18),
                    const SizedBox(width: 6),
                    Expanded(
                      child: Text(
                        'Suất ăn chuẩn HUITMeal — nguyên liệu tươi, giao đúng giờ',
                        style: AppDesignSystem.body(size: 13, color: AppDesignSystem.gray700),
                      ),
                    ),
                  ],
                ),
              ],
            ),
          ),
          const SizedBox(height: 12),
          const CustomerPageIntro(
            title: 'Thông tin món',
            description:
                'Món được phục vụ theo thực đơn tuần của đơn vị. Liên hệ quản lý nếu bạn cần đặt suất tập thể.',
            icon: Icons.info_outline_rounded,
          ),
          const SizedBox(height: 12),
          CustomerGlassCard(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const CustomerSectionHeader(
                  title: 'Đánh giá suất ăn',
                  subtitle: 'Chia sẻ trải nghiệm sau khi dùng bữa',
                ),
                const SizedBox(height: 8),
                Text(
                  'Tính năng gửi đánh giá sẽ được bổ sung trong bản cập nhật tiếp theo.',
                  style: AppDesignSystem.body(size: 13),
                ),
                const SizedBox(height: 14),
                CustomerPrimaryButton(
                  label: 'Quay lại thực đơn',
                  icon: Icons.restaurant_menu_rounded,
                  onPressed: () => Navigator.of(context).pop(),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
