import 'package:flutter/material.dart';

import '../../../../app/app_routes.dart';
import '../../../../core/theme/app_design_system.dart';
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
  int _currentIndex = 0;

  void _goToMenu() => setState(() => _currentIndex = 1);

  @override
  Widget build(BuildContext context) {
    return CustomerTabShell(
      scaffoldKey: _scaffoldKey,
      currentIndex: _currentIndex,
      onIndexChanged: (i) => setState(() => _currentIndex = i),
      onDrawerNavigate: (route) => customerDefaultDrawerNavigate(context, route),
      tabs: [
        _CustomerDashboard(onNavigateToMenu: _goToMenu),
        const MenuPage(),
      ],
    );
  }
}

class _CustomerDashboard extends StatefulWidget {
  final VoidCallback onNavigateToMenu;

  const _CustomerDashboard({required this.onNavigateToMenu});

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

      if (categories.isNotEmpty) {
        final dishRes =
            await OrgRepository.instance.getDishesByCategory(categories.first.id);
        dishes = dishRes.dishes
            .map(
              (d) => {
                'id': d.id,
                'name': d.name,
                'imageUrl': d.imageUrl,
                'categoryName': categories.first.name,
              },
            )
            .toList();
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
            subtitle: 'Khám phá thực đơn HUITMeal',
            bottomPanel: CustomerHeaderSearchPanel(
              controller: _searchCtrl,
              onChanged: (v) => setState(() => _searchQuery = v),
              onSubmit: _submitSearch,
            ),
          ),
          Expanded(
            child: _isLoading
                ? const CustomerLoadingBody(message: 'Đang tải thực đơn gợi ý…')
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
        const SliverToBoxAdapter(
          child: CustomerPageIntro(
            title: 'Suất ăn sạch — giao tận nơi',
            description:
                'Xem món gợi ý, duyệt danh mục hoặc mở thực đơn đầy đủ theo từng loại món.',
            icon: Icons.eco_rounded,
          ),
        ),
        SliverToBoxAdapter(child: CustomerQuickActions(onOpenMenu: widget.onNavigateToMenu)),
        SliverToBoxAdapter(
          child: CustomerStatBanner(
            categoryCount: _categories.length,
            dishCount: _featuredDishes.length,
          ),
        ),
        SliverToBoxAdapter(
          child: CustomerSectionHeader(
            title: 'Gợi ý cho bạn',
            subtitle: _searchQuery.isEmpty
                ? 'Món phổ biến từ thực đơn hôm nay'
                : 'Kết quả cho "$_searchQuery"',
            actionLabel: 'Xem tất cả',
            onAction: widget.onNavigateToMenu,
          ),
        ),
        if (featured.isEmpty)
          SliverToBoxAdapter(
            child: CustomerEmptyState(
              title: _searchQuery.isEmpty ? 'Chưa có món gợi ý' : 'Không tìm thấy món phù hợp',
              subtitle: _searchQuery.isEmpty
                  ? 'Kéo xuống để tải lại hoặc mở thực đơn đầy đủ'
                  : 'Thử từ khóa khác hoặc xem toàn bộ thực đơn',
              actionLabel: 'Mở thực đơn',
              onAction: widget.onNavigateToMenu,
              compact: true,
            ),
          )
        else
          SliverToBoxAdapter(
            child: SizedBox(
              height: 256,
              child: ListView.separated(
                primary: false,
                scrollDirection: Axis.horizontal,
                padding: const EdgeInsets.symmetric(horizontal: 20),
                itemCount: featured.take(10).length,
                separatorBuilder: (_, __) => const SizedBox(width: 14),
                itemBuilder: (context, index) {
                  final dish = featured[index];
                  return CustomerFeaturedCard(
                    name: dish['name']?.toString() ?? 'Món ăn',
                    imageUrl: dish['imageUrl']?.toString(),
                    cacheWidth: cacheWidth,
                    onTap: () => _openDishDetail(dish),
                  );
                },
              ),
            ),
          ),
        const SliverToBoxAdapter(
          child: CustomerSectionHeader(
            title: 'Danh mục món ăn',
            subtitle: 'Chọn nhóm món để xem trong thực đơn',
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
            padding: const EdgeInsets.symmetric(horizontal: 20),
            sliver: SliverList(
              delegate: SliverChildBuilderDelegate(
                (context, index) {
                  final cat = _categories[index];
                  return Padding(
                    padding: const EdgeInsets.only(bottom: 10),
                    child: CustomerCategoryCard(
                      name: cat.name,
                      icon: customerCategoryIcon(index),
                      accent: customerCategoryColor(index),
                      onTap: widget.onNavigateToMenu,
                    ),
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
