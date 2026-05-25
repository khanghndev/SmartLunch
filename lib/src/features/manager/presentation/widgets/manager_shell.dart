import 'package:flutter/material.dart';

import '../../../../app/app_routes.dart';
import '../../../../core/widgets/premium_drawer.dart';

abstract final class ManagerShellConfig {
  static const roleBadge = 'Manager';
  static const drawerUserRole = 'Quản lý hệ thống';

  static const navItems = [
    BottomNavigationBarItem(
      icon: Icon(Icons.dashboard_outlined),
      activeIcon: Icon(Icons.dashboard_rounded),
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
      title: 'Quản trị chính',
      items: [
        DrawerItem(
          icon: Icons.dashboard_rounded,
          label: 'dashboard',
          labelVi: 'Dashboard',
          tabIndex: 0,
        ),
        DrawerItem(
          icon: Icons.bar_chart_rounded,
          label: 'statistics',
          labelVi: 'Thống kê tổng quan',
          route: AppRoutes.managerStatistics,
        ),
        DrawerItem(
          icon: Icons.attach_money_rounded,
          label: 'cash_flow',
          labelVi: 'Quản lý thu chi',
          route: AppRoutes.managerCashFlow,
        ),
        DrawerItem(
          icon: Icons.receipt_long_rounded,
          label: 'reconciliation',
          labelVi: 'Đối soát thanh toán',
          route: AppRoutes.managerReconciliation,
        ),
      ],
    ),
    DrawerSection(
      title: 'Vận hành',
      items: [
        DrawerItem(
          icon: Icons.feedback_rounded,
          label: 'feedback',
          labelVi: 'Phản hồi & Khiếu nại',
          route: AppRoutes.managerFeedbackComplaints,
        ),
        DrawerItem(
          icon: Icons.file_download_rounded,
          label: 'reports',
          labelVi: 'Xuất báo cáo',
          route: AppRoutes.managerReports,
        ),
      ],
    ),
    DrawerSection(
      title: 'Cá nhân',
      items: [
        DrawerItem(
          icon: Icons.person_rounded,
          label: 'profile',
          labelVi: 'Hồ sơ cá nhân',
          tabIndex: 1,
        ),
      ],
    ),
  ];
}
