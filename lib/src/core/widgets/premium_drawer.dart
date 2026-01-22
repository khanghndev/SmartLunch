import 'package:flutter/material.dart';

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
      width: MediaQuery.of(context).size.width * 0.85,
      child: Container(
        decoration: BoxDecoration(
          gradient: LinearGradient(
            begin: Alignment.topLeft,
            end: Alignment.bottomRight,
            colors: [
              Colors.white,
              Colors.grey.shade50,
            ],
          ),
        ),
        child: SafeArea(
          child: Column(
            children: [
              // Header với gradient
              Container(
                padding: const EdgeInsets.all(24),
                decoration: BoxDecoration(
                  gradient: LinearGradient(
                    begin: Alignment.topLeft,
                    end: Alignment.bottomRight,
                    colors: gradient,
                  ),
                  boxShadow: [
                    BoxShadow(
                      color: accentColor.withOpacity(0.3),
                      blurRadius: 20,
                      offset: const Offset(0, 10),
                    ),
                  ],
                ),
                child: Column(
                  children: [
                    Row(
                      children: [
                        Container(
                          padding: const EdgeInsets.all(12),
                          decoration: BoxDecoration(
                            color: Colors.white.withOpacity(0.25),
                            borderRadius: BorderRadius.circular(18),
                            border: Border.all(
                              color: Colors.white.withOpacity(0.3),
                              width: 1.5,
                            ),
                          ),
                          child: Image.asset(
                            'assets/images/linh_vat.png',
                            height: 48,
                            fit: BoxFit.contain,
                          ),
                        ),
                        const SizedBox(width: 16),
                        Expanded(
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text(
                                userName,
                                style: Theme.of(context)
                                    .textTheme
                                    .titleLarge
                                    ?.copyWith(
                                      color: Colors.white,
                                      fontWeight: FontWeight.w800,
                                      fontSize: 20,
                                    ),
                              ),
                              const SizedBox(height: 6),
                              Container(
                                padding: const EdgeInsets.symmetric(
                                  horizontal: 10,
                                  vertical: 4,
                                ),
                                decoration: BoxDecoration(
                                  color: Colors.white.withOpacity(0.25),
                                  borderRadius: BorderRadius.circular(12),
                                  border: Border.all(
                                    color: Colors.white.withOpacity(0.3),
                                    width: 1,
                                  ),
                                ),
                                child: Text(
                                  roleBadge,
                                  style: const TextStyle(
                                    color: Colors.white,
                                    fontWeight: FontWeight.w600,
                                    fontSize: 12,
                                  ),
                                ),
                              ),
                            ],
                          ),
                        ),
                      ],
                    ),
                  ],
                ),
              ),

              // Menu items
              Expanded(
                child: ListView(
                  padding: EdgeInsets.zero,
                  children: [
                    ...sections.map((section) => _DrawerSectionWidget(
                          section: section,
                          selectedIndex: selectedIndex,
                          onSelectTab: (index) => _selectTab(context, index),
                          onNavigate: (route) => _navigate(context, route),
                          accentColor: accentColor,
                        )),
                  ],
                ),
              ),

              // Logout button
              Container(
                margin: const EdgeInsets.all(16),
                decoration: BoxDecoration(
                  borderRadius: BorderRadius.circular(16),
                  border: Border.all(
                    color: Colors.red.withOpacity(0.3),
                    width: 1.5,
                  ),
                ),
                child: ListTile(
                  leading: Container(
                    padding: const EdgeInsets.all(8),
                    decoration: BoxDecoration(
                      color: Colors.red.withOpacity(0.1),
                      borderRadius: BorderRadius.circular(10),
                    ),
                    child: const Icon(
                      Icons.logout_rounded,
                      color: Colors.red,
                      size: 20,
                    ),
                  ),
                  title: const Text(
                    'Đăng xuất',
                    style: TextStyle(
                      fontWeight: FontWeight.w700,
                      color: Colors.red,
                    ),
                  ),
                  onTap: () => _signOut(context),
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(16),
                  ),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class DrawerSection {
  final String? title;
  final List<DrawerItem> items;

  const DrawerSection({
    this.title,
    required this.items,
  });
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
        if (section.title != null)
          Padding(
            padding: const EdgeInsets.fromLTRB(20, 20, 20, 12),
            child: Text(
              section.title?.toUpperCase() ?? '',
              style: Theme.of(context).textTheme.labelSmall?.copyWith(
                    color: Colors.grey.shade600,
                    fontWeight: FontWeight.w700,
                    letterSpacing: 1.2,
                    fontSize: 11,
                  ),
            ),
          ),
        ...section.items.map((item) => _DrawerItemWidget(
              item: item,
              isSelected:
                  item.tabIndex != null && selectedIndex == item.tabIndex,
              accentColor: accentColor,
              onSelectTab: onSelectTab,
              onNavigate: onNavigate,
            )),
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
      margin: const EdgeInsets.symmetric(horizontal: 12, vertical: 4),
      decoration: BoxDecoration(
        gradient: isSelected
            ? LinearGradient(
                colors: [
                  accentColor.withOpacity(0.15),
                  accentColor.withOpacity(0.08),
                ],
                begin: Alignment.centerLeft,
                end: Alignment.centerRight,
              )
            : null,
        borderRadius: BorderRadius.circular(14),
        border: isSelected
            ? Border.all(
                color: accentColor.withOpacity(0.3),
                width: 1.5,
              )
            : null,
      ),
      child: ListTile(
        leading: Container(
          padding: const EdgeInsets.all(10),
          decoration: BoxDecoration(
            color: isSelected
                ? accentColor.withOpacity(0.2)
                : iconColor.withOpacity(0.1),
            borderRadius: BorderRadius.circular(12),
          ),
          child: Icon(
            item.icon,
            color: isSelected ? accentColor : iconColor,
            size: 22,
          ),
        ),
        title: Text(
          item.labelVi,
          style: TextStyle(
            fontWeight: isSelected ? FontWeight.w700 : FontWeight.w600,
            color: isSelected ? accentColor : Colors.grey.shade800,
            fontSize: 15,
          ),
        ),
        trailing: isSelected
            ? Container(
                padding: const EdgeInsets.all(4),
                decoration: BoxDecoration(
                  color: accentColor,
                  shape: BoxShape.circle,
                ),
                child: const Icon(
                  Icons.check,
                  color: Colors.white,
                  size: 12,
                ),
              )
            : Icon(
                Icons.chevron_right_rounded,
                color: Colors.grey.shade400,
                size: 20,
              ),
        onTap: () {
          final tabIndex = item.tabIndex;
          final route = item.route;
          if (tabIndex != null) {
            onSelectTab(tabIndex);
          } else if (route != null) {
            onNavigate(route);
          }
        },
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(14),
        ),
      ),
    );
  }
}
