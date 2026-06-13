import 'package:flutter/material.dart';
import 'package:smartlunch_mobile/src/app/app_routes.dart';

import '../theme/app_design_system.dart';
import '../../features/auth/data/auth_storage.dart';
import '../../features/profile/data/models/user_profile_model.dart';
import '../../features/profile/data/profile_repository.dart';

/// Về màn đăng nhập và xóa stack (dùng sau khi đóng drawer).
void navigateAppToLogin(BuildContext context) {
  Navigator.of(context).pushNamedAndRemoveUntil(
    AppRoutes.login,
    (route) => false,
  );
}

/// Drawer điều hướng chuẩn mọi module — gradient theo role, menu section, đăng xuất.
class PremiumDrawer extends StatefulWidget {
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

  @override
  State<PremiumDrawer> createState() => _PremiumDrawerState();
}

class _PremiumDrawerState extends State<PremiumDrawer> {
  UserProfileModel? _profile;
  bool _loadingProfile = true;
  bool _hasSession = false;

  @override
  void initState() {
    super.initState();
    _loadDrawerState();
  }

  Future<void> _loadDrawerState() async {
    final session = await const AuthStorage().readSession();
    UserProfileModel? profile;
    try {
      if (session != null) {
        profile = await ProfileRepository.instance.getProfile();
      }
    } catch (_) {}

    if (!mounted) return;
    setState(() {
      _hasSession = session != null;
      _profile = profile;
      _loadingProfile = false;
    });
  }

  void _handleAuthAction(BuildContext context) {
    final isLoggedIn = _hasSession;
    final navigator = Navigator.of(context);
    navigator.pop();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (isLoggedIn) {
        widget.onLogout();
      } else {
        navigator.pushNamedAndRemoveUntil(
          AppRoutes.login,
          (route) => false,
        );
      }
    });
  }

  void _selectTab(BuildContext context, int index) {
    Navigator.of(context).pop();
    widget.onSelectTab(index);
  }

  void _navigate(BuildContext context, String route) {
    Navigator.of(context).pop();
    widget.onNavigate(route);
  }

  @override
  Widget build(BuildContext context) {
    final profile = _profile;
    final displayName = profile?.displayName ?? widget.userName;
    final displaySubtitle = profile?.email.isNotEmpty == true
        ? profile!.email
        : widget.userRole;
    final badgeRaw = profile?.roles.isNotEmpty == true
        ? profile!.roles.first
        : widget.roleBadge;
    final displayBadge = formatDrawerRoleBadge(badgeRaw);
    final isLoggedIn = _hasSession;

    return Drawer(
      width: (MediaQuery.sizeOf(context).width * 0.86).clamp(280.0, 340.0),
      backgroundColor: Colors.white,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.only(
          topRight: Radius.circular(24),
          bottomRight: Radius.circular(24),
        ),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          _DrawerProfileHeader(
            userName: displayName,
            subtitle: displaySubtitle,
            roleBadge: displayBadge,
            avatarUrl: profile?.avatarUrl,
            gradient: widget.gradient,
            accentColor: widget.accentColor,
            isLoading: _loadingProfile,
            onClose: () => Navigator.of(context).pop(),
          ),
          Expanded(
            child: ListView(
              padding: const EdgeInsets.fromLTRB(16, 12, 16, 16),
              children: [
                for (var i = 0; i < widget.sections.length; i++) ...[
                  if (i > 0) const SizedBox(height: 12),
                  _DrawerSectionWidget(
                    section: widget.sections[i],
                    selectedIndex: widget.selectedIndex,
                    accentColor: widget.accentColor,
                    onSelectTab: (index) => _selectTab(context, index),
                    onNavigate: (route) => _navigate(context, route),
                  ),
                ],
              ],
            ),
          ),
          const _DrawerBrandFooter(),
          const SizedBox(height: 4),
          SafeArea(
            top: false,
            child: _DrawerAuthAction(
              label: isLoggedIn ? 'Đăng xuất' : 'Đăng nhập',
              icon: isLoggedIn ? Icons.logout_rounded : Icons.login_rounded,
              color: isLoggedIn ? AppDesignSystem.danger : widget.accentColor,
              onTap: () => _handleAuthAction(context),
            ),
          ),
          const SizedBox(height: 16),
        ],
      ),
    );
  }
}

/// Hiển thị role badge tiếng Việt khi có thể.
String formatDrawerRoleBadge(String raw) {
  final key = raw.trim().toLowerCase();
  const map = {
    'customer': 'Khách hàng',
    'manager': 'Quản lý',
    'admin': 'Quản trị',
    'shipper': 'Giao hàng',
    'delivery': 'Giao hàng',
    'organization': 'Tổ chức',
    'company': 'Doanh nghiệp',
    'org': 'Tổ chức',
  };
  if (map.containsKey(key)) return map[key]!;
  if (raw.isEmpty) return 'HUITMeal';
  return raw.length <= 3 ? raw.toUpperCase() : raw;
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

class _DrawerProfileHeader extends StatelessWidget {
  final String userName;
  final String subtitle;
  final String roleBadge;
  final String? avatarUrl;
  final List<Color> gradient;
  final Color accentColor;
  final bool isLoading;
  final VoidCallback onClose;

  const _DrawerProfileHeader({
    required this.userName,
    required this.subtitle,
    required this.roleBadge,
    required this.gradient,
    required this.accentColor,
    required this.isLoading,
    required this.onClose,
    this.avatarUrl,
  });

  @override
  Widget build(BuildContext context) {
    final topPadding = MediaQuery.paddingOf(context).top;
    return Container(
      decoration: BoxDecoration(
        gradient: LinearGradient(
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
          colors: gradient,
        ),
        borderRadius: const BorderRadius.vertical(bottom: Radius.circular(24)),
        boxShadow: [
          BoxShadow(
            color: gradient.first.withValues(alpha: 0.15),
            blurRadius: 16,
            offset: const Offset(0, 6),
          ),
        ],
      ),
      child: ClipRRect(
        borderRadius: const BorderRadius.vertical(bottom: Radius.circular(24)),
        child: Stack(
          children: [
            Positioned(
              right: -16,
              top: -16,
              child: Icon(
                Icons.restaurant_menu_rounded,
                size: 110,
                color: Colors.white.withValues(alpha: 0.06),
              ),
            ),
            Padding(
              padding: EdgeInsets.fromLTRB(20, topPadding + 16, 20, 20),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Row(
                    children: [
                      Text(
                        'HUITMeal',
                        style: AppDesignSystem.font.copyWith(
                          color: Colors.white.withValues(alpha: 0.9),
                          fontWeight: FontWeight.w800,
                          fontSize: 13,
                          letterSpacing: 0.4,
                        ),
                      ),
                      const Spacer(),
                      _DrawerIconButton(
                        icon: Icons.close_rounded,
                        onTap: onClose,
                      ),
                    ],
                  ),
                  const SizedBox(height: 18),
                  if (isLoading)
                    const _HeaderSkeleton()
                  else
                    Row(
                      crossAxisAlignment: CrossAxisAlignment.center,
                      children: [
                        _DrawerAvatar(url: avatarUrl, accent: accentColor),
                        const SizedBox(width: 14),
                        Expanded(
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            mainAxisSize: MainAxisSize.min,
                            children: [
                              Text(
                                userName,
                                maxLines: 1,
                                overflow: TextOverflow.ellipsis,
                                style: AppDesignSystem.font.copyWith(
                                  fontSize: 17,
                                  fontWeight: FontWeight.w800,
                                  color: Colors.white,
                                  letterSpacing: -0.2,
                                ),
                              ),
                              const SizedBox(height: 2),
                              Text(
                                subtitle,
                                maxLines: 1,
                                overflow: TextOverflow.ellipsis,
                                style: AppDesignSystem.font.copyWith(
                                  fontSize: 12,
                                  fontWeight: FontWeight.w500,
                                  color: Colors.white.withValues(alpha: 0.8),
                                ),
                              ),
                              const SizedBox(height: 6),
                              Container(
                                padding: const EdgeInsets.symmetric(
                                  horizontal: 8,
                                  vertical: 3,
                                ),
                                decoration: BoxDecoration(
                                  color: Colors.white.withValues(alpha: 0.16),
                                  borderRadius: BorderRadius.circular(999),
                                  border: Border.all(
                                    color: Colors.white.withValues(alpha: 0.2),
                                  ),
                                ),
                                child: Text(
                                  roleBadge,
                                  style: AppDesignSystem.font.copyWith(
                                    fontSize: 10,
                                    fontWeight: FontWeight.w700,
                                    color: Colors.white,
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
          ],
        ),
      ),
    );
  }
}

class _DrawerAvatar extends StatelessWidget {
  final String? url;
  final Color accent;

  const _DrawerAvatar({this.url, required this.accent});

  @override
  Widget build(BuildContext context) {
    final trimmed = url?.trim();
    return Container(
      width: 54,
      height: 54,
      decoration: BoxDecoration(
        color: Colors.white,
        shape: BoxShape.circle,
        border: Border.all(color: Colors.white, width: 2),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.1),
            blurRadius: 8,
            offset: const Offset(0, 3),
          ),
        ],
      ),
      child: ClipOval(
        child: trimmed != null && trimmed.isNotEmpty
            ? Image.network(
                trimmed,
                fit: BoxFit.cover,
                errorBuilder: (_, __, ___) => _fallback(accent),
              )
            : Image.asset(
                'assets/images/linh_vat.png',
                fit: BoxFit.contain,
                errorBuilder: (_, __, ___) => _fallback(accent),
              ),
      ),
    );
  }

  Widget _fallback(Color accent) {
    return ColoredBox(
      color: accent.withValues(alpha: 0.08),
      child: Icon(Icons.person_rounded, color: accent, size: 28),
    );
  }
}

class _HeaderSkeleton extends StatelessWidget {
  const _HeaderSkeleton();

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        Container(
          width: 52,
          height: 52,
          decoration: BoxDecoration(
            color: Colors.white.withValues(alpha: 0.2),
            shape: BoxShape.circle,
          ),
        ),
        const SizedBox(width: 12),
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Container(
                height: 14,
                width: 120,
                decoration: BoxDecoration(
                  color: Colors.white.withValues(alpha: 0.25),
                  borderRadius: BorderRadius.circular(6),
                ),
              ),
              const SizedBox(height: 8),
              Container(
                height: 10,
                width: 160,
                decoration: BoxDecoration(
                  color: Colors.white.withValues(alpha: 0.18),
                  borderRadius: BorderRadius.circular(6),
                ),
              ),
            ],
          ),
        ),
      ],
    );
  }
}

class _DrawerIconButton extends StatelessWidget {
  final IconData icon;
  final VoidCallback onTap;

  const _DrawerIconButton({required this.icon, required this.onTap});

  @override
  Widget build(BuildContext context) {
    return Material(
      color: Colors.white.withValues(alpha: 0.14),
      borderRadius: BorderRadius.circular(10),
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(10),
        child: Padding(
          padding: const EdgeInsets.all(6),
          child: Icon(icon, color: Colors.white, size: 20),
        ),
      ),
    );
  }
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
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        if (section.title != null) ...[
          Padding(
            padding: const EdgeInsets.fromLTRB(8, 14, 8, 8),
            child: Text(
              section.title!.toUpperCase(),
              style: AppDesignSystem.font.copyWith(
                fontSize: 10.5,
                fontWeight: FontWeight.w800,
                color: AppDesignSystem.gray400,
                letterSpacing: 0.8,
              ),
            ),
          ),
        ],
        ...section.items.map(
          (item) => _DrawerItemTile(
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

class _DrawerItemTile extends StatelessWidget {
  final DrawerItem item;
  final bool isSelected;
  final Color accentColor;
  final ValueChanged<int> onSelectTab;
  final ValueChanged<String> onNavigate;

  const _DrawerItemTile({
    required this.item,
    required this.isSelected,
    required this.accentColor,
    required this.onSelectTab,
    required this.onNavigate,
  });

  @override
  Widget build(BuildContext context) {
    final iconColor = item.iconColor ?? accentColor;

    return Padding(
      padding: const EdgeInsets.only(bottom: 4),
      child: Material(
        color: isSelected ? accentColor.withValues(alpha: 0.09) : Colors.transparent,
        borderRadius: BorderRadius.circular(12),
        child: InkWell(
          onTap: () {
            final tabIndex = item.tabIndex;
            final route = item.route;
            if (tabIndex != null) {
              onSelectTab(tabIndex);
            } else if (route != null) {
              onNavigate(route);
            }
          },
          borderRadius: BorderRadius.circular(12),
          child: Padding(
            padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 12),
            child: Row(
              children: [
                Icon(
                  item.icon,
                  color: isSelected ? iconColor : AppDesignSystem.gray500,
                  size: 20,
                ),
                const SizedBox(width: 14),
                Expanded(
                  child: Text(
                    item.labelVi,
                    style: AppDesignSystem.font.copyWith(
                      color: isSelected ? accentColor : AppDesignSystem.gray700,
                      fontWeight: isSelected ? FontWeight.w800 : FontWeight.w600,
                      fontSize: 14,
                      letterSpacing: -0.15,
                    ),
                  ),
                ),
                if (!isSelected)
                  const Icon(
                    Icons.chevron_right_rounded,
                    color: AppDesignSystem.gray400,
                    size: 18,
                  ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}

class _DrawerBrandFooter extends StatelessWidget {
  const _DrawerBrandFooter();

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 4),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Icon(Icons.restaurant_menu_rounded,
              size: 16, color: AppDesignSystem.gray400),
          const SizedBox(width: 6),
          Text(
            'HUITMeal · Suất ăn doanh nghiệp',
            style: AppDesignSystem.body(size: 11, color: AppDesignSystem.gray400),
          ),
        ],
      ),
    );
  }
}

class _DrawerAuthAction extends StatelessWidget {
  final String label;
  final IconData icon;
  final Color color;
  final VoidCallback onTap;

  const _DrawerAuthAction({
    required this.label,
    required this.icon,
    required this.color,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 16),
      child: Material(
        color: color.withValues(alpha: 0.08),
        borderRadius: BorderRadius.circular(12),
        child: InkWell(
          onTap: onTap,
          borderRadius: BorderRadius.circular(12),
          child: Padding(
            padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                Icon(icon, color: color, size: 20),
                const SizedBox(width: 8),
                Text(
                  label,
                  style: AppDesignSystem.font.copyWith(
                    color: color,
                    fontWeight: FontWeight.w700,
                    fontSize: 14,
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
