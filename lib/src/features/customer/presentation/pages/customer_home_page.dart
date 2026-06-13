import 'package:flutter/material.dart';

import '../../../../app/app_routes.dart';
import '../../../../core/theme/app_design_system.dart';
import '../../../../core/widgets/app_confirm_dialog.dart';
import '../../../../core/widgets/role_tab_shell.dart';
import '../../../organization/data/models/bulk_order_models.dart';
import '../../../organization/data/org_repository.dart';
import '../widgets/customer_ui.dart';
import 'menu_page.dart';

class CustomerHomePage extends StatefulWidget {
  const CustomerHomePage({super.key});

  @override
  State<CustomerHomePage> createState() => _CustomerHomePageState();
}

class _CustomerHomePageState extends State<CustomerHomePage> {
  final GlobalKey<ScaffoldState> _scaffoldKey = GlobalKey<ScaffoldState>();
  final GlobalKey _categoriesSectionKey = GlobalKey();
  int _currentIndex = 0;
  int? _menuInitialCategoryId;

  void _goToMenu({int? categoryId}) {
    setState(() {
      _menuInitialCategoryId = categoryId;
      _currentIndex = 1;
    });
  }

  void _scrollToCategories() {
    final ctx = _categoriesSectionKey.currentContext;
    if (ctx != null) {
      Scrollable.ensureVisible(
        ctx,
        duration: const Duration(milliseconds: 400),
        curve: Curves.easeOutCubic,
        alignment: 0.05,
      );
      return;
    }
    _goToMenu();
  }

  Future<void> _handleLogout() async {
    await performAppLogout(context, role: kCustomerRole);
  }

  @override
  Widget build(BuildContext context) {
    return CustomerTabShell(
      scaffoldKey: _scaffoldKey,
      currentIndex: _currentIndex,
      onIndexChanged: (i) => setState(() => _currentIndex = i),
      onLogout: _handleLogout,
      onDrawerNavigate: (route) => customerDefaultDrawerNavigate(
        context,
        route,
        onLogout: _handleLogout,
      ),
      tabs: [
        _CustomerDashboard(
          categoriesSectionKey: _categoriesSectionKey,
          onNavigateToMenu: _goToMenu,
          onScrollToCategories: _scrollToCategories,
        ),
        MenuPage(initialCategoryId: _menuInitialCategoryId),
      ],
    );
  }
}

class _CustomerDashboard extends StatefulWidget {
  final GlobalKey categoriesSectionKey;
  final void Function({int? categoryId}) onNavigateToMenu;
  final VoidCallback onScrollToCategories;

  const _CustomerDashboard({
    required this.categoriesSectionKey,
    required this.onNavigateToMenu,
    required this.onScrollToCategories,
  });

  @override
  State<_CustomerDashboard> createState() => _CustomerDashboardState();
}

class _CustomerDashboardState extends State<_CustomerDashboard> {
  final _searchCtrl = TextEditingController();
  bool _isLoading = true;
  String? _error;
  List<DishCategoryModel> _categories = [];
  List<Map<String, dynamic>> _featuredDishes = [];
  String _searchQuery = '';

  @override
  void initState() {
    super.initState();
    _fetchData();
  }

  @override
  void dispose() {
    _searchCtrl.dispose();
    super.dispose();
  }

  Future<void> _fetchData() async {
    setState(() {
      _isLoading = true;
      _error = null;
    });
    try {
      final catResponse = await OrgRepository.instance.getDishCategories();
      final categories = catResponse.categories;
      var dishes = <Map<String, dynamic>>[];

      for (final cat in categories.take(3)) {
        final dishRes = await OrgRepository.instance.getDishesByCategory(cat.id);
        for (final d in dishRes.dishes.take(4)) {
          dishes.add({
            'id': d.id,
            'name': d.name,
            'imageUrl': d.imageUrl,
            'categoryName': cat.name,
          });
        }
      }

      if (mounted) {
        setState(() {
          _categories = categories;
          _featuredDishes = dishes;
          _isLoading = false;
        });
      }
    } catch (e) {
      if (mounted) {
        setState(() {
          _error = customerApiError(e);
          _isLoading = false;
        });
      }
    }
  }

  List<Map<String, dynamic>> get _filteredFeatured {
    if (_searchQuery.trim().isEmpty) return _featuredDishes;
    final q = _searchQuery.toLowerCase();
    return _featuredDishes
        .where((d) => (d['name']?.toString() ?? '').toLowerCase().contains(q))
        .toList();
  }

  void _submitSearch() {
    setState(() => _searchQuery = _searchCtrl.text.trim());
  }

  void _openDishDetail(Map<String, dynamic> dish) {
    Navigator.of(context).pushNamed(
      AppRoutes.customerMealDetail,
      arguments: CustomerDishDetailArgs(
        id: dish['id'] as int? ?? 0,
        name: dish['name']?.toString() ?? 'Món ăn',
        imageUrl: dish['imageUrl']?.toString(),
        categoryName: dish['categoryName']?.toString(),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final bottom = RoleTabScope.of(context).bottomInset;
    final dpr = MediaQuery.devicePixelRatioOf(context);
    final cacheWidth = (200 * dpr).round();

    return ColoredBox(
      color: AppDesignSystem.gray50,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          CustomerModuleHeader(
            title: 'Trang chủ',
            subtitle: 'Xem thực đơn — không cần đăng nhập',
            bottomPanel: CustomerHeaderSearchPanel(
              controller: _searchCtrl,
              onChanged: (v) => setState(() => _searchQuery = v),
              onSubmit: _submitSearch,
            ),
          ),
          Expanded(
            child: _isLoading
                ? const CustomerLoadingBody(showHomeSkeleton: true)
                : _error != null
                    ? CustomerErrorBody(message: _error!, onRetry: _fetchData)
                    : RefreshIndicator(
                        color: kCustomerRole.primary,
                        onRefresh: _fetchData,
                        child: _buildScrollContent(bottom, cacheWidth),
                      ),
          ),
        ],
      ),
    );
  }

  Widget _buildScrollContent(double bottom, int cacheWidth) {
    final featured = _filteredFeatured;

    return CustomScrollView(
      physics: const AlwaysScrollableScrollPhysics(parent: BouncingScrollPhysics()),
      keyboardDismissBehavior: ScrollViewKeyboardDismissBehavior.onDrag,
      slivers: [
        const SliverToBoxAdapter(child: CustomerWelcomeBanner()),
        const SliverToBoxAdapter(child: CustomerTrustStrip()),
        SliverToBoxAdapter(
          child: CustomerQuickActions(
            onOpenMenu: () => widget.onNavigateToMenu(),
            onScrollCategories: widget.onScrollToCategories,
          ),
        ),
        SliverToBoxAdapter(
          child: CustomerStatBanner(
            categoryCount: _categories.length,
            dishCount: featured.length,
          ),
        ),
        SliverToBoxAdapter(
          child: CustomerSectionHeader(
            title: 'Món gợi ý',
            subtitle: _searchQuery.isEmpty
                ? 'Chọn món để xem ảnh và mô tả chi tiết'
                : 'Kết quả cho "$_searchQuery"',
            actionLabel: 'Thực đơn',
            onAction: () => widget.onNavigateToMenu(),
          ),
        ),
        if (featured.isEmpty)
          SliverToBoxAdapter(
            child: CustomerEmptyState(
              title: _searchQuery.isEmpty ? 'Chưa có món gợi ý' : 'Không tìm thấy món',
              subtitle: _searchQuery.isEmpty
                  ? 'Kéo xuống để tải lại hoặc mở tab Thực đơn'
                  : 'Thử từ khóa khác',
              actionLabel: 'Mở thực đơn',
              onAction: () => widget.onNavigateToMenu(),
              compact: true,
            ),
          )
        else
          SliverToBoxAdapter(
            child: SizedBox(
              height: 268,
              child: ListView.separated(
                primary: false,
                scrollDirection: Axis.horizontal,
                padding: const EdgeInsets.symmetric(horizontal: 20),
                itemCount: featured.take(12).length,
                separatorBuilder: (_, __) => const SizedBox(width: 14),
                itemBuilder: (context, index) {
                  final dish = featured[index];
                  return CustomerFeaturedCard(
                    name: dish['name']?.toString() ?? 'Món ăn',
                    imageUrl: dish['imageUrl']?.toString(),
                    categoryName: dish['categoryName']?.toString(),
                    cacheWidth: cacheWidth,
                    onTap: () => _openDishDetail(dish),
                  );
                },
              ),
            ),
          ),
        SliverToBoxAdapter(
          key: widget.categoriesSectionKey,
          child: const CustomerSectionHeader(
            title: 'Danh mục món ăn',
            subtitle: 'Chạm nhóm món để mở thực đơn tương ứng',
          ),
        ),
        if (_categories.isEmpty)
          SliverToBoxAdapter(
            child: CustomerEmptyState(
              icon: Icons.category_outlined,
              title: 'Chưa có danh mục',
              subtitle: 'Hệ thống đang cập nhật thực đơn',
              actionLabel: 'Tải lại',
              onAction: _fetchData,
              compact: true,
            ),
          )
        else
          SliverPadding(
            padding: const EdgeInsets.fromLTRB(20, 0, 20, 8),
            sliver: SliverGrid(
              gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
                crossAxisCount: 2,
                childAspectRatio: 1.05,
                crossAxisSpacing: 12,
                mainAxisSpacing: 12,
              ),
              delegate: SliverChildBuilderDelegate(
                (context, index) {
                  final cat = _categories[index];
                  return CustomerCategoryTile(
                    name: cat.name,
                    icon: customerCategoryIcon(index),
                    accent: customerCategoryColor(index),
                    onTap: () => widget.onNavigateToMenu(categoryId: cat.id),
                  );
                },
                childCount: _categories.length,
              ),
            ),
          ),
        SliverPadding(padding: EdgeInsets.only(bottom: bottom)),
      ],
    );
  }
}
