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
      backgroundColor: AppDesignSystem.gray50,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.only(
          topRight: Radius.circular(20),
          bottomRight: Radius.circular(20),
        ),
      ),
      child: SafeArea(
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
                padding: const EdgeInsets.fromLTRB(12, 4, 12, 8),
                children: [
                  for (var i = 0; i < widget.sections.length; i++) ...[
                    if (i > 0) const SizedBox(height: 4),
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
            _DrawerAuthAction(
              label: isLoggedIn ? 'Đăng xuất' : 'Đăng nhập',
              icon: isLoggedIn ? Icons.logout_rounded : Icons.login_rounded,
              color: isLoggedIn ? AppDesignSystem.danger : widget.accentColor,
              onTap: () => _handleAuthAction(context),
            ),
            const SizedBox(height: 8),
          ],
        ),
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
    return Container(
      margin: const EdgeInsets.fromLTRB(12, 8, 12, 12),
      decoration: BoxDecoration(
        gradient: LinearGradient(
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
          colors: gradient,
        ),
        borderRadius: BorderRadius.circular(20),
        boxShadow: [
          BoxShadow(
            color: gradient.first.withValues(alpha: 0.28),
            blurRadius: 20,
            offset: const Offset(0, 8),
          ),
        ],
      ),
      child: ClipRRect(
        borderRadius: BorderRadius.circular(20),
        child: Stack(
          children: [
            Positioned(
              right: -24,
              top: -24,
              child: Icon(
                Icons.restaurant_menu_rounded,
                size: 120,
                color: Colors.white.withValues(alpha: 0.08),
              ),
            ),
            Padding(
              padding: const EdgeInsets.fromLTRB(14, 12, 14, 16),
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
                          letterSpacing: 0.2,
                        ),
                      ),
                      const Spacer(),
                      _DrawerIconButton(
                        icon: Icons.close_rounded,
                        onTap: onClose,
                      ),
                    ],
                  ),
                  const SizedBox(height: 14),
                  if (isLoading)
                    const _HeaderSkeleton()
                  else
                    Row(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        _DrawerAvatar(url: avatarUrl, accent: accentColor),
                        const SizedBox(width: 12),
                        Expanded(
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text(
                                userName,
                                maxLines: 1,
                                overflow: TextOverflow.ellipsis,
                                style: AppDesignSystem.title(
                                  size: 18,
                                  color: Colors.white,
                                ),
                              ),
                              const SizedBox(height: 3),
                              Text(
                                subtitle,
                                maxLines: 1,
                                overflow: TextOverflow.ellipsis,
                                style: AppDesignSystem.body(
                                  size: 12,
                                  color: Colors.white.withValues(alpha: 0.88),
                                ),
                              ),
                              const SizedBox(height: 8),
                              Container(
                                padding: const EdgeInsets.symmetric(
                                  horizontal: 10,
                                  vertical: 4,
                                ),
                                decoration: BoxDecoration(
                                  color: Colors.white.withValues(alpha: 0.18),
                                  borderRadius: BorderRadius.circular(999),
                                  border: Border.all(color: Colors.white24),
                                ),
                                child: Text(
                                  roleBadge,
                                  style: AppDesignSystem.roleBadge(Colors.white),
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
      width: 52,
      height: 52,
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: Colors.white.withValues(alpha: 0.5), width: 2),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.12),
            blurRadius: 8,
            offset: const Offset(0, 2),
          ),
        ],
      ),
      child: ClipRRect(
        borderRadius: BorderRadius.circular(12),
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
      color: accent.withValues(alpha: 0.15),
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
            borderRadius: BorderRadius.circular(14),
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
            padding: const EdgeInsets.fromLTRB(4, 10, 4, 6),
            child: Row(
              children: [
                Container(
                  width: 3,
                  height: 14,
                  decoration: BoxDecoration(
                    color: accentColor,
                    borderRadius: BorderRadius.circular(2),
                  ),
                ),
                const SizedBox(width: 8),
                Text(
                  section.title!.toUpperCase(),
                  style: AppDesignSystem.roleBadge(accentColor),
                ),
              ],
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
      padding: const EdgeInsets.only(bottom: 6),
      child: Material(
        color: isSelected ? accentColor.withValues(alpha: 0.1) : Colors.white,
        borderRadius: BorderRadius.circular(14),
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
          borderRadius: BorderRadius.circular(14),
          child: Ink(
            decoration: BoxDecoration(
              borderRadius: BorderRadius.circular(14),
              border: Border.all(
                color: isSelected
                    ? accentColor.withValues(alpha: 0.35)
                    : AppDesignSystem.gray100,
              ),
              boxShadow: isSelected
                  ? null
                  : [
                      BoxShadow(
                        color: Colors.black.withValues(alpha: 0.03),
                        blurRadius: 6,
                        offset: const Offset(0, 2),
                      ),
                    ],
            ),
            child: Padding(
              padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 10),
              child: Row(
                children: [
                  Container(
                    width: 38,
                    height: 38,
                    decoration: BoxDecoration(
                      color: isSelected
                          ? accentColor.withValues(alpha: 0.15)
                          : iconColor.withValues(alpha: 0.1),
                      borderRadius: BorderRadius.circular(11),
                    ),
                    child: Icon(
                      item.icon,
                      color: isSelected ? accentColor : iconColor,
                      size: 20,
                    ),
                  ),
                  const SizedBox(width: 12),
                  Expanded(
                    child: Text(
                      item.labelVi,
                      style: AppDesignSystem.label(
                        color: isSelected ? accentColor : AppDesignSystem.gray900,
                      ).copyWith(
                        fontWeight: isSelected ? FontWeight.w800 : FontWeight.w600,
                        fontSize: 14,
                      ),
                    ),
                  ),
                  Icon(
                    isSelected
                        ? Icons.radio_button_checked_rounded
                        : Icons.chevron_right_rounded,
                    color: isSelected
                        ? accentColor
                        : AppDesignSystem.gray400,
                    size: 20,
                  ),
                ],
              ),
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
      padding: const EdgeInsets.symmetric(horizontal: 12),
      child: Material(
        color: color.withValues(alpha: 0.08),
        borderRadius: BorderRadius.circular(14),
        child: InkWell(
          onTap: onTap,
          borderRadius: BorderRadius.circular(14),
          child: Ink(
            decoration: BoxDecoration(
              borderRadius: BorderRadius.circular(14),
              border: Border.all(color: color.withValues(alpha: 0.22)),
            ),
            child: Padding(
              padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 12),
              child: Row(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Icon(icon, color: color, size: 20),
                  const SizedBox(width: 8),
                  Text(
                    label,
                    style: AppDesignSystem.label(color: color),
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}
