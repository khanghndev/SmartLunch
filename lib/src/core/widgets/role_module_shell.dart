import 'package:flutter/material.dart';

import '../../app/app_routes.dart';
import 'premium_drawer.dart';
import 'role_tab_shell.dart';
import '../theme/app_design_system.dart';

/// Điều hướng drawer mặc định (đăng xuất → login).
void roleModuleDrawerNavigate(
  BuildContext context,
  String route, {
  required VoidCallback onLogout,
}) {
  if (route == AppRoutes.login) {
    onLogout();
  } else {
    Navigator.of(context).pushNamed(route);
  }
}

/// Scaffold tab chuẩn: [PremiumDrawer] + bottom nav + [RoleTabScope].
class RoleModuleTabShell extends StatelessWidget {
  final GlobalKey<ScaffoldState> scaffoldKey;
  final RolePalette role;
  final int currentIndex;
  final ValueChanged<int> onIndexChanged;
  final List<Widget> tabs;
  final List<BottomNavigationBarItem> navItems;
  final String drawerUserName;
  final String drawerUserRole;
  final String drawerRoleBadge;
  final List<DrawerSection> drawerSections;
  final VoidCallback onLogout;
  final void Function(String route)? onDrawerNavigate;

  const RoleModuleTabShell({
    super.key,
    required this.scaffoldKey,
    required this.role,
    required this.currentIndex,
    required this.onIndexChanged,
    required this.tabs,
    required this.navItems,
    required this.drawerUserName,
    required this.drawerUserRole,
    required this.drawerRoleBadge,
    required this.drawerSections,
    required this.onLogout,
    this.onDrawerNavigate,
  });

  @override
  Widget build(BuildContext context) {
    return RoleTabShell(
      scaffoldKey: scaffoldKey,
      currentIndex: currentIndex,
      onIndexChanged: onIndexChanged,
      role: role,
      navItems: navItems,
      drawer: PremiumDrawer(
        userName: drawerUserName,
        userRole: drawerUserRole,
        roleBadge: drawerRoleBadge,
        gradient: role.gradient,
        accentColor: role.primary,
        selectedIndex: currentIndex,
        onSelectTab: (index) {
          if (index < tabs.length) onIndexChanged(index);
        },
        onNavigate: onDrawerNavigate ??
            (route) => roleModuleDrawerNavigate(context, route, onLogout: onLogout),
        onLogout: onLogout,
        sections: drawerSections,
      ),
      tabs: tabs,
    );
  }
}
