import 'package:flutter/material.dart';

import '../theme/app_design_system.dart';
import '../../features/auth/data/repositories/auth_repository.dart';
import '../../app/app_routes.dart';

/// Dialog xác nhận chuẩn HUITMeal — bo góc, Outfit, nút rõ ràng.
Future<bool?> showAppConfirmDialog(
  BuildContext context, {
  required String title,
  required String message,
  String confirmLabel = 'Xác nhận',
  String cancelLabel = 'Hủy',
  IconData icon = Icons.help_outline_rounded,
  Color? iconColor,
  Color? iconBackground,
  bool destructive = false,
  RolePalette role = RolePalette.brand,
}) {
  final accent = destructive ? AppDesignSystem.danger : role.primary;

  return showDialog<bool>(
    context: context,
    barrierDismissible: true,
    builder: (ctx) => Dialog(
      insetPadding: const EdgeInsets.symmetric(horizontal: 28),
      backgroundColor: Colors.white,
      elevation: 0,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(20)),
      child: Padding(
        padding: const EdgeInsets.fromLTRB(24, 28, 24, 20),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Container(
              width: 56,
              height: 56,
              decoration: BoxDecoration(
                color: (iconBackground ?? accent.withValues(alpha: 0.12)),
                borderRadius: BorderRadius.circular(16),
              ),
              child: Icon(icon, color: iconColor ?? accent, size: 28),
            ),
            const SizedBox(height: 20),
            Text(
              title,
              textAlign: TextAlign.center,
              style: AppDesignSystem.sectionTitle(),
            ),
            const SizedBox(height: 10),
            Text(
              message,
              textAlign: TextAlign.center,
              style: AppDesignSystem.body(size: 14, color: AppDesignSystem.gray500),
            ),
            const SizedBox(height: 24),
            Row(
              children: [
                Expanded(
                  child: OutlinedButton(
                    onPressed: () => Navigator.of(ctx).pop(false),
                    style: OutlinedButton.styleFrom(
                      foregroundColor: AppDesignSystem.gray700,
                      side: const BorderSide(color: AppDesignSystem.gray200, width: 1.5),
                      padding: const EdgeInsets.symmetric(vertical: 14),
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(12),
                      ),
                    ),
                    child: Text(cancelLabel, style: AppDesignSystem.label()),
                  ),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: FilledButton(
                    onPressed: () => Navigator.of(ctx).pop(true),
                    style: FilledButton.styleFrom(
                      backgroundColor: accent,
                      foregroundColor: Colors.white,
                      padding: const EdgeInsets.symmetric(vertical: 14),
                      elevation: 0,
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(12),
                      ),
                    ),
                    child: Text(
                      confirmLabel,
                      style: AppDesignSystem.label(color: Colors.white),
                    ),
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    ),
  );
}

/// Hộp thoại đăng xuất — đồng bộ mọi module.
Future<bool?> showAppLogoutDialog(
  BuildContext context, {
  RolePalette role = RolePalette.brand,
}) {
  return showAppConfirmDialog(
    context,
    title: 'Đăng xuất tài khoản?',
    message:
        'Phiên đăng nhập hiện tại sẽ kết thúc. Bạn cần đăng nhập lại để tiếp tục sử dụng ứng dụng.',
    confirmLabel: 'Đăng xuất',
    cancelLabel: 'Ở lại',
    icon: Icons.logout_rounded,
    iconColor: AppDesignSystem.danger,
    iconBackground: AppDesignSystem.danger.withValues(alpha: 0.1),
    destructive: true,
    role: role,
  );
}

/// Hiển thị dialog → xóa session → về màn đăng nhập.
Future<void> performAppLogout(
  BuildContext context, {
  RolePalette role = RolePalette.brand,
}) async {
  final confirm = await showAppLogoutDialog(context, role: role);
  if (confirm != true || !context.mounted) return;

  await AuthRepository.instance.clearSession();
  if (!context.mounted) return;

  Navigator.of(context).pushNamedAndRemoveUntil(
    AppRoutes.login,
    (route) => false,
  );
}
