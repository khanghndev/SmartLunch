import 'package:flutter/material.dart';
import '../../../../app/app_routes.dart';
import '../../../../core/analytics/meal_statistics_repository.dart';
import '../../../../core/theme/app_design_system.dart';
import '../../../../core/widgets/premium_drawer.dart';
import '../../../../core/widgets/role_dashboard.dart';
import '../../../profile/presentation/pages/profile_page.dart';
import '../../../auth/data/repositories/auth_repository.dart';
import '../../../profile/data/profile_repository.dart';
import '../../../profile/data/models/user_profile_model.dart';
import '../../data/models/finance_models.dart';
import '../../data/models/review_complaint_models.dart';
import '../../../../core/widgets/dashboard_format.dart';
import '../../data/repositories/manager_repository.dart';

class ManagerHomePage extends StatefulWidget {
  const ManagerHomePage({super.key});

  @override
  State<ManagerHomePage> createState() => _ManagerHomePageState();
}

class _ManagerHomePageState extends State<ManagerHomePage> {
  final GlobalKey<ScaffoldState> _scaffoldKey = GlobalKey<ScaffoldState>();
  int _currentIndex = 0;

  UserProfileModel? _profile;
  double _totalRevenue = 0;
  int _totalMeals = 0;
  int _todayMeals = 0;
  int _complaintCount = 0;
  int _pendingReconciliation = 0;
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
      final now = DateTime.now();
      final monthStart = DateTime(now.year, now.month, 1);
      final todayStart = DateTime(now.year, now.month, now.day);

      final results = await Future.wait([
        ManagerRepository.instance.getCashFlowSummary(
          startDate: monthStart,
          endDate: now,
          granularity: 'Month',
        ),
        MealStatisticsRepository.instance.getMealStatistics(
          startDate: monthStart,
          endDate: now,
        ),
        MealStatisticsRepository.instance.getMealStatistics(
          startDate: todayStart,
          endDate: now,
        ),
        ManagerRepository.instance.getComplaints(page: 1, pageSize: 5),
        ManagerRepository.instance.getPaymentReconciliation(
          startDate: monthStart,
          endDate: now,
        ),
      ]);

      final cashFlow = results[0] as CashFlowSummaryModel;
      final meals = results[1] as List<MealStatisticItemModel>;
      final todayMeals = results[2] as List<MealStatisticItemModel>;
      final complaints = results[3] as ComplaintPageModel;
      final reconciliation = results[4] as PaymentReconciliationModel;

      final recent = complaints.items
          .take(3)
          .map(
            (c) => DashboardRecentItem(
              title: 'Khiếu nại #${c.id}',
              subtitle: c.title,
              trailing: c.status.label,
              icon: Icons.feedback_outlined,
              iconColor: RolePalette.manager.primary,
              route: AppRoutes.managerFeedbackComplaints,
            ),
          )
          .toList();

      if (!mounted) return;
      setState(() {
        _profile = profile;
        _totalRevenue = cashFlow.totalProfit;
        _totalMeals = meals.fold<int>(0, (a, b) => a + b.totalMeals);
        _todayMeals = todayMeals.fold<int>(0, (a, b) => a + b.totalMeals);
        _complaintCount = complaints.totalCount;
        _pendingReconciliation = reconciliation.items
            .where(
              (l) =>
                  l.status == ReconciliationStatus.pending ||
                  l.status == ReconciliationStatus.disputed,
            )
            .length;
        _recentItems = recent;
        _isLoading = false;
      });
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _isLoading = false;
        _loadError = 'Không tải được dữ liệu quản trị. Kéo xuống để thử lại.';
      });
    }
  }

  List<Widget> get _pages => [
        _buildManagerDashboard(),
        ProfilePage(onMenuPressed: () => _scaffoldKey.currentState?.openDrawer()),
      ];

  Widget _buildManagerDashboard() {
    final mgr = RolePalette.manager;
    final name = _profile?.displayName ?? 'Quản lý';

    return Scaffold(
      appBar: AppBar(
        leading: IconButton(
          icon: const Icon(Icons.menu),
          onPressed: () => _scaffoldKey.currentState?.openDrawer(),
        ),
        title: const Text('Dashboard', style: TextStyle(fontWeight: FontWeight.bold)),
        backgroundColor: mgr.primary,
        foregroundColor: Colors.white,
        elevation: 0,
      ),
      body: RoleDashboardBody(
        title: 'Xin chào, $name',
        subtitle: 'Tổng quan vận hành & tài chính',
        icon: Icons.admin_panel_settings_rounded,
        accent: mgr.primary,
        accentAlt: mgr.primaryAlt,
        isLoading: _isLoading,
        errorMessage: _loadError,
        onRefresh: _loadDashboardData,
        topActions: [
          DashboardTopAction(
            icon: Icons.notifications_rounded,
            tooltip: 'Thông báo',
            route: AppRoutes.notifications,
          ),
        ],
        quickActions: const [
          DashboardQuickAction(
            label: 'Thống kê',
            icon: Icons.bar_chart_rounded,
            route: AppRoutes.managerStatistics,
          ),
          DashboardQuickAction(
            label: 'Thu chi',
            icon: Icons.attach_money_rounded,
            route: AppRoutes.managerCashFlow,
          ),
        ],
        summaryMetrics: [
          DashboardMetricCard(
            title: 'Lợi nhuận tháng',
            value: _isLoading ? '—' : formatDashboardVnd(_totalRevenue),
            icon: Icons.trending_up_rounded,
            color: AppDesignSystem.success,
            trend: _isLoading ? null : 'Tháng ${DateTime.now().month}',
            trendPositive: _totalRevenue >= 0,
          ),
          DashboardMetricCard(
            title: 'Suất ăn tháng',
            value: _isLoading ? '—' : '$_totalMeals',
            icon: Icons.restaurant_rounded,
            color: mgr.primary,
            trend: _isLoading ? null : 'Hôm nay: $_todayMeals',
            trendPositive: true,
          ),
          DashboardMetricCard(
            title: 'Khiếu nại',
            value: _isLoading ? '—' : '$_complaintCount',
            icon: Icons.warning_amber_rounded,
            color: AppDesignSystem.warning,
            trend: _isLoading ? null : 'Cần xử lý',
            trendPositive: _complaintCount == 0,
          ),
          DashboardMetricCard(
            title: 'Đối soát chờ',
            value: _isLoading ? '—' : '$_pendingReconciliation',
            icon: Icons.receipt_long_outlined,
            color: AppDesignSystem.info,
            trend: _isLoading
                ? null
                : (_pendingReconciliation > 0 ? 'Cần đối soát' : 'Đã khớp'),
            trendPositive: _pendingReconciliation == 0,
          ),
        ],
        recentItems: _recentItems,
        features: [
          DashboardFeature(
            title: 'Báo cáo Thống kê',
            subtitle: 'Xem dữ liệu suất ăn, doanh thu',
            icon: Icons.pie_chart_rounded,
            color: mgr.primary,
            route: AppRoutes.managerStatistics,
            preview: _isLoading ? null : '$_totalMeals suất / tháng',
          ),
          DashboardFeature(
            title: 'Quản lý Thu Chi',
            subtitle: 'Theo dõi tài chính dòng tiền',
            icon: Icons.account_balance_wallet_rounded,
            color: AppDesignSystem.success,
            route: AppRoutes.managerCashFlow,
            preview: _isLoading ? null : formatDashboardVnd(_totalRevenue),
          ),
          DashboardFeature(
            title: 'Đối soát Thanh toán',
            subtitle: 'Công nợ tổ chức & NCC',
            icon: Icons.receipt_long_rounded,
            color: AppDesignSystem.info,
            route: AppRoutes.managerReconciliation,
            preview: _isLoading ? null : '$_pendingReconciliation phiếu chờ',
          ),
          DashboardFeature(
            title: 'Phản hồi & Khiếu nại',
            subtitle: 'Đánh giá suất ăn, dịch vụ',
            icon: Icons.feedback_rounded,
            color: AppDesignSystem.warning,
            route: AppRoutes.managerFeedbackComplaints,
            preview: _isLoading ? null : '$_complaintCount vụ việc',
          ),
          DashboardFeature(
            title: 'Xuất Báo cáo',
            subtitle: 'Excel / PDF hệ thống',
            icon: Icons.file_download_rounded,
            color: mgr.primaryAlt,
            route: AppRoutes.managerReports,
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
        userRole: 'Quản lý hệ thống',
        roleBadge: 'Manager',
        gradient: RolePalette.manager.gradient,
        accentColor: RolePalette.manager.primary,
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
            title: 'Quản trị chính',
            items: [
              DrawerItem(
                icon: Icons.dashboard_rounded,
                label: 'dashboard',
                labelVi: 'Dashboard',
                tabIndex: 0,
              ),
              DrawerItem(
                icon: Icons.bar_chart_rounded,
                label: 'statistics',
                labelVi: 'Thống kê tổng quan',
                route: AppRoutes.managerStatistics,
              ),
              DrawerItem(
                icon: Icons.attach_money_rounded,
                label: 'cash_flow',
                labelVi: 'Quản lý thu chi',
                route: AppRoutes.managerCashFlow,
              ),
              DrawerItem(
                icon: Icons.receipt_long_rounded,
                label: 'reconciliation',
                labelVi: 'Đối soát thanh toán',
                route: AppRoutes.managerReconciliation,
              ),
            ],
          ),
          DrawerSection(
            title: 'Vận hành',
            items: [
              DrawerItem(
                icon: Icons.feedback_rounded,
                label: 'feedback',
                labelVi: 'Phản hồi & Khiếu nại',
                route: AppRoutes.managerFeedbackComplaints,
              ),
              DrawerItem(
                icon: Icons.file_download_rounded,
                label: 'reports',
                labelVi: 'Xuất báo cáo',
                route: AppRoutes.managerReports,
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
            selectedItemColor: RolePalette.manager.primary,
            unselectedItemColor: AppDesignSystem.gray400,
            elevation: 0,
            items: const [
              BottomNavigationBarItem(
                icon: Icon(Icons.dashboard_outlined),
                activeIcon: Icon(Icons.dashboard_rounded),
                label: 'Dashboard',
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
