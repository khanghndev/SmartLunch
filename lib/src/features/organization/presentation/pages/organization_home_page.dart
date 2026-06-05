import 'package:flutter/material.dart';
import '../../../../app/app_routes.dart';
import '../../../../core/theme/app_design_system.dart';
import '../../../../core/widgets/dashboard_format.dart';
import '../../../../core/widgets/role_dashboard.dart';
import '../../../../core/widgets/role_module_header.dart';
import '../../../../core/widgets/role_module_shell.dart';
import '../widgets/organization_shell.dart';
import '../../../profile/presentation/pages/profile_page.dart';
import 'contract_settlement_page.dart';
import 'org_bulk_order_mode_page.dart';
import 'org_reviews_page.dart';
import '../../../../core/widgets/app_confirm_dialog.dart';
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

      final paymentResults = await Future.wait(
        activeContracts.map((c) async {
          try {
            return await OrgRepository.instance.getContractPayments(c.id);
          } catch (_) {
            return null;
          }
        }),
      );
      for (final payments in paymentResults) {
        if (payments == null) continue;
        for (final p in payments.items) {
          if (p.status == PaymentStatus.pending) {
            pendingCount++;
            pendingAmount += p.amount;
          }
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

  Widget _buildDashboardTab(BuildContext context) {
    final org = kOrgRoleShell;
    final displayName = _profile?.displayName ?? 'Đơn vị';

    return RoleModuleTabPage(
      role: org,
      headerTitle: 'Dashboard',
      headerSubtitle: 'Xin chào, $displayName · Tổng quan đặt suất & hợp đồng B2B',
      trustPill: 'B2B',
      headerTrailing: RoleModuleIconButton(
        icon: Icons.notifications_rounded,
        onTap: () => Navigator.of(context).pushNamed(AppRoutes.orgNotifications),
      ),
      headerBottomPanel: RoleHeaderQuickActionsPanel(
        actions: [
          RoleHeaderQuickAction(
            label: 'Đặt suất ăn',
            icon: Icons.restaurant_menu_rounded,
            color: org.primary,
            onTap: () => setState(() => _currentIndex = 1),
          ),
          RoleHeaderQuickAction(
            label: 'Hợp đồng',
            icon: Icons.request_quote_rounded,
            color: org.primaryAlt,
            onTap: () => setState(() => _currentIndex = 2),
          ),
          RoleHeaderQuickAction(
            label: 'Đánh giá',
            icon: Icons.star_rounded,
            color: AppDesignSystem.warning,
            onTap: () => setState(() => _currentIndex = 3),
          ),
        ],
      ),
      body: RoleDashboardBody(
        title: 'Xin chào, $displayName',
        subtitle: 'Tổng quan đặt suất & hợp đồng B2B',
        icon: Icons.business_rounded,
        accent: org.primary,
        accentAlt: org.primaryAlt,
        showInternalHeader: false,
        isLoading: _isLoading,
        errorMessage: _loadError,
        onRefresh: _loadDashboardData,
        topActions: const [],
        quickActions: const [],
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
            route: AppRoutes.orgBulkOrderMode,
            preview: _isLoading ? null : '$_categoryCount danh mục món',
          ),
          DashboardFeature(
            title: 'Đánh giá suất ăn',
            subtitle: 'Xem & gửi phản hồi chất lượng',
            icon: Icons.star_rounded,
            color: kOrgRoleShell.primaryAlt,
            route: AppRoutes.orgReviews,
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

  Widget _buildBulkOrderTab() {
    return const RoleModuleTabPage(
      role: kOrgRoleShell,
      headerTitle: 'Đặt suất ăn',
      headerSubtitle: 'Tự động hoặc theo hợp đồng kỳ',
      trustPill: 'B2B',
      body: OrgBulkOrderModePage(embeddedInModuleShell: true),
    );
  }

  Widget _buildContractsTab() {
    return const RoleModuleTabPage(
      role: kOrgRoleShell,
      headerTitle: 'Hợp đồng & thanh toán',
      headerSubtitle: 'Đối soát và thanh toán cọc PayOS',
      trustPill: 'B2B',
      body: ContractSettlementPage(embeddedInModuleShell: true),
    );
  }

  Widget _buildReviewsTab() {
    return const RoleModuleTabPage(
      role: kOrgRoleShell,
      headerTitle: 'Đánh giá suất ăn',
      headerSubtitle: 'Xem và gửi đánh giá theo đơn',
      trustPill: 'B2B',
      body: OrgReviewsPage(embeddedInModuleShell: true),
    );
  }

  Widget _buildProfileTab() {
    final email = _profile?.email ?? 'Tài khoản đơn vị';
    return RoleModuleTabPage(
      role: kOrgRoleShell,
      headerTitle: 'Hồ sơ đơn vị',
      headerSubtitle: email,
      trustPill: 'B2B',
      body: const ProfilePage(embeddedInModuleShell: true),
    );
  }

  Future<void> _handleLogout() async {
    await performAppLogout(context, role: kOrgRoleShell);
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
      role: kOrgRoleShell,
      currentIndex: _currentIndex,
      onIndexChanged: (i) => setState(() => _currentIndex = i),
      drawerUserName: _profile?.displayName ?? 'Đang tải...',
      drawerUserRole: OrganizationShellConfig.drawerUserRole,
      drawerRoleBadge: OrganizationShellConfig.roleBadge,
      drawerSections: OrganizationShellConfig.drawerSections,
      onLogout: _handleLogout,
      onDrawerNavigate: _onDrawerNavigate,
      navItems: OrganizationShellConfig.navItems,
      tabs: [
        _buildDashboardTab(context),
        _buildBulkOrderTab(),
        _buildContractsTab(),
        _buildReviewsTab(),
        _buildProfileTab(),
      ],
    );
  }
}
