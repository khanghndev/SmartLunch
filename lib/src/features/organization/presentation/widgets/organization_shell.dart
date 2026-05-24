import 'package:flutter/material.dart';

import '../../../../app/app_routes.dart';
import '../../../../core/theme/app_design_system.dart';
import '../../../../core/widgets/premium_drawer.dart';
import '../../../../core/widgets/role_module_shell.dart';

const RolePalette kOrgRoleShell = RolePalette.organization;

abstract final class OrganizationShellConfig {
  static const roleBadge = 'Organization';
  static const drawerUserRole = 'Tổ chức B2B';

  static const navItems = [
    BottomNavigationBarItem(
      icon: Icon(Icons.business_outlined),
      activeIcon: Icon(Icons.business_rounded),
      label: 'Dashboard',
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
          route: AppRoutes.orgBulkOrder,
        ),
        DrawerItem(
          icon: Icons.request_quote_rounded,
          label: 'contracts',
          labelVi: 'Hợp đồng & Thanh toán',
          route: AppRoutes.orgContractSettlement,
        ),
        DrawerItem(
          icon: Icons.star_rounded,
          label: 'reviews',
          labelVi: 'Đánh giá suất ăn',
          route: AppRoutes.orgReviews,
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
          tabIndex: 1,
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
