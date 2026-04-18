import 'package:flutter/material.dart';

import '../../../../app/app_routes.dart';
import '../../../../core/constants/app_colors.dart';

class ProfilePage extends StatelessWidget {
  final bool embedded;
  final double bottomInset;

  const ProfilePage({super.key, this.embedded = false, this.bottomInset = 0});

  @override
  Widget build(BuildContext context) {
    final accountItems = [
      const _SettingItem(
        icon: Icons.badge_outlined,
        title: 'Thông tin cá nhân',
        subtitle: 'Tên, email, số điện thoại',
        route: AppRoutes.profilePersonalInfo,
      ),
      const _SettingItem(
        icon: Icons.lock_outline,
        title: 'Bảo mật & mật khẩu',
        subtitle: 'Đổi mật khẩu, bật bảo mật 2 lớp',
        route: AppRoutes.profileSecurity,
      ),
      const _SettingItem(
        icon: Icons.location_on_outlined,
        title: 'Địa chỉ & nơi giao',
        subtitle: 'Thêm địa chỉ giao hàng mặc định',
        route: AppRoutes.profileAddresses,
      ),
      const _SettingItem(
        icon: Icons.notifications_outlined,
        title: 'Thông báo',
        subtitle: 'Chọn loại thông báo muốn nhận',
        route: AppRoutes.profileNotifications,
      ),
    ];

    final supportItems = [
      const _SettingItem(
        icon: Icons.help_outline,
        title: 'Trung tâm trợ giúp',
        subtitle: 'Câu hỏi thường gặp & hướng dẫn',
        route: AppRoutes.profileHelp,
      ),
      const _SettingItem(
        icon: Icons.chat_bubble_outline,
        title: 'Chat với SmartLunch',
        subtitle: 'Trao đổi trực tiếp với CSKH',
        route: AppRoutes.chatbot,
      ),
      _SettingItem(
        icon: Icons.logout,
        title: 'Đăng xuất',
        subtitle: 'Thoát tài khoản hiện tại',
        danger: true,
        onTap: (context) => _showLogoutDialog(context),
      ),
    ];

    final content = SingleChildScrollView(
      physics: const BouncingScrollPhysics(),
      padding: EdgeInsets.fromLTRB(20, embedded ? 6 : 18, 20, 28 + bottomInset),
      child: Column(
        children: [
          const _ProfileHeader(),
          const SizedBox(height: 18),
          _CardSection(title: 'Tài khoản', children: accountItems),
          const SizedBox(height: 16),
          _CardSection(title: 'Hỗ trợ', children: supportItems),
        ],
      ),
    );

    if (embedded) {
      return content;
    }

    return Scaffold(
      backgroundColor: const Color(0xFFF5F7FA),
      appBar: AppBar(
        title: const Text('Hồ sơ'),
        centerTitle: true,
        backgroundColor: Colors.white,
        foregroundColor: AppColors.ink,
        elevation: 0,
        surfaceTintColor: Colors.white,
      ),
      body: content,
    );
  }
}

class _ProfileHeader extends StatelessWidget {
  const _ProfileHeader();

  @override
  Widget build(BuildContext context) {
    return LayoutBuilder(
      builder: (context, _) {
        return Container(
          width: double.infinity,
          padding: const EdgeInsets.all(16),
          decoration: BoxDecoration(
            gradient: const LinearGradient(
              colors: [AppColors.customer, AppColors.customerAlt],
              begin: Alignment.topLeft,
              end: Alignment.bottomRight,
            ),
            borderRadius: BorderRadius.circular(18),
            boxShadow: [
              BoxShadow(
                color: AppColors.customer.withOpacity(0.25),
                blurRadius: 16,
                offset: const Offset(0, 10),
              ),
            ],
          ),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                children: [
                  Container(
                    width: 58,
                    height: 58,
                    decoration: BoxDecoration(
                      color: Colors.white,
                      shape: BoxShape.circle,
                      boxShadow: [
                        BoxShadow(
                          color: Colors.black.withOpacity(0.1),
                          blurRadius: 8,
                          offset: const Offset(0, 4),
                        ),
                      ],
                    ),
                    child: const Icon(
                      Icons.person_rounded,
                      color: AppColors.customer,
                      size: 32,
                    ),
                  ),
                  const SizedBox(width: 14),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          'Nguyễn Văn A',
                          maxLines: 1,
                          overflow: TextOverflow.ellipsis,
                          style: Theme.of(
                            context,
                          ).textTheme.titleLarge?.copyWith(
                            color: Colors.white,
                            fontWeight: FontWeight.w900,
                            letterSpacing: -0.2,
                          ),
                        ),
                        const SizedBox(height: 4),
                        Text(
                          'nguyenvana@email.com',
                          maxLines: 1,
                          overflow: TextOverflow.ellipsis,
                          style: TextStyle(
                            color: Colors.white.withOpacity(0.9),
                            fontWeight: FontWeight.w600,
                          ),
                        ),
                        const SizedBox(height: 6),
                        Wrap(
                          spacing: 8,
                          runSpacing: 8,
                          children: const [
                            _TierChip(
                              icon: Icons.verified_rounded,
                              label: 'Khách hàng (người ăn)',
                            ),
                            _TierChip(
                              icon: Icons.workspace_premium_rounded,
                              label: 'Tier Silver',
                            ),
                          ],
                        ),
                      ],
                    ),
                  ),
                  IconButton(
                    onPressed:
                        () => Navigator.of(
                          context,
                        ).pushNamed(AppRoutes.profilePersonalInfo),
                    icon: const Icon(Icons.edit, color: Colors.white),
                  ),
                ],
              ),
              const SizedBox(height: 14),
            ],
          ),
        );
      },
    );
  }
}

class _TierChip extends StatelessWidget {
  final IconData icon;
  final String label;

  const _TierChip({required this.icon, required this.label});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
      decoration: BoxDecoration(
        color: Colors.white.withOpacity(0.18),
        borderRadius: BorderRadius.circular(10),
        border: Border.all(color: Colors.white.withOpacity(0.25)),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(icon, size: 14, color: Colors.white),
          const SizedBox(width: 6),
          Text(
            label,
            style: const TextStyle(
              color: Colors.white,
              fontWeight: FontWeight.w700,
              fontSize: 12,
            ),
          ),
        ],
      ),
    );
  }
}

class _HeaderStat extends StatelessWidget {
  final String label;
  final String value;
  final IconData icon;
  final double? width;

  const _HeaderStat({
    required this.label,
    required this.value,
    required this.icon,
    this.width,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      width: width,
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 12),
      decoration: BoxDecoration(
        color: Colors.white.withOpacity(0.16),
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: Colors.white.withOpacity(0.2)),
      ),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Container(
            padding: const EdgeInsets.all(8),
            decoration: BoxDecoration(
              color: Colors.white.withOpacity(0.15),
              shape: BoxShape.circle,
            ),
            child: Icon(icon, color: Colors.white, size: 16),
          ),
          const SizedBox(height: 8),
          Text(
            value,
            maxLines: 1,
            overflow: TextOverflow.ellipsis,
            style: const TextStyle(
              color: Colors.white,
              fontWeight: FontWeight.w900,
              fontSize: 16,
            ),
          ),
          const SizedBox(height: 2),
          Text(
            label,
            textAlign: TextAlign.center,
            maxLines: 2,
            overflow: TextOverflow.ellipsis,
            style: TextStyle(
              color: Colors.white.withOpacity(0.85),
              fontWeight: FontWeight.w600,
              height: 1.2,
            ),
          ),
        ],
      ),
    );
  }
}

class _CardSection extends StatelessWidget {
  final String title;
  final List<_SettingItem> children;

  const _CardSection({required this.title, required this.children});

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(14),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withOpacity(0.04),
            blurRadius: 12,
            offset: const Offset(0, 8),
          ),
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            title,
            style: Theme.of(context).textTheme.titleMedium?.copyWith(
              fontWeight: FontWeight.w900,
              color: AppColors.ink,
            ),
          ),
          const SizedBox(height: 6),
          ...children.map(
            (item) => Column(
              children: [
                _SettingTile(item: item),
                if (item != children.last)
                  Divider(
                    height: 14,
                    thickness: 0.6,
                    color: AppColors.ink.withOpacity(0.06),
                  ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class _SettingItem {
  final IconData icon;
  final String title;
  final String subtitle;
  final bool danger;
  final String? route;
  final void Function(BuildContext)? onTap;

  const _SettingItem({
    required this.icon,
    required this.title,
    required this.subtitle,
    this.danger = false,
    this.route,
    this.onTap,
  });
}

class _SettingTile extends StatelessWidget {
  final _SettingItem item;

  const _SettingTile({required this.item});

  @override
  Widget build(BuildContext context) {
    final color =
        item.danger ? Colors.redAccent : AppColors.ink.withOpacity(0.85);
    return InkWell(
      borderRadius: BorderRadius.circular(12),
      onTap: () {
        if (item.onTap != null) {
          item.onTap!(context);
          return;
        }
        if (item.route != null) {
          Navigator.of(context).pushNamed(item.route!);
        }
      },
      child: Padding(
        padding: const EdgeInsets.symmetric(vertical: 10),
        child: Row(
          children: [
            Container(
              padding: const EdgeInsets.all(10),
              decoration: BoxDecoration(
                color:
                    item.danger
                        ? Colors.redAccent.withOpacity(0.12)
                        : AppColors.customer.withOpacity(0.08),
                borderRadius: BorderRadius.circular(12),
              ),
              child: Icon(
                item.icon,
                color: item.danger ? Colors.redAccent : AppColors.customer,
                size: 20,
              ),
            ),
            const SizedBox(width: 12),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    item.title,
                    style: TextStyle(
                      color: color,
                      fontWeight: FontWeight.w800,
                      fontSize: 15,
                    ),
                  ),
                  const SizedBox(height: 2),
                  Text(
                    item.subtitle,
                    style: TextStyle(
                      color: AppColors.ink.withOpacity(0.6),
                      fontWeight: FontWeight.w600,
                    ),
                  ),
                ],
              ),
            ),
            const SizedBox(width: 10),
            Icon(
              Icons.chevron_right_rounded,
              color: AppColors.ink.withOpacity(0.4),
            ),
          ],
        ),
      ),
    );
  }
}

Future<void> _showLogoutDialog(BuildContext context) async {
  await showDialog<void>(
    context: context,
    builder:
        (ctx) => AlertDialog(
          title: const Text('Đăng xuất'),
          content: const Text('Bạn chắc chắn muốn đăng xuất khỏi tài khoản?'),
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(16),
          ),
          actions: [
            TextButton(
              onPressed: () => Navigator.of(ctx).pop(),
              child: const Text('Hủy'),
            ),
            ElevatedButton(
              style: ElevatedButton.styleFrom(
                backgroundColor: AppColors.customer,
                foregroundColor: Colors.white,
              ),
              onPressed: () {
                Navigator.of(ctx).pop();
                Navigator.of(
                  context,
                ).pushNamedAndRemoveUntil(AppRoutes.welcome, (route) => false);
              },
              child: const Text('Đăng xuất'),
            ),
          ],
        ),
  );
}
