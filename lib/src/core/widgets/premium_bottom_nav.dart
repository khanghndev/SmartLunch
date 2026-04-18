import 'package:flutter/material.dart';

import '../constants/app_colors.dart';

class PremiumBottomNav extends StatelessWidget {
  final int selectedIndex;
  final ValueChanged<int> onTap;
  final Color accentColor;
  final List<NavItem> items;

  const PremiumBottomNav({
    super.key,
    required this.selectedIndex,
    required this.onTap,
    required this.accentColor,
    required this.items,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      decoration: BoxDecoration(
        color: Colors.white,
        border: Border(
          top: BorderSide(color: AppColors.border.withOpacity(0.8)),
        ),
        boxShadow: [
          BoxShadow(
            color: AppColors.shadow.withOpacity(0.45),
            blurRadius: 20,
            offset: const Offset(0, -2),
          ),
        ],
      ),
      child: SafeArea(
        top: false,
        child: Padding(
          padding: const EdgeInsets.fromLTRB(10, 8, 10, 10),
          child: Row(
            children: List.generate(
              items.length,
              (index) => Expanded(
                child: _NavItemWidget(
                  item: items[index],
                  isSelected: selectedIndex == index,
                  accentColor: accentColor,
                  onTap: () => onTap(index),
                ),
              ),
            ),
          ),
        ),
      ),
    );
  }
}

class NavItem {
  final IconData icon;
  final IconData selectedIcon;
  final String label;
  final String labelVi;

  const NavItem({
    required this.icon,
    required this.selectedIcon,
    required this.label,
    required this.labelVi,
  });
}

class _NavItemWidget extends StatelessWidget {
  final NavItem item;
  final bool isSelected;
  final Color accentColor;
  final VoidCallback onTap;

  const _NavItemWidget({
    required this.item,
    required this.isSelected,
    required this.accentColor,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    final inactiveColor = AppColors.inkSoft;
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(14),
      child: AnimatedContainer(
        duration: const Duration(milliseconds: 220),
        curve: Curves.easeOutCubic,
        margin: const EdgeInsets.symmetric(horizontal: 4),
        padding: const EdgeInsets.symmetric(vertical: 8, horizontal: 6),
        decoration: BoxDecoration(
          color:
              isSelected ? accentColor.withOpacity(0.12) : Colors.transparent,
          borderRadius: BorderRadius.circular(14),
          border:
              isSelected
                  ? Border.all(color: accentColor.withOpacity(0.28))
                  : null,
        ),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Icon(
              isSelected ? item.selectedIcon : item.icon,
              color: isSelected ? accentColor : inactiveColor,
              size: 22,
            ),
            const SizedBox(height: 4),
            Text(
              item.labelVi,
              maxLines: 1,
              overflow: TextOverflow.ellipsis,
              style: TextStyle(
                fontSize: 11.5,
                fontWeight: isSelected ? FontWeight.w700 : FontWeight.w600,
                color: isSelected ? accentColor : inactiveColor,
                letterSpacing: 0.1,
              ),
            ),
          ],
        ),
      ),
    );
  }
}
