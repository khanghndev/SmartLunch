import 'package:flutter/material.dart';
import '../../../../app/app_routes.dart';
import '../../../../core/theme/app_design_system.dart';
import '../../../../core/widgets/dashboard_format.dart';
import '../../../../core/widgets/premium_drawer.dart';
import '../../../../core/widgets/role_dashboard.dart';
import '../../../profile/presentation/pages/profile_page.dart';
import '../../../auth/data/repositories/auth_repository.dart';
import '../../../profile/data/profile_repository.dart';
import '../../../profile/data/models/user_profile_model.dart';
import '../../data/models/shipper_delivery_models.dart';
import '../../data/repositories/shipper_repository.dart';

class ShipperHomePage extends StatefulWidget {
  const ShipperHomePage({super.key});

  @override
  State<ShipperHomePage> createState() => _ShipperHomePageState();
}

class _ShipperHomePageState extends State<ShipperHomePage> {
  final GlobalKey<ScaffoldState> _scaffoldKey = GlobalKey<ScaffoldState>();
  int _currentIndex = 0;

  UserProfileModel? _profile;
  int _todayTotal = 0;
  int _pendingCount = 0;
  int _inTransitCount = 0;
  int _completedCount = 0;
  int _todayMeals = 0;
  List<DashboardRecentItem> _recentItems = const [];
  bool _isLoading = true;
  String? _loadError;

  @override
  void initState() {
    super.initState();
    _loadDashboardData();
  }

  Future<void> _loadDashboardData() async {
    setState(() {
      _isLoading = true;
      _loadError = null;
    });

    try {
      final profile = await ProfileRepository.instance.getProfile();
      final today = DateTime.now();
      final deliveries = await ShipperRepository.instance.getDeliveries(
        scheduledOn: today,
        pageSize: 100,
      );

      int pending = 0;
      int inTransit = 0;
      int completed = 0;
      int meals = 0;

      for (final d in deliveries.data) {
        meals += d.mealCount;
        final s = d.deliveryStatus.toLowerCase();
        if (s == 'completed') {
          completed++;
        } else if (s == 'in_transit') {
          inTransit++;
        } else if (s != 'failed' && s != 'rejected') {
          pending++;
        }
      }

      final recent = deliveries.data.take(3).map(_toRecentItem).toList();

      if (!mounted) return;
      setState(() {
        _profile = profile;
        _todayTotal = deliveries.totalCount;
        _pendingCount = pending;
        _inTransitCount = inTransit;
        _completedCount = completed;
        _todayMeals = meals;
        _recentItems = recent;
        _isLoading = false;
      });
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _isLoading = false;
        _loadError = 'Không tải được đơn giao hôm nay. Kéo xuống để thử lại.';
      });
    }
  }

  DashboardRecentItem _toRecentItem(ShipperDeliveryListItemModel d) {
    return DashboardRecentItem(
      title: 'Đơn #${d.orderId}',
      subtitle: d.deliveryAddress,
      trailing: shipperDeliveryStatusLabelVi(d.deliveryStatus),
      icon: Icons.local_shipping_outlined,
      iconColor: RolePalette.shipper.primary,
      route: AppRoutes.shipperDeliveryList,
    );
  }

  double get _completionRate =>
      _todayTotal == 0 ? 0 : (_completedCount / _todayTotal) * 100;

  List<Widget> get _pages => [
        _buildShipperDashboard(),
        ProfilePage(onMenuPressed: () => _scaffoldKey.currentState?.openDrawer()),
      ];

  Widget _buildShipperDashboard() {
    final shipper = RolePalette.shipper;
    final name = _profile?.displayName ?? 'Shipper';

    return Scaffold(
      appBar: AppBar(
        leading: IconButton(
          icon: const Icon(Icons.menu),
          onPressed: () => _scaffoldKey.currentState?.openDrawer(),
        ),
        title: const Text('Dashboard', style: TextStyle(fontWeight: FontWeight.bold)),
        backgroundColor: shipper.primary,
        foregroundColor: Colors.white,
        elevation: 0,
      ),
      body: RoleDashboardBody(
        title: 'Xin chào, $name',
        subtitle: 'Tổng quan giao hàng hôm nay',
        icon: Icons.local_shipping_rounded,
        accent: shipper.primary,
        accentAlt: shipper.primaryAlt,
        isLoading: _isLoading,
        errorMessage: _loadError,
        onRefresh: _loadDashboardData,
        topActions: [
          DashboardTopAction(
            icon: Icons.notifications_rounded,
            tooltip: 'Thông báo',
            route: AppRoutes.shipperNotifications,
          ),
        ],
        quickActions: const [
          DashboardQuickAction(
            label: 'Đơn cần giao',
            icon: Icons.list_alt_rounded,
            route: AppRoutes.shipperDeliveryList,
          ),
          DashboardQuickAction(
            label: 'Bản đồ',
            icon: Icons.map_rounded,
            route: AppRoutes.shipperRouteMap,
          ),
        ],
        summaryMetrics: [
          DashboardMetricCard(
            title: 'Đơn hôm nay',
            value: _isLoading ? '—' : '$_todayTotal',
            icon: Icons.inventory_2_outlined,
            color: shipper.primary,
            trend: _isLoading ? null : '$_todayMeals suất',
            trendPositive: true,
          ),
          DashboardMetricCard(
            title: 'Chờ / đang giao',
            value: _isLoading ? '—' : '${_pendingCount + _inTransitCount}',
            icon: Icons.local_shipping_outlined,
            color: AppDesignSystem.warning,
            trend: _isLoading ? null : '$_pendingCount chờ · $_inTransitCount đang giao',
            trendPositive: (_pendingCount + _inTransitCount) <= 3,
          ),
          DashboardMetricCard(
            title: 'Hoàn thành',
            value: _isLoading ? '—' : '$_completedCount',
            icon: Icons.check_circle_outline_rounded,
            color: AppDesignSystem.success,
            trend: _isLoading ? null : formatDashboardPercent(_completionRate),
            trendPositive: _completionRate >= 80,
          ),
          DashboardMetricCard(
            title: 'Đang trên đường',
            value: _isLoading ? '—' : '$_inTransitCount',
            icon: Icons.route_rounded,
            color: shipper.primaryAlt,
            trend: _isLoading ? null : 'Theo lịch hôm nay',
            trendPositive: true,
          ),
        ],
        recentItems: _recentItems,
        features: [
          DashboardFeature(
            title: 'Danh sách Đơn Hàng',
            subtitle: 'Các đơn đang chờ và đang giao',
            icon: Icons.inventory_2_rounded,
            color: shipper.primary,
            route: AppRoutes.shipperDeliveryList,
            preview: _isLoading ? null : '$_pendingCount đơn cần xử lý',
          ),
          DashboardFeature(
            title: 'Lịch trình & Lộ trình',
            subtitle: 'Bản đồ tối ưu đường đi',
            icon: Icons.map_rounded,
            color: AppDesignSystem.success,
            route: AppRoutes.shipperRouteMap,
            preview: _isLoading ? null : '$_inTransitCount đơn đang giao',
          ),
          DashboardFeature(
            title: 'Lịch sử Giao hàng',
            subtitle: 'Đơn đã hoàn thành hoặc thất bại',
            icon: Icons.history_rounded,
            color: AppDesignSystem.gray700,
            route: AppRoutes.shipperHistory,
            preview: _isLoading ? null : '$_completedCount đơn hoàn tất hôm nay',
          ),
          DashboardFeature(
            title: 'Thực đơn tuần',
            subtitle: 'Xem món theo kỳ phục vụ',
            icon: Icons.restaurant_menu_rounded,
            color: AppDesignSystem.gray700,
            route: AppRoutes.shipperWeeklyMenu,
          ),
        ],
      ),
    );
  }

  Future<void> _handleLogout() async {
    final confirm = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Xác nhận đăng xuất'),
        content: const Text('Bạn có chắc chắn muốn đăng xuất khỏi tài khoản này không?'),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(false),
            child: const Text('Hủy', style: TextStyle(color: Colors.grey)),
          ),
          TextButton(
            onPressed: () => Navigator.of(context).pop(true),
            child: const Text('Đăng xuất', style: TextStyle(color: Colors.redAccent)),
          ),
        ],
      ),
    );

    if (confirm == true) {
      await AuthRepository.instance.clearSession();
      if (mounted) {
        Navigator.of(context).pushNamedAndRemoveUntil(
          AppRoutes.login,
          (route) => false,
        );
      }
    }
  }

  void _onDrawerNavigate(String route) {
    if (route == AppRoutes.login) {
      _handleLogout();
    } else {
      Navigator.of(context).pushNamed(route);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      key: _scaffoldKey,
      drawer: PremiumDrawer(
        userName: _profile?.displayName ?? 'Đang tải...',
        userRole: 'Nhân viên giao hàng',
        roleBadge: 'Shipper',
        gradient: RolePalette.shipper.gradient,
        accentColor: RolePalette.shipper.primary,
        selectedIndex: _currentIndex,
        onSelectTab: (index) {
          if (index < _pages.length) {
            setState(() => _currentIndex = index);
          }
        },
        onNavigate: _onDrawerNavigate,
        onLogout: _handleLogout,
        sections: const [
          DrawerSection(
            title: 'Điều phối',
            items: [
              DrawerItem(
                icon: Icons.dashboard_rounded,
                label: 'dashboard',
                labelVi: 'Dashboard',
                tabIndex: 0,
              ),
              DrawerItem(
                icon: Icons.list_alt_rounded,
                label: 'deliveries',
                labelVi: 'Danh sách đơn hàng',
                route: AppRoutes.shipperDeliveryList,
              ),
              DrawerItem(
                icon: Icons.map_rounded,
                label: 'route_map',
                labelVi: 'Bản đồ tuyến đường',
                route: AppRoutes.shipperRouteMap,
              ),
            ],
          ),
          DrawerSection(
            title: 'Công việc',
            items: [
              DrawerItem(
                icon: Icons.restaurant_menu_rounded,
                label: 'weekly_menu',
                labelVi: 'Thực đơn tuần',
                route: AppRoutes.shipperWeeklyMenu,
              ),
              DrawerItem(
                icon: Icons.calendar_month_rounded,
                label: 'schedule',
                labelVi: 'Lịch trình giao',
                route: AppRoutes.shipperSchedule,
              ),
              DrawerItem(
                icon: Icons.history_rounded,
                label: 'history',
                labelVi: 'Lịch sử giao hàng',
                route: AppRoutes.shipperHistory,
              ),
            ],
          ),
          DrawerSection(
            title: 'Cá nhân',
            items: [
              DrawerItem(
                icon: Icons.person_rounded,
                label: 'profile',
                labelVi: 'Hồ sơ cá nhân',
                tabIndex: 1,
              ),
            ],
          ),
        ],
      ),
      body: IndexedStack(index: _currentIndex, children: _pages),
      bottomNavigationBar: Container(
        decoration: BoxDecoration(
          color: Colors.white,
          boxShadow: [
            BoxShadow(
              color: Colors.black.withValues(alpha: 0.05),
              blurRadius: 20,
              offset: const Offset(0, -5),
            ),
          ],
        ),
        child: SafeArea(
          child: BottomNavigationBar(
            currentIndex: _currentIndex,
            onTap: (index) => setState(() => _currentIndex = index),
            type: BottomNavigationBarType.fixed,
            backgroundColor: Colors.white,
            selectedItemColor: RolePalette.shipper.primary,
            unselectedItemColor: AppDesignSystem.gray400,
            elevation: 0,
            items: const [
              BottomNavigationBarItem(
                icon: Icon(Icons.local_shipping_outlined),
                activeIcon: Icon(Icons.local_shipping_rounded),
                label: 'Vận chuyển',
              ),
              BottomNavigationBarItem(
                icon: Icon(Icons.person_outline_rounded),
                activeIcon: Icon(Icons.person_rounded),
                label: 'Hồ sơ',
              ),
            ],
          ),
        ),
      ),
    );
  }
}
