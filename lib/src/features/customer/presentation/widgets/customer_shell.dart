import 'package:flutter/material.dart';

import '../../../../app/app_routes.dart';
import '../../../../core/widgets/premium_drawer.dart';
import '../../../../core/widgets/role_tab_shell.dart';
import 'customer_ui.dart';

/// Cấu hình drawer / bottom nav chuẩn module Customer.
abstract final class CustomerShellConfig {
  static const roleBadge = 'Customer';
  static const userName = 'Khách hàng';
  static const userRole = 'Khám phá thực đơn';

  static const navItems = [
    BottomNavigationBarItem(
      icon: Icon(Icons.home_outlined),
      activeIcon: Icon(Icons.home_rounded),
      label: 'Trang chủ',
    ),
    BottomNavigationBarItem(
      icon: Icon(Icons.restaurant_menu_outlined),
      activeIcon: Icon(Icons.restaurant_menu_rounded),
      label: 'Thực đơn',
    ),
  ];

  static const drawerSections = [
    DrawerSection(
      title: 'Menu chính',
      items: [
        DrawerItem(
          icon: Icons.home_rounded,
          label: 'home',
          labelVi: 'Trang chủ',
          tabIndex: 0,
        ),
        DrawerItem(
          icon: Icons.restaurant_menu_rounded,
          label: 'menu',
          labelVi: 'Thực đơn',
          tabIndex: 1,
        ),
      ],
    ),
  ];
}

/// Scaffold tab Customer: drawer + bottom nav + [RoleTabScope] đồng bộ.
class CustomerTabShell extends StatelessWidget {
  final GlobalKey<ScaffoldState> scaffoldKey;
  final int currentIndex;
  final ValueChanged<int> onIndexChanged;
  final List<Widget> tabs;
  final void Function(String route) onDrawerNavigate;

  const CustomerTabShell({
    super.key,
    required this.scaffoldKey,
    required this.currentIndex,
    required this.onIndexChanged,
    required this.tabs,
    required this.onDrawerNavigate,
  });

  @override
  Widget build(BuildContext context) {
    return RoleTabShell(
      scaffoldKey: scaffoldKey,
      currentIndex: currentIndex,
      onIndexChanged: onIndexChanged,
      role: kCustomerRole,
      navItems: CustomerShellConfig.navItems,
      drawer: PremiumDrawer(
        userName: CustomerShellConfig.userName,
        userRole: CustomerShellConfig.userRole,
        roleBadge: CustomerShellConfig.roleBadge,
        gradient: kCustomerRole.gradient,
        accentColor: kCustomerRole.primary,
        selectedIndex: currentIndex,
        onSelectTab: (index) {
          if (index < tabs.length) onIndexChanged(index);
        },
        onNavigate: onDrawerNavigate,
        onLogout: () {},
        sections: CustomerShellConfig.drawerSections,
      ),
      tabs: tabs,
    );
  }
}

/// Điều hướng drawer mặc định Customer (login = quay về đăng nhập).
void customerDefaultDrawerNavigate(BuildContext context, String route) {
  if (route == AppRoutes.login) {
    Navigator.of(context).pushReplacementNamed(AppRoutes.login);
  } else {
    Navigator.of(context).pushNamed(route);
  }
}
