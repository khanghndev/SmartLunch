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
      body: Center(
        child: OrgCard(
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              Icon(Icons.notifications_none_rounded, size: 48, color: orgAccent),
              const SizedBox(height: 12),
              Text(
                'Thông báo đẩy và tin hệ thống sẽ hiển thị tại đây.',
                textAlign: TextAlign.center,
                style: AppDesignSystem.body(),
              ),
              const SizedBox(height: 16),
              FilledButton(
                onPressed: () =>
                    Navigator.of(context).pushNamed(AppRoutes.notifications),
                style: FilledButton.styleFrom(backgroundColor: orgAccent),
                child: const Text('Mở trung tâm thông báo'),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
