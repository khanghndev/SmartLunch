import 'package:flutter/material.dart';

import '../../../../app/app_routes.dart';
import '../../../../core/widgets/premium_drawer.dart';
import 'shipper_ui.dart';

export 'shipper_ui.dart' show kShipperRole;

/// Alias giữ tương thích code cũ.
const kShipperRoleShell = kShipperRole;

abstract final class ShipperShellConfig {
  static const roleBadge = 'Shipper';
  static const drawerUserRole = 'Nhân viên giao hàng';

  /// Tab 0–4: Tổng quan, Đơn giao, Lịch, Lịch sử, Hồ sơ.
  static const navItems = [
    BottomNavigationBarItem(
      icon: Icon(Icons.dashboard_outlined),
      activeIcon: Icon(Icons.dashboard_rounded),
      label: 'Tổng quan',
    ),
    BottomNavigationBarItem(
      icon: Icon(Icons.list_alt_outlined),
      activeIcon: Icon(Icons.list_alt_rounded),
      label: 'Đơn giao',
    ),
    BottomNavigationBarItem(
      icon: Icon(Icons.calendar_month_outlined),
      activeIcon: Icon(Icons.calendar_month_rounded),
      label: 'Lịch giao',
    ),
    BottomNavigationBarItem(
      icon: Icon(Icons.history_outlined),
      activeIcon: Icon(Icons.history_rounded),
      label: 'Lịch sử',
    ),
    BottomNavigationBarItem(
      icon: Icon(Icons.person_outline_rounded),
      activeIcon: Icon(Icons.person_rounded),
      label: 'Hồ sơ',
    ),
  ];

  static const drawerSections = [
    DrawerSection(
      title: 'Điều phối',
      items: [
        DrawerItem(
          icon: Icons.dashboard_rounded,
          label: 'dashboard',
          labelVi: 'Dashboard',
          tabIndex: 0,
        ),
        DrawerItem(
          icon: Icons.list_alt_rounded,
          label: 'deliveries',
          labelVi: 'Danh sách đơn hàng',
          tabIndex: 1,
        ),
        DrawerItem(
          icon: Icons.map_rounded,
          label: 'route_map',
          labelVi: 'Bản đồ tuyến đường',
          route: AppRoutes.shipperRouteMap,
        ),
      ],
    ),
    DrawerSection(
      title: 'Công việc',
      items: [
        DrawerItem(
          icon: Icons.calendar_month_rounded,
          label: 'schedule',
          labelVi: 'Lịch trình giao',
          tabIndex: 2,
        ),
        DrawerItem(
          icon: Icons.history_rounded,
          label: 'history',
          labelVi: 'Lịch sử giao hàng',
          tabIndex: 3,
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
          tabIndex: 4,
        ),
      ],
    ),
  ];
}
