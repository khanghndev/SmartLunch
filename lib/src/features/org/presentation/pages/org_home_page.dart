import 'package:flutter/material.dart';

import '../../../../app/app_routes.dart';
import '../../../../core/constants/app_colors.dart';
import '../../../../core/widgets/premium_bottom_nav.dart';
import '../../../../core/widgets/premium_drawer.dart';

class OrgHomePage extends StatefulWidget {
  const OrgHomePage({super.key});

  @override
  State<OrgHomePage> createState() => _OrgHomePageState();
}

class _OrgHomePageState extends State<OrgHomePage> {
  static const double _navHeight = 74;
  int _selectedIndex = 0;
  final _scaffoldKey = GlobalKey<ScaffoldState>();

  void _onSelectTab(int index) {
    if (index == _selectedIndex) {
      return;
    }
    setState(() {
      _selectedIndex = index;
    });
  }

  @override
  Widget build(BuildContext context) {
    final bottomInset = _navHeight;
    const tabTitles = [
      'Trang chủ',
      'Đơn hàng',
      'Báo cáo',
      'Tài chính',
      'Hồ sơ',
    ];
    final tabs = [
      Builder(builder: (context) => _OrgDashboardTab(bottomInset: bottomInset)),
      Builder(builder: (context) => _OrgOrdersTab(bottomInset: bottomInset)),
      Builder(builder: (context) => _OrgReportsTab(bottomInset: bottomInset)),
      Builder(builder: (context) => _OrgFinanceTab(bottomInset: bottomInset)),
      Builder(builder: (context) => _OrgProfileTab(bottomInset: bottomInset)),
    ];

    return Scaffold(
      key: _scaffoldKey,
      appBar: AppBar(
        title: Text(tabTitles[_selectedIndex]),
        centerTitle: true,
        backgroundColor: Colors.white,
        foregroundColor: AppColors.ink,
        elevation: 0,
        surfaceTintColor: Colors.white,
        leading: IconButton(
          icon: const Icon(Icons.menu_rounded),
          onPressed: () => _scaffoldKey.currentState?.openDrawer(),
          tooltip: 'Mở menu',
        ),
        actions: [
          IconButton(
            tooltip: 'Thông báo',
            icon: const Icon(Icons.notifications_none_rounded),
            onPressed:
                () =>
                    Navigator.of(context).pushNamed(AppRoutes.orgNotifications),
          ),
          IconButton(
            tooltip: 'Hồ sơ',
            icon: const Icon(Icons.person_outline),
            onPressed:
                () => Navigator.of(context).pushNamed(AppRoutes.orgProfile),
          ),
          const SizedBox(width: 4),
        ],
      ),
      drawerEnableOpenDragGesture: true,
      drawer: PremiumDrawer(
        userName: 'Đơn vị đặt suất ăn',
        userRole: 'Organization account',
        roleBadge: 'Đơn vị đặt suất ăn',
        gradient: const [AppColors.org, AppColors.orgAlt],
        accentColor: AppColors.org,
        selectedIndex: _selectedIndex,
        onSelectTab: _onSelectTab,
        onNavigate: (route) => Navigator.of(context).pushNamed(route),
        onLogout:
            () => Navigator.of(
              context,
            ).pushNamedAndRemoveUntil(AppRoutes.welcome, (route) => false),
        sections: const [
          DrawerSection(
            title: 'Chính',
            items: [
              DrawerItem(
                icon: Icons.home_rounded,
                label: 'Home',
                labelVi: 'Trang chủ',
                tabIndex: 0,
              ),
              DrawerItem(
                icon: Icons.event_note_rounded,
                label: 'Orders',
                labelVi: 'Đơn hàng',
                tabIndex: 1,
              ),
              DrawerItem(
                icon: Icons.description_rounded,
                label: 'Reports',
                labelVi: 'Báo cáo',
                tabIndex: 2,
              ),
              DrawerItem(
                icon: Icons.payments_rounded,
                label: 'Finance',
                labelVi: 'Tài chính',
                tabIndex: 3,
              ),
              DrawerItem(
                icon: Icons.person_rounded,
                label: 'Profile',
                labelVi: 'Hồ sơ',
                tabIndex: 4,
              ),
            ],
          ),
          DrawerSection(
            title: 'Hoạt động',
            items: [
              DrawerItem(
                icon: Icons.groups_rounded,
                label: 'Staff list',
                labelVi: 'Danh sách nhân viên',
                route: AppRoutes.orgStaff,
              ),
              DrawerItem(
                icon: Icons.event_note_rounded,
                label: 'Bulk orders',
                labelVi: 'Đơn hàng số lượng lớn',
                route: AppRoutes.orgBulkOrder,
              ),
              DrawerItem(
                icon: Icons.star_rounded,
                label: 'Rate meals',
                labelVi: 'Đánh giá suất ăn',
                route: AppRoutes.customerRating,
              ),
              DrawerItem(
                icon: Icons.bar_chart_rounded,
                label: 'Statistics',
                labelVi: 'Thống kê',
                route: AppRoutes.orgStatistics,
              ),
              DrawerItem(
                icon: Icons.description_rounded,
                label: 'Reports',
                labelVi: 'Báo cáo',
                route: AppRoutes.orgReports,
              ),
              DrawerItem(
                icon: Icons.payments_rounded,
                label: 'Reconciliation',
                labelVi: 'Đối soát',
                route: AppRoutes.orgReconciliation,
              ),
            ],
          ),
          DrawerSection(
            title: 'Hỗ trợ',
            items: [
              DrawerItem(
                icon: Icons.notifications_rounded,
                label: 'Notifications',
                labelVi: 'Thông báo',
                route: AppRoutes.orgNotifications,
              ),
              DrawerItem(
                icon: Icons.chat_bubble_rounded,
                label: 'Chatbot support',
                labelVi: 'Hỗ trợ chatbot',
                route: AppRoutes.chatbot,
              ),
            ],
          ),
        ],
      ),
      body: IndexedStack(index: _selectedIndex, children: tabs),
      bottomNavigationBar: PremiumBottomNav(
        selectedIndex: _selectedIndex,
        onTap: _onSelectTab,
        accentColor: AppColors.org,
        items: const [
          NavItem(
            icon: Icons.home_outlined,
            selectedIcon: Icons.home_rounded,
            label: 'Home',
            labelVi: 'Trang chủ',
          ),
          NavItem(
            icon: Icons.event_note_outlined,
            selectedIcon: Icons.event_note_rounded,
            label: 'Orders',
            labelVi: 'Đơn hàng',
          ),
          NavItem(
            icon: Icons.description_outlined,
            selectedIcon: Icons.description_rounded,
            label: 'Reports',
            labelVi: 'Báo cáo',
          ),
          NavItem(
            icon: Icons.payments_outlined,
            selectedIcon: Icons.payments_rounded,
            label: 'Finance',
            labelVi: 'Tài chính',
          ),
          NavItem(
            icon: Icons.person_outline,
            selectedIcon: Icons.person_rounded,
            label: 'Profile',
            labelVi: 'Hồ sơ',
          ),
        ],
      ),
    );
  }
}

class _OrgProfileSummary extends StatelessWidget {
  const _OrgProfileSummary();

  @override
  Widget build(BuildContext context) {
    final tiles = const [
      _SummaryTile(label: 'Suất/ngày', value: '150'),
      _SummaryTile(label: 'Đúng giờ', value: '97%'),
      _SummaryTile(label: 'Hài lòng', value: '4.7/5'),
    ];

    return Row(
      children: [
        for (var i = 0; i < tiles.length; i++) ...[
          Expanded(child: tiles[i]),
          if (i != tiles.length - 1) const SizedBox(width: 8),
        ],
      ],
    );
  }
}

class _SummaryTile extends StatelessWidget {
  final String label;
  final String value;

  const _SummaryTile({required this.label, required this.value});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(14),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withOpacity(0.03),
            blurRadius: 10,
            offset: const Offset(0, 6),
          ),
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            value,
            style: const TextStyle(
              color: AppColors.org,
              fontWeight: FontWeight.w900,
              fontSize: 16,
            ),
          ),
          const SizedBox(height: 4),
          Text(
            label,
            style: TextStyle(
              color: AppColors.ink.withOpacity(0.7),
              fontWeight: FontWeight.w700,
            ),
          ),
        ],
      ),
    );
  }
}

class _OrgContactCard extends StatelessWidget {
  const _OrgContactCard();

  @override
  Widget build(BuildContext context) {
    final rows = const [
      _OrgInfoRow(label: 'Đại diện', value: 'Nguyễn Minh Anh'),
      _OrgInfoRow(label: 'Email', value: 'ops@acme.com'),
      _OrgInfoRow(label: 'Điện thoại', value: '0901 234 567'),
      _OrgInfoRow(label: 'Địa chỉ giao', value: '123 Nguyễn Huệ, Q1'),
    ];

    return _SectionCard(
      title: 'Thông tin liên hệ',
      subtitle: 'Người đại diện & kênh liên lạc',
      icon: Icons.contact_mail_outlined,
      color: AppColors.org,
      actionLabel: 'Chỉnh sửa',
      onAction: () {},
      child: Column(
        children:
            rows
                .map(
                  (r) => Padding(
                    padding: const EdgeInsets.symmetric(vertical: 4),
                    child: r,
                  ),
                )
                .toList(),
      ),
    );
  }
}

class _OrgInfoRow extends StatelessWidget {
  final String label;
  final String value;

  const _OrgInfoRow({required this.label, required this.value});

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        Expanded(
          child: Text(
            label,
            style: TextStyle(
              color: AppColors.ink.withOpacity(0.65),
              fontWeight: FontWeight.w700,
            ),
          ),
        ),
        Text(
          value,
          style: const TextStyle(
            color: AppColors.ink,
            fontWeight: FontWeight.w800,
          ),
        ),
      ],
    );
  }
}

class _OrgPreferenceCard extends StatelessWidget {
  const _OrgPreferenceCard();

  @override
  Widget build(BuildContext context) {
    return _SectionCard(
      title: 'Tùy chọn dịch vụ',
      subtitle: 'Địa điểm, khẩu phần, khung giờ giao',
      icon: Icons.tune,
      color: AppColors.org,
      actionLabel: 'Cập nhật',
      onAction: () {},
      child: Column(
        children: const [
          _OrgPrefRow(label: 'Địa điểm ưu tiên', value: 'Văn phòng Q1, Kho Q7'),
          _OrgPrefRow(label: 'Khẩu phần', value: 'Healthy · Ít dầu mỡ'),
          _OrgPrefRow(label: 'Khung giờ giao', value: '11:30 - 12:30'),
          _OrgPrefRow(
            label: 'Bên liên hệ nhận',
            value: 'Lễ tân / phòng hành chính',
          ),
        ],
      ),
    );
  }
}

class _OrgPrefRow extends StatelessWidget {
  final String label;
  final String value;

  const _OrgPrefRow({required this.label, required this.value});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 6),
      child: Row(
        children: [
          Expanded(
            child: Text(
              label,
              style: TextStyle(
                color: AppColors.ink.withOpacity(0.65),
                fontWeight: FontWeight.w700,
              ),
            ),
          ),
          Text(
            value,
            style: const TextStyle(
              color: AppColors.ink,
              fontWeight: FontWeight.w800,
            ),
          ),
        ],
      ),
    );
  }
}

class _OrgLogoutCard extends StatelessWidget {
  const _OrgLogoutCard();

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(16),
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
          Row(
            children: [
              Container(
                padding: const EdgeInsets.all(10),
                decoration: BoxDecoration(
                  color: AppColors.org.withOpacity(0.08),
                  shape: BoxShape.circle,
                ),
                child: const Icon(Icons.logout_rounded, color: AppColors.org),
              ),
              const SizedBox(width: 12),
              Expanded(
                child: Text(
                  'Đăng xuất tài khoản doanh nghiệp',
                  style: Theme.of(context).textTheme.titleMedium?.copyWith(
                    fontWeight: FontWeight.w900,
                    color: AppColors.ink,
                  ),
                ),
              ),
            ],
          ),
          const SizedBox(height: 12),
          SizedBox(
            width: double.infinity,
            child: DecoratedBox(
              decoration: BoxDecoration(
                gradient: const LinearGradient(
                  colors: [AppColors.org, AppColors.orgAlt],
                  begin: Alignment.topLeft,
                  end: Alignment.bottomRight,
                ),
                borderRadius: BorderRadius.circular(12),
                boxShadow: [
                  BoxShadow(
                    color: AppColors.org.withOpacity(0.22),
                    blurRadius: 14,
                    offset: const Offset(0, 8),
                  ),
                ],
              ),
              child: ElevatedButton.icon(
                onPressed:
                    () => Navigator.of(context).pushNamedAndRemoveUntil(
                      AppRoutes.welcome,
                      (route) => false,
                    ),
                style: ElevatedButton.styleFrom(
                  backgroundColor: Colors.transparent,
                  shadowColor: Colors.transparent,
                  foregroundColor: Colors.white,
                  padding: const EdgeInsets.symmetric(vertical: 14),
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(12),
                  ),
                ),
                icon: const Icon(Icons.exit_to_app_rounded),
                label: const Text(
                  'Đăng xuất',
                  style: TextStyle(
                    fontWeight: FontWeight.w800,
                    letterSpacing: 0.1,
                  ),
                ),
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class _OrgDashboardTab extends StatelessWidget {
  final double bottomInset;

  const _OrgDashboardTab({required this.bottomInset});

  @override
  Widget build(BuildContext context) {
    final quickStats = [
      _StatCard(
        label: 'Suất hôm nay',
        value: '186',
        color: AppColors.org,
        icon: Icons.lunch_dining_rounded,
      ),
      _StatCard(
        label: 'Đúng giờ',
        value: '97%',
        color: AppColors.org,
        icon: Icons.access_time_filled_rounded,
      ),
      _StatCard(
        label: 'Công nợ',
        value: '22.7tr',
        color: AppColors.org,
        icon: Icons.payments_rounded,
      ),
    ];

    final schedules = [
      const _Schedule(
        title: 'Ca trưa · Thứ 3',
        time: '11:30 · 26/03',
        pax: 62,
        status: 'Đã chốt',
        color: AppColors.org,
      ),
      const _Schedule(
        title: 'Ca tối · Thứ 4',
        time: '18:00 · 27/03',
        pax: 30,
        status: 'Đang giao',
        color: AppColors.orgAlt,
      ),
    ];

    return Container(
      decoration: const BoxDecoration(
        gradient: LinearGradient(
          colors: [Color(0xFFF5F6F8), Color(0xFFEFF4F5)],
          begin: Alignment.topCenter,
          end: Alignment.bottomCenter,
        ),
      ),
      child: SafeArea(
        child: ListView(
          physics: const BouncingScrollPhysics(),
          padding: EdgeInsets.fromLTRB(20, 16, 20, 24 + bottomInset),
          children: [
            const _HeroHeader(),
            const SizedBox(height: 14),
            SizedBox(
              height: 110,
              child: ListView.separated(
                scrollDirection: Axis.horizontal,
                itemBuilder: (context, i) => quickStats[i],
                separatorBuilder: (_, __) => const SizedBox(width: 10),
                itemCount: quickStats.length,
              ),
            ),
            const SizedBox(height: 14),
            _SectionCard(
              title: 'Lịch giao hôm nay',
              subtitle: 'Theo ca và phòng ban',
              icon: Icons.event_available_rounded,
              color: AppColors.org,
              actionLabel: 'Xem tất cả',
              onAction:
                  () => Navigator.of(context).pushNamed(AppRoutes.orgBulkOrder),
              child: Column(
                children:
                    schedules
                        .map(
                          (s) => Padding(
                            padding: const EdgeInsets.symmetric(vertical: 6),
                            child: _ScheduleTile(schedule: s),
                          ),
                        )
                        .toList(),
              ),
            ),
            const SizedBox(height: 12),
            _SectionCard(
              title: 'Lối tắt',
              subtitle: 'Đặt nhanh hoặc xem báo cáo',
              icon: Icons.flash_on_rounded,
              color: AppColors.org,
              child: Wrap(
                spacing: 10,
                runSpacing: 10,
                children: [
                  _QuickButton(
                    label: 'Đặt theo ca',
                    icon: Icons.event_note,
                    onTap:
                        () => Navigator.of(
                          context,
                        ).pushNamed(AppRoutes.orgBulkOrder),
                  ),
                  _QuickButton(
                    label: 'Thêm nhân viên',
                    icon: Icons.person_add_alt_1_rounded,
                    onTap:
                        () =>
                            Navigator.of(context).pushNamed(AppRoutes.orgStaff),
                  ),
                  _QuickButton(
                    label: 'Tải báo cáo',
                    icon: Icons.description_outlined,
                    onTap:
                        () => Navigator.of(
                          context,
                        ).pushNamed(AppRoutes.orgReports),
                  ),
                  _QuickButton(
                    label: 'Đối soát',
                    icon: Icons.payments_outlined,
                    onTap:
                        () => Navigator.of(
                          context,
                        ).pushNamed(AppRoutes.orgReconciliation),
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

class _HeroHeader extends StatelessWidget {
  const _HeroHeader();

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(18),
      decoration: BoxDecoration(
        gradient: const LinearGradient(
          colors: [AppColors.org, AppColors.orgAlt],
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
        ),
        borderRadius: BorderRadius.circular(18),
        boxShadow: [
          BoxShadow(
            color: AppColors.org.withOpacity(0.22),
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
                padding: const EdgeInsets.all(12),
                decoration: BoxDecoration(
                  color: Colors.white.withOpacity(0.16),
                  borderRadius: BorderRadius.circular(14),
                ),
                child: const Icon(Icons.apartment, color: Colors.white),
              ),
              const SizedBox(width: 12),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      'SmartLunch for Business',
                      style: Theme.of(context).textTheme.titleLarge?.copyWith(
                        color: Colors.white,
                        fontWeight: FontWeight.w900,
                        letterSpacing: -0.2,
                      ),
                    ),
                    const SizedBox(height: 4),
                    Text(
                      'Quản lý suất ăn cho công ty, báo cáo và đối soát hợp đồng.',
                      style: TextStyle(
                        color: Colors.white.withOpacity(0.9),
                        fontWeight: FontWeight.w600,
                      ),
                    ),
                  ],
                ),
              ),
              IconButton(
                onPressed:
                    () => Navigator.of(
                      context,
                    ).pushNamed(AppRoutes.orgNotifications),
                icon: const Icon(
                  Icons.notifications_none_rounded,
                  color: Colors.white,
                ),
              ),
            ],
          ),
          const SizedBox(height: 12),
          Row(
            children: const [
              _HeroChip(
                icon: Icons.verified_rounded,
                label: 'Hợp đồng hiệu lực',
              ),
              SizedBox(width: 8),
              _HeroChip(icon: Icons.groups_rounded, label: '150 suất/ngày'),
            ],
          ),
        ],
      ),
    );
  }
}

class _HeroChip extends StatelessWidget {
  final IconData icon;
  final String label;

  const _HeroChip({required this.icon, required this.label});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
      decoration: BoxDecoration(
        color: Colors.white.withOpacity(0.18),
        borderRadius: BorderRadius.circular(12),
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

class _StatCard extends StatelessWidget {
  final String label;
  final String value;
  final Color color;
  final IconData icon;

  const _StatCard({
    required this.label,
    required this.value,
    required this.color,
    required this.icon,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      width: 180,
      padding: const EdgeInsets.all(12),
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
      child: Row(
        children: [
          Container(
            padding: const EdgeInsets.all(10),
            decoration: BoxDecoration(
              color: color.withOpacity(0.12),
              shape: BoxShape.circle,
            ),
            child: Icon(icon, color: color),
          ),
          const SizedBox(width: 10),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  label,
                  style: TextStyle(
                    color: AppColors.ink.withOpacity(0.7),
                    fontWeight: FontWeight.w700,
                  ),
                ),
                const SizedBox(height: 4),
                Text(
                  value,
                  style: TextStyle(
                    color: color,
                    fontWeight: FontWeight.w900,
                    fontSize: 16,
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

class _Schedule {
  final String title;
  final String time;
  final int pax;
  final String status;
  final Color color;

  const _Schedule({
    required this.title,
    required this.time,
    required this.pax,
    required this.status,
    required this.color,
  });
}

class _ScheduleTile extends StatelessWidget {
  final _Schedule schedule;

  const _ScheduleTile({required this.schedule});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: AppColors.ink.withOpacity(0.01),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: AppColors.ink.withOpacity(0.05)),
      ),
      child: Row(
        children: [
          Container(
            padding: const EdgeInsets.all(10),
            decoration: BoxDecoration(
              color: schedule.color.withOpacity(0.12),
              borderRadius: BorderRadius.circular(12),
            ),
            child: const Icon(Icons.schedule, color: AppColors.org),
          ),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  children: [
                    Expanded(
                      child: Text(
                        schedule.title,
                        style: const TextStyle(
                          color: AppColors.ink,
                          fontWeight: FontWeight.w900,
                        ),
                      ),
                    ),
                    Container(
                      padding: const EdgeInsets.symmetric(
                        horizontal: 8,
                        vertical: 4,
                      ),
                      decoration: BoxDecoration(
                        color: schedule.color.withOpacity(0.14),
                        borderRadius: BorderRadius.circular(8),
                      ),
                      child: Text(
                        schedule.status,
                        style: TextStyle(
                          color: schedule.color,
                          fontWeight: FontWeight.w800,
                          fontSize: 12.5,
                        ),
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 4),
                Text(
                  '${schedule.time} · ${schedule.pax} suất',
                  style: TextStyle(
                    color: AppColors.ink.withOpacity(0.65),
                    fontWeight: FontWeight.w700,
                  ),
                ),
              ],
            ),
          ),
          IconButton(
            onPressed:
                () => Navigator.of(context).pushNamed(AppRoutes.orgBulkOrder),
            icon: const Icon(Icons.chevron_right_rounded),
          ),
        ],
      ),
    );
  }
}

class _QuickButton extends StatelessWidget {
  final String label;
  final IconData icon;
  final VoidCallback onTap;

  const _QuickButton({
    required this.label,
    required this.icon,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return OutlinedButton.icon(
      onPressed: onTap,
      style: OutlinedButton.styleFrom(
        foregroundColor: AppColors.org,
        side: BorderSide(color: AppColors.org.withOpacity(0.35)),
        padding: const EdgeInsets.symmetric(vertical: 12, horizontal: 12),
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      ),
      icon: Icon(icon),
      label: Text(label, style: const TextStyle(fontWeight: FontWeight.w800)),
    );
  }
}

class _SectionCard extends StatelessWidget {
  final String title;
  final String subtitle;
  final IconData icon;
  final Color color;
  final Widget child;
  final VoidCallback? onAction;
  final String? actionLabel;

  const _SectionCard({
    required this.title,
    required this.subtitle,
    required this.icon,
    required this.color,
    required this.child,
    this.onAction,
    this.actionLabel,
  });

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
          Row(
            children: [
              Container(
                padding: const EdgeInsets.all(10),
                decoration: BoxDecoration(
                  color: color.withOpacity(0.12),
                  shape: BoxShape.circle,
                ),
                child: Icon(icon, color: color),
              ),
              const SizedBox(width: 10),
              Expanded(
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
                    Text(
                      subtitle,
                      style: TextStyle(
                        color: AppColors.ink.withOpacity(0.6),
                        fontWeight: FontWeight.w600,
                      ),
                    ),
                  ],
                ),
              ),
              if (onAction != null && actionLabel != null)
                TextButton(onPressed: onAction, child: Text(actionLabel!)),
            ],
          ),
          const SizedBox(height: 10),
          child,
        ],
      ),
    );
  }
}

class _OrgOrdersTab extends StatelessWidget {
  final double bottomInset;

  const _OrgOrdersTab({required this.bottomInset});

  @override
  Widget build(BuildContext context) {
    final upcoming = [
      const _Schedule(
        title: 'Ca trưa · Thứ 3',
        time: '11:30 · 26/03',
        pax: 62,
        status: 'Đã chốt',
        color: AppColors.org,
      ),
      const _Schedule(
        title: 'Ca sáng · Thứ 2',
        time: '08:30 · 25/03',
        pax: 45,
        status: 'Đang chuẩn bị',
        color: AppColors.org,
      ),
    ];

    return _TabScaffold(
      bottomInset: bottomInset,
      header: _TabHeader(
        title: 'Đơn hàng',
        subtitle: 'Điều phối suất ăn theo ca và phòng ban',
        icon: Icons.event_note_rounded,
        accent: AppColors.org,
        accentAlt: AppColors.orgAlt,
        actions: [
          IconButton(
            onPressed:
                () => Navigator.of(context).pushNamed(AppRoutes.orgBulkOrder),
            icon: const Icon(Icons.add_task_rounded, color: Colors.white),
          ),
        ],
      ),
      children: [
        _SectionCard(
          title: 'Tóm tắt hôm nay',
          subtitle: 'Cập nhật nhanh cho từng ca',
          icon: Icons.lunch_dining_rounded,
          color: AppColors.org,
          actionLabel: 'Xem lịch',
          onAction:
              () => Navigator.of(context).pushNamed(AppRoutes.orgBulkOrder),
          child: Column(
            children:
                upcoming
                    .map(
                      (s) => Padding(
                        padding: const EdgeInsets.symmetric(vertical: 6),
                        child: _ScheduleTile(schedule: s),
                      ),
                    )
                    .toList(),
          ),
        ),
        const SizedBox(height: 12),
        _SectionCard(
          title: 'Tác vụ nhanh',
          subtitle: 'Tạo đơn, phân bổ hoặc xem danh sách',
          icon: Icons.flash_on_rounded,
          color: AppColors.org,
          child: Wrap(
            spacing: 10,
            runSpacing: 10,
            children: [
              _QuickButton(
                label: 'Đặt theo ca',
                icon: Icons.event_note,
                onTap:
                    () =>
                        Navigator.of(context).pushNamed(AppRoutes.orgBulkOrder),
              ),
              _QuickButton(
                label: 'Nhân viên',
                icon: Icons.groups_rounded,
                onTap:
                    () => Navigator.of(context).pushNamed(AppRoutes.orgStaff),
              ),
              _QuickButton(
                label: 'Theo phòng ban',
                icon: Icons.account_tree_rounded,
                onTap:
                    () => Navigator.of(
                      context,
                    ).pushNamed(AppRoutes.orgStatistics),
              ),
              _QuickButton(
                label: 'Chat CSKH',
                icon: Icons.chat_bubble_outline,
                onTap: () => Navigator.of(context).pushNamed(AppRoutes.chatbot),
              ),
            ],
          ),
        ),
      ],
    );
  }
}

class _OrgReportsTab extends StatelessWidget {
  final double bottomInset;

  const _OrgReportsTab({required this.bottomInset});

  @override
  Widget build(BuildContext context) {
    return _TabScaffold(
      bottomInset: bottomInset,
      header: _TabHeader(
        title: 'Báo cáo',
        subtitle: 'Tải tổng hợp, xem thống kê, đánh giá',
        icon: Icons.description_outlined,
        accent: AppColors.org,
        accentAlt: AppColors.orgAlt,
        actions: [
          IconButton(
            onPressed:
                () => Navigator.of(context).pushNamed(AppRoutes.orgReports),
            icon: const Icon(Icons.download_rounded, color: Colors.white),
          ),
        ],
      ),
      children: [
        _SectionCard(
          title: 'Tải nhanh',
          subtitle: 'Báo cáo kỳ mới nhất',
          icon: Icons.cloud_download_rounded,
          color: AppColors.org,
          child: Wrap(
            spacing: 10,
            runSpacing: 10,
            children: [
              _QuickButton(
                label: 'Tổng hợp chi phí',
                icon: Icons.description_rounded,
                onTap:
                    () => Navigator.of(context).pushNamed(AppRoutes.orgReports),
              ),
              _QuickButton(
                label: 'Biên bản đối soát',
                icon: Icons.verified_outlined,
                onTap:
                    () => Navigator.of(
                      context,
                    ).pushNamed(AppRoutes.orgReconciliation),
              ),
              _QuickButton(
                label: 'Điểm chất lượng',
                icon: Icons.star_rounded,
                onTap:
                    () => Navigator.of(
                      context,
                    ).pushNamed(AppRoutes.orgStatistics),
              ),
            ],
          ),
        ),
        const SizedBox(height: 12),
        _SectionCard(
          title: 'Thống kê nhanh',
          subtitle: 'Suất ăn, đúng giờ, phản hồi',
          icon: Icons.bar_chart_rounded,
          color: AppColors.org,
          child: Column(
            children: const [
              _SummaryRow(label: 'Suất tuần này', value: '366'),
              _SummaryRow(label: 'Đúng giờ', value: '97%'),
              _SummaryRow(label: 'Điểm hài lòng', value: '4.7/5'),
            ],
          ),
        ),
      ],
    );
  }
}

class _OrgFinanceTab extends StatelessWidget {
  final double bottomInset;

  const _OrgFinanceTab({required this.bottomInset});

  @override
  Widget build(BuildContext context) {
    return _TabScaffold(
      bottomInset: bottomInset,
      header: _TabHeader(
        title: 'Tài chính',
        subtitle: 'Công nợ, hóa đơn, hỗ trợ thanh toán',
        icon: Icons.payments_outlined,
        accent: AppColors.org,
        accentAlt: AppColors.orgAlt,
        actions: [
          IconButton(
            onPressed:
                () => Navigator.of(
                  context,
                ).pushNamed(AppRoutes.orgReconciliation),
            icon: const Icon(Icons.receipt_long_rounded, color: Colors.white),
          ),
        ],
      ),
      children: [
        _SectionCard(
          title: 'Công nợ hiện tại',
          subtitle: 'Hạn thanh toán 05/04',
          icon: Icons.account_balance_wallet_rounded,
          color: AppColors.org,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              const Text(
                '22.700.000đ',
                style: TextStyle(
                  color: AppColors.ink,
                  fontWeight: FontWeight.w900,
                  fontSize: 22,
                  letterSpacing: -0.2,
                ),
              ),
              const SizedBox(height: 8),
              Wrap(
                spacing: 10,
                runSpacing: 10,
                children: [
                  OutlinedButton.icon(
                    onPressed:
                        () => Navigator.of(
                          context,
                        ).pushNamed(AppRoutes.orgReconciliation),
                    style: OutlinedButton.styleFrom(
                      foregroundColor: AppColors.org,
                      side: BorderSide(color: AppColors.org.withOpacity(0.35)),
                      padding: const EdgeInsets.symmetric(
                        vertical: 12,
                        horizontal: 14,
                      ),
                      minimumSize: const Size(0, 0),
                    ),
                    icon: const Icon(Icons.picture_as_pdf_rounded),
                    label: const Text('Hóa đơn'),
                  ),
                  ElevatedButton.icon(
                    onPressed:
                        () => Navigator.of(
                          context,
                        ).pushNamed(AppRoutes.orgReconciliation),
                    style: ElevatedButton.styleFrom(
                      backgroundColor: AppColors.org,
                      foregroundColor: Colors.white,
                      padding: const EdgeInsets.symmetric(
                        vertical: 12,
                        horizontal: 14,
                      ),
                      minimumSize: const Size(0, 0),
                    ),
                    icon: const Icon(Icons.verified_outlined),
                    label: const Text('Xác nhận chuyển'),
                  ),
                ],
              ),
            ],
          ),
        ),
        const SizedBox(height: 12),
        _SectionCard(
          title: 'Hóa đơn gần đây',
          subtitle: 'Theo kỳ đối soát',
          icon: Icons.receipt_long_rounded,
          color: AppColors.org,
          child: Column(
            children: const [
              _SummaryRow(label: '#INV-0324-01', value: 'Chờ thanh toán'),
              _SummaryRow(label: '#INV-0224-02', value: 'Đang đối soát'),
              _SummaryRow(label: '#INV-0224-01', value: 'Đã thanh toán'),
            ],
          ),
        ),
      ],
    );
  }
}

class _OrgProfileTab extends StatelessWidget {
  final double bottomInset;

  const _OrgProfileTab({required this.bottomInset});

  @override
  Widget build(BuildContext context) {
    return _TabScaffold(
      bottomInset: bottomInset,
      header: _TabHeader(
        title: 'Hồ sơ',
        subtitle: 'Thông tin doanh nghiệp & quyền truy cập',
        icon: Icons.apartment_rounded,
        accent: AppColors.org,
        accentAlt: AppColors.orgAlt,
        actions: [
          IconButton(
            onPressed:
                () => Navigator.of(context).pushNamed(AppRoutes.orgProfile),
            icon: const Icon(Icons.edit_outlined, color: Colors.white),
          ),
        ],
      ),
      children: [
        _SectionCard(
          title: 'Thông tin công ty',
          subtitle: 'Công ty ACME · Mã hợp đồng SL-ACME-024',
          icon: Icons.badge_rounded,
          color: AppColors.org,
          actionLabel: 'Xem chi tiết',
          onAction: () => Navigator.of(context).pushNamed(AppRoutes.orgProfile),
          child: Wrap(
            spacing: 8,
            runSpacing: 8,
            children: const [
              _HeroChip(
                icon: Icons.verified_rounded,
                label: 'Hợp đồng hiệu lực',
              ),
              _HeroChip(icon: Icons.groups_rounded, label: '150 suất/ngày'),
              _HeroChip(icon: Icons.schedule, label: 'Giao trưa 11:30'),
            ],
          ),
        ),
        const SizedBox(height: 12),
        const _OrgProfileSummary(),
        const SizedBox(height: 12),
        const _OrgContactCard(),
        const SizedBox(height: 12),
        const _OrgPreferenceCard(),
        const SizedBox(height: 12),
        const _OrgLogoutCard(),
        const SizedBox(height: 12),
      ],
    );
  }
}

class _TabScaffold extends StatelessWidget {
  final double bottomInset;
  final _TabHeader header;
  final List<Widget> children;

  const _TabScaffold({
    required this.bottomInset,
    required this.header,
    required this.children,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      decoration: const BoxDecoration(
        gradient: LinearGradient(
          colors: [Color(0xFFF5F6F8), Color(0xFFEFF4F5)],
          begin: Alignment.topCenter,
          end: Alignment.bottomCenter,
        ),
      ),
      child: SafeArea(
        child: ListView(
          physics: const BouncingScrollPhysics(),
          padding: EdgeInsets.fromLTRB(20, 16, 20, 24 + bottomInset),
          children: [header, const SizedBox(height: 14), ...children],
        ),
      ),
    );
  }
}

class _TabHeader extends StatelessWidget {
  final String title;
  final String subtitle;
  final IconData icon;
  final Color accent;
  final Color accentAlt;
  final List<Widget> actions;

  const _TabHeader({
    required this.title,
    required this.subtitle,
    required this.icon,
    required this.accent,
    required this.accentAlt,
    required this.actions,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(18),
      decoration: BoxDecoration(
        gradient: LinearGradient(
          colors: [accent, accentAlt],
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
        ),
        borderRadius: BorderRadius.circular(18),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withOpacity(0.12),
            blurRadius: 18,
            offset: const Offset(0, 10),
          ),
        ],
      ),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Container(
            padding: const EdgeInsets.all(12),
            decoration: BoxDecoration(
              color: Colors.white.withOpacity(0.2),
              borderRadius: BorderRadius.circular(16),
            ),
            child: Icon(icon, color: Colors.white),
          ),
          const SizedBox(width: 14),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  title,
                  style: Theme.of(context).textTheme.titleLarge?.copyWith(
                    color: Colors.white,
                    fontWeight: FontWeight.w700,
                  ),
                ),
                const SizedBox(height: 6),
                Text(
                  subtitle,
                  style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                    color: Colors.white.withOpacity(0.9),
                  ),
                ),
              ],
            ),
          ),
          Wrap(spacing: 6, runSpacing: 6, children: actions),
        ],
      ),
    );
  }
}

class _SummaryRow extends StatelessWidget {
  final String label;
  final String value;

  const _SummaryRow({required this.label, required this.value});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 6),
      child: Row(
        children: [
          Expanded(
            child: Text(
              label,
              style: TextStyle(
                color: AppColors.ink.withOpacity(0.7),
                fontWeight: FontWeight.w700,
              ),
            ),
          ),
          Text(
            value,
            style: const TextStyle(
              color: AppColors.ink,
              fontWeight: FontWeight.w900,
            ),
          ),
        ],
      ),
    );
  }
}
