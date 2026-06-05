import 'package:flutter/material.dart';

import '../../../../app/app_routes.dart';
import '../../../../core/theme/app_design_system.dart';
import '../widgets/organization_ui.dart';

/// Đặt suất ăn tập trung — chọn mode:
/// - Đặt suất ăn tự động (luồng cũ: OrganizationMealOrder / bulk order)
/// - Đặt suất ăn theo hợp đồng (Period-Based)
class OrgBulkOrderModePage extends StatelessWidget {
  final bool embeddedInModuleShell;

  const OrgBulkOrderModePage({
    super.key,
    this.embeddedInModuleShell = false,
  });

  @override
  Widget build(BuildContext context) {
    return OrgPageShell(
      title: 'Đặt suất ăn tập trung',
      embeddedInModuleShell: embeddedInModuleShell,
      body: ModuleListView(
        padding: orgListPadding(context),
        children: [
          const OrgPageIntro(
            title: 'Chọn hình thức đặt suất',
            description:
                'Option 1: Đặt suất ăn tự động theo ngày.\n'
                'Option 2: Đặt suất ăn theo hợp đồng theo kỳ (tháng).',
            icon: Icons.restaurant_rounded,
          ),
          const SizedBox(height: 12),
          _ModeCard(
            title: 'Đặt suất ăn tự động',
            subtitle: 'Chọn ngày → chọn món → khuyến mãi → giao hàng → ký phụ lục → PayOS',
            icon: Icons.auto_awesome_rounded,
            color: orgAccent,
            onTap: () => Navigator.of(context).pushNamed(AppRoutes.orgBulkOrder),
          ),
          const SizedBox(height: 10),
          _ModeCard(
            title: 'Đặt suất ăn theo hợp đồng',
            subtitle:
                'Thiết lập thời hạn + ngày nghỉ + số suất/ngày + giá/suất → tạo hợp đồng → đặt cọc → chọn món theo tuần',
            icon: Icons.description_outlined,
            color: RolePalette.organization.primaryAlt,
            onTap: () =>
                Navigator.of(context).pushNamed(AppRoutes.orgMealPeriodContractIndex),
          ),
        ],
      ),
    );
  }
}

class _ModeCard extends StatelessWidget {
  const _ModeCard({
    required this.title,
    required this.subtitle,
    required this.icon,
    required this.color,
    required this.onTap,
  });

  final String title;
  final String subtitle;
  final IconData icon;
  final Color color;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(16),
      child: Container(
        decoration: AppDesignSystem.card(radius: 16),
        padding: const EdgeInsets.all(16),
        child: Row(
          children: [
            Container(
              width: 44,
              height: 44,
              decoration: BoxDecoration(
                color: color.withValues(alpha: 0.12),
                borderRadius: BorderRadius.circular(14),
              ),
              child: Icon(icon, color: color),
            ),
            const SizedBox(width: 12),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(title, style: AppDesignSystem.sectionTitle()),
                  const SizedBox(height: 4),
                  Text(
                    subtitle,
                    style: AppDesignSystem.body(
                      size: 12,
                      color: AppDesignSystem.gray500,
                    ),
                  ),
                ],
              ),
            ),
            const SizedBox(width: 8),
            Icon(Icons.chevron_right_rounded, color: AppDesignSystem.gray400),
          ],
        ),
      ),
    );
  }
}

