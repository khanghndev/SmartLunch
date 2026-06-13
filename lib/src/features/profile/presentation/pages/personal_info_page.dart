import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import '../../../../core/theme/app_design_system.dart';
import '../../data/profile_repository.dart';
import '../../data/models/user_profile_model.dart';

// ─────────────────────────────────────────────────────────────────────────────
// Shared premium AppBar for profile sub-pages
// ─────────────────────────────────────────────────────────────────────────────
class _ProfileSubPageBar extends StatelessWidget implements PreferredSizeWidget {
  final String title;
  final String? subtitle;
  final List<Widget>? actions;

  const _ProfileSubPageBar({
    required this.title,
    this.subtitle,
    this.actions,
  });

  @override
  Size get preferredSize => Size.fromHeight(subtitle != null ? 64 : kToolbarHeight);

  @override
  Widget build(BuildContext context) {
    final palette = RolePalette.organization;
    return AppBar(
      backgroundColor: Colors.white,
      foregroundColor: AppDesignSystem.gray900,
      elevation: 0,
      scrolledUnderElevation: 0,
      systemOverlayStyle: SystemUiOverlayStyle.dark,
      leadingWidth: 52,
      leading: Padding(
        padding: const EdgeInsets.only(left: 8),
        child: IconButton(
          icon: Container(
            width: 36,
            height: 36,
            decoration: BoxDecoration(
              color: AppDesignSystem.gray100,
              borderRadius: BorderRadius.circular(11),
            ),
            child: const Icon(Icons.arrow_back_ios_new_rounded, size: 16),
          ),
          onPressed: () => Navigator.of(context).pop(),
        ),
      ),
      title: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        mainAxisSize: MainAxisSize.min,
        children: [
          Text(
            title,
            style: AppDesignSystem.font.copyWith(
              fontSize: 17,
              fontWeight: FontWeight.w800,
              color: AppDesignSystem.gray900,
              letterSpacing: -0.3,
            ),
          ),
          if (subtitle != null) ...[
            const SizedBox(height: 1),
            Text(
              subtitle!,
              style: AppDesignSystem.font.copyWith(
                fontSize: 11.5,
                fontWeight: FontWeight.w500,
                color: palette.primary,
              ),
            ),
          ],
        ],
      ),
      actions: actions,
      bottom: PreferredSize(
        preferredSize: const Size.fromHeight(1),
        child: Container(height: 0.5, color: AppDesignSystem.gray100),
      ),
    );
  }
}

// ─────────────────────────────────────────────────────────────────────────────
// Shared info row widget
// ─────────────────────────────────────────────────────────────────────────────
class _InfoRow extends StatelessWidget {
  final IconData icon;
  final Color iconColor;
  final String label;
  final String value;
  final bool isEmpty;

  const _InfoRow({
    required this.icon,
    required this.iconColor,
    required this.label,
    required this.value,
    this.isEmpty = false,
  });

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 13),
      child: Row(
        children: [
          Container(
            width: 40,
            height: 40,
            decoration: BoxDecoration(
              gradient: LinearGradient(
                begin: Alignment.topLeft,
                end: Alignment.bottomRight,
                colors: [
                  iconColor.withValues(alpha: 0.18),
                  iconColor.withValues(alpha: 0.07),
                ],
              ),
              borderRadius: BorderRadius.circular(12),
            ),
            child: Icon(icon, color: iconColor, size: 19),
          ),
          const SizedBox(width: 14),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  label,
                  style: AppDesignSystem.font.copyWith(
                    fontSize: 11,
                    fontWeight: FontWeight.w600,
                    color: AppDesignSystem.gray400,
                    letterSpacing: 0.2,
                  ),
                ),
                const SizedBox(height: 2),
                Text(
                  isEmpty ? 'Chưa cập nhật' : value,
                  style: AppDesignSystem.font.copyWith(
                    fontSize: 14.5,
                    fontWeight: FontWeight.w600,
                    color: isEmpty ? AppDesignSystem.gray400 : AppDesignSystem.gray900,
                    fontStyle: isEmpty ? FontStyle.italic : FontStyle.normal,
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


// ─────────────────────────────────────────────────────────────────────────────
// Shared section card
// ─────────────────────────────────────────────────────────────────────────────
class _SectionCard extends StatelessWidget {
  final String? title;
  final List<Widget> children;
  final Color? accent;

  const _SectionCard({this.title, required this.children, this.accent});

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        if (title != null)
          Padding(
            padding: const EdgeInsets.fromLTRB(4, 0, 0, 10),
            child: Row(
              children: [
                if (accent != null) ...[
                  Container(
                    width: 3,
                    height: 16,
                    decoration: BoxDecoration(
                      color: accent,
                      borderRadius: BorderRadius.circular(99),
                    ),
                  ),
                  const SizedBox(width: 8),
                ],
                Text(
                  title!,
                  style: AppDesignSystem.font.copyWith(
                    fontSize: 13,
                    fontWeight: FontWeight.w800,
                    color: AppDesignSystem.gray700,
                  ),
                ),
              ],
            ),
          ),
        Container(
          decoration: BoxDecoration(
            color: Colors.white,
            borderRadius: BorderRadius.circular(20),
            boxShadow: [
              BoxShadow(
                color: Colors.black.withValues(alpha: 0.05),
                blurRadius: 16,
                offset: const Offset(0, 5),
              ),
            ],
          ),
          child: ClipRRect(
            borderRadius: BorderRadius.circular(20),
            child: Column(
              children: [
                for (var i = 0; i < children.length; i++) ...[
                  if (i > 0)
                    Padding(
                      padding: const EdgeInsets.only(left: 70),
                      child: Divider(height: 0.5, thickness: 0.5, color: AppDesignSystem.gray100),
                    ),
                  children[i],
                ],
              ],
            ),
          ),
        ),
      ],
    );
  }
}

// ─────────────────────────────────────────────────────────────────────────────
// Personal Info Page
// ─────────────────────────────────────────────────────────────────────────────
class PersonalInfoPage extends StatefulWidget {
  const PersonalInfoPage({super.key});

  @override
  State<PersonalInfoPage> createState() => _PersonalInfoPageState();
}

class _PersonalInfoPageState extends State<PersonalInfoPage> {
  UserProfileModel? _profile;
  bool _isLoading = false;
  String? _errorMessage;
  bool _isEditing = false;
  bool _isSaving = false;

  final _formKey = GlobalKey<FormState>();
  final _nameCtrl = TextEditingController();
  final _phoneCtrl = TextEditingController();
  final _addressCtrl = TextEditingController();

  @override
  void initState() {
    super.initState();
    _loadProfile();
  }

  Future<void> _loadProfile() async {
    setState(() {
      _isLoading = true;
      _errorMessage = null;
    });
    try {
      final p = await ProfileRepository.instance.getProfile();
      setState(() {
        _profile = p;
        _isLoading = false;
        _initControllers(p);
      });
    } catch (e) {
      setState(() {
        _errorMessage = e.toString();
        _isLoading = false;
      });
    }
  }

  void _initControllers(UserProfileModel p) {
    _nameCtrl.text = p.fullName ?? '${p.firstName ?? ''} ${p.lastName ?? ''}'.trim();
    _phoneCtrl.text = p.phoneNumber ?? '';
    _addressCtrl.text = p.address ?? '';
  }

  @override
  void dispose() {
    _nameCtrl.dispose();
    _phoneCtrl.dispose();
    _addressCtrl.dispose();
    super.dispose();
  }

  String _fmtDate(DateTime? d) {
    if (d == null) return '';
    return '${d.day.toString().padLeft(2, '0')}/${d.month.toString().padLeft(2, '0')}/${d.year}';
  }

  Future<void> _saveProfile() async {
    if (!(_formKey.currentState?.validate() ?? false)) return;

    setState(() {
      _isSaving = true;
    });

    try {
      final fullNameVal = _nameCtrl.text.trim();
      final phoneVal = _phoneCtrl.text.trim();
      final addressVal = _addressCtrl.text.trim();

      final parts = fullNameVal.split(' ').where((p) => p.isNotEmpty).toList();
      String firstNameVal = '';
      String lastNameVal = '';
      if (parts.isNotEmpty) {
        if (parts.length == 1) {
          firstNameVal = parts[0];
          lastNameVal = '';
        } else {
          firstNameVal = parts.last;
          lastNameVal = parts.sublist(0, parts.length - 1).join(' ');
        }
      }

      final updatedProfile = await ProfileRepository.instance.updateProfile(
        firstName: firstNameVal,
        lastName: lastNameVal,
        phoneNumber: phoneVal,
        address: addressVal,
      );

      setState(() {
        _profile = updatedProfile;
        _isSaving = false;
        _isEditing = false;
        _initControllers(updatedProfile);
      });

      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Row(
              children: [
                const Icon(Icons.check_circle_rounded, color: Colors.white),
                const SizedBox(width: 10),
                const Text('Cập nhật hồ sơ thành công!'),
              ],
            ),
            backgroundColor: AppDesignSystem.success,
            behavior: SnackBarBehavior.floating,
            shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
          ),
        );
      }
    } catch (e) {
      setState(() {
        _isSaving = false;
      });
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Row(
              children: [
                const Icon(Icons.error_outline_rounded, color: Colors.white),
                const SizedBox(width: 10),
                Expanded(child: Text('Cập nhật thất bại: $e')),
              ],
            ),
            backgroundColor: AppDesignSystem.danger,
            behavior: SnackBarBehavior.floating,
            shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
          ),
        );
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final accent = RolePalette.organization.primary;
    
    return Scaffold(
      backgroundColor: const Color(0xFFF5F7FA),
      appBar: _ProfileSubPageBar(
        title: 'Thông tin cá nhân',
        subtitle: _isEditing ? 'Chỉnh sửa hồ sơ' : 'Xem và chỉnh sửa hồ sơ',
        actions: _profile == null
            ? null
            : [
                Padding(
                  padding: const EdgeInsets.only(right: 12),
                  child: _isEditing
                      ? TextButton(
                          onPressed: () {
                            setState(() {
                              _isEditing = false;
                              _initControllers(_profile!);
                            });
                          },
                          child: Text(
                            'Huỷ',
                            style: AppDesignSystem.font.copyWith(
                              fontSize: 14,
                              fontWeight: FontWeight.w700,
                              color: AppDesignSystem.gray500,
                            ),
                          ),
                        )
                      : IconButton(
                          icon: Container(
                            width: 36,
                            height: 36,
                            decoration: BoxDecoration(
                              color: accent.withValues(alpha: 0.12),
                              borderRadius: BorderRadius.circular(11),
                            ),
                            child: Icon(Icons.edit_rounded, color: accent, size: 18),
                          ),
                          onPressed: () {
                            setState(() {
                              _isEditing = true;
                            });
                          },
                        ),
                ),
              ],
      ),
      body: _buildBody(accent),
    );
  }

  Widget _buildBody(Color accent) {
    if (_isLoading) {
      return const Center(child: CircularProgressIndicator());
    }
    if (_errorMessage != null || _profile == null) {
      return Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Text(_errorMessage ?? 'Không tải được dữ liệu', style: AppDesignSystem.body()),
            const SizedBox(height: 16),
            ElevatedButton(
              onPressed: _loadProfile,
              child: const Text('Thử lại'),
            ),
          ],
        ),
      );
    }

    final p = _profile!;
    final isOrg = p.isOrganizationRole;

    if (_isEditing) {
      return Form(
        key: _formKey,
        child: ListView(
          padding: const EdgeInsets.fromLTRB(16, 20, 16, 32),
          children: [
            _CompletionBanner(
              percent: p.profileCompletionPercent,
              missing: p.missingProfileHints,
              accent: accent,
            ),
            const SizedBox(height: 16),
            _SectionCard(
              title: 'Chỉnh sửa thông tin',
              accent: accent,
              children: [
                Padding(
                  padding: const EdgeInsets.all(16),
                  child: Column(
                    children: [
                      _PremiumTextField(
                        label: 'Họ và tên',
                        hint: 'Nhập họ và tên',
                        icon: Icons.badge_outlined,
                        accent: accent,
                        controller: _nameCtrl,
                        validator: (v) =>
                            (v?.trim().isEmpty ?? true) ? 'Vui lòng nhập họ và tên' : null,
                      ),
                      const SizedBox(height: 16),
                      _PremiumTextField(
                        label: 'Số điện thoại',
                        hint: 'Nhập số điện thoại',
                        icon: Icons.phone_outlined,
                        accent: accent,
                        controller: _phoneCtrl,
                        validator: (v) {
                          final val = v?.trim() ?? '';
                          if (val.isEmpty) return 'Vui lòng nhập số điện thoại';
                          if (!RegExp(r'^\+?[0-9]{9,15}$').hasMatch(val)) {
                            return 'Số điện thoại không hợp lệ';
                          }
                          return null;
                        },
                      ),
                      const SizedBox(height: 16),
                      _PremiumTextField(
                        label: 'Địa chỉ',
                        hint: 'Nhập địa chỉ liên hệ',
                        icon: Icons.location_on_outlined,
                        accent: accent,
                        controller: _addressCtrl,
                      ),
                    ],
                  ),
                ),
              ],
            ),
            const SizedBox(height: 32),
            SizedBox(
              width: double.infinity,
              child: FilledButton(
                style: FilledButton.styleFrom(
                  backgroundColor: accent,
                  padding: const EdgeInsets.symmetric(vertical: 16),
                  shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(14)),
                ),
                onPressed: _isSaving ? null : _saveProfile,
                child: _isSaving
                    ? const SizedBox(
                        width: 20,
                        height: 20,
                        child: CircularProgressIndicator(strokeWidth: 2, color: Colors.white),
                      )
                    : const Text('Lưu thay đổi',
                        style: TextStyle(fontSize: 15, fontWeight: FontWeight.w700)),
              ),
            ),
          ],
        ),
      );
    }

    return ListView(
      padding: const EdgeInsets.fromLTRB(16, 20, 16, 32),
      children: [
        // Completion banner
        _CompletionBanner(
          percent: p.profileCompletionPercent,
          missing: p.missingProfileHints,
          accent: accent,
        ),
        if (p.missingProfileHints.isNotEmpty) const SizedBox(height: 16),

        // Personal section
        _SectionCard(
          title: 'Thông tin cá nhân',
          accent: accent,
          children: [
            _InfoRow(
              icon: Icons.badge_outlined,
              iconColor: AppDesignSystem.info,
              label: 'Họ và tên',
              value: p.displayName,
              isEmpty: !p.hasPersonalName,
            ),
            _InfoRow(
              icon: Icons.alternate_email_rounded,
              iconColor: accent,
              label: 'Tên đăng nhập',
              value: p.username,
            ),
            _InfoRow(
              icon: Icons.mail_outline_rounded,
              iconColor: AppDesignSystem.success,
              label: 'Email',
              value: p.isEmailVerified ? '${p.email}  ✓' : p.email,
              isEmpty: p.email.isEmpty,
            ),
            _InfoRow(
              icon: Icons.phone_outlined,
              iconColor: AppDesignSystem.warning,
              label: 'Số điện thoại',
              value: p.phoneNumber ?? '',
              isEmpty: !p.hasPhone,
            ),
            _InfoRow(
              icon: Icons.location_on_outlined,
              iconColor: const Color(0xFF3B82F6),
              label: 'Địa chỉ liên hệ',
              value: p.address ?? '',
              isEmpty: p.address?.trim().isNotEmpty != true,
            ),
            if (!isOrg)
              _InfoRow(
                icon: Icons.cake_outlined,
                iconColor: const Color(0xFFEC4899),
                label: 'Ngày sinh',
                value: _fmtDate(p.birthDate),
                isEmpty: p.birthDate == null,
              ),
          ],
        ),

        // Unit section for org
        if (isOrg && p.unit != null) ...[
          const SizedBox(height: 20),
          _SectionCard(
            title: 'Thông tin đơn vị',
            accent: accent,
            children: [
              _InfoRow(
                icon: Icons.business_outlined,
                iconColor: accent,
                label: 'Tên tổ chức',
                value: p.unit!.name,
                isEmpty: p.unit!.name.isEmpty,
              ),
              _InfoRow(
                icon: Icons.location_city_outlined,
                iconColor: AppDesignSystem.info,
                label: 'Địa chỉ',
                value: p.unit!.address ?? '',
                isEmpty: p.unit!.address?.isEmpty != false,
              ),
              _InfoRow(
                icon: Icons.phone_in_talk_outlined,
                iconColor: AppDesignSystem.success,
                label: 'Điện thoại đơn vị',
                value: p.unit!.phone ?? '',
                isEmpty: p.unit!.phone?.isEmpty != false,
              ),
              _InfoRow(
                icon: Icons.mail_outlined,
                iconColor: AppDesignSystem.warning,
                label: 'Email liên hệ',
                value: p.unit!.contactEmail ?? '',
                isEmpty: p.unit!.contactEmail?.isEmpty != false,
              ),
              _InfoRow(
                icon: Icons.person_pin_outlined,
                iconColor: RolePalette.manager.primary,
                label: 'Người đại diện',
                value: p.unit!.legalRepresentative ?? '',
                isEmpty: p.unit!.legalRepresentative?.isEmpty != false,
              ),
            ],
          ),
        ],

        const SizedBox(height: 24),
        // Edit hint card
        Container(
          padding: const EdgeInsets.all(16),
          decoration: BoxDecoration(
            gradient: LinearGradient(
              colors: [
                accent.withValues(alpha: 0.08),
                accent.withValues(alpha: 0.03),
              ],
            ),
            borderRadius: BorderRadius.circular(16),
            border: Border.all(color: accent.withValues(alpha: 0.15)),
          ),
          child: Row(
            children: [
              Icon(Icons.info_outline_rounded, color: accent, size: 20),
              const SizedBox(width: 12),
              Expanded(
                child: Text(
                  'Để chỉnh sửa thông tin cá nhân, vui lòng nhấn nút chỉnh sửa (biểu tượng cây bút) ở góc trên bên phải.',
                  style: AppDesignSystem.font.copyWith(
                    fontSize: 12.5,
                    color: accent,
                    fontWeight: FontWeight.w500,
                    height: 1.4,
                  ),
                ),
              ),
            ],
          ),
        ),
      ],
    );
  }
}

// ─────────────────────────────────────────────────────────────────────────────
// Completion Banner
// ─────────────────────────────────────────────────────────────────────────────
class _CompletionBanner extends StatelessWidget {
  final int percent;
  final List<String> missing;
  final Color accent;

  const _CompletionBanner({
    required this.percent,
    required this.missing,
    required this.accent,
  });

  @override
  Widget build(BuildContext context) {
    final isComplete = percent >= 100;
    final color = isComplete ? AppDesignSystem.success : accent;
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(20),
        boxShadow: [
          BoxShadow(
            color: color.withValues(alpha: 0.1),
            blurRadius: 16,
            offset: const Offset(0, 6),
          ),
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Container(
                width: 36,
                height: 36,
                decoration: BoxDecoration(
                  color: color.withValues(alpha: 0.12),
                  shape: BoxShape.circle,
                ),
                child: Icon(
                  isComplete ? Icons.verified_rounded : Icons.account_circle_outlined,
                  color: color,
                  size: 20,
                ),
              ),
              const SizedBox(width: 12),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      isComplete ? 'Hồ sơ hoàn chỉnh' : 'Hồ sơ $percent% hoàn thiện',
                      style: AppDesignSystem.font.copyWith(
                        fontSize: 14,
                        fontWeight: FontWeight.w800,
                        color: AppDesignSystem.gray900,
                      ),
                    ),
                    Text(
                      isComplete
                          ? 'Tất cả thông tin đã được cập nhật đầy đủ'
                          : 'Còn thiếu: ${missing.join(', ')}',
                      style: AppDesignSystem.font.copyWith(
                        fontSize: 12,
                        color: AppDesignSystem.gray500,
                        fontWeight: FontWeight.w500,
                      ),
                    ),
                  ],
                ),
              ),
              Text(
                '$percent%',
                style: AppDesignSystem.font.copyWith(
                  fontSize: 18,
                  fontWeight: FontWeight.w800,
                  color: color,
                ),
              ),
            ],
          ),
          const SizedBox(height: 12),
          ClipRRect(
            borderRadius: BorderRadius.circular(99),
            child: LinearProgressIndicator(
              value: percent / 100,
              minHeight: 6,
              backgroundColor: color.withValues(alpha: 0.12),
              valueColor: AlwaysStoppedAnimation<Color>(color),
            ),
          ),
        ],
      ),
    );
  }
}

// ─────────────────────────────────────────────────────────────────────────────
// Address Book Page
// ─────────────────────────────────────────────────────────────────────────────
class AddressBookPage extends StatefulWidget {
  const AddressBookPage({super.key});

  @override
  State<AddressBookPage> createState() => _AddressBookPageState();
}

class _AddressBookPageState extends State<AddressBookPage> {
  // Mock addresses – replace with real data source when available
  final List<_AddressEntry> _addresses = [
    const _AddressEntry(
      id: 1,
      label: 'Văn phòng chính',
      address: '227 Nguyễn Văn Cừ, Quận 5, TP.HCM',
      isPrimary: true,
      icon: Icons.business_rounded,
    ),
    const _AddressEntry(
      id: 2,
      label: 'Chi nhánh Q1',
      address: '12 Lê Thánh Tôn, Quận 1, TP.HCM',
      isPrimary: false,
      icon: Icons.location_city_rounded,
    ),
  ];

  final accent = RolePalette.organization.primary;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFFF5F7FA),
      appBar: _ProfileSubPageBar(
        title: 'Sổ địa chỉ',
        subtitle: 'Quản lý địa điểm giao nhận',
        actions: [
          Padding(
            padding: const EdgeInsets.only(right: 12),
            child: IconButton(
              icon: Container(
                width: 36,
                height: 36,
                decoration: BoxDecoration(
                  color: accent.withValues(alpha: 0.12),
                  borderRadius: BorderRadius.circular(11),
                ),
                child: Icon(Icons.add_rounded, color: accent, size: 20),
              ),
              onPressed: () => _showAddAddressSheet(context),
            ),
          ),
        ],
      ),
      body: _addresses.isEmpty
          ? _EmptyState(
              icon: Icons.location_off_outlined,
              title: 'Chưa có địa chỉ',
              subtitle: 'Thêm địa điểm giao nhận để sử dụng khi đặt suất',
              accent: accent,
              onAction: () => _showAddAddressSheet(context),
              actionLabel: 'Thêm địa chỉ',
            )
          : ListView(
              padding: const EdgeInsets.fromLTRB(16, 16, 16, 32),
              children: [
                for (final addr in _addresses) ...[
                  _AddressCard(
                    entry: addr,
                    accent: accent,
                    onDelete: () => setState(() => _addresses.remove(addr)),
                    onSetPrimary: () => setState(() {
                      for (final a in _addresses) {
                        (a as dynamic)._isPrimary = a.id == addr.id;
                      }
                    }),
                  ),
                  const SizedBox(height: 10),
                ],
                const SizedBox(height: 8),
                _AddAddressButton(
                  accent: accent,
                  onTap: () => _showAddAddressSheet(context),
                ),
              ],
            ),
    );
  }

  void _showAddAddressSheet(BuildContext context) {
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      backgroundColor: Colors.transparent,
      builder: (_) => _AddAddressSheet(accent: accent),
    );
  }
}

class _AddressEntry {
  final int id;
  final String label;
  final String address;
  final bool isPrimary;
  final IconData icon;

  const _AddressEntry({
    required this.id,
    required this.label,
    required this.address,
    required this.isPrimary,
    required this.icon,
  });
}

class _AddressCard extends StatefulWidget {
  final _AddressEntry entry;
  final Color accent;
  final VoidCallback onDelete;
  final VoidCallback onSetPrimary;

  const _AddressCard({
    required this.entry,
    required this.accent,
    required this.onDelete,
    required this.onSetPrimary,
  });

  @override
  State<_AddressCard> createState() => _AddressCardState();
}

class _AddressCardState extends State<_AddressCard> {
  bool _pressed = false;

  @override
  Widget build(BuildContext context) {
    final e = widget.entry;
    return GestureDetector(
      onTapDown: (_) => setState(() => _pressed = true),
      onTapUp: (_) => setState(() => _pressed = false),
      onTapCancel: () => setState(() => _pressed = false),
      child: AnimatedScale(
        scale: _pressed ? 0.98 : 1.0,
        duration: const Duration(milliseconds: 100),
        child: Container(
          decoration: BoxDecoration(
            color: Colors.white,
            borderRadius: BorderRadius.circular(20),
            border: e.isPrimary
                ? Border.all(color: widget.accent.withValues(alpha: 0.3))
                : null,
            boxShadow: [
              BoxShadow(
                color: Colors.black.withValues(alpha: 0.05),
                blurRadius: 14,
                offset: const Offset(0, 5),
              ),
            ],
          ),
          child: Padding(
            padding: const EdgeInsets.all(16),
            child: Row(
              children: [
                Container(
                  width: 48,
                  height: 48,
                  decoration: BoxDecoration(
                    gradient: LinearGradient(
                      colors: [
                        widget.accent.withValues(alpha: 0.18),
                        widget.accent.withValues(alpha: 0.07),
                      ],
                      begin: Alignment.topLeft,
                      end: Alignment.bottomRight,
                    ),
                    borderRadius: BorderRadius.circular(14),
                  ),
                  child: Icon(e.icon, color: widget.accent, size: 24),
                ),
                const SizedBox(width: 14),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Row(
                        children: [
                          Text(
                            e.label,
                            style: AppDesignSystem.font.copyWith(
                              fontSize: 14.5,
                              fontWeight: FontWeight.w800,
                              color: AppDesignSystem.gray900,
                            ),
                          ),
                          if (e.isPrimary) ...[
                            const SizedBox(width: 8),
                            Container(
                              padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
                              decoration: BoxDecoration(
                                color: widget.accent.withValues(alpha: 0.12),
                                borderRadius: BorderRadius.circular(999),
                              ),
                              child: Text(
                                'Mặc định',
                                style: AppDesignSystem.font.copyWith(
                                  fontSize: 10,
                                  fontWeight: FontWeight.w700,
                                  color: widget.accent,
                                ),
                              ),
                            ),
                          ],
                        ],
                      ),
                      const SizedBox(height: 4),
                      Text(
                        e.address,
                        style: AppDesignSystem.font.copyWith(
                          fontSize: 12.5,
                          color: AppDesignSystem.gray500,
                          fontWeight: FontWeight.w500,
                        ),
                        maxLines: 2,
                      ),
                    ],
                  ),
                ),
                PopupMenuButton<String>(
                  icon: const Icon(Icons.more_vert_rounded, size: 20, color: AppDesignSystem.gray400),
                  shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(14)),
                  itemBuilder: (_) => [
                    if (!e.isPrimary)
                      const PopupMenuItem(value: 'primary', child: Text('Đặt làm mặc định')),
                    const PopupMenuItem(value: 'delete', child: Text('Xoá địa chỉ')),
                  ],
                  onSelected: (v) {
                    if (v == 'primary') widget.onSetPrimary();
                    if (v == 'delete') widget.onDelete();
                  },
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}

class _AddAddressButton extends StatelessWidget {
  final Color accent;
  final VoidCallback onTap;

  const _AddAddressButton({required this.accent, required this.onTap});

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      child: Container(
        padding: const EdgeInsets.symmetric(vertical: 16),
        decoration: BoxDecoration(
          color: accent.withValues(alpha: 0.06),
          borderRadius: BorderRadius.circular(18),
          border: Border.all(
            color: accent.withValues(alpha: 0.2),
            width: 1,
            style: BorderStyle.solid,
          ),
        ),
        child: Row(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(Icons.add_location_alt_outlined, color: accent, size: 20),
            const SizedBox(width: 8),
            Text(
              'Thêm địa chỉ mới',
              style: AppDesignSystem.font.copyWith(
                fontSize: 14,
                fontWeight: FontWeight.w700,
                color: accent,
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _AddAddressSheet extends StatelessWidget {
  final Color accent;

  const _AddAddressSheet({required this.accent});

  @override
  Widget build(BuildContext context) {
    return Container(
      decoration: const BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.vertical(top: Radius.circular(28)),
      ),
      padding: EdgeInsets.fromLTRB(
        20, 16, 20, MediaQuery.viewInsetsOf(context).bottom + 24,
      ),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        crossAxisAlignment: CrossAxisAlignment.start,
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
            'Thêm địa chỉ mới',
            style: AppDesignSystem.font.copyWith(
              fontSize: 18,
              fontWeight: FontWeight.w800,
              color: AppDesignSystem.gray900,
            ),
          ),
          const SizedBox(height: 20),
          _PremiumTextField(label: 'Tên địa chỉ', hint: 'VD: Văn phòng chính', icon: Icons.label_outline_rounded, accent: accent),
          const SizedBox(height: 12),
          _PremiumTextField(label: 'Địa chỉ chi tiết', hint: 'Số nhà, đường, quận, thành phố', icon: Icons.location_on_outlined, accent: accent, maxLines: 2),
          const SizedBox(height: 20),
          SizedBox(
            width: double.infinity,
            child: FilledButton(
              style: FilledButton.styleFrom(
                backgroundColor: accent,
                padding: const EdgeInsets.symmetric(vertical: 16),
                shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(14)),
              ),
              onPressed: () => Navigator.pop(context),
              child: const Text('Lưu địa chỉ', style: TextStyle(fontSize: 15, fontWeight: FontWeight.w700)),
            ),
          ),
        ],
      ),
    );
  }
}

// ─────────────────────────────────────────────────────────────────────────────
// Security Page
// ─────────────────────────────────────────────────────────────────────────────
class SecurityPage extends StatefulWidget {
  const SecurityPage({super.key});

  @override
  State<SecurityPage> createState() => _SecurityPageState();
}

class _SecurityPageState extends State<SecurityPage> {
  final _oldPwCtrl = TextEditingController();
  final _newPwCtrl = TextEditingController();
  final _confirmPwCtrl = TextEditingController();
  bool _oldVisible = false;
  bool _newVisible = false;
  bool _confirmVisible = false;
  bool _loading = false;
  final _formKey = GlobalKey<FormState>();
  final accent = RolePalette.organization.primary;

  @override
  void dispose() {
    _oldPwCtrl.dispose();
    _newPwCtrl.dispose();
    _confirmPwCtrl.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    if (!(_formKey.currentState?.validate() ?? false)) return;
    setState(() => _loading = true);
    await Future.delayed(const Duration(seconds: 1));
    if (!mounted) return;
    setState(() => _loading = false);
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Row(
          children: [
            const Icon(Icons.check_circle_rounded, color: Colors.white),
            const SizedBox(width: 10),
            const Text('Đổi mật khẩu thành công!'),
          ],
        ),
        backgroundColor: AppDesignSystem.success,
        behavior: SnackBarBehavior.floating,
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      ),
    );
    _oldPwCtrl.clear();
    _newPwCtrl.clear();
    _confirmPwCtrl.clear();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFFF5F7FA),
      appBar: _ProfileSubPageBar(
        title: 'Bảo mật & Mật khẩu',
        subtitle: 'Cập nhật thông tin bảo mật',
      ),
      body: ListView(
        padding: const EdgeInsets.fromLTRB(16, 20, 16, 32),
        children: [
          // Security status card
          _SecurityStatusCard(accent: accent),
          const SizedBox(height: 20),

          // Change password
          _SectionCard(
            title: 'Đổi mật khẩu',
            accent: accent,
            children: [
              Padding(
                padding: const EdgeInsets.all(16),
                child: Form(
                  key: _formKey,
                  child: Column(
                    children: [
                      _PremiumTextField(
                        label: 'Mật khẩu hiện tại',
                        hint: '••••••••',
                        icon: Icons.lock_outline_rounded,
                        accent: accent,
                        controller: _oldPwCtrl,
                        obscure: !_oldVisible,
                        suffixIcon: IconButton(
                          icon: Icon(_oldVisible ? Icons.visibility_off_outlined : Icons.visibility_outlined, size: 20),
                          onPressed: () => setState(() => _oldVisible = !_oldVisible),
                        ),
                        validator: (v) => (v?.isEmpty ?? true) ? 'Nhập mật khẩu hiện tại' : null,
                      ),
                      const SizedBox(height: 12),
                      _PremiumTextField(
                        label: 'Mật khẩu mới',
                        hint: 'Tối thiểu 8 ký tự',
                        icon: Icons.lock_rounded,
                        accent: accent,
                        controller: _newPwCtrl,
                        obscure: !_newVisible,
                        suffixIcon: IconButton(
                          icon: Icon(_newVisible ? Icons.visibility_off_outlined : Icons.visibility_outlined, size: 20),
                          onPressed: () => setState(() => _newVisible = !_newVisible),
                        ),
                        validator: (v) {
                          if (v?.isEmpty ?? true) return 'Nhập mật khẩu mới';
                          if ((v?.length ?? 0) < 8) return 'Tối thiểu 8 ký tự';
                          return null;
                        },
                      ),
                      const SizedBox(height: 12),
                      _PremiumTextField(
                        label: 'Xác nhận mật khẩu mới',
                        hint: 'Nhập lại mật khẩu mới',
                        icon: Icons.lock_clock_outlined,
                        accent: accent,
                        controller: _confirmPwCtrl,
                        obscure: !_confirmVisible,
                        suffixIcon: IconButton(
                          icon: Icon(_confirmVisible ? Icons.visibility_off_outlined : Icons.visibility_outlined, size: 20),
                          onPressed: () => setState(() => _confirmVisible = !_confirmVisible),
                        ),
                        validator: (v) {
                          if (v != _newPwCtrl.text) return 'Mật khẩu không khớp';
                          return null;
                        },
                      ),
                      const SizedBox(height: 20),
                      SizedBox(
                        width: double.infinity,
                        child: FilledButton(
                          style: FilledButton.styleFrom(
                            backgroundColor: accent,
                            padding: const EdgeInsets.symmetric(vertical: 16),
                            shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(14)),
                          ),
                          onPressed: _loading ? null : _submit,
                          child: _loading
                              ? const SizedBox(
                                  width: 20,
                                  height: 20,
                                  child: CircularProgressIndicator(strokeWidth: 2, color: Colors.white),
                                )
                              : const Text('Cập nhật mật khẩu', style: TextStyle(fontSize: 15, fontWeight: FontWeight.w700)),
                        ),
                      ),
                    ],
                  ),
                ),
              ),
            ],
          ),

          const SizedBox(height: 20),
          // Security tips
          _SectionCard(
            title: 'Mẹo bảo mật',
            accent: AppDesignSystem.warning,
            children: [
              _SecurityTip(icon: Icons.check_circle_outline_rounded, color: AppDesignSystem.success, text: 'Sử dụng mật khẩu ít nhất 8 ký tự, kết hợp chữ và số'),
              _SecurityTip(icon: Icons.check_circle_outline_rounded, color: AppDesignSystem.success, text: 'Không dùng thông tin cá nhân làm mật khẩu'),
              _SecurityTip(icon: Icons.check_circle_outline_rounded, color: AppDesignSystem.success, text: 'Đổi mật khẩu định kỳ mỗi 3-6 tháng'),
            ],
          ),
        ],
      ),
    );
  }
}

class _SecurityStatusCard extends StatelessWidget {
  final Color accent;

  const _SecurityStatusCard({required this.accent});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(18),
      decoration: BoxDecoration(
        gradient: LinearGradient(
          colors: [accent.withValues(alpha: 0.12), accent.withValues(alpha: 0.04)],
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
        ),
        borderRadius: BorderRadius.circular(20),
        border: Border.all(color: accent.withValues(alpha: 0.2)),
      ),
      child: Row(
        children: [
          Container(
            width: 52,
            height: 52,
            decoration: BoxDecoration(
              color: accent.withValues(alpha: 0.15),
              shape: BoxShape.circle,
            ),
            child: Icon(Icons.shield_outlined, color: accent, size: 28),
          ),
          const SizedBox(width: 16),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text('Tài khoản được bảo vệ',
                    style: AppDesignSystem.font.copyWith(fontSize: 15, fontWeight: FontWeight.w800, color: AppDesignSystem.gray900)),
                const SizedBox(height: 3),
                Text('Cập nhật mật khẩu để tăng tính an toàn',
                    style: AppDesignSystem.font.copyWith(fontSize: 12.5, color: AppDesignSystem.gray500)),
              ],
            ),
          ),
          Container(
            padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 5),
            decoration: BoxDecoration(
              color: AppDesignSystem.success.withValues(alpha: 0.12),
              borderRadius: BorderRadius.circular(999),
            ),
            child: Text('An toàn',
                style: AppDesignSystem.font.copyWith(fontSize: 11, fontWeight: FontWeight.w700, color: AppDesignSystem.success)),
          ),
        ],
      ),
    );
  }
}

class _SecurityTip extends StatelessWidget {
  final IconData icon;
  final Color color;
  final String text;

  const _SecurityTip({required this.icon, required this.color, required this.text});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 11),
      child: Row(
        children: [
          Icon(icon, color: color, size: 18),
          const SizedBox(width: 12),
          Expanded(
            child: Text(text,
                style: AppDesignSystem.font.copyWith(fontSize: 13, color: AppDesignSystem.gray700, fontWeight: FontWeight.w500)),
          ),
        ],
      ),
    );
  }
}

// ─────────────────────────────────────────────────────────────────────────────
// Notification Settings Page
// ─────────────────────────────────────────────────────────────────────────────
class NotificationSettingsPage extends StatefulWidget {
  const NotificationSettingsPage({super.key});

  @override
  State<NotificationSettingsPage> createState() => _NotificationSettingsPageState();
}

class _NotificationSettingsPageState extends State<NotificationSettingsPage> {
  final accent = RolePalette.organization.primary;

  bool _pushOrders = true;
  bool _pushContracts = true;
  bool _pushPayments = true;
  bool _pushReviews = false;
  bool _emailDigest = true;
  bool _emailPromotions = false;
  bool _soundEnabled = true;
  bool _vibrationEnabled = true;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFFF5F7FA),
      appBar: _ProfileSubPageBar(
        title: 'Cài đặt thông báo',
        subtitle: 'Quản lý kênh và tần suất nhận tin',
      ),
      body: ListView(
        padding: const EdgeInsets.fromLTRB(16, 20, 16, 32),
        children: [
          _SectionCard(
            title: 'Thông báo đẩy (Push)',
            accent: accent,
            children: [
              _ToggleTile(
                icon: Icons.restaurant_rounded,
                iconColor: accent,
                title: 'Đơn đặt suất ăn',
                subtitle: 'Xác nhận, cập nhật trạng thái đơn hàng',
                value: _pushOrders,
                onChanged: (v) => setState(() => _pushOrders = v),
              ),
              _ToggleTile(
                icon: Icons.description_rounded,
                iconColor: AppDesignSystem.info,
                title: 'Hợp đồng',
                subtitle: 'Ký kết, gia hạn, hết hạn hợp đồng',
                value: _pushContracts,
                onChanged: (v) => setState(() => _pushContracts = v),
              ),
              _ToggleTile(
                icon: Icons.payments_rounded,
                iconColor: AppDesignSystem.warning,
                title: 'Thanh toán',
                subtitle: 'Nhắc nhở kỳ thanh toán sắp đến hạn',
                value: _pushPayments,
                onChanged: (v) => setState(() => _pushPayments = v),
              ),
              _ToggleTile(
                icon: Icons.star_rounded,
                iconColor: const Color(0xFFEC4899),
                title: 'Đánh giá',
                subtitle: 'Phản hồi và đánh giá suất ăn',
                value: _pushReviews,
                onChanged: (v) => setState(() => _pushReviews = v),
              ),
            ],
          ),
          const SizedBox(height: 20),
          _SectionCard(
            title: 'Email',
            accent: AppDesignSystem.info,
            children: [
              _ToggleTile(
                icon: Icons.summarize_outlined,
                iconColor: AppDesignSystem.info,
                title: 'Báo cáo định kỳ',
                subtitle: 'Tóm tắt hoạt động hàng tuần qua email',
                value: _emailDigest,
                onChanged: (v) => setState(() => _emailDigest = v),
              ),
              _ToggleTile(
                icon: Icons.campaign_outlined,
                iconColor: AppDesignSystem.success,
                title: 'Khuyến mãi & tin tức',
                subtitle: 'Ưu đãi mới từ HUITMeal',
                value: _emailPromotions,
                onChanged: (v) => setState(() => _emailPromotions = v),
              ),
            ],
          ),
          const SizedBox(height: 20),
          _SectionCard(
            title: 'Âm thanh & Rung',
            accent: AppDesignSystem.gray700,
            children: [
              _ToggleTile(
                icon: Icons.volume_up_rounded,
                iconColor: AppDesignSystem.gray700,
                title: 'Âm thanh thông báo',
                subtitle: 'Phát âm thanh khi có thông báo mới',
                value: _soundEnabled,
                onChanged: (v) => setState(() => _soundEnabled = v),
              ),
              _ToggleTile(
                icon: Icons.vibration_rounded,
                iconColor: AppDesignSystem.gray500,
                title: 'Rung',
                subtitle: 'Rung thiết bị khi nhận thông báo',
                value: _vibrationEnabled,
                onChanged: (v) => setState(() => _vibrationEnabled = v),
              ),
            ],
          ),
        ],
      ),
    );
  }
}

class _ToggleTile extends StatelessWidget {
  final IconData icon;
  final Color iconColor;
  final String title;
  final String subtitle;
  final bool value;
  final ValueChanged<bool> onChanged;

  const _ToggleTile({
    required this.icon,
    required this.iconColor,
    required this.title,
    required this.subtitle,
    required this.value,
    required this.onChanged,
  });

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 10),
      child: Row(
        children: [
          Container(
            width: 42,
            height: 42,
            decoration: BoxDecoration(
              gradient: LinearGradient(
                colors: [
                  iconColor.withValues(alpha: 0.18),
                  iconColor.withValues(alpha: 0.07),
                ],
                begin: Alignment.topLeft,
                end: Alignment.bottomRight,
              ),
              borderRadius: BorderRadius.circular(13),
            ),
            child: Icon(icon, color: iconColor, size: 20),
          ),
          const SizedBox(width: 14),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(title,
                    style: AppDesignSystem.font.copyWith(
                      fontSize: 14,
                      fontWeight: FontWeight.w700,
                      color: AppDesignSystem.gray900,
                    )),
                const SizedBox(height: 2),
                Text(subtitle,
                    style: AppDesignSystem.font.copyWith(
                      fontSize: 12,
                      color: AppDesignSystem.gray400,
                    )),
              ],
            ),
          ),
          Switch.adaptive(
            value: value,
            onChanged: onChanged,
            activeColor: RolePalette.organization.primary,
          ),
        ],
      ),
    );
  }
}

// ─────────────────────────────────────────────────────────────────────────────
// Help Center Page
// ─────────────────────────────────────────────────────────────────────────────
class HelpCenterPage extends StatefulWidget {
  const HelpCenterPage({super.key});

  @override
  State<HelpCenterPage> createState() => _HelpCenterPageState();
}

class _HelpCenterPageState extends State<HelpCenterPage> {
  final accent = RolePalette.organization.primary;
  int? _expandedIndex;

  static const _faqs = [
    _FaqItem('Làm sao để đặt suất ăn theo hợp đồng?', 'Vào tab "Đặt suất" → chọn "Đặt theo kỳ hợp đồng" → chọn menu và số lượng → xác nhận. Đơn sẽ được tự động lên lịch theo kỳ hợp đồng của bạn.'),
    _FaqItem('Tôi có thể thay đổi đơn đặt suất không?', 'Bạn có thể chỉnh sửa đơn trước thời hạn chốt (thường là 24h trước giờ giao). Vào mục "Đơn đặt suất" → chọn đơn cần sửa → nhấn "Chỉnh sửa".'),
    _FaqItem('Hóa đơn và thanh toán được xử lý như thế nào?', 'Hóa đơn được tổng hợp theo kỳ hợp đồng (tuần/tháng). Bạn có thể thanh toán qua cổng PayOS hoặc chuyển khoản trực tiếp. Vào "Hợp đồng" → "Thanh toán" để theo dõi.'),
    _FaqItem('Làm sao để xem báo cáo sử dụng suất ăn?', 'Vào tab "Tổng quan" → nhấn "Thống kê suất ăn". Bạn có thể lọc theo ngày, ca, hoặc kỳ hợp đồng và xuất báo cáo PDF.'),
    _FaqItem('Tôi cần liên hệ ai nếu có vấn đề với chất lượng suất ăn?', 'Sử dụng tính năng "Đánh giá suất ăn" ngay sau khi nhận hàng, hoặc liên hệ trực tiếp qua Chatbot CSKH trong mục "Hỗ trợ CSKH".'),
  ];

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFFF5F7FA),
      appBar: _ProfileSubPageBar(
        title: 'Trung tâm trợ giúp',
        subtitle: 'Câu hỏi thường gặp & hỗ trợ',
      ),
      body: ListView(
        padding: const EdgeInsets.fromLTRB(16, 20, 16, 32),
        children: [
          // Contact card
          _ContactSupportCard(accent: accent),
          const SizedBox(height: 20),

          // FAQ
          Padding(
            padding: const EdgeInsets.only(bottom: 10, left: 4),
            child: Row(
              children: [
                Container(
                  width: 3, height: 16,
                  decoration: BoxDecoration(color: accent, borderRadius: BorderRadius.circular(99)),
                ),
                const SizedBox(width: 8),
                Text('Câu hỏi thường gặp',
                    style: AppDesignSystem.font.copyWith(
                      fontSize: 13, fontWeight: FontWeight.w800, color: AppDesignSystem.gray700)),
              ],
            ),
          ),
          Container(
            decoration: BoxDecoration(
              color: Colors.white,
              borderRadius: BorderRadius.circular(20),
              boxShadow: [BoxShadow(color: Colors.black.withValues(alpha: 0.05), blurRadius: 14, offset: const Offset(0, 5))],
            ),
            child: ClipRRect(
              borderRadius: BorderRadius.circular(20),
              child: Column(
                children: [
                  for (var i = 0; i < _faqs.length; i++) ...[
                    if (i > 0) Divider(height: 0.5, thickness: 0.5, color: AppDesignSystem.gray100),
                    _FaqTile(
                      faq: _faqs[i],
                      expanded: _expandedIndex == i,
                      accent: accent,
                      onTap: () => setState(() => _expandedIndex = _expandedIndex == i ? null : i),
                    ),
                  ],
                ],
              ),
            ),
          ),

          const SizedBox(height: 20),
          // App version
          Center(
            child: Text(
              'HUITMeal v1.0.0  •  Hỗ trợ: support@huitmeal.vn',
              style: AppDesignSystem.font.copyWith(
                fontSize: 11.5,
                color: AppDesignSystem.gray400,
                fontWeight: FontWeight.w500,
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class _FaqItem {
  final String question;
  final String answer;
  const _FaqItem(this.question, this.answer);
}

class _FaqTile extends StatelessWidget {
  final _FaqItem faq;
  final bool expanded;
  final Color accent;
  final VoidCallback onTap;

  const _FaqTile({
    required this.faq,
    required this.expanded,
    required this.accent,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return InkWell(
      onTap: onTap,
      child: Padding(
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Expanded(
                  child: Text(
                    faq.question,
                    style: AppDesignSystem.font.copyWith(
                      fontSize: 13.5,
                      fontWeight: FontWeight.w700,
                      color: expanded ? accent : AppDesignSystem.gray900,
                    ),
                  ),
                ),
                const SizedBox(width: 8),
                AnimatedRotation(
                  turns: expanded ? 0.5 : 0,
                  duration: const Duration(milliseconds: 200),
                  child: Icon(Icons.keyboard_arrow_down_rounded,
                      color: expanded ? accent : AppDesignSystem.gray400, size: 22),
                ),
              ],
            ),
            AnimatedCrossFade(
              firstChild: const SizedBox.shrink(),
              secondChild: Padding(
                padding: const EdgeInsets.only(top: 10),
                child: Text(
                  faq.answer,
                  style: AppDesignSystem.font.copyWith(
                    fontSize: 13,
                    color: AppDesignSystem.gray500,
                    fontWeight: FontWeight.w500,
                    height: 1.5,
                  ),
                ),
              ),
              crossFadeState: expanded ? CrossFadeState.showSecond : CrossFadeState.showFirst,
              duration: const Duration(milliseconds: 200),
            ),
          ],
        ),
      ),
    );
  }
}

class _ContactSupportCard extends StatelessWidget {
  final Color accent;

  const _ContactSupportCard({required this.accent});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(18),
      decoration: BoxDecoration(
        gradient: LinearGradient(
          colors: [accent, accent.withValues(alpha: 0.75)],
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
        ),
        borderRadius: BorderRadius.circular(22),
        boxShadow: [
          BoxShadow(color: accent.withValues(alpha: 0.3), blurRadius: 20, offset: const Offset(0, 8)),
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Container(
                padding: const EdgeInsets.all(10),
                decoration: BoxDecoration(
                  color: Colors.white.withValues(alpha: 0.2),
                  borderRadius: BorderRadius.circular(14),
                ),
                child: const Icon(Icons.support_agent_rounded, color: Colors.white, size: 26),
              ),
              const SizedBox(width: 14),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text('Cần hỗ trợ thêm?',
                        style: AppDesignSystem.font.copyWith(
                          fontSize: 16, fontWeight: FontWeight.w800, color: Colors.white)),
                    Text('Đội ngũ CSKH sẵn sàng hỗ trợ bạn',
                        style: AppDesignSystem.font.copyWith(
                          fontSize: 12.5, color: Colors.white.withValues(alpha: 0.85))),
                  ],
                ),
              ),
            ],
          ),
          const SizedBox(height: 16),
          Row(
            children: [
              Expanded(
                child: _ContactButton(
                  icon: Icons.chat_bubble_outline_rounded,
                  label: 'Chat hỗ trợ',
                  onTap: () {},
                ),
              ),
              const SizedBox(width: 10),
              Expanded(
                child: _ContactButton(
                  icon: Icons.mail_outline_rounded,
                  label: 'Gửi email',
                  onTap: () {},
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }
}

class _ContactButton extends StatelessWidget {
  final IconData icon;
  final String label;
  final VoidCallback onTap;

  const _ContactButton({required this.icon, required this.label, required this.onTap});

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      child: Container(
        padding: const EdgeInsets.symmetric(vertical: 12),
        decoration: BoxDecoration(
          color: Colors.white.withValues(alpha: 0.2),
          borderRadius: BorderRadius.circular(14),
          border: Border.all(color: Colors.white.withValues(alpha: 0.3)),
        ),
        child: Row(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(icon, color: Colors.white, size: 18),
            const SizedBox(width: 6),
            Text(label,
                style: AppDesignSystem.font.copyWith(
                  fontSize: 13, fontWeight: FontWeight.w700, color: Colors.white)),
          ],
        ),
      ),
    );
  }
}

// ─────────────────────────────────────────────────────────────────────────────
// Shared: Premium Text Field
// ─────────────────────────────────────────────────────────────────────────────
class _PremiumTextField extends StatelessWidget {
  final String label;
  final String hint;
  final IconData icon;
  final Color accent;
  final TextEditingController? controller;
  final bool obscure;
  final Widget? suffixIcon;
  final int maxLines;
  final String? Function(String?)? validator;

  const _PremiumTextField({
    required this.label,
    required this.hint,
    required this.icon,
    required this.accent,
    this.controller,
    this.obscure = false,
    this.suffixIcon,
    this.maxLines = 1,
    this.validator,
  });

  @override
  Widget build(BuildContext context) {
    return TextFormField(
      controller: controller,
      obscureText: obscure,
      maxLines: maxLines,
      validator: validator,
      style: AppDesignSystem.font.copyWith(fontSize: 14.5, fontWeight: FontWeight.w600, color: AppDesignSystem.gray900),
      decoration: InputDecoration(
        labelText: label,
        hintText: hint,
        prefixIcon: Icon(icon, color: accent, size: 20),
        suffixIcon: suffixIcon,
        labelStyle: AppDesignSystem.font.copyWith(fontSize: 13, color: AppDesignSystem.gray500, fontWeight: FontWeight.w600),
        hintStyle: AppDesignSystem.font.copyWith(fontSize: 13.5, color: AppDesignSystem.gray400),
        filled: true,
        fillColor: const Color(0xFFF8FAFB),
        contentPadding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(14),
          borderSide: const BorderSide(color: AppDesignSystem.gray200, width: 1),
        ),
        enabledBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(14),
          borderSide: const BorderSide(color: AppDesignSystem.gray200, width: 1),
        ),
        focusedBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(14),
          borderSide: BorderSide(color: accent, width: 1.5),
        ),
        errorBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(14),
          borderSide: const BorderSide(color: AppDesignSystem.danger, width: 1),
        ),
        focusedErrorBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(14),
          borderSide: const BorderSide(color: AppDesignSystem.danger, width: 1.5),
        ),
      ),
    );
  }
}

// ─────────────────────────────────────────────────────────────────────────────
// Shared: Empty State
// ─────────────────────────────────────────────────────────────────────────────
class _EmptyState extends StatelessWidget {
  final IconData icon;
  final String title;
  final String subtitle;
  final Color accent;
  final VoidCallback? onAction;
  final String? actionLabel;

  const _EmptyState({
    required this.icon,
    required this.title,
    required this.subtitle,
    required this.accent,
    this.onAction,
    this.actionLabel,
  });

  @override
  Widget build(BuildContext context) {
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(32),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Container(
              padding: const EdgeInsets.all(24),
              decoration: BoxDecoration(
                color: accent.withValues(alpha: 0.08),
                shape: BoxShape.circle,
              ),
              child: Icon(icon, size: 48, color: accent),
            ),
            const SizedBox(height: 20),
            Text(title,
                style: AppDesignSystem.font.copyWith(
                  fontSize: 18, fontWeight: FontWeight.w800, color: AppDesignSystem.gray900)),
            const SizedBox(height: 8),
            Text(subtitle,
                textAlign: TextAlign.center,
                style: AppDesignSystem.font.copyWith(fontSize: 13.5, color: AppDesignSystem.gray400)),
            if (onAction != null && actionLabel != null) ...[
              const SizedBox(height: 24),
              FilledButton.icon(
                style: FilledButton.styleFrom(
                  backgroundColor: accent,
                  padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 14),
                  shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(14)),
                ),
                onPressed: onAction,
                icon: const Icon(Icons.add_rounded),
                label: Text(actionLabel!, style: const TextStyle(fontWeight: FontWeight.w700)),
              ),
            ],
          ],
        ),
      ),
    );
  }
}
