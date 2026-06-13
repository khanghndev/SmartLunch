import 'dart:ui';

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

  static double heroBodyHeight(BuildContext context) {
    final width = MediaQuery.sizeOf(context).width;
    if (width < 360) return 112;
    if (width < 400) return 120;
    return _heroBodyHeight;
  }

  static const _heroBodyHeight = 128.0;
  static const _panelOverlap = 14.0;
  /// Chiều cao panel quick actions (đo theo layout thực tế).
  static const _quickPanelHeight = 76.0;

  @override
  Widget build(BuildContext context) {
    final openDrawer = RoleTabScope.of(context).openDrawer;
    final top = MediaQuery.paddingOf(context).top;
    final dpr = MediaQuery.devicePixelRatioOf(context);
    final screenWidth = MediaQuery.sizeOf(context).width;
    final compact = screenWidth < 380;
    final heroHeight = heroBodyHeight(context);
    final cacheWidth = (screenWidth * dpr).round();
    final hasPanel = bottomPanel != null;
    final overlay = _gradientOverlay(role);
    final heroTotal = heroHeight + top;
    final panelCard = hasPanel ? _buildPanelCard(bottomPanel!, role) : null;

    return DecoratedBox(
      decoration: const BoxDecoration(color: AppDesignSystem.gray50),
      child: hasPanel
          ? SizedBox(
              height: heroTotal + _quickPanelHeight - _panelOverlap,
              width: double.infinity,
              child: Stack(
                clipBehavior: Clip.none,
                children: [
                  Positioned(
                    top: 0,
                    left: 0,
                    right: 0,
                    height: heroTotal,
                    child: _buildHeroClip(
                      role: role,
                      overlay: overlay,
                      cacheWidth: cacheWidth,
                      top: top,
                      compact: compact,
                      openDrawer: openDrawer,
                      title: title,
                      subtitle: subtitle,
                      trailing: trailing,
                      trustPill: trustPill,
                    ),
                  ),
                  Positioned(
                    top: heroTotal - _panelOverlap,
                    left: 16,
                    right: 16,
                    child: panelCard!,
                  ),
                ],
              ),
            )
          : Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                SizedBox(
                  height: heroTotal,
                  width: double.infinity,
                  child: _buildHeroClip(
                    role: role,
                    overlay: overlay,
                    cacheWidth: cacheWidth,
                    top: top,
                    compact: compact,
                    openDrawer: openDrawer,
                    title: title,
                    subtitle: subtitle,
                    trailing: trailing,
                    trustPill: trustPill,
                  ),
                ),
              ],
            ),
    );
  }

  Widget _buildPanelCard(Widget bottomPanel, RolePalette role) {
    return DecoratedBox(
      decoration: BoxDecoration(
        borderRadius: BorderRadius.circular(20),
        boxShadow: [
          BoxShadow(
            color: role.primary.withValues(alpha: 0.16),
            blurRadius: 32,
            offset: const Offset(0, 12),
          ),
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.08),
            blurRadius: 16,
            offset: const Offset(0, 4),
          ),
        ],
      ),
      child: ClipRRect(
        borderRadius: BorderRadius.circular(20),
        child: Material(color: Colors.white, child: bottomPanel),
      ),
    );
  }

  Widget _buildHeroClip({
    required RolePalette role,
    required List<Color> overlay,
    required int cacheWidth,
    required double top,
    required bool compact,
    required VoidCallback openDrawer,
    required String title,
    required String subtitle,
    required Widget? trailing,
    required String trustPill,
  }) {
    return ClipRRect(
      borderRadius: const BorderRadius.only(
        bottomLeft: Radius.circular(28),
        bottomRight: Radius.circular(28),
      ),
      child: Stack(
        fit: StackFit.expand,
        children: [
          IgnorePointer(
            child: Image.network(
              kModuleHeroImageUrl,
              fit: BoxFit.cover,
              alignment: const Alignment(0, -0.2),
              cacheWidth: cacheWidth,
              errorBuilder: (_, __, ___) => ColoredBox(color: role.primary),
            ),
          ),
          IgnorePointer(
            child: DecoratedBox(
              decoration: BoxDecoration(
                gradient: LinearGradient(
                  begin: Alignment.topCenter,
                  end: Alignment.bottomCenter,
                  colors: overlay,
                  stops: const [0.0, 0.45, 1.0],
                ),
              ),
            ),
          ),
          IgnorePointer(
            child: Stack(
              children: [
                Positioned(
                  top: -40,
                  right: -30,
                  child: _HeroGlow(
                    color: role.primaryAlt.withValues(alpha: 0.35),
                    size: 140,
                  ),
                ),
                Positioned(
                  bottom: 20,
                  left: -50,
                  child: _HeroGlow(
                    color: role.primary.withValues(alpha: 0.25),
                    size: 100,
                  ),
                ),
              ],
            ),
          ),
          Padding(
            padding: EdgeInsets.fromLTRB(16, top + 6, 16, 12),
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
                    RoleModuleBrandMark(accent: role.primary, compact: compact),
                    const Spacer(),
                    if (trailing != null) trailing,
                    if (trailing == null) RoleModuleTrustPill(label: trustPill),
                  ],
                ),
                const Spacer(),
                Text(
                  title,
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                  style: AppDesignSystem.title(
                    size: compact ? 22 : 24,
                    color: Colors.white,
                  ).copyWith(
                    height: 1.1,
                    shadows: [
                      Shadow(
                        color: Colors.black.withValues(alpha: 0.25),
                        blurRadius: 8,
                        offset: const Offset(0, 2),
                      ),
                    ],
                  ),
                ),
                const SizedBox(height: 3),
                Text(
                  subtitle,
                  maxLines: 2,
                  overflow: TextOverflow.ellipsis,
                  style: AppDesignSystem.body(
                    size: compact ? 12 : 13,
                    color: Colors.white.withValues(alpha: 0.9),
                  ).copyWith(height: 1.3),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  static List<Color> _gradientOverlay(RolePalette role) {
    final p = role.primary;
    return [
      const Color(0xCC0A0A0A),
      p.withValues(alpha: 0.72),
      p.withValues(alpha: 0.88),
    ];
  }
}

class _HeroGlow extends StatelessWidget {
  final Color color;
  final double size;

  const _HeroGlow({required this.color, required this.size});

  @override
  Widget build(BuildContext context) {
    return Container(
      width: size,
      height: size,
      decoration: BoxDecoration(
        shape: BoxShape.circle,
        gradient: RadialGradient(
          colors: [color, color.withValues(alpha: 0)],
        ),
      ),
    );
  }
}

class RoleModuleBrandMark extends StatelessWidget {
  final Color accent;
  final bool compact;

  const RoleModuleBrandMark({super.key, required this.accent, this.compact = false});

  @override
  Widget build(BuildContext context) {
    final box = compact ? 32.0 : 34.0;
    return Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        Container(
          width: box,
          height: box,
          decoration: BoxDecoration(
            color: Colors.white,
            borderRadius: BorderRadius.circular(10),
            boxShadow: [
              BoxShadow(
                color: Colors.black.withValues(alpha: 0.12),
                blurRadius: 8,
                offset: const Offset(0, 2),
              ),
            ],
          ),
          child: Icon(Icons.restaurant_menu_rounded, color: accent, size: compact ? 17 : 18),
        ),
        const SizedBox(width: 8),
        Text(
          'HUITMeal',
          style: AppDesignSystem.font.copyWith(
            color: Colors.white,
            fontWeight: FontWeight.w800,
            fontSize: compact ? 16 : 17,
            letterSpacing: -0.3,
            height: 1.2,
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
      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
      decoration: BoxDecoration(
        color: Colors.white.withValues(alpha: 0.18),
        borderRadius: BorderRadius.circular(999),
        border: Border.all(color: Colors.white.withValues(alpha: 0.28)),
      ),
      child: Text(
        label,
        style: AppDesignSystem.body(size: 11, color: Colors.white).copyWith(
          fontWeight: FontWeight.w700,
          letterSpacing: 0.5,
        ),
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
    return ClipRRect(
      borderRadius: BorderRadius.circular(12),
      child: BackdropFilter(
        filter: ImageFilter.blur(sigmaX: 8, sigmaY: 8),
        child: Material(
          color: Colors.white.withValues(alpha: 0.16),
          borderRadius: BorderRadius.circular(12),
          child: InkWell(
            onTap: onTap,
            borderRadius: BorderRadius.circular(12),
            child: Container(
              padding: const EdgeInsets.all(9),
              decoration: BoxDecoration(
                borderRadius: BorderRadius.circular(12),
                border: Border.all(color: Colors.white.withValues(alpha: 0.22)),
              ),
              child: Icon(icon, color: Colors.white, size: 22),
            ),
          ),
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

/// Shortcut trong panel header (dashboard): icon + nhãn ngang, gọn chiều cao.
class RoleHeaderQuickActionsPanel extends StatelessWidget {
  final List<RoleHeaderQuickAction> actions;

  const RoleHeaderQuickActionsPanel({super.key, required this.actions});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 2, vertical: 2),
      child: Row(
        children: [
          for (var i = 0; i < actions.length; i++) ...[
            if (i > 0)
              Container(
                width: 1,
                height: 36,
                color: AppDesignSystem.gray100,
              ),
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

class _QuickTile extends StatefulWidget {
  final RoleHeaderQuickAction action;

  const _QuickTile({required this.action});

  @override
  State<_QuickTile> createState() => _QuickTileState();
}

class _QuickTileState extends State<_QuickTile> {
  bool _pressed = false;

  @override
  Widget build(BuildContext context) {
    final compact = MediaQuery.sizeOf(context).width < 380;
    final iconBox = compact ? 34.0 : 38.0;
    final color = widget.action.color;

    return GestureDetector(
      onTapDown: (_) => setState(() => _pressed = true),
      onTapUp: (_) {
        setState(() => _pressed = false);
        widget.action.onTap();
      },
      onTapCancel: () => setState(() => _pressed = false),
      child: AnimatedScale(
        scale: _pressed ? 0.93 : 1.0,
        duration: const Duration(milliseconds: 100),
        curve: Curves.easeOut,
        child: Padding(
          padding: EdgeInsets.symmetric(
            horizontal: compact ? 4 : 6,
            vertical: 8,
          ),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              Container(
                width: iconBox,
                height: iconBox,
                decoration: BoxDecoration(
                  gradient: LinearGradient(
                    begin: Alignment.topLeft,
                    end: Alignment.bottomRight,
                    colors: [
                      color.withValues(alpha: 0.22),
                      color.withValues(alpha: 0.09),
                    ],
                  ),
                  borderRadius: BorderRadius.circular(11),
                  border: Border.all(
                    color: color.withValues(alpha: 0.18),
                    width: 0.5,
                  ),
                  boxShadow: [
                    BoxShadow(
                      color: color.withValues(alpha: 0.18),
                      blurRadius: 8,
                      offset: const Offset(0, 3),
                    ),
                  ],
                ),
                child: Icon(widget.action.icon, color: color, size: compact ? 18 : 20),
              ),
              const SizedBox(height: 6),
              Text(
                widget.action.label,
                textAlign: TextAlign.center,
                maxLines: 2,
                overflow: TextOverflow.ellipsis,
                style: AppDesignSystem.font.copyWith(
                  fontSize: compact ? 10.5 : 11,
                  height: 1.15,
                  fontWeight: FontWeight.w700,
                  color: AppDesignSystem.gray700,
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
