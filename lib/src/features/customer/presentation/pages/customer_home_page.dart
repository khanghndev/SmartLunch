import 'package:flutter/material.dart';

import '../../../../app/app_routes.dart';
import '../../../../core/constants/app_colors.dart';
import '../../../../core/widgets/premium_bottom_nav.dart';
import '../../../../core/widgets/premium_drawer.dart';
import '../tabs/dashboard_tab.dart';
import '../tabs/menu_tab.dart';
import '../../order/presentation/pages/order_page.dart';
import '../../payment/presentation/pages/payment_page.dart';
import '../../../profile/presentation/pages/profile_page.dart';

class CustomerHomePage extends StatefulWidget {
  const CustomerHomePage({super.key});

  @override
  State<CustomerHomePage> createState() => _CustomerHomePageState();
}

class _CustomerHomePageState extends State<CustomerHomePage> {
  static const double _navHeight = 74;
  int _selectedIndex = 0;
  final _drawerController = GlobalKey<ScaffoldState>();

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
      'Thực đơn',
      'Đơn hàng',
      'Thanh toán',
      'Hồ sơ',
    ];
    final tabs = [
      DashboardTab(bottomInset: bottomInset),
      MenuTab(bottomInset: bottomInset),
      OrderPage(embedded: true, bottomInset: bottomInset),
      PaymentPage(embedded: true, bottomInset: bottomInset),
      ProfilePage(embedded: true, bottomInset: bottomInset),
    ];

    return Scaffold(
      extendBody: true,
      key: _drawerController,
      appBar: AppBar(
        leading: IconButton(
          icon: const Icon(Icons.menu_rounded),
          onPressed: () => _drawerController.currentState?.openDrawer(),
        ),
        title: Text(tabTitles[_selectedIndex]),
        centerTitle: true,
        backgroundColor: Colors.white,
        foregroundColor: AppColors.ink,
        elevation: 0,
        surfaceTintColor: Colors.white,
        actions: [
          IconButton(
            icon: const Icon(Icons.notifications_none_rounded),
            onPressed: () => Navigator.of(context).pushNamed(AppRoutes.notifications),
          ),
          IconButton(
            icon: const Icon(Icons.person_outline),
            onPressed: () => Navigator.of(context).pushNamed(AppRoutes.profile),
          ),
          const SizedBox(width: 4),
        ],
      ),
      drawer: PremiumDrawer(
        userName: 'Khách hàng',
        userRole: 'Individual account',
        roleBadge: 'Khách hàng cá nhân',
        gradient: const [AppColors.customer, AppColors.customerAlt],
        accentColor: AppColors.customer,
        selectedIndex: _selectedIndex,
        onSelectTab: _onSelectTab,
        onNavigate: (route) => Navigator.of(context).pushNamed(route),
        onLogout: () => Navigator.of(context)
            .pushNamedAndRemoveUntil(AppRoutes.welcome, (route) => false),
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
                icon: Icons.menu_book_rounded,
                label: 'Menu',
                labelVi: 'Thực đơn',
                tabIndex: 1,
              ),
              DrawerItem(
                icon: Icons.shopping_bag_rounded,
                label: 'Orders',
                labelVi: 'Đơn hàng',
                tabIndex: 2,
              ),
              DrawerItem(
                icon: Icons.payments_rounded,
                label: 'Payments',
                labelVi: 'Thanh toán',
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
            title: 'Đơn hàng & Hỗ trợ',
            items: [
              DrawerItem(
                icon: Icons.history_rounded,
                label: 'Order history',
                labelVi: 'Lịch sử đơn hàng',
                route: AppRoutes.customerHistory,
              ),
              DrawerItem(
                icon: Icons.star_rounded,
                label: 'Rate meals',
                labelVi: 'Đánh giá món ăn',
                route: AppRoutes.customerRating,
              ),
              DrawerItem(
                icon: Icons.notifications_rounded,
                label: 'Notifications',
                labelVi: 'Thông báo',
                route: AppRoutes.notifications,
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
      body: IndexedStack(
        index: _selectedIndex,
        children: tabs,
      ),
      bottomNavigationBar: PremiumBottomNav(
        selectedIndex: _selectedIndex,
        onTap: _onSelectTab,
        accentColor: AppColors.customer,
        items: const [
          NavItem(
            icon: Icons.home_outlined,
            selectedIcon: Icons.home_rounded,
            label: 'Home',
            labelVi: 'Trang chủ',
          ),
          NavItem(
            icon: Icons.menu_book_outlined,
            selectedIcon: Icons.menu_book_rounded,
            label: 'Menu',
            labelVi: 'Thực đơn',
          ),
          NavItem(
            icon: Icons.shopping_bag_outlined,
            selectedIcon: Icons.shopping_bag_rounded,
            label: 'Orders',
            labelVi: 'Đơn hàng',
          ),
          NavItem(
            icon: Icons.payments_outlined,
            selectedIcon: Icons.payments_rounded,
            label: 'Payments',
            labelVi: 'Thanh toán',
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

class _StatusChips extends StatelessWidget {
  const _StatusChips();

  @override
  Widget build(BuildContext context) {
    final chips = [
      const _StatusChip(
        icon: Icons.timelapse_rounded,
        label: 'Còn 25 phút để đặt bữa trưa',
        color: AppColors.customer,
      ),
      const _StatusChip(
        icon: Icons.redeem_rounded,
        label: 'Điểm thưởng: 120',
        color: Color(0xFF1F3C88),
      ),
      const _StatusChip(
        icon: Icons.location_on_outlined,
        label: 'Địa điểm: Văn phòng Q1',
        color: Color(0xFFF1AE62),
      ),
    ];

    return Wrap(
      spacing: 10,
      runSpacing: 10,
      children: chips,
    );
  }
}

class _StatusChip extends StatelessWidget {
  final IconData icon;
  final String label;
  final Color color;

  const _StatusChip({
    required this.icon,
    required this.label,
    required this.color,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 10),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(14),
        border: Border.all(
          color: color.withOpacity(0.16),
          width: 1.2,
        ),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withOpacity(0.03),
            blurRadius: 12,
            offset: const Offset(0, 6),
          ),
        ],
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Container(
            padding: const EdgeInsets.all(8),
            decoration: BoxDecoration(
              color: color.withOpacity(0.12),
              borderRadius: BorderRadius.circular(10),
            ),
            child: Icon(
              icon,
              size: 16,
              color: color,
            ),
          ),
          const SizedBox(width: 10),
          Text(
            label,
            style: TextStyle(
              color: AppColors.ink.withOpacity(0.85),
              fontWeight: FontWeight.w700,
              fontSize: 13,
            ),
          ),
        ],
      ),
    );
  }
}


class _CustomerTabPage extends StatelessWidget {
  final String title;
  final String subtitle;
  final IconData icon;
  final Color accent;
  final Color accentAlt;
  final double bottomInset;
  final List<_CustomerTabAction> actions;

  const _CustomerTabPage({
    required this.title,
    required this.subtitle,
    required this.icon,
    required this.accent,
    required this.accentAlt,
    required this.bottomInset,
    required this.actions,
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
          primary: false,
          physics: const BouncingScrollPhysics(),
          padding: EdgeInsets.fromLTRB(20, 16, 20, 24 + bottomInset),
          children: [
            _CustomerTabHeader(
              title: title,
              subtitle: subtitle,
              icon: icon,
              accent: accent,
              accentAlt: accentAlt,
            ),
            const SizedBox(height: 16),
            ...actions.map((action) => _CustomerActionCard(action: action)),
          ],
        ),
      ),
    );
  }
}

class _CustomerTabAction {
  final String title;
  final String subtitle;
  final IconData icon;
  final Color color;
  final String route;

  const _CustomerTabAction({
    required this.title,
    required this.subtitle,
    required this.icon,
    required this.color,
    required this.route,
  });
}

class _CustomerActionCard extends StatelessWidget {
  final _CustomerTabAction action;

  const _CustomerActionCard({required this.action});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 14),
      child: InkWell(
        borderRadius: BorderRadius.circular(20),
        onTap: () => Navigator.of(context).pushNamed(action.route),
        child: Ink(
          decoration: BoxDecoration(
            color: Colors.white,
            borderRadius: BorderRadius.circular(20),
            boxShadow: [
              BoxShadow(
                color: Colors.black.withOpacity(0.06),
                blurRadius: 16,
                offset: const Offset(0, 8),
              ),
            ],
          ),
          child: Padding(
            padding: const EdgeInsets.all(16),
            child: Row(
              children: [
                Container(
                  width: 46,
                  height: 46,
                  decoration: BoxDecoration(
                    color: action.color.withOpacity(0.15),
                    borderRadius: BorderRadius.circular(14),
                  ),
                  child: Icon(action.icon, color: action.color),
                ),
                const SizedBox(width: 14),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        action.title,
                        style:
                            Theme.of(context).textTheme.titleMedium?.copyWith(
                                  fontWeight: FontWeight.w700,
                                ),
                      ),
                      const SizedBox(height: 4),
                      Text(
                        action.subtitle,
                        style:
                            Theme.of(context).textTheme.bodyMedium?.copyWith(
                                  color: Colors.black.withOpacity(0.6),
                                ),
                      ),
                    ],
                  ),
                ),
                Icon(Icons.arrow_forward, color: action.color),
              ],
            ),
          ),
        ),
      ),
    );
  }
}

class _CustomerTabHeader extends StatelessWidget {
  final String title;
  final String subtitle;
  final IconData icon;
  final Color accent;
  final Color accentAlt;

  const _CustomerTabHeader({
    required this.title,
    required this.subtitle,
    required this.icon,
    required this.accent,
    required this.accentAlt,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(20),
      decoration: BoxDecoration(
        gradient: LinearGradient(
          colors: [accent, accentAlt],
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
        ),
        borderRadius: BorderRadius.circular(24),
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
        ],
      ),
    );
  }
}
