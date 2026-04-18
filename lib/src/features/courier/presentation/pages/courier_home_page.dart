import 'package:flutter/material.dart';

import '../../../../app/app_routes.dart';
import '../../../../core/constants/app_colors.dart';
import '../../../../core/widgets/premium_bottom_nav.dart';
import '../../../../core/widgets/premium_drawer.dart';

class CourierHomePage extends StatefulWidget {
  const CourierHomePage({super.key});

  @override
  State<CourierHomePage> createState() => _CourierHomePageState();
}

class _CourierHomePageState extends State<CourierHomePage> {
  static const double _navHeight = 74;
  int _selectedIndex = 0;
  final _scaffoldKey = GlobalKey<ScaffoldState>();

  void _onSelectTab(int index) {
    if (index == _selectedIndex) return;
    setState(() => _selectedIndex = index);
  }

  @override
  Widget build(BuildContext context) {
    final bottomInset = _navHeight;
    const tabTitles = [
      'Trang chủ',
      'Giao hàng',
      'Tuyến đường',
      'Minh chứng',
      'Hồ sơ',
    ];
    final tabs = [
      _CourierDashboardTab(bottomInset: bottomInset),
      _DeliveriesTab(bottomInset: bottomInset),
      _RoutesTab(bottomInset: bottomInset),
      _ProofTab(bottomInset: bottomInset),
      _ProfileTab(bottomInset: bottomInset),
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
      ),
      drawerEnableOpenDragGesture: true,
      drawer: PremiumDrawer(
        userName: 'Nhân viên giao hàng',
        userRole: 'Delivery team',
        roleBadge: 'Nhân viên giao hàng',
        gradient: const [AppColors.courier, AppColors.courierAlt],
        accentColor: AppColors.courier,
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
                icon: Icons.list_alt_rounded,
                label: 'Deliveries',
                labelVi: 'Giao hàng',
                tabIndex: 1,
              ),
              DrawerItem(
                icon: Icons.map_rounded,
                label: 'Routes',
                labelVi: 'Tuyến đường',
                tabIndex: 2,
              ),
              DrawerItem(
                icon: Icons.camera_alt_rounded,
                label: 'Proof',
                labelVi: 'Minh chứng',
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
            title: 'Nhiệm vụ giao hàng',
            items: [
              DrawerItem(
                icon: Icons.list_alt_rounded,
                label: 'Delivery list',
                labelVi: 'Danh sách giao hàng',
                route: AppRoutes.courierDeliveryList,
              ),
              DrawerItem(
                icon: Icons.receipt_long_rounded,
                label: 'Delivery detail',
                labelVi: 'Chi tiết giao hàng',
                route: AppRoutes.courierDeliveryDetail,
              ),
              DrawerItem(
                icon: Icons.map_rounded,
                label: 'Route map',
                labelVi: 'Bản đồ tuyến đường',
                route: AppRoutes.courierRouteMap,
              ),
              DrawerItem(
                icon: Icons.camera_alt_rounded,
                label: 'Proof of delivery',
                labelVi: 'Minh chứng giao hàng',
                route: AppRoutes.courierProof,
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
                route: AppRoutes.courierNotifications,
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
        accentColor: AppColors.courier,
        items: const [
          NavItem(
            icon: Icons.home_outlined,
            selectedIcon: Icons.home_rounded,
            label: 'Home',
            labelVi: 'Trang chủ',
          ),
          NavItem(
            icon: Icons.list_alt_outlined,
            selectedIcon: Icons.list_alt_rounded,
            label: 'Deliveries',
            labelVi: 'Giao hàng',
          ),
          NavItem(
            icon: Icons.map_outlined,
            selectedIcon: Icons.map_rounded,
            label: 'Routes',
            labelVi: 'Tuyến đường',
          ),
          NavItem(
            icon: Icons.camera_alt_outlined,
            selectedIcon: Icons.camera_alt_rounded,
            label: 'Proof',
            labelVi: 'Minh chứng',
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

class _CourierDashboardTab extends StatelessWidget {
  final double bottomInset;

  const _CourierDashboardTab({required this.bottomInset});

  @override
  Widget build(BuildContext context) {
    final stats = [
      _StatCard(
        label: 'Đơn hôm nay',
        value: '12',
        color: AppColors.courier,
        icon: Icons.list_alt_rounded,
      ),
      _StatCard(
        label: 'Đã giao',
        value: '7',
        color: AppColors.courier,
        icon: Icons.check_circle_rounded,
      ),
      _StatCard(
        label: 'Đang giao',
        value: '3',
        color: AppColors.courierAlt,
        icon: Icons.delivery_dining_rounded,
      ),
    ];

    final tasks = [
      const _Task(
        title: 'Ca sáng · Văn phòng Q1',
        time: '08:30 · 25/03',
        stops: 4,
        status: 'Đang giao',
        color: AppColors.courierAlt,
      ),
      const _Task(
        title: 'Ca trưa · KTX Zone B',
        time: '11:30 · 25/03',
        stops: 6,
        status: 'Chưa bắt đầu',
        color: AppColors.courier,
      ),
    ];

    return _TabScaffold(
      bottomInset: bottomInset,
      header: const _WelcomeHeader(name: 'Nguyễn Văn B'),
      children: [
        SizedBox(
          height: 110,
          child: ListView.separated(
            scrollDirection: Axis.horizontal,
            itemBuilder: (context, i) => stats[i],
            separatorBuilder: (_, __) => const SizedBox(width: 10),
            itemCount: stats.length,
          ),
        ),
        const SizedBox(height: 12),
        _SectionCard(
          title: 'Lịch giao hôm nay',
          subtitle: 'Ca giao và số điểm dừng',
          icon: Icons.event_available_rounded,
          color: AppColors.courier,
          actionLabel: 'Xem danh sách',
          onAction:
              () => Navigator.of(
                context,
              ).pushNamed(AppRoutes.courierDeliveryList),
          child: Column(
            children:
                tasks
                    .map(
                      (t) => Padding(
                        padding: const EdgeInsets.symmetric(vertical: 6),
                        child: _TaskTile(task: t),
                      ),
                    )
                    .toList(),
          ),
        ),
        const SizedBox(height: 12),
        _SectionCard(
          title: 'Lối tắt',
          subtitle: 'Nhận đơn, xem bản đồ, chụp Minh chứng',
          icon: Icons.flash_on_rounded,
          color: AppColors.courier,
          child: Wrap(
            spacing: 10,
            runSpacing: 10,
            children: [
              _QuickButton(
                label: 'Danh sách',
                icon: Icons.list_alt_rounded,
                onTap:
                    () => Navigator.of(
                      context,
                    ).pushNamed(AppRoutes.courierDeliveryList),
              ),
              _QuickButton(
                label: 'Bản đồ',
                icon: Icons.map_outlined,
                onTap:
                    () => Navigator.of(
                      context,
                    ).pushNamed(AppRoutes.courierRouteMap),
              ),
              _QuickButton(
                label: 'Minh chứng',
                icon: Icons.camera_alt_outlined,
                onTap:
                    () =>
                        Navigator.of(context).pushNamed(AppRoutes.courierProof),
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

class _DeliveriesTab extends StatelessWidget {
  final double bottomInset;

  const _DeliveriesTab({required this.bottomInset});

  @override
  Widget build(BuildContext context) {
    final filters = ['Tất cả', 'Đang giao', 'Chờ lấy', 'Hoàn tất'];
    return _TabScaffold(
      bottomInset: bottomInset,
      header: _TabHeader(
        title: 'Giao hàng',
        subtitle: 'Tiếp nhận và cập nhật trạng thái',
        icon: Icons.list_alt_rounded,
        accent: AppColors.courier,
        accentAlt: AppColors.courierAlt,
        actions: [
          IconButton(
            onPressed:
                () => Navigator.of(
                  context,
                ).pushNamed(AppRoutes.courierDeliveryList),
            icon: const Icon(Icons.filter_list, color: Colors.white),
          ),
        ],
      ),
      children: [
        SingleChildScrollView(
          scrollDirection: Axis.horizontal,
          child: Row(
            children:
                filters
                    .map(
                      (f) => Padding(
                        padding: const EdgeInsets.only(right: 8),
                        child: FilterChip(
                          label: Text(f),
                          selected: f == 'Tất cả',
                          onSelected: (_) {},
                          selectedColor: AppColors.courier.withOpacity(0.12),
                          checkmarkColor: AppColors.courier,
                          shape: RoundedRectangleBorder(
                            borderRadius: BorderRadius.circular(12),
                            side: BorderSide(
                              color:
                                  f == 'Tất cả'
                                      ? AppColors.courier.withOpacity(0.5)
                                      : AppColors.ink.withOpacity(0.08),
                            ),
                          ),
                          labelStyle: TextStyle(
                            color:
                                f == 'Tất cả'
                                    ? AppColors.courier
                                    : AppColors.ink.withOpacity(0.75),
                            fontWeight: FontWeight.w700,
                          ),
                        ),
                      ),
                    )
                    .toList(),
          ),
        ),
        const SizedBox(height: 12),
        _AssignmentCard(
          title: 'Ca sáng · Văn phòng Q1',
          code: '#DL-2301',
          time: '08:30 · 25/03',
          stops: 4,
          status: 'Đang giao',
          statusColor: AppColors.courierAlt,
          onTap:
              () => Navigator.of(
                context,
              ).pushNamed(AppRoutes.courierDeliveryDetail),
        ),
        const SizedBox(height: 10),
        _AssignmentCard(
          title: 'Ca trưa · KTX Zone B',
          code: '#DL-2302',
          time: '11:30 · 25/03',
          stops: 6,
          status: 'Chờ lấy',
          statusColor: AppColors.courier,
          onTap:
              () => Navigator.of(
                context,
              ).pushNamed(AppRoutes.courierDeliveryDetail),
        ),
      ],
    );
  }
}

class _RoutesTab extends StatelessWidget {
  final double bottomInset;

  const _RoutesTab({required this.bottomInset});

  @override
  Widget build(BuildContext context) {
    return _TabScaffold(
      bottomInset: bottomInset,
      header: _TabHeader(
        title: 'Tuyến đường',
        subtitle: 'Xem điểm dừng và điều hướng',
        icon: Icons.map_outlined,
        accent: AppColors.courier,
        accentAlt: AppColors.courierAlt,
        actions: [
          IconButton(
            onPressed:
                () =>
                    Navigator.of(context).pushNamed(AppRoutes.courierRouteMap),
            icon: const Icon(Icons.navigation_rounded, color: Colors.white),
          ),
        ],
      ),
      children: [
        _SectionCard(
          title: 'Điểm dừng tiếp theo',
          subtitle: 'Ấn để mở điều hướng',
          icon: Icons.location_on_rounded,
          color: AppColors.courier,
          child: Column(
            children: const [
              _StopRow(
                title: 'Văn phòng Q1',
                detail: '10 Nguyễn Trãi, Q1',
                eta: '08:45',
                index: 1,
              ),
              SizedBox(height: 8),
              _StopRow(
                title: 'KTX Zone B',
                detail: '268 Lý Thường Kiệt, Q10',
                eta: '09:15',
                index: 2,
              ),
            ],
          ),
        ),
        const SizedBox(height: 12),
        _SectionCard(
          title: 'Hỗ trợ nhanh',
          subtitle: 'Gọi người liên hệ hoặc báo sự cố',
          icon: Icons.support_agent,
          color: AppColors.courier,
          child: Wrap(
            spacing: 10,
            runSpacing: 10,
            children: [
              _QuickButton(
                label: 'Gọi liên hệ',
                icon: Icons.call_rounded,
                onTap: () {},
              ),
              _QuickButton(
                label: 'Báo kẹt xe',
                icon: Icons.warning_amber_rounded,
                onTap:
                    () => Navigator.of(
                      context,
                    ).pushNamed(AppRoutes.courierDeliveryDetail),
              ),
            ],
          ),
        ),
      ],
    );
  }
}

class _ProofTab extends StatelessWidget {
  final double bottomInset;

  const _ProofTab({required this.bottomInset});

  @override
  Widget build(BuildContext context) {
    return _TabScaffold(
      bottomInset: bottomInset,
      header: _TabHeader(
        title: 'Minh chứng',
        subtitle: 'Chụp ảnh và xác nhận thời gian',
        icon: Icons.camera_alt_outlined,
        accent: AppColors.courier,
        accentAlt: AppColors.courierAlt,
        actions: [
          IconButton(
            onPressed:
                () => Navigator.of(context).pushNamed(AppRoutes.courierProof),
            icon: const Icon(Icons.add_a_photo_rounded, color: Colors.white),
          ),
        ],
      ),
      children: [
        _SectionCard(
          title: 'Nhiệm vụ gần đây',
          subtitle: 'Chọn để thêm ảnh hoặc ký tên',
          icon: Icons.receipt_long_rounded,
          color: AppColors.courier,
          child: Column(
            children: [
              _AssignmentCard(
                title: 'Ca sáng · Văn phòng Q1',
                code: '#DL-2301',
                time: '08:30 · 25/03',
                stops: 4,
                status: 'Cần minh chứng',
                statusColor: AppColors.courier,
                onTap:
                    () =>
                        Navigator.of(context).pushNamed(AppRoutes.courierProof),
              ),
            ],
          ),
        ),
      ],
    );
  }
}

class _ProfileTab extends StatelessWidget {
  final double bottomInset;

  const _ProfileTab({required this.bottomInset});

  @override
  Widget build(BuildContext context) {
    return _TabScaffold(
      bottomInset: bottomInset,
      header: _TabHeader(
        title: 'Hồ sơ',
        subtitle: 'Thông tin tài xế & cài đặt',
        icon: Icons.person_outline,
        accent: AppColors.courier,
        accentAlt: AppColors.courierAlt,
        actions: [
          IconButton(
            onPressed:
                () => Navigator.of(context).pushNamed(AppRoutes.courierProfile),
            icon: const Icon(Icons.edit_outlined, color: Colors.white),
          ),
        ],
      ),
      children: [
        _SectionCard(
          title: 'Thông tin nhanh',
          subtitle: 'Nguyễn Văn B · Biển số 51A-123.45',
          icon: Icons.badge_outlined,
          color: AppColors.courier,
          child: Wrap(
            spacing: 8,
            runSpacing: 6,
            children: const [
              _HeroChip(icon: Icons.verified_rounded, label: 'Tài xế đối tác'),
              _HeroChip(icon: Icons.schedule, label: 'Ca sáng / trưa'),
              _HeroChip(icon: Icons.call, label: '0987 654 321'),
            ],
          ),
        ),
        const SizedBox(height: 8),
        const SizedBox(height: 8),
        _SectionCard(
          title: 'Phương tiện & giấy tờ',
          subtitle: 'Xe máy · Honda · 2021',
          icon: Icons.motorcycle_rounded,
          color: AppColors.courier,
          actionLabel: 'Cập nhật',
          onAction: () {},
          child: Column(
            children: [
              _InfoRow(
                label: 'Biển số',
                value: '51A-123.45',
                color: AppColors.courier,
              ),
              _InfoRow(
                label: 'Bảo hiểm',
                value: 'Hết hạn: 12/2024',
                color: AppColors.courier,
              ),
              _InfoRow(
                label: 'GPLX',
                value: 'Hạng A1 · Hợp lệ',
                color: AppColors.courier,
              ),
            ],
          ),
        ),
        const SizedBox(height: 8),
        _SectionCard(
          title: 'Hiệu suất gần đây',
          subtitle: 'Theo tuần',
          icon: Icons.insights_rounded,
          color: AppColors.courier,
          child: Wrap(
            spacing: 10,
            runSpacing: 10,
            children: const [
              _MiniStat(label: 'Hoàn tất', value: '42 đơn'),
              _MiniStat(label: 'Đúng giờ', value: '96%'),
              _MiniStat(label: 'Đánh giá', value: '4.8/5'),
            ],
          ),
        ),
        const SizedBox(height: 8),
        _SectionCard(
          title: 'Thanh toán',
          subtitle: 'Thu nhập & đối soát',
          icon: Icons.account_balance_wallet_rounded,
          color: AppColors.courierAlt,
          actionLabel: 'Xem lịch sử',
          onAction: () {},
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                children: [
                  const Icon(Icons.payments_rounded, color: AppColors.courier),
                  const SizedBox(width: 8),
                  Text(
                    'Tổng tuần này',
                    style: TextStyle(
                      color: AppColors.ink.withOpacity(0.7),
                      fontWeight: FontWeight.w700,
                    ),
                  ),
                  const Spacer(),
                  const Text(
                    '3.200.000đ',
                    style: TextStyle(
                      color: AppColors.courier,
                      fontWeight: FontWeight.w900,
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 8),
              Wrap(
                spacing: 8,
                runSpacing: 8,
                children: const [
                  _HeroChip(icon: Icons.account_balance, label: 'Chuyển khoản'),
                  _HeroChip(icon: Icons.calendar_today, label: 'Chu kỳ: Thứ 6'),
                  _HeroChip(icon: Icons.security, label: 'Bảo hiểm đơn hàng'),
                ],
              ),
            ],
          ),
        ),
        const SizedBox(height: 8),
        _SectionCard(
          title: 'Thông báo',
          subtitle: 'Giao hàng, sự cố, nhắc nhở',
          icon: Icons.notifications_active_rounded,
          color: AppColors.courier,
          actionLabel: 'Thiết lập',
          onAction:
              () => Navigator.of(
                context,
              ).pushNamed(AppRoutes.courierNotifications),
          child: Wrap(
            spacing: 8,
            runSpacing: 8,
            children: const [
              _HeroChip(icon: Icons.list_alt, label: 'Đơn mới'),
              _HeroChip(icon: Icons.warning_amber, label: 'Sự cố'),
              _HeroChip(icon: Icons.star, label: 'Đánh giá'),
            ],
          ),
        ),
        const SizedBox(height: 8),
        _SectionCard(
          title: 'Hỗ trợ',
          subtitle: 'Liên hệ điều phối hoặc CSKH',
          icon: Icons.support_agent_rounded,
          color: AppColors.courier,
          child: Wrap(
            spacing: 10,
            runSpacing: 10,
            children: [
              _QuickButton(
                label: 'Gọi điều phối',
                icon: Icons.call_rounded,
                onTap: () {},
              ),
              _QuickButton(
                label: 'Chatbot',
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

class _MiniStat extends StatelessWidget {
  final String label;
  final String value;

  const _MiniStat({required this.label, required this.value});

  @override
  Widget build(BuildContext context) {
    return Container(
      width: 120,
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: const Color(0xFFF8F9FB),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: AppColors.ink.withOpacity(0.06)),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            label,
            style: TextStyle(
              color: AppColors.ink.withOpacity(0.65),
              fontWeight: FontWeight.w700,
            ),
          ),
          const SizedBox(height: 4),
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

class _InfoRow extends StatelessWidget {
  final String label;
  final String value;
  final Color color;

  const _InfoRow({
    required this.label,
    required this.value,
    required this.color,
  });

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 6),
      child: Row(
        children: [
          CircleAvatar(
            radius: 16,
            backgroundColor: color.withOpacity(0.12),
            child: Icon(Icons.checklist_outlined, color: color, size: 16),
          ),
          const SizedBox(width: 10),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  label,
                  style: TextStyle(
                    color: AppColors.ink.withOpacity(0.65),
                    fontWeight: FontWeight.w700,
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
          ),
        ],
      ),
    );
  }
}

// Shared widgets

class _TabScaffold extends StatelessWidget {
  final double bottomInset;
  final Widget header;
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

class _WelcomeHeader extends StatelessWidget {
  final String name;

  const _WelcomeHeader({required this.name});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(18),
      decoration: BoxDecoration(
        gradient: const LinearGradient(
          colors: [AppColors.courier, AppColors.courierAlt],
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
        children: [
          CircleAvatar(
            radius: 26,
            backgroundColor: Colors.white,
            child: Text(
              name.isNotEmpty ? name[0] : '?',
              style: const TextStyle(
                color: AppColors.courier,
                fontWeight: FontWeight.w800,
                fontSize: 20,
              ),
            ),
          ),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  'Chào, $name!',
                  style: Theme.of(context).textTheme.titleLarge?.copyWith(
                    color: Colors.white,
                    fontWeight: FontWeight.w900,
                    letterSpacing: -0.2,
                  ),
                ),
                Text(
                  'Chúc bạn một ngày giao hàng năng suất 🚚',
                  style: TextStyle(
                    color: Colors.white.withOpacity(0.92),
                    fontWeight: FontWeight.w700,
                  ),
                ),
              ],
            ),
          ),
          Container(
            padding: const EdgeInsets.all(10),
            decoration: BoxDecoration(
              color: Colors.white.withOpacity(0.16),
              borderRadius: BorderRadius.circular(14),
            ),
            child: const Icon(
              Icons.local_shipping_outlined,
              color: Colors.white,
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

class _Task {
  final String title;
  final String time;
  final int stops;
  final String status;
  final Color color;

  const _Task({
    required this.title,
    required this.time,
    required this.stops,
    required this.status,
    required this.color,
  });
}

class _TaskTile extends StatelessWidget {
  final _Task task;

  const _TaskTile({required this.task});

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
              color: task.color.withOpacity(0.12),
              borderRadius: BorderRadius.circular(12),
            ),
            child: const Icon(Icons.route, color: AppColors.courier),
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
                        task.title,
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
                        color: task.color.withOpacity(0.14),
                        borderRadius: BorderRadius.circular(8),
                      ),
                      child: Text(
                        task.status,
                        style: TextStyle(
                          color: task.color,
                          fontWeight: FontWeight.w800,
                          fontSize: 12.5,
                        ),
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 4),
                Text(
                  '${task.time} · ${task.stops} điểm dừng',
                  style: TextStyle(
                    color: AppColors.ink.withOpacity(0.65),
                    fontWeight: FontWeight.w700,
                  ),
                ),
              ],
            ),
          ),
          const Icon(
            Icons.chevron_right_rounded,
            color: AppColors.ink,
            size: 22,
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
        foregroundColor: AppColors.courier,
        side: BorderSide(color: AppColors.courier.withOpacity(0.35)),
        padding: const EdgeInsets.symmetric(vertical: 12, horizontal: 12),
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      ),
      icon: Icon(icon),
      label: Text(label, style: const TextStyle(fontWeight: FontWeight.w800)),
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

class _AssignmentCard extends StatelessWidget {
  final String title;
  final String code;
  final String time;
  final int stops;
  final String status;
  final Color statusColor;
  final VoidCallback onTap;

  const _AssignmentCard({
    required this.title,
    required this.code,
    required this.time,
    required this.stops,
    required this.status,
    required this.statusColor,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return InkWell(
      borderRadius: BorderRadius.circular(14),
      onTap: onTap,
      child: Container(
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
                    color: statusColor.withOpacity(0.12),
                    shape: BoxShape.circle,
                  ),
                  child: const Icon(
                    Icons.delivery_dining_rounded,
                    color: AppColors.courier,
                  ),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        title,
                        style: const TextStyle(
                          color: AppColors.ink,
                          fontWeight: FontWeight.w900,
                          fontSize: 16,
                        ),
                      ),
                      const SizedBox(height: 4),
                      Text(
                        '$code · $time',
                        style: TextStyle(
                          color: AppColors.ink.withOpacity(0.6),
                          fontWeight: FontWeight.w600,
                        ),
                      ),
                    ],
                  ),
                ),
                Container(
                  padding: const EdgeInsets.symmetric(
                    horizontal: 8,
                    vertical: 4,
                  ),
                  decoration: BoxDecoration(
                    color: statusColor.withOpacity(0.14),
                    borderRadius: BorderRadius.circular(8),
                  ),
                  child: Text(
                    status,
                    style: TextStyle(
                      color: statusColor,
                      fontWeight: FontWeight.w800,
                      fontSize: 12.5,
                    ),
                  ),
                ),
              ],
            ),
            const SizedBox(height: 8),
            Row(
              children: [
                const Icon(
                  Icons.pin_drop_outlined,
                  size: 18,
                  color: AppColors.ink,
                ),
                const SizedBox(width: 6),
                Text(
                  '$stops điểm dừng',
                  style: TextStyle(
                    color: AppColors.ink.withOpacity(0.75),
                    fontWeight: FontWeight.w700,
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}

class _StopRow extends StatelessWidget {
  final String title;
  final String detail;
  final String eta;
  final int index;

  const _StopRow({
    required this.title,
    required this.detail,
    required this.eta,
    required this.index,
  });

  @override
  Widget build(BuildContext context) {
    return Row(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        CircleAvatar(
          radius: 18,
          backgroundColor: AppColors.courier.withOpacity(0.12),
          child: Text(
            '$index',
            style: const TextStyle(
              color: AppColors.courier,
              fontWeight: FontWeight.w800,
            ),
          ),
        ),
        const SizedBox(width: 10),
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                title,
                style: const TextStyle(
                  color: AppColors.ink,
                  fontWeight: FontWeight.w800,
                ),
              ),
              const SizedBox(height: 2),
              Text(
                detail,
                style: TextStyle(
                  color: AppColors.ink.withOpacity(0.65),
                  fontWeight: FontWeight.w600,
                ),
              ),
              const SizedBox(height: 2),
              Text(
                'ETA: $eta',
                style: TextStyle(
                  color: AppColors.ink.withOpacity(0.6),
                  fontWeight: FontWeight.w600,
                ),
              ),
            ],
          ),
        ),
        IconButton(
          onPressed:
              () => Navigator.of(context).pushNamed(AppRoutes.courierRouteMap),
          icon: const Icon(Icons.navigation_rounded),
          color: AppColors.courier,
        ),
      ],
    );
  }
}
