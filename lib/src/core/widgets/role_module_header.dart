import 'package:flutter/material.dart';

import '../theme/app_design_system.dart';
import 'role_tab_shell.dart';

/// Ảnh hero mặc định (đồng bộ Customer / Auth).
const String kModuleHeroImageUrl =
    'https://images.unsplash.com/photo-1555939594-58d7cb561ad1?q=80&w=1200';

/// Header chuẩn mọi tab module — cùng layout; chỉ đổi [title] / [subtitle] / [bottomPanel].
class RoleModuleHeader extends StatelessWidget {
  final RolePalette role;
  final String title;
  final String subtitle;
  final Widget? bottomPanel;
  final Widget? trailing;
  final String trustPill;

  const RoleModuleHeader({
    super.key,
    required this.role,
    required this.title,
    required this.subtitle,
    this.bottomPanel,
    this.trailing,
    this.trustPill = 'HUITMeal',
  });

  static const _heroBodyHeight = 148.0;
  static const _panelOverlap = 14.0;

  @override
  Widget build(BuildContext context) {
    final openDrawer = RoleTabScope.of(context).openDrawer;
    final top = MediaQuery.paddingOf(context).top;
    final dpr = MediaQuery.devicePixelRatioOf(context);
    final cacheWidth = (MediaQuery.sizeOf(context).width * dpr).round();
    final hasPanel = bottomPanel != null;
    final overlay = _gradientOverlay(role);

    return DecoratedBox(
      decoration: BoxDecoration(
        color: AppDesignSystem.gray50,
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.06),
            blurRadius: 12,
            offset: const Offset(0, 4),
          ),
        ],
      ),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          ClipRRect(
            borderRadius: const BorderRadius.only(
              bottomLeft: Radius.circular(24),
              bottomRight: Radius.circular(24),
            ),
            child: SizedBox(
              height: _heroBodyHeight + top,
              width: double.infinity,
              child: Stack(
                fit: StackFit.expand,
                children: [
                  IgnorePointer(
                    child: Image.network(
                      kModuleHeroImageUrl,
                      fit: BoxFit.cover,
                      alignment: Alignment.center,
                      cacheWidth: cacheWidth,
                      errorBuilder: (_, __, ___) => ColoredBox(color: role.primary),
                    ),
                  ),
                  IgnorePointer(
                    child: DecoratedBox(
                      decoration: BoxDecoration(
                        gradient: LinearGradient(
                          begin: Alignment.topLeft,
                          end: Alignment.bottomRight,
                          colors: overlay,
                        ),
                      ),
                    ),
                  ),
                  Padding(
                    padding: EdgeInsets.fromLTRB(12, top + 6, 12, 16),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Row(
                          children: [
                            RoleModuleIconButton(
                              icon: Icons.menu_rounded,
                              onTap: openDrawer,
                            ),
                            const SizedBox(width: 8),
                            RoleModuleBrandMark(accent: role.primary),
                            const Spacer(),
                            if (trailing != null) trailing!,
                            if (trailing == null) RoleModuleTrustPill(label: trustPill),
                          ],
                        ),
                        const Spacer(),
                        Text(
                          title,
                          maxLines: 2,
                          overflow: TextOverflow.ellipsis,
                          style: AppDesignSystem.title(size: 26, color: Colors.white),
                        ),
                        const SizedBox(height: 4),
                        Text(
                          subtitle,
                          maxLines: 2,
                          overflow: TextOverflow.ellipsis,
                          style: AppDesignSystem.body(
                            size: 13,
                            color: Colors.white.withValues(alpha: 0.88),
                          ),
                        ),
                      ],
                    ),
                  ),
                ],
              ),
            ),
          ),
          if (hasPanel)
            Transform.translate(
              offset: const Offset(0, -_panelOverlap),
              child: Padding(
                padding: const EdgeInsets.fromLTRB(16, 0, 16, 12),
                child: Material(
                  color: Colors.white,
                  elevation: 6,
                  shadowColor: Colors.black.withValues(alpha: 0.08),
                  borderRadius: BorderRadius.circular(16),
                  child: Container(
                    width: double.infinity,
                    decoration: AppDesignSystem.card(radius: 16),
                    child: bottomPanel!,
                  ),
                ),
              ),
            )
          else
            const SizedBox(height: 4),
        ],
      ),
    );
  }

  static List<Color> _gradientOverlay(RolePalette role) {
    final p = role.primary;
    final a = role.primaryAlt;
    return [
      const Color(0x99000000),
      p.withValues(alpha: 0.45),
      Color.lerp(a, p, 0.65) ?? p,
    ];
  }
}

class RoleModuleBrandMark extends StatelessWidget {
  final Color accent;

  const RoleModuleBrandMark({super.key, required this.accent});

  @override
  Widget build(BuildContext context) {
    return Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        Container(
          width: 36,
          height: 36,
          decoration: BoxDecoration(
            color: Colors.white,
            borderRadius: BorderRadius.circular(10),
          ),
          child: Icon(Icons.restaurant_menu_rounded, color: accent, size: 20),
        ),
        const SizedBox(width: 8),
        Text(
          'HUITMeal',
          style: AppDesignSystem.font.copyWith(
            color: Colors.white,
            fontWeight: FontWeight.w900,
            fontSize: 18,
            letterSpacing: -0.3,
          ),
        ),
      ],
    );
  }
}

class RoleModuleTrustPill extends StatelessWidget {
  final String label;

  const RoleModuleTrustPill({super.key, required this.label});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 5),
      decoration: BoxDecoration(
        color: Colors.white.withValues(alpha: 0.15),
        borderRadius: BorderRadius.circular(999),
        border: Border.all(color: Colors.white24),
      ),
      child: Text(
        label,
        style: AppDesignSystem.body(size: 11, color: Colors.white),
      ),
    );
  }
}

class RoleModuleIconButton extends StatelessWidget {
  final IconData icon;
  final VoidCallback onTap;

  const RoleModuleIconButton({super.key, required this.icon, required this.onTap});

  @override
  Widget build(BuildContext context) {
    return Material(
      color: Colors.white.withValues(alpha: 0.14),
      borderRadius: BorderRadius.circular(10),
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(10),
        child: Padding(
          padding: const EdgeInsets.all(8),
          child: Icon(icon, color: Colors.white, size: 22),
        ),
      ),
    );
  }
}

/// Chip chọn (danh mục, bộ lọc…) trong panel header.
class RoleHeaderChip extends StatelessWidget {
  final String label;
  final bool selected;
  final RolePalette role;
  final VoidCallback onTap;

  const RoleHeaderChip({
    super.key,
    required this.label,
    required this.selected,
    required this.role,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return Material(
      color: Colors.transparent,
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(999),
        child: AnimatedContainer(
          duration: const Duration(milliseconds: 200),
          padding: const EdgeInsets.symmetric(horizontal: 18, vertical: 10),
          decoration: BoxDecoration(
            gradient: selected
                ? LinearGradient(colors: [role.primary, role.primaryAlt])
                : null,
            color: selected ? null : Colors.white,
            borderRadius: BorderRadius.circular(999),
            border: Border.all(
              color: selected ? Colors.transparent : AppDesignSystem.gray200,
            ),
            boxShadow: selected
                ? [
                    BoxShadow(
                      color: role.primary.withValues(alpha: 0.35),
                      blurRadius: 10,
                      offset: const Offset(0, 4),
                    ),
                  ]
                : null,
          ),
          child: Text(
            label,
            style: AppDesignSystem.body(
              color: selected ? Colors.white : AppDesignSystem.gray700,
            ).copyWith(fontWeight: selected ? FontWeight.w700 : FontWeight.w500),
          ),
        ),
      ),
    );
  }
}

/// Hai nút shortcut trong panel header (dashboard).
class RoleHeaderQuickActionsPanel extends StatelessWidget {
  final List<RoleHeaderQuickAction> actions;

  const RoleHeaderQuickActionsPanel({super.key, required this.actions});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.all(12),
      child: Row(
        children: [
          for (var i = 0; i < actions.length; i++) ...[
            if (i > 0) const SizedBox(width: 10),
            Expanded(child: _QuickTile(action: actions[i])),
          ],
        ],
      ),
    );
  }
}

class RoleHeaderQuickAction {
  final String label;
  final IconData icon;
  final Color color;
  final VoidCallback onTap;

  const RoleHeaderQuickAction({
    required this.label,
    required this.icon,
    required this.color,
    required this.onTap,
  });
}

class _QuickTile extends StatelessWidget {
  final RoleHeaderQuickAction action;

  const _QuickTile({required this.action});

  @override
  Widget build(BuildContext context) {
    return Material(
      color: Colors.transparent,
      child: InkWell(
        onTap: action.onTap,
        borderRadius: BorderRadius.circular(12),
        child: Ink(
          padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 12),
          decoration: AppDesignSystem.card(radius: 12),
          child: Row(
            children: [
              Container(
                padding: const EdgeInsets.all(8),
                decoration: BoxDecoration(
                  color: action.color.withValues(alpha: 0.12),
                  borderRadius: BorderRadius.circular(10),
                ),
                child: Icon(action.icon, color: action.color, size: 20),
              ),
              const SizedBox(width: 10),
              Expanded(
                child: Text(
                  action.label,
                  style: AppDesignSystem.label().copyWith(fontSize: 13),
                  maxLines: 2,
                  overflow: TextOverflow.ellipsis,
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

/// Tab trong shell: header cố định + nội dung scroll.
class RoleModuleTabPage extends StatelessWidget {
  final RolePalette role;
  final String headerTitle;
  final String headerSubtitle;
  final Widget? headerBottomPanel;
  final Widget? headerTrailing;
  final String trustPill;
  final Widget body;

  const RoleModuleTabPage({
    super.key,
    required this.role,
    required this.headerTitle,
    required this.headerSubtitle,
    required this.body,
    this.headerBottomPanel,
    this.headerTrailing,
    this.trustPill = 'HUITMeal',
  });

  @override
  Widget build(BuildContext context) {
    return ColoredBox(
      color: AppDesignSystem.gray50,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          RoleModuleHeader(
            role: role,
            title: headerTitle,
            subtitle: headerSubtitle,
            bottomPanel: headerBottomPanel,
            trailing: headerTrailing,
            trustPill: trustPill,
          ),
          Expanded(child: body),
        ],
      ),
    );
  }
}
