import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';

import '../app_config.dart';
import '../../core/constants/app_colors.dart';
import '../../core/theme/app_design_system.dart';

class AppTheme {
  static ThemeData light([AppFlavor flavor = AppFlavor.all]) {
    final role = _roleFor(flavor);
    final accent = role.primary;
    final outfitFamily = GoogleFonts.outfit().fontFamily;
    final textTheme = AppDesignSystem.typography();

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

    return ThemeData(
      colorScheme: scheme,
      useMaterial3: true,
      fontFamily: outfitFamily,
      scaffoldBackgroundColor: AppDesignSystem.gray50,
      canvasColor: AppDesignSystem.gray50,
      textTheme: textTheme,
      primaryTextTheme: textTheme,
      appBarTheme: AppBarTheme(
        backgroundColor: Colors.white,
        foregroundColor: AppDesignSystem.gray900,
        elevation: 0,
        scrolledUnderElevation: 0,
        surfaceTintColor: Colors.transparent,
        centerTitle: true,
        titleTextStyle: textTheme.titleLarge,
        toolbarTextStyle: textTheme.bodyMedium,
      ),
      inputDecorationTheme: InputDecorationTheme(
        filled: true,
        fillColor: AppDesignSystem.inputFill,
        hintStyle: textTheme.bodyMedium?.copyWith(color: AppDesignSystem.gray400),
        labelStyle: textTheme.labelLarge,
        floatingLabelStyle: textTheme.labelMedium?.copyWith(color: role.focus),
        helperStyle: textTheme.bodySmall,
        errorStyle: textTheme.bodySmall?.copyWith(color: AppDesignSystem.danger),
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
          textStyle: textTheme.labelLarge?.copyWith(color: Colors.white),
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
          textStyle: textTheme.labelLarge?.copyWith(color: Colors.white),
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
          textStyle: textTheme.bodyMedium?.copyWith(color: AppDesignSystem.gray700),
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(AppDesignSystem.radiusMd),
          ),
        ),
      ),
      textButtonTheme: TextButtonThemeData(
        style: TextButton.styleFrom(
          foregroundColor: role.link,
          textStyle: textTheme.bodyMedium?.copyWith(
            color: role.link,
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
        labelStyle: textTheme.bodyMedium?.copyWith(color: AppDesignSystem.gray700),
        secondaryLabelStyle: textTheme.bodySmall,
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
      listTileTheme: ListTileThemeData(
        iconColor: AppDesignSystem.gray500,
        titleTextStyle: textTheme.titleSmall,
        subtitleTextStyle: textTheme.bodySmall,
        leadingAndTrailingTextStyle: textTheme.labelMedium,
      ),
      dialogTheme: DialogTheme(
        backgroundColor: Colors.white,
        surfaceTintColor: Colors.transparent,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(AppDesignSystem.radiusLg),
        ),
        titleTextStyle: textTheme.titleLarge,
        contentTextStyle: textTheme.bodyMedium,
      ),
      snackBarTheme: SnackBarThemeData(
        backgroundColor: AppDesignSystem.gray900,
        contentTextStyle: textTheme.bodyMedium?.copyWith(color: Colors.white),
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(AppDesignSystem.radiusMd),
        ),
        behavior: SnackBarBehavior.floating,
      ),
      bottomNavigationBarTheme: BottomNavigationBarThemeData(
        backgroundColor: Colors.white,
        selectedItemColor: accent,
        unselectedItemColor: AppDesignSystem.gray400,
        selectedLabelStyle: textTheme.labelSmall,
        unselectedLabelStyle: textTheme.labelSmall?.copyWith(
          fontWeight: FontWeight.w500,
        ),
        type: BottomNavigationBarType.fixed,
        elevation: 8,
      ),
      navigationBarTheme: NavigationBarThemeData(
        backgroundColor: Colors.white,
        indicatorColor: role.primary.withValues(alpha: 0.12),
        labelTextStyle: WidgetStateProperty.resolveWith((states) {
          final selected = states.contains(WidgetState.selected);
          return textTheme.labelSmall?.copyWith(
            fontWeight: selected ? FontWeight.w700 : FontWeight.w500,
            color: selected ? accent : AppDesignSystem.gray400,
          );
        }),
      ),
      tabBarTheme: TabBarTheme(
        labelStyle: textTheme.labelLarge,
        unselectedLabelStyle: textTheme.labelMedium?.copyWith(
          fontWeight: FontWeight.w500,
          color: AppDesignSystem.gray500,
        ),
      ),
      dropdownMenuTheme: DropdownMenuThemeData(
        textStyle: textTheme.bodyMedium,
      ),
      popupMenuTheme: PopupMenuThemeData(
        textStyle: textTheme.bodyMedium,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(AppDesignSystem.radiusMd),
        ),
      ),
      tooltipTheme: TooltipThemeData(
        textStyle: textTheme.bodySmall?.copyWith(color: Colors.white),
        decoration: BoxDecoration(
          color: AppDesignSystem.gray900,
          borderRadius: BorderRadius.circular(AppDesignSystem.radiusSm),
        ),
      ),
      bottomSheetTheme: const BottomSheetThemeData(
        backgroundColor: Colors.white,
        surfaceTintColor: Colors.transparent,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.vertical(
            top: Radius.circular(AppDesignSystem.radiusXl),
          ),
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
