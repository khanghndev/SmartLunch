import 'dart:ui';
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

/// Padding list trong tab bottom nav — tránh nội dung bị che.
EdgeInsets moduleTabListPadding(
  BuildContext context, {
  double horizontal = 16,
  double top = 16,
}) {
  final scope = RoleTabScope.maybeOf(context);
  if (scope != null) {
    return EdgeInsets.fromLTRB(horizontal, top, horizontal, scope.bottomInset);
  }
  return moduleListPadding(context);
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
      bottomNavigationBar: _PremiumBottomNav(
        items: widget.navItems,
        currentIndex: _index,
        onTap: _setIndex,
        role: widget.role,
      ),
    );
  }
}

/// Premium pill-style bottom navigation bar.
class _PremiumBottomNav extends StatelessWidget {
  final List<BottomNavigationBarItem> items;
  final int currentIndex;
  final ValueChanged<int> onTap;
  final RolePalette role;

  const _PremiumBottomNav({
    required this.items,
    required this.currentIndex,
    required this.onTap,
    required this.role,
  });

  @override
  Widget build(BuildContext context) {
    return ClipRect(
      child: BackdropFilter(
        filter: ImageFilter.blur(sigmaX: 0, sigmaY: 0),
        child: Container(
          decoration: BoxDecoration(
            color: Colors.white.withValues(alpha: 0.98),
            border: Border(
              top: BorderSide(
                color: role.primary.withValues(alpha: 0.08),
                width: 0.5,
              ),
            ),
            boxShadow: [
              BoxShadow(
                color: Colors.black.withValues(alpha: 0.06),
                blurRadius: 32,
                offset: const Offset(0, -8),
              ),
              BoxShadow(
                color: role.primary.withValues(alpha: 0.04),
                blurRadius: 16,
                offset: const Offset(0, -4),
              ),
            ],
          ),
          child: SafeArea(
            top: false,
            child: Padding(
              padding: const EdgeInsets.fromLTRB(8, 6, 8, 6),
              child: Row(
                children: List.generate(items.length, (index) {
                  final selected = index == currentIndex;
                  final item = items[index];
                  return Expanded(
                    child: _NavPillItem(
                      item: item,
                      selected: selected,
                      role: role,
                      onTap: () => onTap(index),
                    ),
                  );
                }),
              ),
            ),
          ),
        ),
      ),
    );
  }
}

class _NavPillItem extends StatefulWidget {
  final BottomNavigationBarItem item;
  final bool selected;
  final RolePalette role;
  final VoidCallback onTap;

  const _NavPillItem({
    required this.item,
    required this.selected,
    required this.role,
    required this.onTap,
  });

  @override
  State<_NavPillItem> createState() => _NavPillItemState();
}

class _NavPillItemState extends State<_NavPillItem>
    with SingleTickerProviderStateMixin {
  late final AnimationController _ctrl;
  late final Animation<double> _scaleAnim;

  @override
  void initState() {
    super.initState();
    _ctrl = AnimationController(
      vsync: this,
      duration: const Duration(milliseconds: 180),
      value: widget.selected ? 1.0 : 0.0,
    );
    _scaleAnim = CurvedAnimation(parent: _ctrl, curve: Curves.easeOutCubic);
  }

  @override
  void didUpdateWidget(_NavPillItem oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (widget.selected != oldWidget.selected) {
      if (widget.selected) {
        _ctrl.forward();
      } else {
        _ctrl.reverse();
      }
    }
  }

  @override
  void dispose() {
    _ctrl.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final primary = widget.role.primary;
    final label = widget.item.label ?? '';
    final activeIcon = widget.item.activeIcon ?? widget.item.icon; // ignore: dead_null_aware_expression

    return GestureDetector(
      onTap: widget.onTap,
      behavior: HitTestBehavior.opaque,
      child: AnimatedBuilder(
        animation: _scaleAnim,
        builder: (context, child) {
          final t = _scaleAnim.value;
          return Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              AnimatedContainer(
                duration: const Duration(milliseconds: 200),
                curve: Curves.easeOutCubic,
                padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 7),
                decoration: BoxDecoration(
                  gradient: widget.selected
                      ? LinearGradient(
                          colors: [
                            primary.withValues(alpha: 0.15),
                            primary.withValues(alpha: 0.08),
                          ],
                          begin: Alignment.topLeft,
                          end: Alignment.bottomRight,
                        )
                      : null,
                  borderRadius: BorderRadius.circular(14),
                  border: widget.selected
                      ? Border.all(
                          color: primary.withValues(alpha: 0.2),
                          width: 0.5,
                        )
                      : null,
                ),
                child: Transform.scale(
                  scale: 1.0 + (t * 0.08),
                  child: widget.selected
                      ? IconTheme(
                          data: IconThemeData(color: primary, size: 24),
                          child: activeIcon,
                        )
                      : IconTheme(
                          data: IconThemeData(
                            color: AppDesignSystem.gray400,
                            size: 23,
                          ),
                          child: widget.item.icon,
                        ),
                ),
              ),
              const SizedBox(height: 3),
              AnimatedDefaultTextStyle(
                duration: const Duration(milliseconds: 200),
                style: AppDesignSystem.font.copyWith(
                  fontSize: 10.5,
                  fontWeight:
                      widget.selected ? FontWeight.w800 : FontWeight.w500,
                  color: widget.selected ? primary : AppDesignSystem.gray400,
                  height: 1.2,
                ),
                child: Text(label, maxLines: 1, overflow: TextOverflow.ellipsis),
              ),
            ],
          );
        },
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
