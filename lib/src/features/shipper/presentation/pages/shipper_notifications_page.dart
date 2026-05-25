import 'package:flutter/material.dart';

import '../../../../app/app_routes.dart';
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
          const SizedBox(height: 20),
          ShipperPlaceholderCard(
            icon: Icons.notifications_none_rounded,
            title: 'Chưa có thông báo mới',
            message:
                'Khi hệ thống gửi tin về đơn giao hoặc lịch trình, bạn sẽ thấy tại đây. Bạn cũng có thể mở trung tâm thông báo chung.',
            actionLabel: 'Mở trung tâm thông báo',
            onAction: () => Navigator.of(context).pushNamed(AppRoutes.notifications),
          ),
        ],
      ),
    );
  }
}
