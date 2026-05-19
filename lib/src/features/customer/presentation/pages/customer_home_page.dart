import 'package:flutter/material.dart';
import '../../../../app/app_routes.dart';
import '../../../../core/theme/app_design_system.dart';
import '../../../../core/widgets/role_dashboard.dart';
import '../../../../core/widgets/premium_drawer.dart';
import '../../../organization/data/org_repository.dart';
import '../../../organization/data/models/bulk_order_models.dart';
import 'menu_page.dart';

class CustomerHomePage extends StatefulWidget {
  const CustomerHomePage({super.key});

  @override
  State<CustomerHomePage> createState() => _CustomerHomePageState();
}

class _CustomerHomePageState extends State<CustomerHomePage> {
  int _currentIndex = 0;

  late final List<Widget> _pages;

  @override
  void initState() {
    super.initState();
    _pages = [
      _CustomerDashboard(
        onNavigateToMenu: () => setState(() => _currentIndex = 1),
      ),
      const MenuPage(),
    ];
  }

  void _onDrawerNavigate(String route) {
    if (route == AppRoutes.login) {
      Navigator.of(context).pushReplacementNamed(AppRoutes.login);
    } else {
      Navigator.of(context).pushNamed(route);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      drawer: PremiumDrawer(
        userName: 'Khách hàng',
        userRole: 'Khám phá thực đơn',
        roleBadge: 'Customer',
        gradient: RolePalette.customer.gradient,
        accentColor: RolePalette.customer.primary,
        selectedIndex: _currentIndex,
        onSelectTab: (index) {
          if (index < _pages.length) {
            setState(() => _currentIndex = index);
          }
        },
        onNavigate: _onDrawerNavigate,
        onLogout: () {}, // Not used since profile is null
        sections: const [
          DrawerSection(
            title: 'Menu chính',
            items: [
              DrawerItem(
                icon: Icons.home_rounded,
                label: 'home',
                labelVi: 'Trang chủ',
                tabIndex: 0,
              ),
              DrawerItem(
                icon: Icons.restaurant_menu_rounded,
                label: 'menu',
                labelVi: 'Thực đơn',
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
            selectedItemColor: RolePalette.customer.primary,
            unselectedItemColor: Colors.grey.shade400,
            elevation: 0,
            items: const [
              BottomNavigationBarItem(
                icon: Icon(Icons.home_outlined),
                activeIcon: Icon(Icons.home),
                label: 'Trang chủ',
              ),
              BottomNavigationBarItem(
                icon: Icon(Icons.restaurant_menu_outlined),
                activeIcon: Icon(Icons.restaurant_menu),
                label: 'Thực đơn',
              ),
            ],
          ),
        ),
      ),
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
  bool _isLoading = true;
  List<DishCategoryModel> _categories = [];
  List<Map<String, dynamic>> _featuredDishes = [];

  @override
  void initState() {
    super.initState();
    _fetchData();
  }

  Future<void> _fetchData() async {
    try {
      final catResponse = await OrgRepository.instance.getDishCategories();
      final categories = catResponse.categories;
      List<Map<String, dynamic>> dishes = [];
      
      if (categories.isNotEmpty) {
        // Fetch dishes from the first category as featured
        final dishRes =
            await OrgRepository.instance.getDishesByCategory(categories.first.id);
        dishes = dishRes.dishes
            .map(
              (d) => {
                'id': d.id,
                'name': d.name,
                'price': d.price,
                'imageUrl': d.imageUrl,
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
        setState(() => _isLoading = false);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.grey.shade50,
      appBar: AppBar(
        leading: IconButton(
          icon: const Icon(Icons.menu),
          onPressed: () => Scaffold.of(context).openDrawer(),
        ),
        title: const Text(
          'HUITMeal',
          style: TextStyle(fontWeight: FontWeight.bold),
        ),
        backgroundColor: RolePalette.customer.primary,
        foregroundColor: Colors.white,
        elevation: 0,
      ),
      body: _isLoading 
          ? Center(child: CircularProgressIndicator(color: RolePalette.customer.primary))
          : RefreshIndicator(
              onRefresh: _fetchData,
              color: RolePalette.customer.primary,
              child: SingleChildScrollView(
                physics: const AlwaysScrollableScrollPhysics(),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    // Header Section
                    Container(
                      padding: const EdgeInsets.all(24),
                      decoration: const BoxDecoration(
                        color: AppDesignSystem.orange500,
                        borderRadius: BorderRadius.only(
                          bottomLeft: Radius.circular(32),
                          bottomRight: Radius.circular(32),
                        ),
                      ),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          const Text(
                            'Xin chào,',
                            style: TextStyle(color: Colors.white70, fontSize: 16),
                          ),
                          const SizedBox(height: 8),
                          const Text(
                            'Hôm nay bạn muốn thưởng thức món gì?',
                            style: TextStyle(
                              color: Colors.white,
                              fontSize: 24,
                              fontWeight: FontWeight.bold,
                            ),
                          ),
                          const SizedBox(height: 24),
                          Container(
                            padding: const EdgeInsets.symmetric(
                              horizontal: 16,
                              vertical: 4,
                            ),
                            decoration: BoxDecoration(
                              color: Colors.white.withValues(alpha: 0.2),
                              borderRadius: BorderRadius.circular(16),
                            ),
                            child: const TextField(
                              style: TextStyle(color: Colors.white),
                              decoration: InputDecoration(
                                icon: Icon(Icons.search, color: Colors.white70),
                                hintText: 'Tìm kiếm món ăn...',
                                hintStyle: TextStyle(color: Colors.white70),
                                border: InputBorder.none,
                              ),
                            ),
                          ),
                        ],
                      ),
                    ),
                    Padding(
                      padding: const EdgeInsets.fromLTRB(20, 24, 20, 0),
                      child: GridView.count(
                        crossAxisCount: 2,
                        shrinkWrap: true,
                        physics: const NeverScrollableScrollPhysics(),
                        crossAxisSpacing: 12,
                        mainAxisSpacing: 12,
                        childAspectRatio: 1.4,
                        children: [
                          DashboardMetricCard(
                            title: 'Danh mục món',
                            value: '${_categories.length}',
                            icon: Icons.category_rounded,
                            color: AppDesignSystem.orange500,
                            trend: _categories.isEmpty ? 'Đang cập nhật' : 'Sẵn sàng',
                            trendPositive: _categories.isNotEmpty,
                          ),
                          DashboardMetricCard(
                            title: 'Món gợi ý',
                            value: '${_featuredDishes.length}',
                            icon: Icons.restaurant_rounded,
                            color: AppDesignSystem.orange400,
                            trend: 'Từ thực đơn hôm nay',
                            trendPositive: true,
                          ),
                        ],
                      ),
                    ),
                    const SizedBox(height: 24),

                    // Featured Dishes Section
                    Padding(
                      padding: const EdgeInsets.symmetric(horizontal: 24),
                      child: Row(
                        mainAxisAlignment: MainAxisAlignment.spaceBetween,
                        children: [
                          Text(
                            'Gợi ý cho bạn',
                            style: TextStyle(
                              fontSize: 20,
                              fontWeight: FontWeight.bold,
                              color: Colors.grey.shade800,
                            ),
                          ),
                          TextButton(
                            onPressed: widget.onNavigateToMenu,
                            child: Text(
                              'Xem tất cả',
                              style: TextStyle(color: RolePalette.customer.link),
                            ),
                          ),
                        ],
                      ),
                    ),
                    const SizedBox(height: 16),
                    if (_featuredDishes.isEmpty)
                      const Padding(
                        padding: EdgeInsets.symmetric(horizontal: 24),
                        child: Text('Chưa có gợi ý món ăn nào.', style: TextStyle(color: Colors.grey)),
                      )
                    else
                      SingleChildScrollView(
                        scrollDirection: Axis.horizontal,
                        padding: const EdgeInsets.symmetric(horizontal: 16),
                        child: Row(
                          children: _featuredDishes.take(5).map((dish) {
                            final name = dish['name'] ?? 'Món ăn';
                            final price = dish['price'] ?? 0;
                            final imageUrl = dish['imageUrl'];
                            
                            return Container(
                              width: 160,
                              margin: const EdgeInsets.only(right: 16),
                              decoration: BoxDecoration(
                                color: Colors.white,
                                borderRadius: BorderRadius.circular(20),
                                boxShadow: [
                                  BoxShadow(
                                    color: Colors.black.withValues(alpha: 0.05),
                                    blurRadius: 10,
                                    offset: const Offset(0, 5),
                                  ),
                                ],
                              ),
                              child: Column(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  Container(
                                    height: 120,
                                    decoration: BoxDecoration(
                                      color: Colors.grey.shade200,
                                      borderRadius: const BorderRadius.vertical(top: Radius.circular(20)),
                                      image: imageUrl != null && imageUrl.toString().isNotEmpty
                                          ? DecorationImage(
                                              image: NetworkImage(imageUrl),
                                              fit: BoxFit.cover,
                                            )
                                          : null,
                                    ),
                                    child: imageUrl == null || imageUrl.toString().isEmpty
                                        ? const Center(child: Icon(Icons.fastfood, color: Colors.grey, size: 48))
                                        : null,
                                  ),
                                  Padding(
                                    padding: const EdgeInsets.all(12),
                                    child: Column(
                                      crossAxisAlignment: CrossAxisAlignment.start,
                                      children: [
                                        Text(
                                          name,
                                          maxLines: 2,
                                          overflow: TextOverflow.ellipsis,
                                          style: const TextStyle(
                                            fontWeight: FontWeight.bold,
                                            fontSize: 14,
                                          ),
                                        ),
                                        const SizedBox(height: 6),
                                        Text(
                                          '${price}đ',
                                          style: const TextStyle(
                                            color: AppDesignSystem.orange500,
                                            fontWeight: FontWeight.bold,
                                            fontSize: 14,
                                          ),
                                        ),
                                      ],
                                    ),
                                  ),
                                ],
                              ),
                            );
                          }).toList(),
                        ),
                      ),

                    const SizedBox(height: 32),

                    // Dish Categories Section
                    Padding(
                      padding: const EdgeInsets.symmetric(horizontal: 24),
                      child: Row(
                        mainAxisAlignment: MainAxisAlignment.spaceBetween,
                        children: [
                          Text(
                            'Danh mục món ăn',
                            style: TextStyle(
                              fontSize: 20,
                              fontWeight: FontWeight.bold,
                              color: Colors.grey.shade800,
                            ),
                          ),
                        ],
                      ),
                    ),
                    const SizedBox(height: 16),
                    if (_categories.isEmpty)
                      const Padding(
                        padding: EdgeInsets.symmetric(horizontal: 24),
                        child: Text('Chưa có danh mục nào.', style: TextStyle(color: Colors.grey)),
                      )
                    else
                      Padding(
                        padding: const EdgeInsets.symmetric(horizontal: 24),
                        child: Wrap(
                          spacing: 12,
                          runSpacing: 12,
                          children: _categories.map((cat) {
                            return GestureDetector(
                              onTap: widget.onNavigateToMenu,
                              child: Container(
                                padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
                                decoration: BoxDecoration(
                                  color: Colors.white,
                                  borderRadius: BorderRadius.circular(16),
                                  border: Border.all(color: Colors.grey.shade200),
                                  boxShadow: [
                                    BoxShadow(
                                      color: Colors.black.withValues(alpha: 0.02),
                                      blurRadius: 5,
                                      offset: const Offset(0, 2),
                                    ),
                                  ],
                                ),
                                child: Row(
                                  mainAxisSize: MainAxisSize.min,
                                  children: [
                                    Icon(Icons.restaurant, size: 18, color: Colors.blue.shade400),
                                    const SizedBox(width: 8),
                                    Text(
                                      cat.name,
                                      style: TextStyle(
                                        color: Colors.grey.shade800,
                                        fontWeight: FontWeight.w600,
                                      ),
                                    ),
                                  ],
                                ),
                              ),
                            );
                          }).toList(),
                        ),
                      ),
                      
                    const SizedBox(height: 48),
                  ],
                ),
              ),
            ),
    );
  }
}
