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
import '../../data/org_repository.dart';
import '../../data/models/contract_models.dart';

class OrganizationHomePage extends StatefulWidget {
  const OrganizationHomePage({super.key});

  @override
  State<OrganizationHomePage> createState() => _OrganizationHomePageState();
}

class _OrganizationHomePageState extends State<OrganizationHomePage> {
  final GlobalKey<ScaffoldState> _scaffoldKey = GlobalKey<ScaffoldState>();
  int _currentIndex = 0;

  UserProfileModel? _profile;
  int _contractCount = 0;
  int _activeContractCount = 0;
  double _totalContractValue = 0;
  int _pendingPaymentCount = 0;
  double _pendingPaymentAmount = 0;
  int _categoryCount = 0;
  List<ContractModel> _recentContracts = const [];
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
      final contracts = await OrgRepository.instance.getContracts(pageSize: 50);
      final catResponse = await OrgRepository.instance.getDishCategories();
      final categories = catResponse.categories;

      double totalVal = 0;
      int active = 0;
      for (final c in contracts.items) {
        totalVal += c.value;
        if (c.status == ContractStatus.active || c.status == ContractStatus.signed) {
          active++;
        }
      }

      int pendingCount = 0;
      double pendingAmount = 0;
      final activeContracts = contracts.items
          .where((c) => c.status == ContractStatus.active || c.status == ContractStatus.signed)
          .take(3)
          .toList();

      for (final c in activeContracts) {
        try {
          final payments = await OrgRepository.instance.getContractPayments(c.id);
          for (final p in payments.items) {
            if (p.status == PaymentStatus.pending) {
              pendingCount++;
              pendingAmount += p.amount;
            }
          }
        } catch (_) {
          // Bỏ qua nếu không tải được payments từng HĐ
        }
      }

      final sorted = List<ContractModel>.from(contracts.items)
        ..sort((a, b) => b.id.compareTo(a.id));
      final recent = sorted.take(3).toList();

      final recentItems = recent
          .map(
            (c) => DashboardRecentItem(
              title: c.code,
              subtitle: '${c.type.isNotEmpty ? c.type : "Hợp đồng"} · ${c.status.label}',
              trailing: formatDashboardVnd(c.value),
              icon: Icons.description_outlined,
              iconColor: RolePalette.organization.primary,
              route: AppRoutes.orgContractSettlement,
            ),
          )
          .toList();

      if (!mounted) return;
      setState(() {
        _profile = profile;
        _contractCount = contracts.totalCount;
        _activeContractCount = active;
        _totalContractValue = totalVal;
        _pendingPaymentCount = pendingCount;
        _pendingPaymentAmount = pendingAmount;
        _categoryCount = categories.length;
        _recentContracts = recent;
        _recentItems = recentItems;
        _isLoading = false;
      });
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _isLoading = false;
        _loadError = 'Không tải được dữ liệu tổng quan. Kéo xuống để thử lại.';
      });
    }
  }

  List<Widget> get _pages => [
        _buildOrganizationDashboard(),
        ProfilePage(onMenuPressed: () => _scaffoldKey.currentState?.openDrawer()),
      ];

  Widget _buildOrganizationDashboard() {
    final org = RolePalette.organization;
    final displayName = _profile?.displayName ?? 'Đơn vị';

    return Scaffold(
      appBar: AppBar(
        leading: IconButton(
          icon: const Icon(Icons.menu),
          onPressed: () => _scaffoldKey.currentState?.openDrawer(),
        ),
        title: const Text('Dashboard', style: TextStyle(fontWeight: FontWeight.bold)),
        backgroundColor: org.primary,
        foregroundColor: Colors.white,
        elevation: 0,
      ),
      body: RoleDashboardBody(
        title: 'Xin chào, $displayName',
        subtitle: 'Tổng quan đặt suất & hợp đồng B2B',
        icon: Icons.business_rounded,
        accent: org.primary,
        accentAlt: org.primaryAlt,
        isLoading: _isLoading,
        errorMessage: _loadError,
        onRefresh: _loadDashboardData,
        topActions: [
          DashboardTopAction(
            icon: Icons.notifications_rounded,
            tooltip: 'Thông báo',
            route: AppRoutes.orgNotifications,
          ),
        ],
        quickActions: const [
          DashboardQuickAction(
            label: 'Đặt suất ăn',
            icon: Icons.restaurant_menu_rounded,
            route: AppRoutes.orgBulkOrder,
          ),
          DashboardQuickAction(
            label: 'Hợp đồng',
            icon: Icons.request_quote_rounded,
            route: AppRoutes.orgContractSettlement,
          ),
        ],
        summaryMetrics: [
          DashboardMetricCard(
            title: 'Hợp đồng',
            value: _isLoading ? '—' : '$_contractCount',
            icon: Icons.request_quote_rounded,
            color: org.primary,
            trend: _isLoading ? null : '$_activeContractCount đang hiệu lực',
            trendPositive: true,
          ),
          DashboardMetricCard(
            title: 'Giá trị HĐ',
            value: _isLoading ? '—' : formatDashboardVnd(_totalContractValue),
            icon: Icons.monetization_on_rounded,
            color: org.primaryAlt,
            trend: _isLoading ? null : 'Tổng lũy kế',
            trendPositive: true,
          ),
          DashboardMetricCard(
            title: 'Thanh toán chờ',
            value: _isLoading ? '—' : '$_pendingPaymentCount kỳ',
            icon: Icons.payments_outlined,
            color: AppDesignSystem.warning,
            trend: _isLoading ? null : formatDashboardVnd(_pendingPaymentAmount),
            trendPositive: _pendingPaymentCount == 0,
          ),
          DashboardMetricCard(
            title: 'Danh mục món',
            value: _isLoading ? '—' : '$_categoryCount',
            icon: Icons.restaurant_rounded,
            color: AppDesignSystem.info,
            trend: _isLoading ? null : 'Sẵn sàng đặt',
            trendPositive: true,
          ),
        ],
        recentItems: _recentItems,
        features: [
          DashboardFeature(
            title: 'Hợp đồng & Thanh toán',
            subtitle: 'Ký kết và đối soát hóa đơn',
            icon: Icons.request_quote_rounded,
            color: org.primary,
            route: AppRoutes.orgContractSettlement,
            preview: _isLoading
                ? null
                : '$_activeContractCount / $_contractCount hợp đồng hiệu lực',
          ),
          DashboardFeature(
            title: 'Đặt Suất Ăn',
            subtitle: 'Dịch vụ suất ăn tập trung',
            icon: Icons.restaurant_rounded,
            color: AppDesignSystem.info,
            route: AppRoutes.orgBulkOrder,
            preview: _isLoading ? null : '$_categoryCount danh mục món',
          ),
          DashboardFeature(
            title: 'Thống kê suất ăn',
            subtitle: 'Theo ngày / ca trong kỳ',
            icon: Icons.analytics_rounded,
            color: AppDesignSystem.gray700,
            route: AppRoutes.orgStatistics,
            preview: _recentContracts.isEmpty
                ? 'Chưa có dữ liệu'
                : 'HĐ mới: ${_recentContracts.first.code}',
          ),
          DashboardFeature(
            title: 'Báo cáo & Đối soát',
            subtitle: 'Tóm tắt và kỳ thanh toán',
            icon: Icons.receipt_long_rounded,
            color: AppDesignSystem.warning,
            route: AppRoutes.orgReconciliation,
            preview: _isLoading
                ? null
                : '$_pendingPaymentCount kỳ chờ thanh toán',
          ),
          DashboardFeature(
            title: 'Danh sách Nhân sự',
            subtitle: 'Quản lý thông tin nhân viên',
            icon: Icons.badge_rounded,
            color: AppDesignSystem.success,
            route: AppRoutes.orgStaff,
          ),
          DashboardFeature(
            title: 'Hỗ trợ CSKH',
            subtitle: 'Chatbot và liên hệ giải đáp',
            icon: Icons.support_agent_rounded,
            color: AppDesignSystem.gray500,
            route: AppRoutes.chatbot,
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
        userRole: 'Tổ chức B2B',
        roleBadge: 'Organization',
        gradient: RolePalette.organization.gradient,
        accentColor: RolePalette.organization.primary,
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
            title: 'Dịch vụ chính',
            items: [
              DrawerItem(
                icon: Icons.dashboard_rounded,
                label: 'dashboard',
                labelVi: 'Dashboard',
                tabIndex: 0,
              ),
              DrawerItem(
                icon: Icons.restaurant_menu_rounded,
                label: 'bulk_order',
                labelVi: 'Đặt suất ăn tập trung',
                route: AppRoutes.orgBulkOrder,
              ),
              DrawerItem(
                icon: Icons.request_quote_rounded,
                label: 'contracts',
                labelVi: 'Hợp đồng & Thanh toán',
                route: AppRoutes.orgContractSettlement,
              ),
            ],
          ),
          DrawerSection(
            title: 'Quản lý đơn vị',
            items: [
              DrawerItem(
                icon: Icons.people_alt_rounded,
                label: 'staff',
                labelVi: 'Danh sách nhân sự',
                route: AppRoutes.orgStaff,
              ),
              DrawerItem(
                icon: Icons.analytics_rounded,
                label: 'statistics',
                labelVi: 'Thống kê suất ăn',
                route: AppRoutes.orgStatistics,
              ),
              DrawerItem(
                icon: Icons.description_outlined,
                label: 'reports',
                labelVi: 'Báo cáo',
                route: AppRoutes.orgReports,
              ),
              DrawerItem(
                icon: Icons.account_balance_wallet_outlined,
                label: 'reconciliation',
                labelVi: 'Đối soát thanh toán',
                route: AppRoutes.orgReconciliation,
              ),
            ],
          ),
          DrawerSection(
            title: 'Cá nhân & Hỗ trợ',
            items: [
              DrawerItem(
                icon: Icons.person_rounded,
                label: 'profile',
                labelVi: 'Hồ sơ đơn vị',
                tabIndex: 1,
              ),
              DrawerItem(
                icon: Icons.support_agent_rounded,
                label: 'support',
                labelVi: 'Chatbot CSKH',
                route: AppRoutes.chatbot,
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
            selectedItemColor: RolePalette.organization.primary,
            unselectedItemColor: AppDesignSystem.gray400,
            elevation: 0,
            items: const [
              BottomNavigationBarItem(
                icon: Icon(Icons.business_outlined),
                activeIcon: Icon(Icons.business_rounded),
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
