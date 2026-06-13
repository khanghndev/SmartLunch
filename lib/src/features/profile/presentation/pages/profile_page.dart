import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';
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

class _ProfilePageState extends State<ProfilePage> with TickerProviderStateMixin {
  late Future<UserProfileModel> _profileFuture;
  late AnimationController _fadeCtrl;
  late Animation<double> _fadeAnim;
  bool _isUploadingAvatar = false;

  Future<void> _changeAvatar() async {
    final picker = ImagePicker();
    final profile = await _profileFuture;
    final accent = _paletteForRoles(profile.roles).primary;
    if (!mounted) return;

    showModalBottomSheet(
      context: context,
      backgroundColor: Colors.transparent,
      builder: (context) => Container(
        decoration: const BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.vertical(top: Radius.circular(24)),
        ),
        padding: EdgeInsets.fromLTRB(
          20,
          16,
          20,
          MediaQuery.of(context).padding.bottom + 24,
        ),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Center(
              child: Container(
                width: 36,
                height: 4,
                decoration: BoxDecoration(
                  color: AppDesignSystem.gray200,
                  borderRadius: BorderRadius.circular(99),
                ),
              ),
            ),
            const SizedBox(height: 20),
            Text(
              'Thay đổi ảnh đại diện',
              textAlign: TextAlign.center,
              style: AppDesignSystem.font.copyWith(
                fontSize: 16.5,
                fontWeight: FontWeight.w800,
                color: AppDesignSystem.gray900,
              ),
            ),
            const SizedBox(height: 20),
            ListTile(
              leading: Icon(Icons.camera_alt_rounded, color: accent),
              title: Text(
                'Chụp ảnh mới',
                style: AppDesignSystem.font.copyWith(
                  fontSize: 14.5,
                  fontWeight: FontWeight.w600,
                ),
              ),
              onTap: () {
                Navigator.pop(context);
                _pickAndUploadImage(picker, ImageSource.camera);
              },
            ),
            const SizedBox(height: 4),
            ListTile(
              leading: Icon(Icons.photo_library_rounded, color: accent),
              title: Text(
                'Chọn từ thư viện',
                style: AppDesignSystem.font.copyWith(
                  fontSize: 14.5,
                  fontWeight: FontWeight.w600,
                ),
              ),
              onTap: () {
                Navigator.pop(context);
                _pickAndUploadImage(picker, ImageSource.gallery);
              },
            ),
            const SizedBox(height: 8),
            TextButton(
              onPressed: () => Navigator.pop(context),
              child: Text(
                'Huỷ',
                style: AppDesignSystem.font.copyWith(
                  fontSize: 14.5,
                  fontWeight: FontWeight.w700,
                  color: AppDesignSystem.gray500,
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Future<void> _pickAndUploadImage(ImagePicker picker, ImageSource source) async {
    try {
      final XFile? image = await picker.pickImage(
        source: source,
        maxWidth: 512,
        maxHeight: 512,
        imageQuality: 85,
      );
      if (image == null) return;

      setState(() {
        _isUploadingAvatar = true;
      });

      final bytes = await image.readAsBytes();
      final extension = image.name.split('.').last.toLowerCase();
      final contentType = extension == 'png'
          ? 'image/png'
          : extension == 'webp'
              ? 'image/webp'
              : 'image/jpeg';

      await ProfileRepository.instance.uploadAvatar(
        bytes: bytes,
        filename: image.name,
        contentType: contentType,
      );

      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Row(
              children: [
                const Icon(Icons.check_circle_rounded, color: Colors.white),
                const SizedBox(width: 10),
                const Text('Cập nhật ảnh đại diện thành công!'),
              ],
            ),
            backgroundColor: AppDesignSystem.success,
            behavior: SnackBarBehavior.floating,
            shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
          ),
        );
      }
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Row(
              children: [
                const Icon(Icons.error_outline_rounded, color: Colors.white),
                const SizedBox(width: 10),
                Expanded(child: Text('Tải ảnh thất bại: $e')),
              ],
            ),
            backgroundColor: AppDesignSystem.danger,
            behavior: SnackBarBehavior.floating,
            shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
          ),
        );
      }
    } finally {
      if (mounted) {
        setState(() {
          _isUploadingAvatar = false;
          _profileFuture = ProfileRepository.instance.getProfile();
        });
      }
    }
  }

  @override
  void initState() {
    super.initState();
    _profileFuture = ProfileRepository.instance.getProfile();
    _fadeCtrl = AnimationController(
      vsync: this,
      duration: const Duration(milliseconds: 600),
    );
    _fadeAnim = CurvedAnimation(parent: _fadeCtrl, curve: Curves.easeOut);
    _fadeCtrl.forward();
  }

  Future<void> _refreshProfile() async {
    setState(() {
      _profileFuture = ProfileRepository.instance.getProfile();
    });
    try {
      await _profileFuture;
    } catch (_) {}
  }

  @override
  void dispose() {
    _fadeCtrl.dispose();
    super.dispose();
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

  IconData _getRoleIcon(List<String> roles) {
    if (roles.contains('Manager')) return Icons.admin_panel_settings_rounded;
    if (roles.contains('Organization')) return Icons.business_rounded;
    if (roles.contains('Shipper')) return Icons.delivery_dining_rounded;
    return Icons.person_rounded;
  }

  @override
  Widget build(BuildContext context) {
    final content = FutureBuilder<UserProfileModel>(
      future: _profileFuture,
      builder: (context, snapshot) {
        if (snapshot.connectionState == ConnectionState.waiting) {
          return const Center(child: _LoadingSkeleton());
        }

        if (snapshot.hasError) {
          return _ErrorState(
            message: snapshot.error.toString(),
            onRetry: () => setState(() {
              _profileFuture = ProfileRepository.instance.getProfile();
            }),
          );
        }

        final profile = snapshot.data!;
        final palette = _paletteForRoles(profile.roles);
        final roleColor = palette.primary;
        final roleColorAlt = palette.primaryAlt;
        final roleLabel = _getRoleLabelVi(profile.roles);
        final roleIcon = _getRoleIcon(profile.roles);
        final initials = profile.displayName.isNotEmpty
            ? profile.displayName.trim().split(' ').map((w) => w.isNotEmpty ? w[0] : '').take(2).join()
            : '?';
        final tabBottom = RoleTabScope.maybeOf(context)?.bottomInset ?? 24;

        return FadeTransition(
          opacity: _fadeAnim,
          child: RefreshIndicator(
            onRefresh: _refreshProfile,
            color: roleColor,
            child: CustomScrollView(
              physics: const AlwaysScrollableScrollPhysics(
                parent: BouncingScrollPhysics(),
              ),
              slivers: [
                // ── Hero Avatar Card ───────────────────────────────────────
                SliverToBoxAdapter(
                  child: Transform.translate(
                    offset: Offset(0, widget.embeddedInModuleShell ? -10 : 0),
                    child: Padding(
                      padding: EdgeInsets.fromLTRB(
                        16,
                        widget.embeddedInModuleShell ? 4 : 16,
                        16,
                        0,
                      ),
                      child: _ProfileHeroCard(
                        profile: profile,
                        roleLabel: roleLabel,
                        roleIcon: roleIcon,
                        roleColor: roleColor,
                        roleColorAlt: roleColorAlt,
                        initials: initials,
                        onCompleteProfile: () async {
                          await Navigator.of(context).pushNamed(AppRoutes.profilePersonalInfo);
                          _refreshProfile();
                        },
                        onAvatarTap: _changeAvatar,
                        isUploadingAvatar: _isUploadingAvatar,
                      ),
                    ),
                  ),
                ),

              // ── Tài khoản ──────────────────────────────────────────────
              SliverToBoxAdapter(
                child: Padding(
                  padding: const EdgeInsets.fromLTRB(16, 16, 16, 8),
                  child: _SectionLabel(title: 'Tài khoản', accent: roleColor),
                ),
              ),
              SliverToBoxAdapter(
                child: Padding(
                  padding: const EdgeInsets.symmetric(horizontal: 16),
                  child: _SettingsGroup(
                    items: [
                      _SettingItem(
                        icon: Icons.person_outline_rounded,
                        iconColor: AppDesignSystem.info,
                        title: 'Thông tin cá nhân',
                        subtitle: 'Họ tên, ngày sinh, số điện thoại',
                        route: AppRoutes.profilePersonalInfo,
                        onReturn: _refreshProfile,
                      ),
                      _SettingItem(
                        icon: Icons.location_on_outlined,
                        iconColor: AppDesignSystem.success,
                        title: 'Sổ địa chỉ',
                        subtitle: 'Danh sách địa điểm nhận hàng',
                        route: AppRoutes.profileAddresses,
                      ),
                    ],
                  ),
                ),
              ),

              // ── Cài đặt & Bảo mật ─────────────────────────────────────
              SliverToBoxAdapter(
                child: Padding(
                  padding: const EdgeInsets.fromLTRB(16, 20, 16, 8),
                  child: _SectionLabel(title: 'Cài đặt & Bảo mật', accent: roleColor),
                ),
              ),
              SliverToBoxAdapter(
                child: Padding(
                  padding: const EdgeInsets.symmetric(horizontal: 16),
                  child: _SettingsGroup(
                    items: [
                      _SettingItem(
                        icon: Icons.security_outlined,
                        iconColor: RolePalette.manager.primary,
                        title: 'Bảo mật & Mật khẩu',
                        subtitle: 'Đổi mật khẩu, tăng tính an toàn',
                        route: AppRoutes.profileSecurity,
                      ),
                      _SettingItem(
                        icon: Icons.notifications_none_rounded,
                        iconColor: AppDesignSystem.warning,
                        title: 'Cài đặt thông báo',
                        subtitle: 'Tần suất nhận tin, cập nhật trạng thái',
                        route: AppRoutes.profileNotifications,
                      ),
                    ],
                  ),
                ),
              ),

              // ── Khác ───────────────────────────────────────────────────
              SliverToBoxAdapter(
                child: Padding(
                  padding: const EdgeInsets.fromLTRB(16, 20, 16, 8),
                  child: _SectionLabel(title: 'Khác', accent: roleColor),
                ),
              ),
              SliverToBoxAdapter(
                child: Padding(
                  padding: const EdgeInsets.symmetric(horizontal: 16),
                  child: _SettingsGroup(
                    items: [
                      _SettingItem(
                        icon: Icons.help_outline_rounded,
                        iconColor: RolePalette.organization.primary,
                        title: 'Trung tâm trợ giúp',
                        subtitle: 'Giải đáp thắc mắc, gửi góp ý',
                        route: AppRoutes.profileHelp,
                      ),
                    ],
                  ),
                ),
              ),

              // ── Logout ─────────────────────────────────────────────────
              SliverToBoxAdapter(
                child: Padding(
                  padding: EdgeInsets.fromLTRB(16, 28, 16, tabBottom + 8),
                  child: _LogoutButton(onTap: _handleLogout),
                ),
              ),
            ],
          ),
        ),
      );
      },
    );

    if (widget.embeddedInModuleShell) {
      return ColoredBox(
        color: const Color(0xFFF5F7FA),
        child: content,
      );
    }

    return Scaffold(
      backgroundColor: const Color(0xFFF5F7FA),
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
}

// ─────────────────────────────────────────────────────────────────────────────
// Profile Hero Card
// ─────────────────────────────────────────────────────────────────────────────
class _ProfileHeroCard extends StatelessWidget {
  final UserProfileModel profile;
  final String roleLabel;
  final IconData roleIcon;
  final Color roleColor;
  final Color roleColorAlt;
  final String initials;
  final VoidCallback onCompleteProfile;
  final VoidCallback? onAvatarTap;
  final bool isUploadingAvatar;

  const _ProfileHeroCard({
    required this.profile,
    required this.roleLabel,
    required this.roleIcon,
    required this.roleColor,
    required this.roleColorAlt,
    required this.initials,
    required this.onCompleteProfile,
    this.onAvatarTap,
    this.isUploadingAvatar = false,
  });

  static const _verifiedBlue = Color(0xFF1D9BF0);

  @override
  Widget build(BuildContext context) {
    final unit = profile.unit;
    final isOrg = profile.isOrganizationRole;
    final isVerified = profile.isProfileComplete;
    final completion = profile.profileCompletionPercent;

    return Container(
      width: double.infinity,
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(20),
        border: Border.all(color: AppDesignSystem.gray100),
        boxShadow: [
          BoxShadow(
            color: roleColor.withValues(alpha: 0.1),
            blurRadius: 20,
            offset: const Offset(0, 8),
          ),
        ],
      ),
      child: ClipRRect(
        borderRadius: BorderRadius.circular(20),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Container(
              height: 4,
              decoration: BoxDecoration(
                gradient: LinearGradient(colors: [roleColor, roleColorAlt]),
              ),
            ),
            Padding(
              padding: const EdgeInsets.fromLTRB(16, 16, 16, 14),
              child: Column(
                children: [
                  Row(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      _ProfileAvatar(
                        avatarUrl: profile.avatarUrl,
                        initials: initials,
                        roleColor: roleColor,
                        roleColorAlt: roleColorAlt,
                        isVerified: isVerified,
                        onTap: onAvatarTap,
                        isUploading: isUploadingAvatar,
                      ),
                      const SizedBox(width: 14),
                      Expanded(
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            _NameWithBadge(
                              name: profile.displayName,
                              isVerified: isVerified,
                            ),
                            if (isOrg && profile.unitDisplayName.isNotEmpty) ...[
                              const SizedBox(height: 4),
                              Row(
                                children: [
                                  Icon(
                                    Icons.business_rounded,
                                    size: 14,
                                    color: roleColor,
                                  ),
                                  const SizedBox(width: 4),
                                  Expanded(
                                    child: Text(
                                      profile.unitDisplayName,
                                      maxLines: 2,
                                      overflow: TextOverflow.ellipsis,
                                      style: AppDesignSystem.font.copyWith(
                                        fontSize: 14,
                                        fontWeight: FontWeight.w800,
                                        color: roleColor,
                                        height: 1.25,
                                      ),
                                    ),
                                  ),
                                ],
                              ),
                            ],
                            const SizedBox(height: 8),
                            Wrap(
                              spacing: 6,
                              runSpacing: 6,
                              children: [
                                _MetaChip(
                                  icon: roleIcon,
                                  label: roleLabel,
                                  color: roleColor,
                                ),
                                if (profile.isEmailVerified)
                                  const _MetaChip(
                                    icon: Icons.mark_email_read_rounded,
                                    label: 'Email xác thực',
                                    color: _verifiedBlue,
                                  ),
                                if (isVerified)
                                  const _MetaChip(
                                    icon: Icons.verified_rounded,
                                    label: 'Đã xác minh',
                                    color: _verifiedBlue,
                                  ),
                              ],
                            ),
                          ],
                        ),
                      ),
                    ],
                  ),
                  const SizedBox(height: 14),
                  const Divider(height: 1, color: AppDesignSystem.gray100),
                  const SizedBox(height: 12),
                  _ProfileInfoRow(
                    icon: Icons.alternate_email_rounded,
                    label: 'Email',
                    value: profile.email,
                  ),
                  if (profile.hasPhone) ...[
                    const SizedBox(height: 8),
                    _ProfileInfoRow(
                      icon: Icons.phone_rounded,
                      label: 'Điện thoại',
                      value: profile.phoneNumber!,
                    ),
                  ],
                  if (isOrg && unit?.phone?.trim().isNotEmpty == true) ...[
                    const SizedBox(height: 8),
                    _ProfileInfoRow(
                      icon: Icons.phone_in_talk_rounded,
                      label: 'Hotline đơn vị',
                      value: unit!.phone!,
                    ),
                  ],
                  if (isOrg && unit?.address?.trim().isNotEmpty == true) ...[
                    const SizedBox(height: 8),
                    _ProfileInfoRow(
                      icon: Icons.location_on_rounded,
                      label: 'Địa chỉ',
                      value: unit!.address!,
                    ),
                  ],
                  if (isOrg && unit?.legalRepresentative?.trim().isNotEmpty == true) ...[
                    const SizedBox(height: 8),
                    _ProfileInfoRow(
                      icon: Icons.badge_rounded,
                      label: 'Người đại diện',
                      value: unit!.legalRepresentative!,
                    ),
                  ],
                  if (!isOrg && profile.birthDate != null) ...[
                    const SizedBox(height: 8),
                    _ProfileInfoRow(
                      icon: Icons.cake_rounded,
                      label: 'Ngày sinh',
                      value: _formatBirthDate(profile.birthDate!),
                    ),
                  ],
                  if (!isVerified) ...[
                    const SizedBox(height: 14),
                    _ProfileCompletionBanner(
                      percent: completion,
                      missing: profile.missingProfileHints,
                      accent: roleColor,
                      onTap: onCompleteProfile,
                    ),
                  ],
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }

  String _formatBirthDate(DateTime date) {
    final d = date.day.toString().padLeft(2, '0');
    final m = date.month.toString().padLeft(2, '0');
    return '$d/$m/${date.year}';
  }
}

class _ProfileAvatar extends StatelessWidget {
  final String? avatarUrl;
  final String initials;
  final Color roleColor;
  final Color roleColorAlt;
  final bool isVerified;
  final VoidCallback? onTap;
  final bool isUploading;

  const _ProfileAvatar({
    required this.avatarUrl,
    required this.initials,
    required this.roleColor,
    required this.roleColorAlt,
    required this.isVerified,
    this.onTap,
    this.isUploading = false,
  });

  @override
  Widget build(BuildContext context) {
    const size = 84.0;
    const inner = 76.0;

    return SizedBox(
      width: size,
      height: size,
      child: Stack(
        clipBehavior: Clip.none,
        children: [
          Container(
            width: size,
            height: size,
            decoration: BoxDecoration(
              shape: BoxShape.circle,
              gradient: LinearGradient(
                begin: Alignment.topLeft,
                end: Alignment.bottomRight,
                colors: [
                  roleColor.withValues(alpha: 0.45),
                  roleColorAlt.withValues(alpha: 0.35),
                ],
              ),
            ),
          ),
          Positioned(
            top: 4,
            left: 4,
            child: GestureDetector(
              onTap: isUploading ? null : onTap,
              child: Container(
                width: inner,
                height: inner,
                decoration: BoxDecoration(
                  shape: BoxShape.circle,
                  color: roleColor.withValues(alpha: 0.08),
                  border: Border.all(color: Colors.white, width: 2),
                ),
                clipBehavior: Clip.antiAlias,
                child: _buildAvatarContent(inner),
              ),
            ),
          ),
          if (isVerified)
            Positioned(
              bottom: -2,
              right: -2,
              child: Container(
                padding: const EdgeInsets.all(2),
                decoration: const BoxDecoration(
                  color: Colors.white,
                  shape: BoxShape.circle,
                ),
                child: const Icon(
                  Icons.verified_rounded,
                  color: Color(0xFF1D9BF0),
                  size: 22,
                ),
              ),
            ),
          Positioned(
            bottom: -2,
            left: -2,
            child: GestureDetector(
              onTap: isUploading ? null : onTap,
              child: Container(
                padding: const EdgeInsets.all(6),
                decoration: BoxDecoration(
                  color: roleColor,
                  shape: BoxShape.circle,
                  border: Border.all(color: Colors.white, width: 2),
                  boxShadow: [
                    BoxShadow(
                      color: Colors.black.withValues(alpha: 0.15),
                      blurRadius: 6,
                      offset: const Offset(0, 2),
                    ),
                  ],
                ),
                child: const Icon(
                  Icons.camera_alt_rounded,
                  color: Colors.white,
                  size: 14,
                ),
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildAvatarContent(double size) {
    if (isUploading) {
      return const Center(
        child: SizedBox(
          width: 24,
          height: 24,
          child: CircularProgressIndicator(
            strokeWidth: 2.5,
          ),
        ),
      );
    }
    if (avatarUrl != null && avatarUrl!.trim().isNotEmpty) {
      return Image.network(
        avatarUrl!,
        width: size,
        height: size,
        fit: BoxFit.cover,
        errorBuilder: (_, __, ___) => _initialsFallback(),
      );
    }
    return _initialsFallback();
  }

  Widget _initialsFallback() {
    return Center(
      child: Text(
        initials.toUpperCase(),
        style: AppDesignSystem.font.copyWith(
          fontSize: 28,
          fontWeight: FontWeight.w800,
          color: roleColor,
          height: 1,
        ),
      ),
    );
  }
}

class _NameWithBadge extends StatelessWidget {
  final String name;
  final bool isVerified;

  const _NameWithBadge({required this.name, required this.isVerified});

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        Flexible(
          child: Text(
            name,
            maxLines: 2,
            overflow: TextOverflow.ellipsis,
            style: AppDesignSystem.font.copyWith(
              fontSize: 18,
              fontWeight: FontWeight.w800,
              color: AppDesignSystem.gray900,
              letterSpacing: -0.3,
              height: 1.2,
            ),
          ),
        ),
        if (isVerified) ...[
          const SizedBox(width: 4),
          const Icon(
            Icons.verified_rounded,
            color: Color(0xFF1D9BF0),
            size: 20,
          ),
        ],
      ],
    );
  }
}

class _MetaChip extends StatelessWidget {
  final IconData icon;
  final String label;
  final Color color;

  const _MetaChip({
    required this.icon,
    required this.label,
    required this.color,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
      decoration: BoxDecoration(
        color: color.withValues(alpha: 0.08),
        borderRadius: BorderRadius.circular(999),
        border: Border.all(color: color.withValues(alpha: 0.15)),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(icon, size: 12, color: color),
          const SizedBox(width: 4),
          Text(
            label,
            style: AppDesignSystem.font.copyWith(
              fontSize: 10.5,
              fontWeight: FontWeight.w700,
              color: color,
            ),
          ),
        ],
      ),
    );
  }
}

class _ProfileInfoRow extends StatelessWidget {
  final IconData icon;
  final String label;
  final String value;

  const _ProfileInfoRow({
    required this.icon,
    required this.label,
    required this.value,
  });

  @override
  Widget build(BuildContext context) {
    return Row(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Container(
          width: 32,
          height: 32,
          decoration: BoxDecoration(
            color: AppDesignSystem.gray50,
            borderRadius: BorderRadius.circular(9),
          ),
          child: Icon(icon, size: 16, color: AppDesignSystem.gray500),
        ),
        const SizedBox(width: 10),
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                label,
                style: AppDesignSystem.font.copyWith(
                  fontSize: 10.5,
                  fontWeight: FontWeight.w600,
                  color: AppDesignSystem.gray400,
                ),
              ),
              const SizedBox(height: 1),
              Text(
                value,
                style: AppDesignSystem.font.copyWith(
                  fontSize: 13,
                  fontWeight: FontWeight.w600,
                  color: AppDesignSystem.gray900,
                  height: 1.35,
                ),
              ),
            ],
          ),
        ),
      ],
    );
  }
}

class _ProfileCompletionBanner extends StatelessWidget {
  final int percent;
  final List<String> missing;
  final Color accent;
  final VoidCallback onTap;

  const _ProfileCompletionBanner({
    required this.percent,
    required this.missing,
    required this.accent,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return Material(
      color: accent.withValues(alpha: 0.06),
      borderRadius: BorderRadius.circular(12),
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(12),
        child: Padding(
          padding: const EdgeInsets.all(12),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                children: [
                  Expanded(
                    child: Text(
                      'Hoàn thiện hồ sơ ($percent%)',
                      style: AppDesignSystem.font.copyWith(
                        fontSize: 12.5,
                        fontWeight: FontWeight.w800,
                        color: accent,
                      ),
                    ),
                  ),
                  Icon(Icons.chevron_right_rounded, size: 18, color: accent),
                ],
              ),
              const SizedBox(height: 8),
              ClipRRect(
                borderRadius: BorderRadius.circular(999),
                child: LinearProgressIndicator(
                  value: percent / 100,
                  minHeight: 5,
                  backgroundColor: accent.withValues(alpha: 0.12),
                  color: accent,
                ),
              ),
              if (missing.isNotEmpty) ...[
                const SizedBox(height: 6),
                Text(
                  'Còn thiếu: ${missing.join(', ')}',
                  maxLines: 2,
                  overflow: TextOverflow.ellipsis,
                  style: AppDesignSystem.font.copyWith(
                    fontSize: 11,
                    fontWeight: FontWeight.w500,
                    color: AppDesignSystem.gray500,
                  ),
                ),
              ],
            ],
          ),
        ),
      ),
    );
  }
}

// ─────────────────────────────────────────────────────────────────────────────
// Section Label
// ─────────────────────────────────────────────────────────────────────────────
class _SectionLabel extends StatelessWidget {
  final String title;
  final Color accent;

  const _SectionLabel({required this.title, required this.accent});

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        Container(
          width: 3,
          height: 18,
          decoration: BoxDecoration(
            gradient: LinearGradient(
              begin: Alignment.topCenter,
              end: Alignment.bottomCenter,
              colors: [accent, accent.withValues(alpha: 0.3)],
            ),
            borderRadius: BorderRadius.circular(99),
          ),
        ),
        const SizedBox(width: 8),
        Text(
          title,
          style: AppDesignSystem.font.copyWith(
            fontSize: 13.5,
            fontWeight: FontWeight.w800,
            color: AppDesignSystem.gray700,
            letterSpacing: 0.1,
          ),
        ),
      ],
    );
  }
}

// ─────────────────────────────────────────────────────────────────────────────
// Settings Group + Item
// ─────────────────────────────────────────────────────────────────────────────
class _SettingItem {
  final IconData icon;
  final Color iconColor;
  final String title;
  final String subtitle;
  final String route;
  final VoidCallback? onReturn;

  const _SettingItem({
    required this.icon,
    required this.iconColor,
    required this.title,
    required this.subtitle,
    required this.route,
    this.onReturn,
  });
}

class _SettingsGroup extends StatelessWidget {
  final List<_SettingItem> items;

  const _SettingsGroup({required this.items});

  @override
  Widget build(BuildContext context) {
    return Container(
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(20),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.05),
            blurRadius: 16,
            offset: const Offset(0, 6),
          ),
        ],
      ),
      child: ClipRRect(
        borderRadius: BorderRadius.circular(20),
        child: Column(
          children: [
            for (var i = 0; i < items.length; i++) ...[
              if (i > 0)
                Padding(
                  padding: const EdgeInsets.only(left: 68),
                  child: Divider(
                    height: 1,
                    thickness: 0.5,
                    color: AppDesignSystem.gray100,
                  ),
                ),
              _SettingTileWidget(item: items[i]),
            ],
          ],
        ),
      ),
    );
  }
}

class _SettingTileWidget extends StatefulWidget {
  final _SettingItem item;

  const _SettingTileWidget({required this.item});

  @override
  State<_SettingTileWidget> createState() => _SettingTileWidgetState();
}

class _SettingTileWidgetState extends State<_SettingTileWidget> {
  bool _pressed = false;

  @override
  Widget build(BuildContext context) {
    final item = widget.item;
    return GestureDetector(
      onTapDown: (_) => setState(() => _pressed = true),
      onTapUp: (_) async {
        setState(() => _pressed = false);
        await Navigator.of(context).pushNamed(item.route);
        if (item.onReturn != null) {
          item.onReturn!();
        }
      },
      onTapCancel: () => setState(() => _pressed = false),
      child: AnimatedContainer(
        duration: const Duration(milliseconds: 100),
        color: _pressed
            ? item.iconColor.withValues(alpha: 0.04)
            : Colors.transparent,
        child: Padding(
          padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 13),
          child: Row(
            children: [
              Container(
                width: 42,
                height: 42,
                decoration: BoxDecoration(
                  gradient: LinearGradient(
                    begin: Alignment.topLeft,
                    end: Alignment.bottomRight,
                    colors: [
                      item.iconColor.withValues(alpha: 0.18),
                      item.iconColor.withValues(alpha: 0.07),
                    ],
                  ),
                  borderRadius: BorderRadius.circular(13),
                  border: Border.all(
                    color: item.iconColor.withValues(alpha: 0.15),
                    width: 0.5,
                  ),
                ),
                child: Icon(item.icon, color: item.iconColor, size: 21),
              ),
              const SizedBox(width: 14),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      item.title,
                      style: AppDesignSystem.font.copyWith(
                        fontSize: 14,
                        fontWeight: FontWeight.w700,
                        color: AppDesignSystem.gray900,
                        letterSpacing: -0.1,
                      ),
                    ),
                    const SizedBox(height: 2),
                    Text(
                      item.subtitle,
                      style: AppDesignSystem.font.copyWith(
                        fontSize: 12,
                        fontWeight: FontWeight.w500,
                        color: AppDesignSystem.gray400,
                      ),
                    ),
                  ],
                ),
              ),
              const SizedBox(width: 8),
              Container(
                width: 28,
                height: 28,
                decoration: BoxDecoration(
                  color: AppDesignSystem.gray100,
                  borderRadius: BorderRadius.circular(9),
                ),
                child: const Icon(
                  Icons.arrow_forward_ios_rounded,
                  size: 12,
                  color: AppDesignSystem.gray400,
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

// ─────────────────────────────────────────────────────────────────────────────
// Logout Button
// ─────────────────────────────────────────────────────────────────────────────
class _LogoutButton extends StatefulWidget {
  final VoidCallback onTap;

  const _LogoutButton({required this.onTap});

  @override
  State<_LogoutButton> createState() => _LogoutButtonState();
}

class _LogoutButtonState extends State<_LogoutButton> {
  bool _pressed = false;

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTapDown: (_) => setState(() => _pressed = true),
      onTapUp: (_) {
        setState(() => _pressed = false);
        widget.onTap();
      },
      onTapCancel: () => setState(() => _pressed = false),
      child: AnimatedScale(
        scale: _pressed ? 0.97 : 1.0,
        duration: const Duration(milliseconds: 100),
        child: Container(
          width: double.infinity,
          padding: const EdgeInsets.symmetric(vertical: 16),
          decoration: BoxDecoration(
            color: AppDesignSystem.danger.withValues(alpha: 0.06),
            borderRadius: BorderRadius.circular(18),
            border: Border.all(
              color: AppDesignSystem.danger.withValues(alpha: 0.18),
              width: 1,
            ),
            boxShadow: [
              BoxShadow(
                color: AppDesignSystem.danger.withValues(alpha: 0.08),
                blurRadius: 12,
                offset: const Offset(0, 4),
              ),
            ],
          ),
          child: Row(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              Icon(
                Icons.logout_rounded,
                color: AppDesignSystem.danger,
                size: 20,
              ),
              const SizedBox(width: 10),
              Text(
                'Đăng xuất tài khoản',
                style: AppDesignSystem.font.copyWith(
                  fontSize: 15,
                  fontWeight: FontWeight.w700,
                  color: AppDesignSystem.danger,
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

// ─────────────────────────────────────────────────────────────────────────────
// Loading Skeleton
// ─────────────────────────────────────────────────────────────────────────────
class _LoadingSkeleton extends StatelessWidget {
  const _LoadingSkeleton();

  @override
  Widget build(BuildContext context) {
    return const SingleChildScrollView(
      physics: NeverScrollableScrollPhysics(),
      child: Padding(
        padding: EdgeInsets.fromLTRB(16, 20, 16, 20),
        child: Column(
          children: [
            _SkeletonBox(height: 260),
            SizedBox(height: 20),
            _SkeletonBox(height: 130),
            SizedBox(height: 16),
            _SkeletonBox(height: 130),
          ],
        ),
      ),
    );
  }
}

class _SkeletonBox extends StatelessWidget {
  final double height;
  const _SkeletonBox({required this.height});

  @override
  Widget build(BuildContext context) {
    return Container(
      height: height,
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(20),
      ),
    );
  }
}

// ─────────────────────────────────────────────────────────────────────────────
// Error State
// ─────────────────────────────────────────────────────────────────────────────
class _ErrorState extends StatelessWidget {
  final String message;
  final VoidCallback onRetry;

  const _ErrorState({required this.message, required this.onRetry});

  @override
  Widget build(BuildContext context) {
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(24),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Container(
              padding: const EdgeInsets.all(20),
              decoration: BoxDecoration(
                color: AppDesignSystem.danger.withValues(alpha: 0.08),
                shape: BoxShape.circle,
              ),
              child: const Icon(
                Icons.error_outline_rounded,
                size: 48,
                color: AppDesignSystem.danger,
              ),
            ),
            const SizedBox(height: 20),
            Text(
              'Không thể tải thông tin',
              style: AppDesignSystem.font.copyWith(
                fontSize: 18,
                fontWeight: FontWeight.w800,
                color: AppDesignSystem.gray900,
              ),
              textAlign: TextAlign.center,
            ),
            const SizedBox(height: 8),
            Text(
              message,
              textAlign: TextAlign.center,
              style: AppDesignSystem.body(size: 13),
            ),
            const SizedBox(height: 24),
            FilledButton.icon(
              onPressed: onRetry,
              icon: const Icon(Icons.refresh_rounded),
              label: const Text('Thử lại'),
            ),
          ],
        ),
      ),
    );
  }
}
