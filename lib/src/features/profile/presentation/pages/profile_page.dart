import 'package:flutter/material.dart';
import '../../../../app/app_routes.dart';
import '../../../../core/theme/app_design_system.dart';
import '../../../../core/widgets/app_confirm_dialog.dart';
import '../../../../core/widgets/role_tab_shell.dart';
import '../../data/profile_repository.dart';
import '../../data/models/user_profile_model.dart';

class ProfilePage extends StatefulWidget {
  final VoidCallback? onMenuPressed;
  /// `true` khi nằm trong tab shell (header module bên ngoài).
  final bool embeddedInModuleShell;

  const ProfilePage({
    super.key,
    this.onMenuPressed,
    this.embeddedInModuleShell = false,
  });

  @override
  State<ProfilePage> createState() => _ProfilePageState();
}

class _ProfilePageState extends State<ProfilePage> {
  late Future<UserProfileModel> _profileFuture;

  @override
  void initState() {
    super.initState();
    _profileFuture = ProfileRepository.instance.getProfile();
  }

  RolePalette _paletteForRoles(List<String> roles) {
    if (roles.contains('Manager')) return RolePalette.manager;
    if (roles.contains('Organization') || roles.contains('Company')) {
      return RolePalette.organization;
    }
    if (roles.contains('Shipper')) return RolePalette.shipper;
    return RolePalette.customer;
  }

  Future<void> _handleLogout() async {
    final profile = await _profileFuture;
    if (!mounted) return;
    await performAppLogout(context, role: _paletteForRoles(profile.roles));
  }

  String _getRoleLabelVi(List<String> roles) {
    if (roles.contains('Manager')) return 'Quản lý';
    if (roles.contains('Organization')) return 'Tổ chức B2B';
    if (roles.contains('Shipper')) return 'Nhân viên giao hàng';
    return 'Khách hàng';
  }

  Color _getRoleColor(List<String> roles) {
    return _paletteForRoles(roles).primary;
  }

  @override
  Widget build(BuildContext context) {
    final content = FutureBuilder<UserProfileModel>(
        future: _profileFuture,
        builder: (context, snapshot) {
          if (snapshot.connectionState == ConnectionState.waiting) {
            return Center(
              child: CircularProgressIndicator(
                color: Theme.of(context).colorScheme.primary,
              ),
            );
          }

          if (snapshot.hasError) {
            return Center(
              child: Padding(
                padding: const EdgeInsets.all(24),
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    Icon(
                      Icons.error_outline_rounded,
                      size: 64,
                      color: AppDesignSystem.danger,
                    ),
                    const SizedBox(height: 16),
                    Text(
                      'Không thể tải thông tin hồ sơ',
                      style: AppDesignSystem.sectionTitle(),
                      textAlign: TextAlign.center,
                    ),
                    const SizedBox(height: 8),
                    Text(
                      snapshot.error.toString(),
                      textAlign: TextAlign.center,
                      style: AppDesignSystem.body(size: 13),
                    ),
                    const SizedBox(height: 24),
                    FilledButton(
                      onPressed: () => setState(() {
                        _profileFuture = ProfileRepository.instance.getProfile();
                      }),
                      child: const Text('Thử lại'),
                    ),
                  ],
                ),
              ),
            );
          }

          final profile = snapshot.data!;
          final roleColor = _getRoleColor(profile.roles);
          final roleLabel = _getRoleLabelVi(profile.roles);

          final tabBottom = RoleTabScope.maybeOf(context)?.bottomInset ?? 24;

          return SingleChildScrollView(
            keyboardDismissBehavior: ScrollViewKeyboardDismissBehavior.onDrag,
            padding: EdgeInsets.fromLTRB(20, 24, 20, tabBottom),
            child: Column(
              children: [
                Container(
                  padding: const EdgeInsets.all(24),
                  decoration: AppDesignSystem.card(),
                  child: Column(
                    children: [
                      CircleAvatar(
                        radius: 50,
                        backgroundColor: roleColor.withValues(alpha: 0.1),
                        backgroundImage: profile.avatarUrl != null &&
                                profile.avatarUrl!.isNotEmpty
                            ? NetworkImage(profile.avatarUrl!)
                            : null,
                        child: profile.avatarUrl == null ||
                                profile.avatarUrl!.isEmpty
                            ? Text(
                                profile.displayName
                                    .substring(0, 1)
                                    .toUpperCase(),
                                style: AppDesignSystem.title(
                                  size: 36,
                                  color: roleColor,
                                ),
                              )
                            : null,
                      ),
                      const SizedBox(height: 16),
                      Text(
                        profile.displayName,
                        style: AppDesignSystem.title(size: 22),
                        textAlign: TextAlign.center,
                      ),
                      const SizedBox(height: 6),
                      Text(
                        profile.email,
                        style: AppDesignSystem.body(size: 14),
                        textAlign: TextAlign.center,
                      ),
                      const SizedBox(height: 16),
                      Container(
                        padding: const EdgeInsets.symmetric(
                          horizontal: 16,
                          vertical: 6,
                        ),
                        decoration: BoxDecoration(
                          color: roleColor.withValues(alpha: 0.12),
                          borderRadius: BorderRadius.circular(20),
                        ),
                        child: Text(
                          roleLabel,
                          style: AppDesignSystem.label(color: roleColor)
                              .copyWith(fontSize: 12),
                        ),
                      ),
                    ],
                  ),
                ),
                const SizedBox(height: 28),
                _buildSectionTitle('Tài khoản'),
                const SizedBox(height: 8),
                _buildSettingCard([
                  _buildSettingTile(
                    icon: Icons.person_outline_rounded,
                    iconColor: AppDesignSystem.info,
                    title: 'Thông tin cá nhân',
                    subtitle: 'Họ tên, ngày sinh, số điện thoại...',
                    route: AppRoutes.profilePersonalInfo,
                  ),
                  _buildSettingDivider(),
                  _buildSettingTile(
                    icon: Icons.location_on_outlined,
                    iconColor: AppDesignSystem.success,
                    title: 'Sổ địa chỉ',
                    subtitle: 'Danh sách địa điểm nhận hàng',
                    route: AppRoutes.profileAddresses,
                  ),
                ]),
                const SizedBox(height: 24),
                _buildSectionTitle('Cài đặt & Bảo mật'),
                const SizedBox(height: 8),
                _buildSettingCard([
                  _buildSettingTile(
                    icon: Icons.security_outlined,
                    iconColor: RolePalette.manager.primary,
                    title: 'Bảo mật & Mật khẩu',
                    subtitle: 'Đổi mật khẩu, tăng tính an toàn',
                    route: AppRoutes.profileSecurity,
                  ),
                  _buildSettingDivider(),
                  _buildSettingTile(
                    icon: Icons.notifications_none_rounded,
                    iconColor: AppDesignSystem.warning,
                    title: 'Cài đặt thông báo',
                    subtitle: 'Tần suất nhận tin nhắn, cập nhật',
                    route: AppRoutes.profileNotifications,
                  ),
                ]),
                const SizedBox(height: 24),
                _buildSectionTitle('Khác'),
                const SizedBox(height: 8),
                _buildSettingCard([
                  _buildSettingTile(
                    icon: Icons.help_outline_rounded,
                    iconColor: RolePalette.organization.primary,
                    title: 'Trung tâm trợ giúp',
                    subtitle: 'Giải đáp thắc mắc, gửi góp ý',
                    route: AppRoutes.profileHelp,
                  ),
                ]),
                const SizedBox(height: 36),
                SizedBox(
                  width: double.infinity,
                  child: OutlinedButton.icon(
                    style: OutlinedButton.styleFrom(
                      foregroundColor: AppDesignSystem.danger,
                      backgroundColor: AppDesignSystem.danger.withValues(alpha: 0.06),
                      side: BorderSide(
                        color: AppDesignSystem.danger.withValues(alpha: 0.2),
                      ),
                      padding: const EdgeInsets.symmetric(vertical: 16),
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(16),
                      ),
                    ),
                    onPressed: _handleLogout,
                    icon: const Icon(Icons.logout_rounded, size: 20),
                    label: const Text('Đăng xuất tài khoản'),
                  ),
                ),
                const SizedBox(height: 24),
              ],
            ),
          );
        },
      );

    if (widget.embeddedInModuleShell) {
      return ColoredBox(
        color: AppDesignSystem.gray50,
        child: content,
      );
    }

    return Scaffold(
      backgroundColor: AppDesignSystem.gray50,
      appBar: AppBar(
        leading: IconButton(
          icon: const Icon(Icons.menu),
          onPressed:
              widget.onMenuPressed ?? () => Scaffold.of(context).openDrawer(),
        ),
        title: const Text('Hồ sơ cá nhân'),
      ),
      body: content,
    );
  }

  Widget _buildSectionTitle(String title) {
    return Align(
      alignment: Alignment.centerLeft,
      child: Text(
        title,
        style: AppDesignSystem.label(color: AppDesignSystem.gray500)
            .copyWith(fontSize: 13, letterSpacing: 0.5),
      ),
    );
  }

  Widget _buildSettingCard(List<Widget> children) {
    return Container(
      decoration: AppDesignSystem.card(),
      child: Column(children: children),
    );
  }

  Widget _buildSettingTile({
    required IconData icon,
    required Color iconColor,
    required String title,
    required String subtitle,
    required String route,
  }) {
    return ListTile(
      contentPadding: const EdgeInsets.symmetric(horizontal: 20, vertical: 6),
      leading: Container(
        padding: const EdgeInsets.all(10),
        decoration: BoxDecoration(
          color: iconColor.withValues(alpha: 0.1),
          borderRadius: BorderRadius.circular(12),
        ),
        child: Icon(icon, color: iconColor, size: 22),
      ),
      title: Text(title, style: AppDesignSystem.label()),
      subtitle: Padding(
        padding: const EdgeInsets.only(top: 4),
        child: Text(subtitle, style: AppDesignSystem.body(size: 12)),
      ),
      trailing: Icon(
        Icons.chevron_right_rounded,
        color: AppDesignSystem.gray400,
      ),
      onTap: () => Navigator.of(context).pushNamed(route),
    );
  }

  Widget _buildSettingDivider() {
    return const Divider(
      height: 1,
      thickness: 1,
      color: AppDesignSystem.gray100,
      indent: 20,
      endIndent: 20,
    );
  }
}
