import 'package:flutter/material.dart';

import '../../../../core/theme/app_design_system.dart';

/// Tokens Auth — delegate tới [AppDesignSystem] (single source of truth).
abstract final class AuthTheme {
  static const orange500 = AppDesignSystem.orange500;
  static const orange400 = AppDesignSystem.orange400;
  static const orange600 = AppDesignSystem.orange600;

  static const emerald500 = Color(0xFF059669);
  static const emerald400 = Color(0xFF34D399);
  static const emerald600 = Color(0xFF047857);

  static const indigo600 = AppDesignSystem.info;
  static const indigo700 = Color(0xFF4338CA);
  static const slate900 = AppDesignSystem.gray900;
  static const slate800 = Color(0xFF1E293B);
  static const amber500 = Color(0xFFF59E0B);
  static const amber600 = Color(0xFFD97706);

  static const gray50 = AppDesignSystem.gray50;
  static const gray100 = AppDesignSystem.gray100;
  static const gray200 = AppDesignSystem.gray200;
  static const gray400 = AppDesignSystem.gray400;
  static const gray500 = AppDesignSystem.gray500;
  static const gray700 = AppDesignSystem.gray700;
  static const gray900 = AppDesignSystem.gray900;
  static const inputFill = AppDesignSystem.inputFill;

  static TextStyle get displayFont => AppDesignSystem.font;
  static TextStyle titleStyle({double size = 28, Color color = gray900}) =>
      AppDesignSystem.title(size: size, color: color);
  static TextStyle roleBadgeStyle(Color accent) => AppDesignSystem.roleBadge(accent);
  static TextStyle bodyStyle({Color color = gray500}) =>
      AppDesignSystem.body(color: color);
  static TextStyle labelStyle() => AppDesignSystem.label();
}

/// Cấu hình trang auth.
class AuthPageStyle {
  final String roleLabel;
  final String title;
  final String subtitle;
  final String heroTitle;
  final String heroSubtitle;
  final String heroImageUrl;
  final IconData icon;
  final Color accent;
  final Color buttonColor;
  final Color buttonHover;
  final Color linkColor;
  final Color focusColor;
  final bool showTrustStats;
  final bool minimalHero;
  final bool showFormRoleBadge;
  final List<({String value, String label})>? trustStats;
  final List<({IconData icon, String text})>? features;

  const AuthPageStyle({
    required this.roleLabel,
    required this.title,
    required this.subtitle,
    required this.heroTitle,
    required this.heroSubtitle,
    required this.heroImageUrl,
    required this.icon,
    required this.accent,
    required this.buttonColor,
    required this.buttonHover,
    required this.linkColor,
    required this.focusColor,
    this.showTrustStats = false,
    this.minimalHero = false,
    this.showFormRoleBadge = true,
    this.trustStats,
    this.features,
  });

  static const login = AuthPageStyle(
    roleLabel: 'NỀN TẢNG B2B',
    title: 'Đăng nhập',
    subtitle: 'Quản lý suất ăn doanh nghiệp — an toàn, minh bạch & tiện lợi.',
    heroTitle: 'Suất ăn sạch cho mọi đơn vị',
    heroSubtitle: 'Đặt suất · Hợp đồng · Theo dõi giao hàng',
    heroImageUrl:
        'https://images.unsplash.com/photo-1555939594-58d7cb561ad1?q=80&w=1200',
    icon: Icons.restaurant_menu_rounded,
    accent: AuthTheme.emerald600,
    buttonColor: AuthTheme.emerald500,
    buttonHover: AuthTheme.emerald400,
    linkColor: AuthTheme.emerald600,
    focusColor: AuthTheme.emerald500,
    showTrustStats: true,
    minimalHero: false,
    showFormRoleBadge: true,
    trustStats: [
      (value: '500+', label: 'Đơn vị'),
      (value: '150k', label: 'Suất/ngày'),
      (value: '4.9★', label: 'Đánh giá'),
    ],
  );

  static const customer = AuthPageStyle(
    roleLabel: 'KHÁCH HÀNG',
    title: 'Đăng nhập Khách hàng',
    subtitle: 'Đặt suất ăn nhanh chóng và theo dõi đơn hàng dễ dàng.',
    heroTitle: 'Trải nghiệm đặt suất ăn chuẩn doanh nghiệp',
    heroSubtitle: 'Hàng trăm doanh nghiệp tin tưởng HUITMeal mỗi ngày.',
    heroImageUrl:
        'https://images.unsplash.com/photo-1555939594-58d7cb561ad1?q=80&w=1200',
    icon: Icons.restaurant_rounded,
    accent: AuthTheme.emerald600,
    buttonColor: AuthTheme.emerald500,
    buttonHover: AuthTheme.emerald400,
    linkColor: AuthTheme.emerald600,
    focusColor: AuthTheme.emerald500,
    showTrustStats: true,
    trustStats: [
      (value: '500+', label: 'Doanh nghiệp'),
      (value: '150k', label: 'Suất/ngày'),
      (value: '4.9★', label: 'Đánh giá'),
    ],
  );

  static const register = AuthPageStyle(
    roleLabel: 'GIA NHẬP CỘNG ĐỒNG',
    title: 'Tạo tài khoản',
    subtitle: 'Vui lòng điền thông tin để đăng ký thành viên HUITMeal.',
    showFormRoleBadge: false,
    heroTitle: 'Bắt đầu hành trình',
    heroSubtitle: 'Ăn sạch & sống khỏe cùng HUITMeal.',
    heroImageUrl:
        'https://images.unsplash.com/photo-1547592180-85f173990554?q=80&w=1200',
    icon: Icons.person_add_rounded,
    accent: AuthTheme.emerald600,
    buttonColor: AuthTheme.emerald500,
    buttonHover: AuthTheme.emerald400,
    linkColor: AuthTheme.emerald600,
    focusColor: AuthTheme.emerald500,
    features: [
      (icon: Icons.bolt_rounded, text: 'Đặt hàng siêu tốc trong 30s'),
      (icon: Icons.card_giftcard_rounded, text: 'Tích điểm đổi quà hấp dẫn'),
      (icon: Icons.notifications_active_rounded, text: 'Thông báo thực đơn mỗi ngày'),
    ],
  );

  static const forgotPassword = AuthPageStyle(
    roleLabel: 'BẢO MẬT',
    title: 'Đặt lại mật khẩu',
    subtitle: 'Nhập email và mật khẩu hiện tại để tạo mật khẩu mới.',
    heroTitle: 'Bảo vệ tài khoản của bạn',
    heroSubtitle: 'Thông tin được mã hóa an toàn.',
    heroImageUrl:
        'https://images.unsplash.com/photo-1563986768609-322da13575f3?q=80&w=1200',
    icon: Icons.lock_reset_rounded,
    accent: AuthTheme.emerald600,
    buttonColor: AuthTheme.emerald500,
    buttonHover: AuthTheme.emerald400,
    linkColor: AuthTheme.emerald600,
    focusColor: AuthTheme.emerald500,
  );
}
