import 'package:flutter/material.dart';

import '../../../../app/app_routes.dart';
import '../../../../core/analytics/meal_statistics_repository.dart';
import '../../../../core/theme/app_design_system.dart';
import '../../../../core/widgets/dashboard_format.dart';
import '../../../../core/widgets/role_dashboard.dart';
import '../../../../core/widgets/role_module_header.dart';
import '../../../../core/widgets/role_module_shell.dart';
import '../widgets/manager_shell.dart';
import '../../../../core/widgets/app_confirm_dialog.dart';
import '../../../profile/data/models/user_profile_model.dart';
import '../../../profile/data/profile_repository.dart';
import '../../../profile/presentation/pages/profile_page.dart';
import '../../data/models/finance_models.dart';
import '../../data/models/review_complaint_models.dart';
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

  Future<void> _handleLogout() async {
    await performAppLogout(context, role: kManagerRole);
  }

  void _onDrawerNavigate(String route) {
    if (route == AppRoutes.login) {
      _handleLogout();
    } else {
      Navigator.of(context).pushNamed(route);
    }
  }

  Widget _buildDashboardTab(BuildContext context) {
    final mgr = kManagerRole;
    final name = _profile?.displayName ?? 'Quản lý';

    return RoleModuleTabPage(
      role: mgr,
      headerTitle: 'Dashboard',
      headerSubtitle: 'Xin chào, $name · Tổng quan vận hành & tài chính',
      trustPill: 'Quản trị',
      headerTrailing: RoleModuleIconButton(
        icon: Icons.notifications_rounded,
        onTap: () => Navigator.of(context).pushNamed(AppRoutes.notifications),
      ),
      headerBottomPanel: RoleHeaderQuickActionsPanel(
        actions: [
          RoleHeaderQuickAction(
            label: 'Thống kê',
            icon: Icons.bar_chart_rounded,
            color: mgr.primary,
            onTap: () => Navigator.of(context).pushNamed(AppRoutes.managerStatistics),
          ),
          RoleHeaderQuickAction(
            label: 'Thu chi',
            icon: Icons.attach_money_rounded,
            color: AppDesignSystem.success,
            onTap: () => Navigator.of(context).pushNamed(AppRoutes.managerCashFlow),
          ),
        ],
      ),
      body: RoleDashboardBody(
          title: 'Xin chào, $name',
          subtitle: 'Tổng quan vận hành & tài chính',
          icon: Icons.admin_panel_settings_rounded,
          accent: mgr.primary,
          accentAlt: mgr.primaryAlt,
          showInternalHeader: false,
          isLoading: _isLoading,
          errorMessage: _loadError,
          onRefresh: _loadDashboardData,
          topActions: const [],
          quickActions: const [],
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

  Widget _buildProfileTab() {
    final email = _profile?.email ?? 'Tài khoản của bạn';
    return RoleModuleTabPage(
      role: kManagerRole,
      headerTitle: 'Hồ sơ cá nhân',
      headerSubtitle: email,
      trustPill: 'Quản trị',
      body: const ProfilePage(embeddedInModuleShell: true),
    );
  }

  @override
  Widget build(BuildContext context) {
    return RoleModuleTabShell(
      scaffoldKey: _scaffoldKey,
      role: kManagerRole,
      currentIndex: _currentIndex,
      onIndexChanged: (i) => setState(() => _currentIndex = i),
      drawerUserName: _profile?.displayName ?? 'Đang tải...',
      drawerUserRole: ManagerShellConfig.drawerUserRole,
      drawerRoleBadge: ManagerShellConfig.roleBadge,
      drawerSections: ManagerShellConfig.drawerSections,
      onLogout: _handleLogout,
      onDrawerNavigate: _onDrawerNavigate,
      navItems: ManagerShellConfig.navItems,
      tabs: [
        _buildDashboardTab(context),
        _buildProfileTab(),
      ],
    );
  }
}
