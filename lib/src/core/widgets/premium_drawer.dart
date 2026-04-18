import 'package:flutter/material.dart';

import '../constants/app_colors.dart';

class PremiumDrawer extends StatelessWidget {
  final String userName;
  final String userRole;
  final String roleBadge;
  final List<Color> gradient;
  final Color accentColor;
  final int selectedIndex;
  final ValueChanged<int> onSelectTab;
  final ValueChanged<String> onNavigate;
  final VoidCallback onLogout;
  final List<DrawerSection> sections;

  const PremiumDrawer({
    super.key,
    required this.userName,
    required this.userRole,
    required this.roleBadge,
    required this.gradient,
    required this.accentColor,
    required this.selectedIndex,
    required this.onSelectTab,
    required this.onNavigate,
    required this.onLogout,
    required this.sections,
  });

  void _selectTab(BuildContext context, int index) {
    Navigator.of(context).pop();
    onSelectTab(index);
  }

  void _navigate(BuildContext context, String route) {
    Navigator.of(context).pop();
    onNavigate(route);
  }

  void _signOut(BuildContext context) {
    Navigator.of(context).pop();
    onLogout();
  }

  @override
  Widget build(BuildContext context) {
    return Drawer(
      width: MediaQuery.of(context).size.width * 0.87,
      child: Container(
        color: AppColors.surface,
        child: SafeArea(
          child: Column(
            children: [
              _DrawerHeader(
                userName: userName,
                userRole: userRole,
                roleBadge: roleBadge,
                gradient: gradient,
              ),
              Expanded(
                child: ListView(
                  padding: const EdgeInsets.fromLTRB(10, 8, 10, 12),
                  children: [
                    ...sections.map(
                      (section) => _DrawerSectionWidget(
                        section: section,
                        selectedIndex: selectedIndex,
                        accentColor: accentColor,
                        onSelectTab: (index) => _selectTab(context, index),
                        onNavigate: (route) => _navigate(context, route),
                      ),
                    ),
                  ],
                ),
              ),
              _LogoutTile(onTap: () => _signOut(context)),
            ],
          ),
        ),
      ),
    );
  }
}

class _DrawerHeader extends StatelessWidget {
  final String userName;
  final String userRole;
  final String roleBadge;
  final List<Color> gradient;

  const _DrawerHeader({
    required this.userName,
    required this.userRole,
    required this.roleBadge,
    required this.gradient,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: const EdgeInsets.fromLTRB(10, 10, 10, 8),
      padding: const EdgeInsets.all(18),
      decoration: BoxDecoration(
        gradient: LinearGradient(
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
          colors: gradient,
        ),
        borderRadius: BorderRadius.circular(22),
        boxShadow: [
          BoxShadow(
            color: gradient.first.withOpacity(0.32),
            blurRadius: 22,
            offset: const Offset(0, 10),
            spreadRadius: -6,
          ),
        ],
      ),
      child: Row(
        children: [
          Container(
            width: 56,
            height: 56,
            decoration: BoxDecoration(
              color: Colors.white.withOpacity(0.2),
              borderRadius: BorderRadius.circular(16),
              border: Border.all(color: Colors.white.withOpacity(0.38)),
            ),
            child: ClipRRect(
              borderRadius: BorderRadius.circular(16),
              child: Image.asset(
                'assets/images/linh_vat.png',
                fit: BoxFit.contain,
                errorBuilder: (_, __, ___) {
                  return const Icon(
                    Icons.restaurant_rounded,
                    color: Colors.white,
                  );
                },
              ),
            ),
          ),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  userName,
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                  style: Theme.of(context).textTheme.titleMedium?.copyWith(
                    color: Colors.white,
                    fontWeight: FontWeight.w800,
                  ),
                ),
                const SizedBox(height: 3),
                Text(
                  userRole,
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                  style: Theme.of(context).textTheme.bodySmall?.copyWith(
                    color: Colors.white.withOpacity(0.95),
                    fontWeight: FontWeight.w600,
                  ),
                ),
                const SizedBox(height: 8),
                Container(
                  padding: const EdgeInsets.symmetric(
                    horizontal: 10,
                    vertical: 4,
                  ),
                  decoration: BoxDecoration(
                    color: Colors.white.withOpacity(0.2),
                    borderRadius: BorderRadius.circular(999),
                    border: Border.all(color: Colors.white.withOpacity(0.4)),
                  ),
                  child: Text(
                    roleBadge,
                    style: Theme.of(context).textTheme.labelSmall?.copyWith(
                      color: Colors.white,
                      fontWeight: FontWeight.w700,
                      letterSpacing: 0.25,
                    ),
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class _LogoutTile extends StatelessWidget {
  final VoidCallback onTap;

  const _LogoutTile({required this.onTap});

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: const EdgeInsets.fromLTRB(14, 0, 14, 16),
      decoration: BoxDecoration(
        color: AppColors.danger.withOpacity(0.08),
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: AppColors.danger.withOpacity(0.2)),
      ),
      child: ListTile(
        onTap: onTap,
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(14)),
        leading: const Icon(Icons.logout_rounded, color: AppColors.danger),
        title: Text(
          'Đăng xuất',
          style: Theme.of(context).textTheme.bodyMedium?.copyWith(
            color: AppColors.danger,
            fontWeight: FontWeight.w700,
          ),
        ),
      ),
    );
  }
}

class DrawerSection {
  final String? title;
  final List<DrawerItem> items;

  const DrawerSection({this.title, required this.items});
}

class DrawerItem {
  final IconData icon;
  final String label;
  final String labelVi;
  final int? tabIndex;
  final String? route;
  final Color? iconColor;

  const DrawerItem({
    required this.icon,
    required this.label,
    required this.labelVi,
    this.tabIndex,
    this.route,
    this.iconColor,
  });
}

class _DrawerSectionWidget extends StatelessWidget {
  final DrawerSection section;
  final int selectedIndex;
  final ValueChanged<int> onSelectTab;
  final ValueChanged<String> onNavigate;
  final Color accentColor;

  const _DrawerSectionWidget({
    required this.section,
    required this.selectedIndex,
    required this.onSelectTab,
    required this.onNavigate,
    required this.accentColor,
  });

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        if (section.title != null) ...[
          Padding(
            padding: const EdgeInsets.fromLTRB(8, 14, 8, 8),
            child: Text(
              section.title!.toUpperCase(),
              style: Theme.of(context).textTheme.labelSmall?.copyWith(
                color: AppColors.inkSoft,
                fontWeight: FontWeight.w700,
                letterSpacing: 1,
              ),
            ),
          ),
        ],
        ...section.items.map(
          (item) => _DrawerItemWidget(
            item: item,
            isSelected: item.tabIndex != null && item.tabIndex == selectedIndex,
            accentColor: accentColor,
            onSelectTab: onSelectTab,
            onNavigate: onNavigate,
          ),
        ),
      ],
    );
  }
}

class _DrawerItemWidget extends StatelessWidget {
  final DrawerItem item;
  final bool isSelected;
  final Color accentColor;
  final ValueChanged<int> onSelectTab;
  final ValueChanged<String> onNavigate;

  const _DrawerItemWidget({
    required this.item,
    required this.isSelected,
    required this.accentColor,
    required this.onSelectTab,
    required this.onNavigate,
  });

  @override
  Widget build(BuildContext context) {
    final iconColor = item.iconColor ?? accentColor;

    return Container(
      margin: const EdgeInsets.symmetric(vertical: 4),
      decoration: BoxDecoration(
        color: isSelected ? AppColors.tint(accentColor, 0.12) : Colors.white,
        borderRadius: BorderRadius.circular(14),
        border: Border.all(
          color: isSelected ? accentColor.withOpacity(0.32) : AppColors.border,
        ),
      ),
      child: ListTile(
        onTap: () {
          final tabIndex = item.tabIndex;
          final route = item.route;
          if (tabIndex != null) {
            onSelectTab(tabIndex);
          } else if (route != null) {
            onNavigate(route);
          }
        },
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(14)),
        leading: Container(
          width: 34,
          height: 34,
          decoration: BoxDecoration(
            color:
                isSelected
                    ? accentColor.withOpacity(0.18)
                    : iconColor.withOpacity(0.12),
            borderRadius: BorderRadius.circular(10),
          ),
          child: Icon(
            item.icon,
            color: isSelected ? accentColor : iconColor,
            size: 20,
          ),
        ),
        title: Text(
          item.labelVi,
          style: Theme.of(context).textTheme.bodyMedium?.copyWith(
            color: isSelected ? accentColor : AppColors.ink,
            fontWeight: isSelected ? FontWeight.w700 : FontWeight.w600,
          ),
        ),
        trailing: Icon(
          isSelected ? Icons.check_rounded : Icons.chevron_right_rounded,
          color: isSelected ? accentColor : AppColors.inkSoft.withOpacity(0.8),
          size: isSelected ? 18 : 20,
        ),
      ),
    );
  }
}
