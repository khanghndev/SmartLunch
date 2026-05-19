import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';

import '../app_config.dart';
import '../../core/constants/app_colors.dart';
import '../../core/theme/app_design_system.dart';

class AppTheme {
  static ThemeData light([AppFlavor flavor = AppFlavor.all]) {
    final role = _roleFor(flavor);
    final accent = role.primary;

    final scheme = ColorScheme.fromSeed(
      seedColor: accent,
      brightness: Brightness.light,
      surface: AppDesignSystem.gray50,
    ).copyWith(
      primary: accent,
      tertiary: AppDesignSystem.info,
      surface: AppDesignSystem.gray50,
      outline: AppDesignSystem.gray200,
      shadow: AppColors.shadow,
      error: AppDesignSystem.danger,
    );

    final textTheme = GoogleFonts.outfitTextTheme().apply(
      bodyColor: AppDesignSystem.gray900,
      displayColor: AppDesignSystem.gray900,
    );

    return ThemeData(
      colorScheme: scheme,
      useMaterial3: true,
      scaffoldBackgroundColor: AppDesignSystem.gray50,
      canvasColor: AppDesignSystem.gray50,
      textTheme: textTheme,
      appBarTheme: AppBarTheme(
        backgroundColor: Colors.white,
        foregroundColor: AppDesignSystem.gray900,
        elevation: 0,
        scrolledUnderElevation: 0,
        surfaceTintColor: Colors.transparent,
        centerTitle: true,
        titleTextStyle: AppDesignSystem.sectionTitle(),
      ),
      inputDecorationTheme: InputDecorationTheme(
        filled: true,
        fillColor: AppDesignSystem.inputFill,
        hintStyle: AppDesignSystem.body(color: AppDesignSystem.gray400),
        labelStyle: AppDesignSystem.label(),
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(AppDesignSystem.radiusMd),
          borderSide: const BorderSide(color: AppDesignSystem.gray200, width: 1.5),
        ),
        enabledBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(AppDesignSystem.radiusMd),
          borderSide: const BorderSide(color: AppDesignSystem.gray200, width: 1.5),
        ),
        focusedBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(AppDesignSystem.radiusMd),
          borderSide: BorderSide(color: role.focus, width: 1.5),
        ),
        errorBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(AppDesignSystem.radiusMd),
          borderSide: const BorderSide(color: AppDesignSystem.danger, width: 1.2),
        ),
        contentPadding: const EdgeInsets.symmetric(horizontal: 14, vertical: 14),
      ),
      filledButtonTheme: FilledButtonThemeData(
        style: FilledButton.styleFrom(
          padding: const EdgeInsets.symmetric(vertical: 14, horizontal: 18),
          textStyle: AppDesignSystem.label().copyWith(color: Colors.white),
          backgroundColor: accent,
          foregroundColor: Colors.white,
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(AppDesignSystem.radiusMd),
          ),
        ),
      ),
      elevatedButtonTheme: ElevatedButtonThemeData(
        style: ElevatedButton.styleFrom(
          padding: const EdgeInsets.symmetric(vertical: 14, horizontal: 18),
          backgroundColor: accent,
          foregroundColor: Colors.white,
          elevation: 0,
          textStyle: AppDesignSystem.label().copyWith(color: Colors.white),
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(AppDesignSystem.radiusMd),
          ),
        ),
      ),
      outlinedButtonTheme: OutlinedButtonThemeData(
        style: OutlinedButton.styleFrom(
          padding: const EdgeInsets.symmetric(vertical: 12, horizontal: 14),
          foregroundColor: AppDesignSystem.gray900,
          side: const BorderSide(color: AppDesignSystem.gray200, width: 1.5),
          textStyle: AppDesignSystem.body(color: AppDesignSystem.gray700),
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(AppDesignSystem.radiusMd),
          ),
        ),
      ),
      textButtonTheme: TextButtonThemeData(
        style: TextButton.styleFrom(
          foregroundColor: role.link,
          textStyle: AppDesignSystem.body(color: role.link).copyWith(
            fontWeight: FontWeight.w700,
          ),
        ),
      ),
      chipTheme: ChipThemeData(
        backgroundColor: Colors.white,
        selectedColor: role.primary.withValues(alpha: 0.15),
        side: const BorderSide(color: AppDesignSystem.gray200),
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(AppDesignSystem.radiusMd),
        ),
        labelStyle: AppDesignSystem.body(color: AppDesignSystem.gray700),
      ),
      dividerColor: AppDesignSystem.gray100,
      cardTheme: CardTheme(
        color: Colors.white,
        elevation: 0,
        margin: EdgeInsets.zero,
        surfaceTintColor: Colors.transparent,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(AppDesignSystem.radiusLg),
          side: const BorderSide(color: AppDesignSystem.gray100),
        ),
      ),
    );
  }

  static RolePalette _roleFor(AppFlavor flavor) {
    switch (flavor) {
      case AppFlavor.customer:
        return RolePalette.customer;
      case AppFlavor.shipper:
        return RolePalette.shipper;
      case AppFlavor.manager:
        return RolePalette.manager;
      case AppFlavor.organization:
        return RolePalette.organization;
      default:
        return RolePalette.brand;
    }
  }
}
