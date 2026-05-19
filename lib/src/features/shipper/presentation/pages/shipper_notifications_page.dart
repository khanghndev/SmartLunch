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
      body: Center(
        child: ShipperCard(
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              Icon(Icons.notifications_none_rounded,
                  size: 48, color: kShipperRole.primary),
              const SizedBox(height: 12),
              Text(
                'Thông báo đẩy sẽ hiển thị tại đây.',
                textAlign: TextAlign.center,
                style: AppDesignSystem.body(),
              ),
              const SizedBox(height: 16),
              FilledButton(
                onPressed: () =>
                    Navigator.of(context).pushNamed(AppRoutes.notifications),
                style: FilledButton.styleFrom(backgroundColor: kShipperRole.primary),
                child: const Text('Mở trung tâm thông báo'),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
