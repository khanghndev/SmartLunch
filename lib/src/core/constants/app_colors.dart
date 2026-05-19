import 'package:flutter/material.dart';

import '../theme/app_design_system.dart';

/// Màu legacy — ưu tiên dùng [AppDesignSystem] và [RolePalette] cho UI mới.
class AppColors {
  static const customer = AppDesignSystem.orange500;
  static const customerAlt = AppDesignSystem.orange400;
  static const shipper = Color(0xFF2563EB);
  static const shipperAlt = Color(0xFF3B82F6);
  static const manager = Color(0xFFD97706);
  static const managerAlt = Color(0xFFF59E0B);
  static const org = Color(0xFF0D9488);
  static const orgAlt = Color(0xFF2DD4BF);

  static const ink = AppDesignSystem.gray900;
  static const inkSoft = AppDesignSystem.gray500;
  static const border = AppDesignSystem.gray200;
  static const surface = AppDesignSystem.gray50;
  static const surfaceStrong = AppDesignSystem.gray100;
  static const card = Colors.white;

  static const success = AppDesignSystem.success;
  static const warning = AppDesignSystem.warning;
  static const danger = AppDesignSystem.danger;
  static const info = AppDesignSystem.info;

  static const shadow = Color(0x1A0C1524);

  static List<Color> roleGradient(Color base, Color alt) => [base, alt];

  static Color tint(Color base, [double opacity = 0.12]) {
    return base.withValues(alpha: opacity);
  }
}
