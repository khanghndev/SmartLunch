import 'package:flutter/material.dart';

import '../../../../core/theme/app_design_system.dart';
import '../../../profile/data/profile_repository.dart';
import '../../../profile/data/models/user_profile_model.dart';
import '../widgets/organization_ui.dart';

/// Thông tin đơn vị & nhân sự (API danh sách nhân viên chưa mở cho role Organization).
class StaffPage extends StatefulWidget {
  const StaffPage({super.key});

  @override
  State<StaffPage> createState() => _StaffPageState();
}

class _StaffPageState extends State<StaffPage> {
  UserProfileModel? _profile;
  bool _loading = true;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    setState(() => _loading = true);
    try {
      final p = await ProfileRepository.instance.getProfile();
      if (!mounted) return;
      setState(() {
        _profile = p;
        _loading = false;
      });
    } catch (_) {
      if (mounted) setState(() => _loading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return OrgPageShell(
      title: 'Nhân sự đơn vị',
      onRefresh: _load,
      body: _loading
          ? const OrgLoadingBody()
          : ListView(
              padding: const EdgeInsets.all(16),
              children: [
                OrgCard(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text('Đơn vị được gán', style: AppDesignSystem.sectionTitle()),
                      const SizedBox(height: 12),
                      if (_profile?.unit != null) ...[
                        _row(Icons.business_rounded, 'Tên đơn vị', _profile!.unit!.name),
                        _row(Icons.tag_rounded, 'Mã đơn vị', '${_profile!.unit!.id}'),
                      ] else
                        Text(
                          'Tài khoản chưa gán đơn vị. Liên hệ quản trị hệ thống.',
                          style: AppDesignSystem.body(color: AppDesignSystem.danger),
                        ),
                    ],
                  ),
                ),
                const SizedBox(height: 12),
                OrgCard(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text('Liên hệ đại diện', style: AppDesignSystem.sectionTitle()),
                      const SizedBox(height: 12),
                      _row(Icons.person_rounded, 'Họ tên', _profile?.displayName ?? '—'),
                      _row(Icons.email_outlined, 'Email', _profile?.email ?? '—'),
                      _row(Icons.phone_outlined, 'Điện thoại', _profile?.phoneNumber ?? '—'),
                    ],
                  ),
                ),
                const SizedBox(height: 12),
                Container(
                  padding: const EdgeInsets.all(14),
                  decoration: BoxDecoration(
                    color: AppDesignSystem.warning.withValues(alpha: 0.1),
                    borderRadius: BorderRadius.circular(12),
                  ),
                  child: Row(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Icon(Icons.info_outline, color: AppDesignSystem.warning),
                      const SizedBox(width: 10),
                      Expanded(
                        child: Text(
                          'Danh sách nhân viên chi tiết theo đơn vị hiện chỉ quản trị trên web. '
                          'Mobile hiển thị thông tin đơn vị từ hồ sơ đăng nhập.',
                          style: AppDesignSystem.body(size: 12),
                        ),
                      ),
                    ],
                  ),
                ),
              ],
            ),
    );
  }

  Widget _row(IconData icon, String label, String value) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 10),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Icon(icon, size: 20, color: orgAccent),
          const SizedBox(width: 10),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(label, style: AppDesignSystem.body(size: 12)),
                Text(value, style: AppDesignSystem.label()),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
