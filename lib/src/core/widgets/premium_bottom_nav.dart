import 'package:flutter/material.dart';

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
    final borderColor = Colors.grey.withOpacity(0.16);
    return Container(
      decoration: BoxDecoration(
        color: Colors.white,
        border: Border(top: BorderSide(color: borderColor, width: 1)),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withOpacity(0.04),
            blurRadius: 12,
            offset: const Offset(0, -2),
          ),
        ],
      ),
      child: SafeArea(
        top: false,
        child: Padding(
          padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 6),
          child: Row(
            mainAxisAlignment: MainAxisAlignment.spaceAround,
            children: List.generate(
              items.length,
              (index) => _NavItemWidget(
                item: items[index],
                isSelected: selectedIndex == index,
                accentColor: accentColor,
                onTap: () => onTap(index),
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
    final baseColor = Colors.grey.shade600;
    return Expanded(
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(12),
        child: Padding(
          padding: const EdgeInsets.symmetric(vertical: 6),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              AnimatedContainer(
                duration: const Duration(milliseconds: 200),
                curve: Curves.easeInOut,
                height: 3,
                width: isSelected ? 26 : 0,
                margin: const EdgeInsets.only(bottom: 6),
                decoration: BoxDecoration(
                  color: isSelected ? accentColor : Colors.transparent,
                  borderRadius: BorderRadius.circular(12),
                ),
              ),
              Icon(
                isSelected ? item.selectedIcon : item.icon,
                color: isSelected ? accentColor : baseColor,
                size: 22,
              ),
              const SizedBox(height: 4),
              Text(
                item.labelVi,
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
                style: TextStyle(
                  fontSize: 11,
                  fontWeight: isSelected ? FontWeight.w700 : FontWeight.w500,
                  color: isSelected ? accentColor : baseColor,
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
