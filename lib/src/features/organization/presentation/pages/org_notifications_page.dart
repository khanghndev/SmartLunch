import 'package:flutter/material.dart';

import '../../../../app/app_routes.dart';
import '../../../../core/theme/app_design_system.dart';
import '../widgets/organization_ui.dart';

class OrgNotificationsPage extends StatelessWidget {
  const OrgNotificationsPage({super.key});

  @override
  Widget build(BuildContext context) {
    return OrgPageShell(
      title: 'Thông báo',
      body: ModuleListView(
        padding: orgListPadding(context),
        children: [
          const OrgPageIntro(
            title: 'Thông báo đơn vị',
            description:
                'Tin hệ thống, nhắc thanh toán và cập nhật đơn hàng sẽ hiển thị tại đây.',
            icon: Icons.notifications_active_rounded,
          ),
          const SizedBox(height: 24),
          OrgCard(
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                Container(
                  padding: const EdgeInsets.all(16),
                  decoration: BoxDecoration(
                    color: orgAccent.withValues(alpha: 0.1),
                    shape: BoxShape.circle,
                  ),
                  child: Icon(Icons.notifications_none_rounded, size: 40, color: orgAccent),
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
                OrgPrimaryButton(
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
