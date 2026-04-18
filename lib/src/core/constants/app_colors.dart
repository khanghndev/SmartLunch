import 'package:flutter/material.dart';

class AppColors {
  // Role colors
  static const customer = Color(0xFF17824F);
  static const customerAlt = Color(0xFF2BB673);
  static const courier = Color(0xFF1565C0);
  static const courierAlt = Color(0xFF2E86F7);
  static const org = Color(0xFF8A6328);
  static const orgAlt = Color(0xFFC9963A);

  // Base palette
  static const ink = Color(0xFF111827);
  static const inkSoft = Color(0xFF475467);
  static const border = Color(0xFFD7DFEA);
  static const surface = Color(0xFFF4F7FB);
  static const surfaceStrong = Color(0xFFEBF1F8);
  static const card = Color(0xFFFFFFFF);

  // Status
  static const success = Color(0xFF16A34A);
  static const warning = Color(0xFFD97706);
  static const danger = Color(0xFFDC2626);
  static const info = Color(0xFF2563EB);

  static const shadow = Color(0x1A0C1524);

  static List<Color> roleGradient(Color base, Color alt) => [base, alt];

  static Color tint(Color base, [double opacity = 0.12]) {
    return base.withOpacity(opacity);
  }
}
