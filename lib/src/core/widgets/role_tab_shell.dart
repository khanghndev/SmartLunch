import 'package:flutter/material.dart';

import '../theme/app_design_system.dart';
import 'module_scroll.dart';

/// Scope cho tab nằm trong [RoleTabShell] (bottom nav + drawer).
class RoleTabScope extends InheritedWidget {
  final double bottomInset;
  final VoidCallback openDrawer;

  const RoleTabScope({
    super.key,
    required this.bottomInset,
    required this.openDrawer,
    required super.child,
  });

  static RoleTabScope of(BuildContext context) {
    final scope = context.dependOnInheritedWidgetOfExactType<RoleTabScope>();
    assert(scope != null, 'RoleTabScope not found. Wrap tab in RoleTabShell.');
    return scope!;
  }

  static RoleTabScope? maybeOf(BuildContext context) {
    return context.dependOnInheritedWidgetOfExactType<RoleTabScope>();
  }

  @override
  bool updateShouldNotify(RoleTabScope oldWidget) =>
      bottomInset != oldWidget.bottomInset;
}

/// Scaffold chuẩn: drawer + IndexedStack tabs + bottom navigation.
class RoleTabShell extends StatefulWidget {
  final GlobalKey<ScaffoldState>? scaffoldKey;
  final Widget drawer;
  final List<Widget> tabs;
  final List<BottomNavigationBarItem> navItems;
  final RolePalette role;
  final int initialIndex;
  final int? currentIndex;
  final ValueChanged<int>? onIndexChanged;

  const RoleTabShell({
    super.key,
    this.scaffoldKey,
    required this.drawer,
    required this.tabs,
    required this.navItems,
    required this.role,
    this.initialIndex = 0,
    this.currentIndex,
    this.onIndexChanged,
  });

  @override
  State<RoleTabShell> createState() => _RoleTabShellState();
}

class _RoleTabShellState extends State<RoleTabShell> {
  late int _internalIndex;

  @override
  void initState() {
    super.initState();
    _internalIndex = widget.initialIndex;
  }

  int get _index => widget.currentIndex ?? _internalIndex;

  void _setIndex(int value) {
    if (widget.onIndexChanged != null) {
      widget.onIndexChanged!(value);
    } else {
      setState(() => _internalIndex = value);
    }
  }

  double _bottomInset(BuildContext context) {
    return moduleScrollBottomPadding(
      context,
      extra: 12,
      bottomBarInset: kModuleBottomNavHeight,
      includeKeyboard: false,
    );
  }

  void _openDrawer() {
    widget.scaffoldKey?.currentState?.openDrawer();
  }

  @override
  Widget build(BuildContext context) {
    final bottomInset = _bottomInset(context);

    return Scaffold(
      key: widget.scaffoldKey,
      drawer: widget.drawer,
      resizeToAvoidBottomInset: false,
      body: RoleTabScope(
        bottomInset: bottomInset,
        openDrawer: _openDrawer,
        child: IndexedStack(
          index: _index,
          children: [
            for (final tab in widget.tabs)
              SizedBox.expand(child: tab),
          ],
        ),
      ),
      bottomNavigationBar: DecoratedBox(
        decoration: BoxDecoration(
          color: Colors.white,
          boxShadow: [
            BoxShadow(
              color: Colors.black.withValues(alpha: 0.05),
              blurRadius: 20,
              offset: const Offset(0, -5),
            ),
          ],
        ),
        child: SafeArea(
          top: false,
          child: BottomNavigationBar(
            currentIndex: _index,
            onTap: _setIndex,
            type: BottomNavigationBarType.fixed,
            backgroundColor: Colors.white,
            selectedItemColor: widget.role.primary,
            unselectedItemColor: AppDesignSystem.gray400,
            elevation: 0,
            items: widget.navItems,
          ),
        ),
      ),
    );
  }
}

/// AppBar gọn cho tab trong [RoleTabShell] (menu + tiêu đề).
class RoleTabAppBar extends StatelessWidget implements PreferredSizeWidget {
  final String title;
  final RolePalette role;
  final List<Widget>? actions;

  const RoleTabAppBar({
    super.key,
    required this.title,
    required this.role,
    this.actions,
  });

  @override
  Size get preferredSize => const Size.fromHeight(kToolbarHeight);

  @override
  Widget build(BuildContext context) {
    final openDrawer = RoleTabScope.of(context).openDrawer;

    return AppBar(
      leading: IconButton(
        icon: const Icon(Icons.menu_rounded),
        onPressed: openDrawer,
      ),
      title: Text(
        title,
        style: AppDesignSystem.font.copyWith(
          fontWeight: FontWeight.w800,
          fontSize: 18,
        ),
      ),
      actions: actions,
      elevation: 0,
      flexibleSpace: Container(
        decoration: BoxDecoration(gradient: AppDesignSystem.headerGradient(role)),
      ),
      foregroundColor: Colors.white,
    );
  }
}
