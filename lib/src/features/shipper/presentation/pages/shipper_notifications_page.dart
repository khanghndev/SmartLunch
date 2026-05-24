import 'package:flutter/material.dart';

import '../../../../app/app_routes.dart';
import '../../../../core/theme/app_design_system.dart';
import '../widgets/shipper_ui.dart';

/// Shipper dùng chung màn thông báo hệ thống.
class ShipperNotificationsPage extends StatelessWidget {
  const ShipperNotificationsPage({super.key});

  @override
  Widget build(BuildContext context) {
    return ShipperPageShell(
      title: 'Thông báo',
      body: ModuleListView(
        padding: shipperListPadding(context),
        children: [
          const ShipperPageIntro(
            title: 'Thông báo Shipper',
            description:
                'Nhắc đơn mới, thay đổi lịch giao và cập nhật trạng thái sẽ hiển thị tại đây.',
            icon: Icons.notifications_active_rounded,
          ),
          const SizedBox(height: 24),
          ShipperCard(
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                Container(
                  padding: const EdgeInsets.all(16),
                  decoration: BoxDecoration(
                    color: shipperAccent.withValues(alpha: 0.1),
                    shape: BoxShape.circle,
                  ),
                  child: Icon(Icons.notifications_none_rounded, size: 40, color: shipperAccent),
                ),
                const SizedBox(height: 16),
                Text(
                  'Chưa có thông báo mới',
                  style: AppDesignSystem.sectionTitle(),
                  textAlign: TextAlign.center,
                ),
                const SizedBox(height: 8),
                Text(
                  'Mở trung tâm thông báo để xem lịch sử tin đã gửi trên tài khoản của bạn.',
                  textAlign: TextAlign.center,
                  style: AppDesignSystem.body(size: 13, color: AppDesignSystem.gray500),
                ),
                const SizedBox(height: 20),
                ShipperPrimaryButton(
                  label: 'Mở trung tâm thông báo',
                  icon: Icons.open_in_new_rounded,
                  onPressed: () =>
                      Navigator.of(context).pushNamed(AppRoutes.notifications),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
