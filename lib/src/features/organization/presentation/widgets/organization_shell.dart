import 'package:flutter/material.dart';

import '../../../../app/app_routes.dart';
import '../../../../core/theme/app_design_system.dart';
import '../../../../core/widgets/premium_drawer.dart';

const RolePalette kOrgRoleShell = RolePalette.organization;

abstract final class OrganizationShellConfig {
  static const roleBadge = 'Organization';
  static const drawerUserRole = 'Tổ chức B2B';

  /// Tab 0–4: Tổng quan, Đặt suất, Hợp đồng, Đánh giá, Hồ sơ.
  static const navItems = [
    BottomNavigationBarItem(
      icon: Icon(Icons.dashboard_outlined),
      activeIcon: Icon(Icons.dashboard_rounded),
      label: 'Tổng quan',
    ),
    BottomNavigationBarItem(
      icon: Icon(Icons.restaurant_menu_outlined),
      activeIcon: Icon(Icons.restaurant_menu_rounded),
      label: 'Đặt suất',
    ),
    BottomNavigationBarItem(
      icon: Icon(Icons.description_outlined),
      activeIcon: Icon(Icons.description_rounded),
      label: 'Hợp đồng',
    ),
    BottomNavigationBarItem(
      icon: Icon(Icons.star_outline_rounded),
      activeIcon: Icon(Icons.star_rounded),
      label: 'Đánh giá',
    ),
    BottomNavigationBarItem(
      icon: Icon(Icons.person_outline_rounded),
      activeIcon: Icon(Icons.person_rounded),
      label: 'Hồ sơ',
    ),
  ];

  static const drawerSections = [
    DrawerSection(
      title: 'Dịch vụ chính',
      items: [
        DrawerItem(
          icon: Icons.dashboard_rounded,
          label: 'dashboard',
          labelVi: 'Dashboard',
          tabIndex: 0,
        ),
        DrawerItem(
          icon: Icons.restaurant_menu_rounded,
          label: 'bulk_order',
          labelVi: 'Đặt suất ăn tập trung',
          tabIndex: 1,
        ),
        DrawerItem(
          icon: Icons.request_quote_rounded,
          label: 'contracts',
          labelVi: 'Hợp đồng & Thanh toán',
          tabIndex: 2,
        ),
        DrawerItem(
          icon: Icons.receipt_long_rounded,
          label: 'orders',
          labelVi: 'Lịch sử đặt món',
          route: AppRoutes.orgOrders,
        ),
        DrawerItem(
          icon: Icons.star_rounded,
          label: 'reviews',
          labelVi: 'Đánh giá suất ăn',
          tabIndex: 3,
        ),
      ],
    ),
    DrawerSection(
      title: 'Quản lý đơn vị',
      items: [
        DrawerItem(
          icon: Icons.people_alt_rounded,
          label: 'staff',
          labelVi: 'Danh sách nhân sự',
          route: AppRoutes.orgStaff,
        ),
        DrawerItem(
          icon: Icons.analytics_rounded,
          label: 'statistics',
          labelVi: 'Thống kê suất ăn',
          route: AppRoutes.orgStatistics,
        ),
        DrawerItem(
          icon: Icons.description_outlined,
          label: 'reports',
          labelVi: 'Báo cáo',
          route: AppRoutes.orgReports,
        ),
        DrawerItem(
          icon: Icons.account_balance_wallet_outlined,
          label: 'reconciliation',
          labelVi: 'Đối soát thanh toán',
          route: AppRoutes.orgReconciliation,
        ),
      ],
    ),
    DrawerSection(
      title: 'Cá nhân & Hỗ trợ',
      items: [
        DrawerItem(
          icon: Icons.person_rounded,
          label: 'profile',
          labelVi: 'Hồ sơ đơn vị',
          tabIndex: 4,
        ),
        DrawerItem(
          icon: Icons.support_agent_rounded,
          label: 'support',
          labelVi: 'Chatbot CSKH',
          route: AppRoutes.chatbot,
        ),
      ],
    ),
  ];
}
