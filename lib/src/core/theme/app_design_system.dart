import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';

/// Design system HUITMeal — nguồn chuẩn từ module Auth (`auth_theme.dart`).
abstract final class AppDesignSystem {
  // ─── Brand (cam — khớp web HuitMeal) ─────────────────────────────────────
  static const orange500 = Color(0xFFF97316);
  static const orange400 = Color(0xFFFB923C);
  static const orange600 = Color(0xFFEA580C);

  // ─── Neutrals ──────────────────────────────────────────────────────────────
  static const gray50 = Color(0xFFF9FAFB);
  static const gray100 = Color(0xFFF3F4F6);
  static const gray200 = Color(0xFFE5E7EB);
  static const gray400 = Color(0xFF9CA3AF);
  static const gray500 = Color(0xFF6B7280);
  static const gray700 = Color(0xFF374151);
  static const gray900 = Color(0xFF111827);
  static const inputFill = Color(0xFFFAFAFA);

  // ─── Status ────────────────────────────────────────────────────────────────
  static const success = Color(0xFF16A34A);
  static const warning = Color(0xFFD97706);
  static const danger = Color(0xFFDC2626);
  static const info = Color(0xFF2563EB);

  // ─── Layout tokens ─────────────────────────────────────────────────────────
  static const double radiusSm = 8;
  static const double radiusMd = 12;
  static const double radiusLg = 16;
  static const double radiusXl = 20;

  static TextStyle get font => GoogleFonts.outfit();

  /// TextTheme Material 3 — một nguồn Outfit cho toàn app (UI_MOB / Auth).
  static TextTheme typography() {
    final base = GoogleFonts.outfitTextTheme();
    return base.copyWith(
      displayLarge: base.displayLarge?.copyWith(
        fontWeight: FontWeight.w900,
        letterSpacing: -0.4,
        height: 1.1,
        color: gray900,
      ),
      displayMedium: base.displayMedium?.copyWith(
        fontWeight: FontWeight.w900,
        letterSpacing: -0.3,
        height: 1.12,
        color: gray900,
      ),
      displaySmall: base.displaySmall?.copyWith(
        fontWeight: FontWeight.w800,
        height: 1.15,
        color: gray900,
      ),
      headlineLarge: base.headlineLarge?.copyWith(
        fontWeight: FontWeight.w800,
        height: 1.2,
        color: gray900,
      ),
      headlineMedium: base.headlineMedium?.copyWith(
        fontWeight: FontWeight.w800,
        height: 1.22,
        color: gray900,
      ),
      headlineSmall: base.headlineSmall?.copyWith(
        fontWeight: FontWeight.w800,
        height: 1.25,
        color: gray900,
      ),
      titleLarge: base.titleLarge?.copyWith(
        fontWeight: FontWeight.w800,
        fontSize: 18,
        height: 1.25,
        color: gray900,
      ),
      titleMedium: base.titleMedium?.copyWith(
        fontWeight: FontWeight.w700,
        height: 1.3,
        color: gray900,
      ),
      titleSmall: base.titleSmall?.copyWith(
        fontWeight: FontWeight.w700,
        height: 1.3,
        color: gray700,
      ),
      bodyLarge: base.bodyLarge?.copyWith(
        fontWeight: FontWeight.w500,
        fontSize: 16,
        height: 1.45,
        color: gray700,
      ),
      bodyMedium: base.bodyMedium?.copyWith(
        fontWeight: FontWeight.w500,
        fontSize: 14,
        height: 1.45,
        color: gray500,
      ),
      bodySmall: base.bodySmall?.copyWith(
        fontWeight: FontWeight.w500,
        fontSize: 12,
        height: 1.4,
        color: gray500,
      ),
      labelLarge: base.labelLarge?.copyWith(
        fontWeight: FontWeight.w700,
        fontSize: 14,
        height: 1.3,
        color: gray700,
      ),
      labelMedium: base.labelMedium?.copyWith(
        fontWeight: FontWeight.w700,
        fontSize: 12,
        height: 1.3,
        color: gray700,
      ),
      labelSmall: base.labelSmall?.copyWith(
        fontWeight: FontWeight.w700,
        fontSize: 11,
        height: 1.25,
        color: gray500,
      ),
    );
  }

  /// Gộp [overrides] lên nền Outfit — thay cho `TextStyle(...)` không font.
  static TextStyle mergeWith(TextStyle? overrides) =>
      body(color: gray900).merge(overrides ?? const TextStyle());

  static TextStyle title({double size = 28, Color color = gray900}) =>
      font.copyWith(
        fontSize: size,
        fontWeight: FontWeight.w900,
        color: color,
        letterSpacing: -0.4,
        height: 1.1,
      );

  static TextStyle sectionTitle({Color color = gray900}) => font.copyWith(
        fontSize: 16,
        fontWeight: FontWeight.w800,
        color: color,
      );

  static TextStyle body({Color color = gray500, double size = 14}) =>
      font.copyWith(
        fontSize: size,
        fontWeight: FontWeight.w500,
        color: color,
        height: 1.45,
      );

  static TextStyle label({Color color = gray700}) => font.copyWith(
        fontSize: 14,
        fontWeight: FontWeight.w700,
        color: color,
      );

  static TextStyle roleBadge(Color accent) => font.copyWith(
        fontSize: 10,
        fontWeight: FontWeight.w900,
        color: accent,
        letterSpacing: 3.2,
      );

  static BoxDecoration card({
    Color background = Colors.white,
    Color borderColor = gray100,
    double radius = radiusLg,
  }) =>
      BoxDecoration(
        color: background,
        borderRadius: BorderRadius.circular(radius),
        border: Border.all(color: borderColor, width: 1),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.04),
            blurRadius: 16,
            offset: const Offset(0, 6),
          ),
        ],
      );

  static LinearGradient headerGradient(RolePalette role) => LinearGradient(
        colors: [role.primary, role.primaryAlt],
        begin: Alignment.topLeft,
        end: Alignment.bottomRight,
      );

  static InputDecoration inputDecoration({
    required String label,
    String? hint,
    IconData? prefixIcon,
    Color focusColor = orange500,
  }) =>
      InputDecoration(
        labelText: label,
        hintText: hint,
        labelStyle: AppDesignSystem.label(),
        hintStyle: body(color: gray400),
        prefixIcon: prefixIcon != null ? Icon(prefixIcon, color: gray400, size: 20) : null,
        filled: true,
        fillColor: inputFill,
        contentPadding: const EdgeInsets.symmetric(horizontal: 14, vertical: 14),
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(radiusMd),
          borderSide: const BorderSide(color: gray200, width: 1.5),
        ),
        enabledBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(radiusMd),
          borderSide: const BorderSide(color: gray200, width: 1.5),
        ),
        focusedBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(radiusMd),
          borderSide: BorderSide(color: focusColor, width: 1.5),
        ),
      );
}

/// Bảng màu theo module — accent riêng, layout chung Auth.
class RolePalette {
  final Color primary;
  final Color primaryAlt;
  final Color link;
  final Color focus;

  const RolePalette({
    required this.primary,
    required this.primaryAlt,
    required this.link,
    required this.focus,
  });

  List<Color> get gradient => [primary, primaryAlt];

  /// Cam brand — Customer, CTA chung.
  static const brand = RolePalette(
    primary: AppDesignSystem.orange500,
    primaryAlt: AppDesignSystem.orange400,
    link: AppDesignSystem.orange600,
    focus: AppDesignSystem.orange500,
  );

  /// Manager — amber/vàng (quản trị, tài chính).
  static const manager = RolePalette(
    primary: Color(0xFFD97706),
    primaryAlt: Color(0xFFF59E0B),
    link: Color(0xFFB45309),
    focus: Color(0xFFF59E0B),
  );

  /// Shipper — xanh dương (tốc độ, logistics).
  static const shipper = RolePalette(
    primary: Color(0xFF2563EB),
    primaryAlt: Color(0xFF3B82F6),
    link: Color(0xFF1D4ED8),
    focus: Color(0xFF2563EB),
  );

  /// Organization — xanh ngọc (B2B).
  static const organization = RolePalette(
    primary: Color(0xFF0D9488),
    primaryAlt: Color(0xFF2DD4BF),
    link: Color(0xFF0F766E),
    focus: Color(0xFF14B8A6),
  );

  /// Customer module — dùng brand cam.
  static const customer = brand;
}
