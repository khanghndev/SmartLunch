import 'package:flutter/material.dart';
import '../../../../app/app_routes.dart';
import '../../../../core/theme/app_design_system.dart';
import '../../../../core/widgets/dashboard_format.dart';
import '../../../../core/widgets/role_dashboard.dart';
import '../../../../core/widgets/role_module_header.dart';
import '../../../../core/widgets/role_module_shell.dart';
import '../widgets/shipper_shell.dart';
import '../../../profile/presentation/pages/profile_page.dart';
import '../../../../core/widgets/app_confirm_dialog.dart';
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
      route: AppRoutes.shipperDeliveryDetail,
      routeArguments: d.deliveryId,
    );
  }

  double get _completionRate =>
      _todayTotal == 0 ? 0 : (_completedCount / _todayTotal) * 100;

  Widget _buildDashboardTab(BuildContext context) {
    final shipper = kShipperRoleShell;
    final name = _profile?.displayName ?? 'Shipper';

    return RoleModuleTabPage(
      role: shipper,
      headerTitle: 'Dashboard',
      headerSubtitle: 'Xin chào, $name · Tổng quan giao hàng hôm nay',
      trustPill: 'Giao hàng',
      headerTrailing: RoleModuleIconButton(
        icon: Icons.notifications_rounded,
        onTap: () => Navigator.of(context).pushNamed(AppRoutes.shipperNotifications),
      ),
      headerBottomPanel: RoleHeaderQuickActionsPanel(
        actions: [
          RoleHeaderQuickAction(
            label: 'Đơn cần giao',
            icon: Icons.list_alt_rounded,
            color: shipper.primary,
            onTap: () => Navigator.of(context).pushNamed(AppRoutes.shipperDeliveryList),
          ),
          RoleHeaderQuickAction(
            label: 'Bản đồ',
            icon: Icons.map_rounded,
            color: AppDesignSystem.success,
            onTap: () => Navigator.of(context).pushNamed(AppRoutes.shipperRouteMap),
          ),
          RoleHeaderQuickAction(
            label: 'Lịch giao',
            icon: Icons.calendar_month_rounded,
            color: shipper.primaryAlt,
            onTap: () => Navigator.of(context).pushNamed(AppRoutes.shipperSchedule),
          ),
        ],
      ),
      body: RoleDashboardBody(
        title: 'Xin chào, $name',
        subtitle: 'Tổng quan giao hàng hôm nay',
        icon: Icons.local_shipping_rounded,
        accent: shipper.primary,
        accentAlt: shipper.primaryAlt,
        showInternalHeader: false,
        isLoading: _isLoading,
        errorMessage: _loadError,
        onRefresh: _loadDashboardData,
        topActions: const [],
        quickActions: const [],
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
        ],
      ),
    );
  }

  Widget _buildProfileTab() {
    final email = _profile?.email ?? 'Tài khoản giao hàng';
    return RoleModuleTabPage(
      role: kShipperRoleShell,
      headerTitle: 'Hồ sơ cá nhân',
      headerSubtitle: email,
      trustPill: 'Giao hàng',
      body: const ProfilePage(embeddedInModuleShell: true),
    );
  }

  Future<void> _handleLogout() async {
    await performAppLogout(context, role: kShipperRoleShell);
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
    return RoleModuleTabShell(
      scaffoldKey: _scaffoldKey,
      role: kShipperRoleShell,
      currentIndex: _currentIndex,
      onIndexChanged: (i) => setState(() => _currentIndex = i),
      drawerUserName: _profile?.displayName ?? 'Đang tải...',
      drawerUserRole: ShipperShellConfig.drawerUserRole,
      drawerRoleBadge: ShipperShellConfig.roleBadge,
      drawerSections: ShipperShellConfig.drawerSections,
      onLogout: _handleLogout,
      onDrawerNavigate: _onDrawerNavigate,
      navItems: ShipperShellConfig.navItems,
      tabs: [
        _buildDashboardTab(context),
        _buildProfileTab(),
      ],
    );
  }
}
