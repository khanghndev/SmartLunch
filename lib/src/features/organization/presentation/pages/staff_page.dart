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
  String? _error;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    setState(() {
      _loading = true;
      _error = null;
    });
    try {
      final p = await ProfileRepository.instance.getProfile();
      if (!mounted) return;
      setState(() {
        _profile = p;
        _loading = false;
      });
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _error = orgApiError(e);
        _loading = false;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return OrgPageShell(
      title: 'Nhân sự đơn vị',
      onRefresh: _load,
      body: _loading
          ? const OrgLoadingBody()
          : _error != null
              ? OrgErrorBody(message: _error!, onRetry: _load)
              : ModuleListView(
                  padding: orgListPadding(context),
                  children: [
                    const OrgPageIntro(
                      title: 'Nhân sự & đơn vị',
                      description:
                          'Thông tin đơn vị và đại diện từ hồ sơ đăng nhập. Danh sách nhân viên chi tiết quản trị trên web.',
                      icon: Icons.groups_rounded,
                    ),
                    const SizedBox(height: 14),
                    OrgCard(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          const OrgSectionHeader(title: 'Đơn vị được gán'),
                          const SizedBox(height: 8),
                          if (_profile?.unit != null) ...[
                            OrgDetailField(
                              icon: Icons.business_rounded,
                              label: 'Tên đơn vị',
                              value: _profile!.unit!.name,
                            ),
                            OrgDetailField(
                              icon: Icons.tag_rounded,
                              label: 'Mã đơn vị',
                              value: '${_profile!.unit!.id}',
                            ),
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
                          const OrgSectionHeader(title: 'Liên hệ đại diện'),
                          const SizedBox(height: 8),
                          OrgDetailField(
                            icon: Icons.person_rounded,
                            label: 'Họ tên',
                            value: _profile?.displayName ?? '—',
                          ),
                          OrgDetailField(
                            icon: Icons.email_outlined,
                            label: 'Email',
                            value: _profile?.email ?? '—',
                          ),
                          OrgDetailField(
                            icon: Icons.phone_outlined,
                            label: 'Điện thoại',
                            value: _profile?.phoneNumber ?? '—',
                          ),
                        ],
                      ),
                    ),
                    const SizedBox(height: 12),
                    const OrgInfoBanner(
                      message:
                          'Danh sách nhân viên chi tiết theo đơn vị hiện chỉ quản trị trên web. '
                          'Mobile hiển thị thông tin đơn vị từ hồ sơ đăng nhập.',
                    ),
                  ],
                ),
    );
  }
}
