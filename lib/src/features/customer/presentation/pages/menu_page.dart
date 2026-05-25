import 'package:flutter/material.dart';

import '../../../../app/app_routes.dart';
import '../../../../core/theme/app_design_system.dart';
import '../../../../core/widgets/role_tab_shell.dart';
import '../../../organization/data/models/bulk_order_models.dart';
import '../../../organization/data/org_repository.dart';
import '../widgets/customer_ui.dart';

class MenuPage extends StatefulWidget {
  final int? initialCategoryId;

  const MenuPage({super.key, this.initialCategoryId});

  @override
  State<MenuPage> createState() => _MenuPageState();
}

class _MenuPageState extends State<MenuPage> {
  bool _isLoadingCategories = true;
  String? _error;
  List<DishCategoryModel> _categories = [];

  int _selectedCategoryId = 0;
  bool _isLoadingDishes = false;
  List<Map<String, dynamic>> _dishes = [];

  @override
  void initState() {
    super.initState();
    _fetchCategories();
  }

  @override
  void didUpdateWidget(MenuPage oldWidget) {
    super.didUpdateWidget(oldWidget);
    final nextId = widget.initialCategoryId;
    if (nextId != null &&
        nextId != oldWidget.initialCategoryId &&
        _categories.any((c) => c.id == nextId)) {
      _onCategorySelected(nextId);
    }
  }

  Future<void> _fetchCategories() async {
    try {
      setState(() {
        _isLoadingCategories = true;
        _error = null;
      });
      final data = await OrgRepository.instance.getDishCategories();
      if (!mounted) return;

      var selectedId = 0;
      if (data.categories.isNotEmpty) {
        final preferred = widget.initialCategoryId;
        if (preferred != null && data.categories.any((c) => c.id == preferred)) {
          selectedId = preferred;
        } else {
          selectedId = data.categories.first.id;
        }
      }

      setState(() {
        _categories = data.categories;
        _selectedCategoryId = selectedId;
        _isLoadingCategories = false;
      });

      if (selectedId != 0) {
        await _fetchDishes(selectedId);
      }
    } catch (e) {
      if (mounted) {
        setState(() {
          _error = customerApiError(e);
          _isLoadingCategories = false;
        });
      }
    }
  }

  Future<void> _fetchDishes(int categoryId) async {
    try {
      setState(() => _isLoadingDishes = true);
      final res = await OrgRepository.instance.getDishesByCategory(categoryId);
      final catName = _selectedCategoryName;
      if (mounted) {
        setState(() {
          _dishes = res.dishes
              .map(
                (d) => {
                  'id': d.id,
                  'name': d.name,
                  'imageUrl': d.imageUrl,
                  'categoryName': catName,
                },
              )
              .toList();
          _isLoadingDishes = false;
        });
      }
    } catch (e) {
      if (mounted) {
        setState(() => _isLoadingDishes = false);
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(customerApiError(e)),
            behavior: SnackBarBehavior.floating,
            backgroundColor: AppDesignSystem.danger,
            shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
          ),
        );
      }
    }
  }

  void _onCategorySelected(int id) {
    if (_selectedCategoryId == id) return;
    setState(() => _selectedCategoryId = id);
    _fetchDishes(id);
  }

  String? get _selectedCategoryName {
    for (final c in _categories) {
      if (c.id == _selectedCategoryId) return c.name;
    }
    return null;
  }

  List<CustomerMenuCategory> get _menuCategories => _categories
      .map((c) => CustomerMenuCategory(id: c.id, name: c.name))
      .toList();

  String get _menuSubtitle {
    if (_isLoadingCategories) return 'Đang tải danh mục...';
    if (_isLoadingDishes) return 'Đang tải món...';
    final name = _selectedCategoryName;
    if (name != null) {
      return '${_dishes.length} món · $name';
    }
    return 'Chọn loại món bên dưới';
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

    return ColoredBox(
      color: AppDesignSystem.gray50,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          CustomerModuleHeader(
            title: 'Thực đơn',
            subtitle: _menuSubtitle,
            bottomPanel: CustomerHeaderCategoryPanel(
              categories: _menuCategories,
              selectedCategoryId: _selectedCategoryId,
              onCategorySelected: _onCategorySelected,
              isLoading: _isLoadingCategories,
            ),
          ),
          Expanded(child: _buildBody(bottom)),
        ],
      ),
    );
  }

  Widget _buildBody(double bottomInset) {
    if (_isLoadingCategories) {
      return const CustomerLoadingBody(message: 'Đang tải danh mục món…');
    }
    if (_error != null) {
      return CustomerErrorBody(message: _error!, onRetry: _fetchCategories);
    }
    if (_categories.isEmpty) {
      return CustomerEmptyState(
        title: 'Chưa có danh mục món',
        subtitle: 'Vui lòng quay lại sau hoặc kéo để tải lại',
        actionLabel: 'Tải lại',
        onAction: _fetchCategories,
      );
    }

    if (_isLoadingDishes) {
      return const CustomerDishGridSkeleton();
    }

    if (_dishes.isEmpty) {
      return CustomerEmptyState(
        icon: Icons.restaurant_outlined,
        title: 'Chưa có món trong danh mục',
        subtitle: _selectedCategoryName != null
            ? 'Danh mục: $_selectedCategoryName'
            : null,
        actionLabel: 'Tải lại',
        onAction: () => _fetchDishes(_selectedCategoryId),
      );
    }

    final dpr = MediaQuery.devicePixelRatioOf(context);
    final cacheWidth = (MediaQuery.sizeOf(context).width / 2 * dpr).round();

    return RefreshIndicator(
      color: kCustomerRole.primary,
      onRefresh: () => _fetchDishes(_selectedCategoryId),
      child: CustomScrollView(
        physics: const AlwaysScrollableScrollPhysics(parent: BouncingScrollPhysics()),
        slivers: [
          SliverToBoxAdapter(
            child: Padding(
              padding: const EdgeInsets.fromLTRB(20, 12, 20, 4),
              child: CustomerPageIntro(
                title: 'Đang xem: ${_selectedCategoryName ?? 'Danh mục'}',
                description:
                    '${_dishes.length} món trong nhóm này. Chạm vào từng món để xem ảnh lớn và thông tin.',
                icon: Icons.restaurant_menu_rounded,
              ),
            ),
          ),
          SliverPadding(
            padding: EdgeInsets.fromLTRB(16, 4, 16, bottomInset),
            sliver: SliverGrid(
              gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
                crossAxisCount: 2,
                childAspectRatio: 0.74,
                crossAxisSpacing: 12,
                mainAxisSpacing: 12,
              ),
              delegate: SliverChildBuilderDelegate(
                (context, index) {
                  final dish = _dishes[index];
                  return CustomerDishCard(
                    name: dish['name']?.toString() ?? 'Món ăn',
                    imageUrl: dish['imageUrl']?.toString(),
                    categoryName: dish['categoryName']?.toString(),
                    cacheWidth: cacheWidth,
                    onTap: () => _openDishDetail(dish),
                  );
                },
                childCount: _dishes.length,
              ),
            ),
          ),
        ],
      ),
    );
  }
}
